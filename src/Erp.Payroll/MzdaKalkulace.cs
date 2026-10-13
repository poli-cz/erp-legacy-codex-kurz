using System;
using System.Collections.Generic;
using System.Globalization;
using Erp.Framework;

namespace Erp.Payroll
{
    /// <summary>
    /// Výpočet čisté mzdy za jeden měsíc.
    /// Historie: přepis z Delphi (MZDY.PAS) v roce 2009, od té doby se každý leden upravuje podle novel.
    /// Pořadí kroků NEMĚNIT, navazují na něj sestavy (rekapitulace mezd, přehled pro ČSSZ).
    /// </summary>
    public class MzdaKalkulace
    {
        private readonly IParametryMezd _param;

        // Formulář čte poslední hodnoty po výpočtu (MzdaVypocetForm). Neodstraňovat.
        public decimal PosledniHruba;
        public string PosledniChyba;
        public List<string> PosledniUpozorneni = new List<string>();

        public MzdaKalkulace()
            : this(ServiceRegistry.Get<IParametryMezd>())
        {
        }

        public MzdaKalkulace(IParametryMezd parametry)
        {
            if (parametry == null)
                throw new ArgumentNullException("parametry");
            _param = parametry;
        }

        /// <summary>
        /// Spočítá mzdu zaměstnance za období RRRRMM z hrubé mzdy.
        /// Vrací ErpResult, business chyby nehází výjimkou.
        /// </summary>
        public ErpResult<MzdovyListek> Vypocitej(Zamestnanec zam, decimal hrubaMzda, int obdobi)
        {
            // ------------------------------------------------------------------
            // 1. kontroly vstupu
            // ------------------------------------------------------------------
            PosledniChyba = null;
            PosledniUpozorneni.Clear();
            if (zam == null)
            {
                PosledniChyba = "MZD-001";
                return ErpResult<MzdovyListek>.Fail("MZD-001", "Není zadán zaměstnanec.");
            }
            if (hrubaMzda < 0)
            {
                PosledniChyba = "MZD-002";
                return ErpResult<MzdovyListek>.Fail("MZD-002", "Hrubá mzda nesmí být záporná.");
            }
            int rok = obdobi / 100;
            int mesic = obdobi % 100;
            if (mesic < 1 || mesic > 12)
            {
                PosledniChyba = "MZD-003";
                return ErpResult<MzdovyListek>.Fail("MZD-003", "Neplatné období " + obdobi + " (čekám RRRRMM).");
            }
            if (rok != _param.Rok)
            {
                // parametry jiného roku = špatné sazby, raději stop (incident 2019, leden počítán se starými slevami)
                PosledniChyba = "MZD-004";
                return ErpResult<MzdovyListek>.Fail("MZD-004",
                    "Parametry mezd jsou pro rok " + _param.Rok + ", období " + obdobi + " je mimo.");
            }
            var kontrola = zam.Validate();
            if (!kontrola.Success)
            {
                PosledniChyba = kontrola.ErrorCode;
                return ErpResult<MzdovyListek>.Fail(kontrola.ErrorCode, kontrola.Message);
            }

            Trace.Log("Mzdy", "Výpočet " + zam.OsobniCislo + " " + obdobi + ", typ " + zam.TypUvazku
                              + ", hrubá " + hrubaMzda.ToString(CultureInfo.InvariantCulture));
            PosledniHruba = hrubaMzda;
            var upozorneni = new List<string>();

            var listek = new MzdovyListek();
            listek.ZamestnanecId = zam.Id;
            listek.Obdobi = obdobi;
            listek.TypUvazku = zam.TypUvazku;
            listek.HrubaMzda = hrubaMzda;

            // ------------------------------------------------------------------
            // 2. účast na pojištění
            // HPP vždy. DPP od rozhodného příjmu pro DPP, DPČ od rozhodného příjmu malého rozsahu.
            // ------------------------------------------------------------------
            bool ucastPojisteni;
            switch (zam.TypUvazku)
            {
                case TypUvazku.HPP:
                    // zaměstnání malého rozsahu u pracovního poměru neřešíme (zatím nikdo nemá, viz požadavek 4411)
                    ucastPojisteni = true;
                    break;
                case TypUvazku.DPP:
                    ucastPojisteni = hrubaMzda >= _param.RozhodnyPrijemDpp;
                    break;
                case TypUvazku.DPC:
                    ucastPojisteni = hrubaMzda >= _param.RozhodnyPrijemMalyRozsah;
                    break;
                default:
                    ucastPojisteni = true;
                    break;
            }
            if (!ucastPojisteni)
                Trace.Log("Mzdy", zam.OsobniCislo + ": bez účasti na pojištění (příjem pod rozhodnou částkou).");

            // ------------------------------------------------------------------
            // 3. sociální pojištění zaměstnance: 6,5 % důchodové + 0,6 % nemocenské
            // maximální vyměřovací základ se hlídá ročně (sestava SP-ROK), ne tady
            // ------------------------------------------------------------------
            decimal socZam = 0m;
            decimal socFirma = 0m;
            if (ucastPojisteni)
            {
                decimal vzSoc = hrubaMzda;
                socZam = CzkRounding.NahoruNaKoruny(vzSoc * (0.065m + 0.006m));
                // zaměstnavatel 24,8 % (21,5 důchodové + 2,1 nemocenské + 1,2 nezaměstnanost)
                socFirma = CzkRounding.NahoruNaKoruny(vzSoc * 0.248m);
            }
            listek.SocialniZamestnanec = socZam;
            listek.SocialniZamestnavatel = socFirma;

            // ------------------------------------------------------------------
            // 4. zdravotní pojištění 13,5 %
            // Celkové pojistné se zaokrouhlí nahoru na koruny, zaměstnanec hradí třetinu
            // (zaokrouhlenou nahoru), zbytek zaměstnavatel. NE 4,5 % z hrubé!
            // ------------------------------------------------------------------
            decimal zdrZam = 0m;
            decimal zdrFirma = 0m;
            if (ucastPojisteni)
            {
                decimal vzZdr = hrubaMzda;
                decimal zdrCelkem = CzkRounding.NahoruNaKoruny(vzZdr * 0.135m);
                zdrZam = CzkRounding.NahoruNaKoruny(zdrCelkem / 3m);
                zdrFirma = zdrCelkem - zdrZam;

                // doplatek do minimálního vyměřovacího základu (minimální mzda) platí zaměstnanec
                // jen pracovní poměr; u dohod se minimum neuplatňuje (zjednodušení, viz docs)
                if (zam.TypUvazku == TypUvazku.HPP && !zam.VyjimkaMinZdr && hrubaMzda < _param.MinimalniMzda)
                {
                    decimal rozdil = _param.MinimalniMzda - hrubaMzda;
                    decimal doplatek = CzkRounding.NahoruNaKoruny(rozdil * 0.135m);
                    zdrZam = zdrZam + doplatek;
                    upozorneni.Add("Doplatek zdravotního pojištění do minimálního vyměřovacího základu "
                                   + doplatek.ToString("N0", CultureInfo.GetCultureInfo("cs-CZ")) + " Kč.");
                }
            }
            listek.ZdravotniZamestnanec = zdrZam;
            listek.ZdravotniZamestnavatel = zdrFirma;

            // ------------------------------------------------------------------
            // 5. daň
            // Od 2021 bez superhrubé mzdy: základ = hrubá mzda.
            // ------------------------------------------------------------------
            decimal taxBase = hrubaMzda;
            decimal danPredSlevami = 0m;
            decimal slevaPopl = 0m;
            decimal slevaDeti = 0m;
            decimal bonus = 0m;
            decimal zaloha = 0m;
            decimal srazkova = 0m;

            // srážková daň: bez prohlášení a příjem pod rozhodnou částkou (DPP: rozhodný příjem DPP,
            // ostatní: rozhodný příjem malého rozsahu)
            bool srazkovaDan = false;
            if (!zam.Prohlaseni)
            {
                if (zam.TypUvazku == TypUvazku.DPP)
                    srazkovaDan = hrubaMzda < _param.RozhodnyPrijemDpp;
                else
                    srazkovaDan = hrubaMzda < _param.RozhodnyPrijemMalyRozsah;
            }

            if (srazkovaDan)
            {
                // základ i daň na celé koruny dolů
                taxBase = CzkRounding.DoluNaKoruny(hrubaMzda);
                srazkova = CzkRounding.DoluNaKoruny(taxBase * 0.15m);
                Trace.Log("Mzdy", zam.OsobniCislo + ": srážková daň " + srazkova.ToString(CultureInfo.InvariantCulture));
            }
            else
            {
                // zaokrouhlení základu zálohy
                if (taxBase > StareKonstanty.HRANICE_ZAOKR_ZAKLADU)
                    taxBase = CzkRounding.NahoruNaStovky(taxBase);
                else
                    taxBase = CzkRounding.NahoruNaKoruny(taxBase);

                decimal hranice23 = StareKonstanty.NASOBEK_PM_SAZBA_23 * _param.PrumernaMzda;
                if (taxBase > hranice23)
                {
                    danPredSlevami = hranice23 * 0.15m + (taxBase - hranice23) * 0.23m;
                    upozorneni.Add("Část základu nad " + hranice23.ToString("N0", CultureInfo.GetCultureInfo("cs-CZ"))
                                   + " Kč zdaněna sazbou 23 %.");
                }
                else
                {
                    danPredSlevami = taxBase * 0.15m;
                }
                danPredSlevami = CzkRounding.NahoruNaKoruny(danPredSlevami);

                decimal dan = danPredSlevami;

                // slevy jen s podepsaným prohlášením
                if (zam.Prohlaseni)
                {
                    // sleva na poplatníka, max do výše daně
                    slevaPopl = Math.Min(_param.SlevaNaPoplatnika, dan);
                    dan = dan - slevaPopl;

                    // daňové zvýhodnění na děti: sleva do výše zbylé daně, zbytek bonus
                    if (zam.PocetDeti > 0)
                    {
                        decimal zvyhodneni = 0m;
                        for (int i = 1; i <= zam.PocetDeti; i++)
                            zvyhodneni += _param.ZvyhodneniNaDite(i);

                        slevaDeti = Math.Min(zvyhodneni, dan);
                        dan = dan - slevaDeti;

                        decimal zbytek = zvyhodneni - slevaDeti;
                        if (zbytek > 0m)
                        {
                            if (hrubaMzda < StareKonstanty.MIN_MZDA_POLOVINA)
                            {
                                upozorneni.Add("Daňový bonus nevyplacen: příjem pod polovinou minimální mzdy.");
                            }
                            else if (zbytek < StareKonstanty.BONUS_MIN_MESIC)
                            {
                                upozorneni.Add("Daňový bonus nevyplacen: méně než "
                                               + StareKonstanty.BONUS_MIN_MESIC.ToString("N0", CultureInfo.InvariantCulture)
                                               + " Kč.");
                            }
                            else
                            {
                                bonus = zbytek;
                            }
                        }
                    }
                }
                else if (zam.PocetDeti > 0)
                {
                    upozorneni.Add("Zaměstnanec nemá prohlášení, zvýhodnění na děti nelze uplatnit.");
                }

                zaloha = dan;
            }

            // 2020 a dříve: superhrubá mzda, nechat pro přepočty starých období (nepoužívá se)
            // decimal superHruba = hrubaMzda + socFirma + zdrFirma;
            // taxBase = CzkRounding.NahoruNaStovky(superHruba);

            listek.ZakladDane = taxBase;
            listek.DanPredSlevami = danPredSlevami;
            listek.SlevaPoplatnik = slevaPopl;
            listek.SlevaNaDeti = slevaDeti;
            listek.DanovyBonus = bonus;
            listek.ZalohaDane = zaloha;
            listek.SrazkovaDan = srazkova;

            // ------------------------------------------------------------------
            // 6. čistá mzda
            // ------------------------------------------------------------------
            decimal netWage = hrubaMzda - socZam - zdrZam - zaloha - srazkova + bonus;
            if (netWage < 0)
            {
                // může nastat jen u doplatku zdravotního u velmi nízké mzdy, srážka se převede do dalšího měsíce
                upozorneni.Add("Čistá mzda vychází záporně, rozdíl se převede do dalšího období.");
            }
            listek.CistaMzda = netWage;

            // kontrolní součet (hlídá, že někdo nezapomněl přičíst nový řádek)
            decimal kontrolniSoucet = listek.CistaMzda + listek.SocialniZamestnanec + listek.ZdravotniZamestnanec
                                      + listek.ZalohaDane + listek.SrazkovaDan - listek.DanovyBonus;
            if (kontrolniSoucet != hrubaMzda)
            {
                Trace.Log(TraceLevel.Error, "Mzdy", "Nesedí kontrolní součet " + zam.OsobniCislo + ": "
                                                     + kontrolniSoucet.ToString(CultureInfo.InvariantCulture));
                PosledniChyba = "MZD-099";
                return ErpResult<MzdovyListek>.Fail("MZD-099", "Interní chyba výpočtu (kontrolní součet).");
            }

            Trace.Log("Mzdy", zam.OsobniCislo + ": čistá " + netWage.ToString(CultureInfo.InvariantCulture));

            var vysledek = ErpResult<MzdovyListek>.Ok(listek);
            vysledek.Warnings.AddRange(upozorneni);
            PosledniUpozorneni.AddRange(upozorneni);
            return vysledek;
        }

        /// <summary>
        /// Zkratka pro formulář: spočítá a vrátí jen čistou mzdu, při chybě 0.
        /// </summary>
        public decimal SpocitejCistou(Zamestnanec zam, decimal hrubaMzda, int obdobi)
        {
            var r = Vypocitej(zam, hrubaMzda, obdobi);
            return r.Success ? r.Value.CistaMzda : 0m;
        }
    }
}
