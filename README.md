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
winget install Microsoft.DotNet.SDK.10      # jen pokud dotnet --version nehlásí 10.x (chce admina)
git clone https://github.com/poli-cz/erp-legacy-codex-kurz
cd erp-legacy-codex-kurz
dotnet --info                               # .NET SDK 10.x
dotnet test                                 # 9 testů, všechny zelené
codex                                       # Codex spouštějte v adresáři repozitáře
```

Bez práv administrátora nainstalujte .NET 10 SDK do profilu uživatele skriptem `dotnet-install.ps1`
(místo `winget`):

```powershell
& ([scriptblock]::Create((irm https://dot.net/v1/dotnet-install.ps1))) -Channel 10.0 -InstallDir "$env:USERPROFILE\.dotnet"
$env:PATH = "$env:USERPROFILE\.dotnet;$env:PATH"
dotnet --version
```

`PATH` takto platí jen v aktuálním okně; natrvalo ho přidejte v nastavení proměnných prostředí
uživatele.

Visual Studio 2022 projekty .NET 10 neotevře, pracujte z příkazové řádky nebo ve VS 2026.

### Před prvním spuštěním agenta

Před prvním spuštěním agenta spusťte jednou `dotnet test` sami (restore z NuGetu). V sandboxu
workspace-write je síť vypnutá. Pro tohle cvičné repo můžete použít profil
s `[sandbox_workspace_write] network_access = true`, na vlastním kódu restore dělejte před
spuštěním agenta.

## Začátek (macOS, Linux)

.NET 10 SDK z https://dotnet.microsoft.com/download, pak stejně: `git clone …`, `dotnet test`.
Hlavní řešení `Erp.slnx` je multiplatformní. `legacy-winforms` jde na macOS a Linuxu přeložit,
ale ne spustit.

## C# 7.3 v `src`

`src/Directory.Build.props` nastavuje pro produkční kód `LangVersion` 7.3, jako na .NET Frameworku 4.7.
Records, switch výrazy nebo file-scoped namespace v `src` jsou chyba buildu. Testy v `tests/` píšete
v moderním C#.

## legacy-winforms

`legacy-winforms/` je jeden formulář (`HlavniForm`) jako vzor pro vytažení logiky do presenteru:
obsluha tlačítka sama skládá vstupy, volá výpočet a formátuje výsledek; cílem je nechat ve
formuláři jen ovládací prvky a vazbu na presenter. Aplikace jde spustit
jen na Windows: `dotnet run --project legacy-winforms/Erp.WinForms`. Do hlavního řešení `Erp.slnx`
nepatří, má vlastní `legacy-winforms/Erp.Windows.slnx`.

## Pro týmy na webu a .NET Frameworku

WinForms část ignorujte. `src` je schválně C# 7.3. Doménové pasti (zaokrouhlení, pravidlo v C#
i v proceduře, Windows-1250, parametry na více místech) jsou stejné.

## Mezi pokusy

Mezi pokusy: `git stash -u` nebo commit do vlastní větve; `git clean -fd` smaže i vaše poznámky
(ZJISTENI.md, MAPA.md).

```powershell
git stash -u                      # odložit pokus i s novými soubory
git switch -c pokus-b             # nebo pokus uložit do vlastní větve a commitnout
```

## Čím se liší od produkce

Skutečný systém má miliony řádků, stovky procedur, komerční knihovnu ovládacích prvků a vlastní
testovací nástroj. Tady je zhruba 3 tisíce řádků, aby se agent i člověk zorientovali za pár minut.
Pravidla mezd a DPH jsou zjednodušená (`docs/PARAMETRY-2026.md`), ale zvyklosti, pasti a duplicity jsou
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
