# ERP legacy: mzdy a DPH v malém

Vzorový repozitář ke kurzu OpenAI Codex a agentní vývoj (ICT Pro), produktový den:
**jak nasadit coding agenta na velký a starý systém**. Je to vymyšlený výřez z ERP
(mzdy 2026, rekapitulace DPH, kontrolní hlášení), napsaný schválně jako legacy kód:
vlastní framework, service locator, dlouhé metody, SQL v řetězcích, logika v uložených
procedurách, jeden soubor ve Windows-1250. Žádný firemní kód ani skutečná data, smíte ho dát
jakémukoli agentovi.

- `src/Erp.Framework` – in-house framework (BusinessObject, ServiceRegistry, ErpResult, CzkRounding, Trace)
- `src/Erp.Payroll` – výpočet čisté mzdy (`MzdaKalkulace`), výplatní páska
- `src/Erp.Vat` – rekapitulace DPH, oddíly kontrolního hlášení
- `src/Erp.Data` – přístup k databázi přes `IDbGateway`, repozitáře
- `db/` – T-SQL skripty a uložené procedury (pro čtení, testy databázi nepotřebují)
- `legacy-winforms/` – WinForms aplikace, jen Windows, mimo hlavní řešení
- `tests/Erp.Tests` – pár testů (xUnit), `golden/mzdy-2026.csv` – výstupy starého systému
- `docs/` – architektura, glosář, parametry 2026 se zdroji, kódování souborů

## Začátek (Windows)

```powershell
winget install Microsoft.DotNet.SDK.10      # jen pokud dotnet --version nehlásí 10.x
git clone https://github.com/poli-cz/erp-legacy-codex-kurz
cd erp-legacy-codex-kurz
dotnet --info                               # .NET SDK 10.x
dotnet test                                 # 9 testů, všechny zelené
codex                                       # Codex spouštějte v adresáři repozitáře
```

WinForms aplikace: `dotnet run --project legacy-winforms/Erp.WinForms` nebo otevřít
`legacy-winforms/Erp.Windows.slnx` ve Visual Studiu.

## Začátek (macOS, Linux)

.NET 10 SDK z https://dotnet.microsoft.com/download, pak stejně: `git clone …`, `dotnet test`.
Hlavní řešení `Erp.slnx` je multiplatformní. `legacy-winforms` jde na macOS a Linuxu přeložit,
ale ne spustit.

## Mezi pokusy

```
git restore .
git clean -fd
```

## Čím se liší od produkce

Skutečný systém má miliony řádků, stovky procedur, DevExpress a vlastní testovací nástroj.
Tady je zhruba 3 tisíce řádků, aby se agent i člověk zorientovali za pár minut. Pravidla mezd
a DPH jsou zjednodušená (`docs/PARAMETRY-2026.md`), ale zvyklosti, pasti a duplicity jsou
takové, jaké ve starém ERP bývají. Nejde o daňové poradenství.

## Týmy na TFVC (TFS bez Gitu)

Codex hledá kořen projektu (odkud načítá `AGENTS.md` a projektový `.codex/config.toml`) podle
souborů v `project_root_markers`, výchozí hodnota je `[".git"]`. Pracovní kopie TFVC `.git` nemá,
proto do `~/.codex/config.toml` přidejte značku, která v kořeni řešení vždy je:

```toml
project_root_markers = [".git", "Erp.slnx"]
```

Klíč je v konfiguraci Codex CLI 0.160; v jiné verzi ho ověřte v dokumentaci konfigurace.
Funkce postavené na Gitu (`/diff`, `codex review`, vracení změn) bez Gitu nefungují. Pomůže
lokální `git init` nad pracovní kopií jen pro vlastní potřebu (do `.gitignore` přidat `$tf/`),
změny se dál odevzdávají do TFVC jako dosud.

Úlohy pro cvičení: [ULOHY.md](ULOHY.md).
