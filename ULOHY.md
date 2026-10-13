# Úlohy

Build a testy: `dotnet build`, `dotnet test` (v kořeni repozitáře, řešení `Erp.slnx`).
Mezi pokusy: `git restore .` a `git clean -fd`. Každou úlohu začněte v novém vlákně.

## A. Mapa repozitáře a AGENTS.md

Nechte agenta zmapovat repozitář a napište hierarchii AGENTS.md: jeden v kořeni a aspoň jeden
v podadresáři, kde platí jiná pravidla než ve zbytku repa.

Hotovo, když:
- kořenový AGENTS.md má nejvýš 120 řádků a obsahuje příkazy, které se tu opravdu spouštějí,
- podadresářový AGENTS.md obsahuje jen to, co platí navíc pro ten podadresář,
- v novém vlákně položíte agentovi tři otázky o konvencích repa, které jste si připravili předem,
  a odpoví správně bez toho, aby hledal v celém kódu.

## B. Charakterizační testy výpočtu mzdy

`golden/mzdy-2026.csv` jsou výsledky starého systému. Zakonzervujte chování `MzdaKalkulace` testy.

Hotovo, když:
- každý řádek CSV je jeden testovací případ a porovnávají se všechny výstupní sloupce,
- testy procházejí proti současnému kódu a v `src/` se nic nezměnilo,
- `dotnet test` je zelený.

## C. Strangler: nový výpočet vedle starého

Vytvořte nový výpočet čisté mzdy `NetWageCalculator` za rozhraním. Přepínač v konfiguraci určí,
který výpočet používá uzávěrka mezd. Paralelní běh spočítá oba výpočty a hlásí rozdíly.

Hotovo, když:
- `MzdaKalkulace` je beze změny a ve výchozím stavu přepínače se používá,
- paralelní běh nad `golden/mzdy-2026.csv` vypíše řádky, ve kterých se výpočty liší, a v čem,
- ke každému rozdílu umíte říct, jestli je chyba ve starém, nebo v novém výpočtu, a doložit to zdrojem,
- `dotnet test` je zelený.

## D. Oprava pravidla testem napřed

Hlášení od účetní: „Faktura vydaná plátci na 9 000 Kč bez DPH (10 890 Kč s DPH) se na obrazovce
kontrolního hlášení objevila v oddílu A.5. Čekala jsem ji v A.4.“

Hotovo, když:
- nejdřív existuje test, který chybu reprodukuje a padá (samostatný commit),
- pak oprava, po které test projde, a testy na hranici pravidla,
- víte, jestli stejné pravidlo není v repozitáři implementované ještě jinde, a co s tím,
- `dotnet test` je zelený.

## E. Změna uložené procedury (expand–contract)

`usp_Mzdy_Vypocet_Uloz` má zdravotní pojištění zaměstnance převzít z aplikace místo vlastního
výpočtu. Proveďte změnu metodou expand–contract.

Hotovo, když:
- skript kroku expand je zpětně kompatibilní: volání procedury beze změny funguje dál,
- aplikace posílá novou hodnotu a test s fake databází to ověřuje,
- krok contract je připravený jako samostatný skript, který se zatím nespouští, a je popsané, kdy ho spustit,
- `dotnet test` je zelený.

## F. Soubor ve Windows-1250

Na výplatní pásce se má místo „Sleva na děti“ tisknout „Daňové zvýhodnění na děti“.

Hotovo, když:
- `git diff` ukazuje jen řádky, které se měnily, ne celé soubory,
- všechny dotčené soubory mají po změně stejné kódování a konce řádků jako předtím,
- `dotnet test` je zelený.

## G. Revize změny

Vezměte svou změnu z C, D nebo E a nechte ji zrevidovat v novém vlákně (`/review`, nebo
`codex review --base main` či `--uncommitted`).

Hotovo, když:
- máte seznam nálezů a u každého rozhodnutí: opravit, nebo ne a proč,
- aspoň jeden nález jste ověřili testem nebo spuštěním, ne jen čtením.

## H. Mikado: pryč se ServiceRegistry

Odstraňte z modulu `Erp.Vat` závislost na statickém `ServiceRegistry` metodou Mikado.

Hotovo, když:
- v `src/Erp.Vat` není žádné použití `ServiceRegistry`,
- chování je beze změny a `dotnet test` je zelený,
- máte Mikado graf (cíl a předpoklady) a každý list grafu je samostatný commit se zelenými testy.
