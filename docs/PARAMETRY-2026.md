# Parametry mezd a DPH pro rok 2026

Ověřeno 13. 10. 2026 na oficiálních zdrojích (zákony v aktuálním znění na zakonyprolidi.cz,
weby ČSSZ, Finanční správy, VZP a MPSV). Sloupec „V kódu“ říká, kde parametr v kódu žije.
Pravidla jsou pro výukové účely zjednodušená (viz konec souboru). Nejde o daňové poradenství.

## Pojistné

| Parametr | Hodnota 2026 | V kódu | Zdroj |
|---|---|---|---|
| Průměrná mzda (pro pojistné a daně) | 48 967 Kč | `ParametryMezd2026.PrumernaMzda`, `SYS_PARAMETR` | [ČSSZ, přehled 2026](https://www.cssz.gov.cz/-/prehled-nejdulezitejsich-udaju-pro-socialni-zabezpeceni-v-roce-2026), [Finanční správa](https://financnisprava.gov.cz/cs/dane/dane/dan-z-prijmu/zamestnanci-zamestnavatele/obecne-informace) |
| Maximální vyměřovací základ sociálního pojištění (ročně) | 2 350 416 Kč (48 × průměrná mzda) | neřeší se (hlídá roční sestava) | [ČSSZ](https://www.cssz.gov.cz/maximalni-vymerovaci-zaklad) |
| Sociální pojištění zaměstnance | 7,1 % (důchodové 6,5 + nemocenské 0,6) | `MzdaKalkulace`, `SYS_PARAMETR` | [ČSSZ, výše a sazba](https://www.cssz.gov.cz/web/cz/vyse-a-sazba) |
| Sociální pojištění zaměstnavatele | 24,8 % | `MzdaKalkulace` | tamtéž |
| Zaokrouhlení sociálního pojištění | na celé Kč nahoru | `CzkRounding.NahoruNaKoruny` | [ČSSZ, výpočet](https://www.cssz.gov.cz/web/cz/vypocet-pojistneho) |
| Zdravotní pojištění celkem | 13,5 % z VZ, za každého zaměstnance zvlášť, na celé Kč nahoru | `MzdaKalkulace`, `SYS_PARAMETR` | zák. 592/1992 Sb., § 2 ([text](https://www.zakonyprolidi.cz/cs/1992-592)), [VZP, metodika](https://www.vzp.cz/platci/informace/povinnosti-platcu-metodika/2-4-platce-pojistneho-zamestnavatel) |
| Podíl zaměstnance na zdravotním | 1/3 z celkového (už zaokrouhleného) pojistného, zaměstnavatel 2/3 | `MzdaKalkulace` | zák. 48/1997 Sb., § 9 odst. 2 ([text](https://www.zakonyprolidi.cz/cs/1997-48)), VZP tamtéž |
| Zaokrouhlení třetiny zaměstnance | na celé Kč nahoru, zbytek do celku platí zaměstnavatel | `MzdaKalkulace` | **NEOVĚŘENO oficiálně**: zákon ani VZP neupravují, je to ustálená praxe (odborný tisk). Výukový parametr. |
| Minimální vyměřovací základ zdravotního | = minimální mzda, 22 400 Kč | `ParametryMezd2026.MinimalniMzda` | zák. 592/1992 Sb., § 3 odst. 6 |
| Doplatek do minima | zaměstnanec doplácí 13,5 % z rozdílu (přes zaměstnavatele); výjimky § 3 odst. 8, krácení při části měsíce § 3 odst. 9 | `MzdaKalkulace`, `Zamestnanec.VyjimkaMinZdr` | zák. 592/1992 Sb., § 3 odst. 8 až 10 |
| Minimální mzda | 22 400 Kč měsíčně | `ParametryMezd2026.MinimalniMzda` | [MPSV](https://mpsv.gov.cz/minimalni-mzda-od-ledna-2026-vzroste-o-1-600-korun-na-22-400-korun-mesicne) |
| Rozhodný příjem DPP | 12 000 Kč (25 % průměrné mzdy zaokrouhlené na pětisetkoruny dolů) | `ParametryMezd2026.RozhodnyPrijemDpp` | zák. 187/2006 Sb., § 7a ([text](https://www.zakonyprolidi.cz/cs/2006-187)), ČSSZ přehled 2026 |
| Rozhodný příjem zaměstnání malého rozsahu | 4 500 Kč | `ParametryMezd2026.RozhodnyPrijemMalyRozsah` | ČSSZ přehled 2026 |

## Daň ze závislé činnosti

| Parametr | Hodnota 2026 | V kódu | Zdroj |
|---|---|---|---|
| Základ zálohy | do 100 Kč na celé Kč nahoru, nad 100 Kč na celé stokoruny nahoru | `StareKonstanty.HRANICE_ZAOKR_ZAKLADU`, `CzkRounding` | zák. 586/1992 Sb., § 38h ([text](https://www.zakonyprolidi.cz/cs/1992-586)) |
| Sazba | 15 %, z části základu nad 3násobek průměrné mzdy 23 % | `MzdaKalkulace`, `StareKonstanty.NASOBEK_PM_SAZBA_23` | § 38h |
| Hranice pro 23 % | 3 × 48 967 = 146 901 Kč měsíčně (ročně 36násobek, 1 762 812 Kč) | tamtéž | § 38h, Finanční správa |
| Zaokrouhlení zálohy | na celé Kč nahoru | `CzkRounding.NahoruNaKoruny` | § 38h |
| Sleva na poplatníka | 2 570 Kč měsíčně (30 840 Kč ročně) | `ParametryMezd2026.SlevaNaPoplatnika` | § 35ba odst. 1 písm. a) |
| Daňové zvýhodnění na dítě | 1. dítě 1 267 Kč, 2. dítě 1 860 Kč, 3. a další 2 320 Kč měsíčně (dítě se ZTP/P dvojnásobek, zde neřešeno) | `ParametryMezd2026.ZvyhodneniNaDite` | § 35c odst. 1 |
| Měsíční daňový bonus: minimum | 50 Kč | `StareKonstanty.BONUS_MIN_MESIC` | § 35d odst. 4 |
| Měsíční daňový bonus: podmínka příjmu | aspoň polovina minimální mzdy = 11 200 Kč u téhož plátce za měsíc | `StareKonstanty.MIN_MZDA_POLOVINA` | § 35d odst. 4 |
| Strop daňového bonusu | není | | § 35c odst. 3 |
| Srážková daň | 15 %, bez prohlášení: DPP s příjmem pod 12 000 Kč, ostatní pod 4 500 Kč; základ i daň na celé Kč dolů | `MzdaKalkulace` | § 6 odst. 4, § 36 |

## DPH

| Parametr | Hodnota 2026 | V kódu | Zdroj |
|---|---|---|---|
| Sazby DPH | základní 21 %, snížená 12 % (jediná snížená od 1. 1. 2024) | `DphSazby2024` | zák. 235/2004 Sb., § 47 odst. 1 ([text](https://www.zakonyprolidi.cz/cs/2004-235)) |
| Hranice v kontrolním hlášení | doklad **nad 10 000 Kč včetně DPH** jednotlivě (A.4, B.2); přesně 10 000 Kč a méně souhrnně (A.5, B.3) | `KontrolniHlaseniRozdelovac`, `usp_KH_Podklady`, `SYS_PARAMETR` | [Finanční správa, KH: časté dotazy](https://financnisprava.gov.cz/cs/dane/dane/dan-z-pridane-hodnoty/kontrolni-hlaseni-dph/caste-dotazy-a-odpovedi), sekce III, dotazy 2 a 6 |

## Co výřez zjednodušuje

- Jeden měsíc u jednoho zaměstnavatele, bez mzdových složek, nepřítomností a ročního zúčtování.
- Maximální vyměřovací základ sociálního pojištění se v měsíčním výpočtu nehlídá.
- Zaměstnání malého rozsahu u pracovního poměru se neřeší (HPP je vždy účastno pojištění).
- Minimální vyměřovací základ zdravotního se uplatňuje jen u pracovního poměru, bez krácení za část měsíce.
- Daňové zvýhodnění na dítě se ZTP/P, slevy na invaliditu a studenta: neřešeno.
- Zaměstnavatelské pojistné se počítá za každého zaměstnance zvlášť (ve skutečnosti se sociální
  pojištění zaměstnavatele počítá ze součtu vyměřovacích základů).
- Kontrolní hlášení: jen běžná tuzemská plnění, bez přenesené daňové povinnosti (A.1, B.1) a bez A.2.
