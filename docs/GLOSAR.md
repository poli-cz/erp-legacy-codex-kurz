# Glosář

Doména je česká, identifikátory jsou česky, místy anglicky (podle toho, kdo kdy kód psal).

| Pojem | V kódu | Poznámka |
|---|---|---|
| hrubá mzda | `hrubaMzda`, `HrubaMzda`, `MZD_HRUBA` | měsíční příjem ze závislé činnosti před odvody |
| čistá mzda | `CistaMzda`, `netWage`, `MZD_CISTA` | k výplatě |
| zaměstnanec | `Zamestnanec`, `zam`, tabulka `MZD_ZAMESTNANEC` | |
| osobní číslo | `OsobniCislo`, `ZAM_OSOBNI_CISLO` | identifikátor v mzdách |
| pracovní poměr | `TypUvazku.HPP` | hlavní i vedlejší pracovní poměr |
| dohoda o provedení práce | `TypUvazku.DPP` | |
| dohoda o pracovní činnosti | `TypUvazku.DPC` | v textech „DPČ“, v kódu bez diakritiky |
| období | `obdobi`, `Obdobi`, `*_OBDOBI` | `int` ve tvaru RRRRMM, např. 202603 |
| vyměřovací základ (VZ) | `vzSoc`, `vzZdr` | základ pro pojistné |
| sociální pojištění | `SocialniZamestnanec`, `socZam`, `MZD_SOC_ZAM` | důchodové + nemocenské |
| zdravotní pojištění | `ZdravotniZamestnanec`, `zdrZam`, `MZD_ZDR_ZAM` | |
| doplatek do minima | `doplatek` | zdravotní pojištění z rozdílu do minimálního VZ |
| rozhodný příjem | `RozhodnyPrijemDpp`, `RozhodnyPrijemMalyRozsah` | od této částky je dohoda účastna pojištění |
| prohlášení poplatníka | `Prohlaseni`, `ZAM_PROHLASENI` | podepsané prohlášení k dani, podmínka slev |
| základ daně | `taxBase`, `ZakladDane` | zaokrouhlený základ zálohy |
| záloha na daň | `ZalohaDane`, `zaloha`, `MZD_ZALOHA` | |
| srážková daň | `SrazkovaDan`, `MZD_SRAZKOVA` | zvláštní sazba, bez slev |
| sleva na poplatníka | `SlevaPoplatnik`, `SlevaNaPoplatnika` | |
| daňové zvýhodnění na dítě | `ZvyhodneniNaDite`, `SlevaNaDeti` | sleva + případně bonus |
| daňový bonus | `DanovyBonus`, `bonus` | část zvýhodnění nad daní, vyplácí se |
| uzávěrka mezd | `MzdyZpracovani.ZpracujObdobi` | výpočet všech zaměstnanců za období |
| výplatní páska | `VyplatniPaska` | |
| doklad | `Doklad`, `DPH_DOKLAD` | daňový doklad, faktura |
| faktura vydaná / přijatá | `DruhDokladu.FV` / `DruhDokladu.FP` | |
| DUZP | `Duzp` | datum uskutečnění zdanitelného plnění |
| základ (DPH) | `Zaklad`, `RAD_ZAKLAD` | částka bez DPH |
| sazba DPH | `Sazba`, `RAD_SAZBA` | v procentech: 21, 12, 0 |
| rekapitulace DPH | `DphRekapitulace`, `RekapitulaceRadek` | součty za sazbu |
| kontrolní hlášení (KH) | `KontrolniHlaseniRozdelovac`, `KhPodklady`, `usp_KH_Podklady` | |
| oddíl KH | `KhOddil.A4`, `A5`, `B2`, `B3` | A = výstup (prodej), B = vstup (nákup) |
| DIČ partnera | `DicPartnera`, `DOK_DIC_PARTNERA` | prázdné = neplátce |
| business objekt (BO) | `BusinessObject` | řádek tabulky v paměti |
| výsledek operace | `ErpResult`, `ErpResult<T>` | místo výjimek |
