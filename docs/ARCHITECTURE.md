# Architektura

Výřez z velkého ERP: mzdy, DPH a kontrolní hlášení. Kód je schválně psaný jako starý systém
(přepis z Delphi v roce 2009 a od té doby jen úpravy). Tento soubor říká, **kde co je** a **co
musí platit**. Jak se co počítá, je v kódu.

## Z ptačí perspektivy

```
WinForms (legacy-winforms, jen Windows)
        │
        ▼
Erp.Payroll ──┐        Erp.Vat
  MzdaKalkulace│          DphRekapitulace, KontrolniHlaseniRozdelovac
        │      │              │
        ▼      ▼              ▼
     Erp.Framework  (BusinessObject, ServiceRegistry, ErpResult, CzkRounding, Money, Trace, ErpConfig)
        ▲
        │
     Erp.Data  (IDbGateway, repozitáře, SQL v řetězcích)  ──►  MS SQL: db/*.sql, uložené procedury
```

Moduly se registrují do statického `ServiceRegistry` (`PayrollModule.Register()`,
`VatModule.Register()`, `DataModule.Register(gateway)`). Kdo co potřebuje, si to vytáhne
přes `ServiceRegistry.Get<T>()`, často v bezparametrickém konstruktoru.

## Mapa kódu

| Cesta | Co tam je |
|---|---|
| `src/Erp.Framework` | In-house framework. `BusinessObject` + `[DbField]`/`[DbTable]` (mapování na sloupce), `ServiceRegistry` (service locator), `ErpResult` (business chyby bez výjimek), `CzkRounding` a `Money`, `Trace.Log`, `ErpConfig` (nastavení a přepínače). `Ui/` jsou náhrady ovládacích prvků pro generovaný kód. |
| `src/Erp.Payroll` | Mzdy. `MzdaKalkulace.Vypocitej` je celý výpočet v jedné metodě. Parametry roku: `ParametryMezd2026` (`IParametryMezd`), část konstant historicky v `StareKonstanty.cs` (Windows-1250!). `VyplatniPaska` = textový výstup. `Formulare/` = formulář s generovaným `*.Designer.cs`. |
| `src/Erp.Vat` | DPH. `DphRekapitulace` (DPH za sazbu), `KontrolniHlaseniRozdelovac` (oddíly A.4, A.5, B.2, B.3), `IDphSazby`. |
| `src/Erp.Data` | Databáze. `IDbGateway` (jediná brána k SQL), `AdoDbGateway`, `MzdyRepository`, `KhRepository`, `MzdyZpracovani` (uzávěrka období). |
| `db/` | T-SQL: tabulky, parametry, uložené procedury. Nespouští se z testů. |
| `legacy-winforms/` | WinForms aplikace (`net10.0-windows`), mimo hlavní řešení. |
| `tests/Erp.Tests` | xUnit. Pokrytí je slabé, jako v reálu. `Infrastruktura/`: fake databáze, approval helper. |
| `golden/mzdy-2026.csv` | Výstupy starého systému pro paralelní běh (vstup → výsledek). |

## Invarianty

- **Business chyba = `ErpResult.Fail(kód, text)`, ne výjimka.** Kódy `MZD-xxx`, `DPH-xxx`, `DAT-xxx`.
  Výjimka znamená chybu programu.
- **Peníze jsou `decimal`** (mzdy) nebo `Money` (DPH). Nikdy `double`. Zaokrouhlení jen přes
  `CzkRounding`, nikdy `Math.Round` bez `MidpointRounding`.
- **SQL jen přes `IDbGateway`**, parametry ve slovníku (název bez `@`). Žádný jiný kód nesahá na `DbConnection`.
- **Doménové třídy nevolají databázi.** `Erp.Payroll` a `Erp.Vat` nereferencují `Erp.Data`.
- **Kontrolní součet mzdy:** čistá + pojistné zaměstnance + záloha + srážková − bonus = hrubá.
- **Logování přes `Erp.Framework.Trace.Log`**, ne `Console` ani `System.Diagnostics.Trace`.
- **`*.Designer.cs` je generovaný**, ručně se neupravuje.
- **Kódování:** `.cs` jsou UTF-8 s BOM, výjimka `StareKonstanty.cs` (Windows-1250, CRLF). Důvod v `KODOVANI.md`.

## Hranice a duplicity (pozor)

- Část pravidel je **dvakrát**: v C# a v uložené proceduře (`usp_Mzdy_Vypocet_Uloz` si počítá
  zdravotní pojištění sama, `usp_KH_Podklady` rozděluje KH pro tiskovou sestavu). Parametry jsou
  v `ParametryMezd2026`, v `StareKonstanty` i v tabulce `SYS_PARAMETR` (`db/002_parametry_2026.sql`).
  Kdo mění pravidlo, musí najít všechna místa.
- `ServiceRegistry` je statický a sdílený celým procesem. Testy proto běží v jedné kolekci bez paralelizace.

## Jak se liší od produkce

Výřez nemá mzdové složky, nepřítomnosti, roční zúčtování ani exporty pro úřady. Reálný systém
má miliony řádků, komerční knihovnu ovládacích prvků, vlastní testovací nástroj a stovky
procedur. Pravidla mezd a DPH jsou zjednodušená (viz `PARAMETRY-2026.md`), struktura a zvyklosti ale odpovídají běžnému legacy ERP.
