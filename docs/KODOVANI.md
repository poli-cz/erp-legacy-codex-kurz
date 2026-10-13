# Kódování souborů a konce řádků

## Rozhodnutí

- Zdrojové soubory `.cs`, `.csproj`, `.props` a `.sql` jsou **UTF-8 s BOM** (`.editorconfig`: `charset = utf-8-bom`).
- Ostatní textové soubory (`.md`, `.csv`, `.toml`, `.slnx`) jsou UTF-8 bez BOM.
- **Výjimka: `src/Erp.Payroll/StareKonstanty.cs` je ve Windows-1250 s CRLF** a tak musí zůstat.
- Konce řádků: v repozitáři LF (`.gitattributes`: `* text=auto`), na Windows je Git převede na CRLF.
  `StareKonstanty.cs` má `-text`, Git na něm nic nepřevádí.

## Proč UTF-8 s BOM

Projekt `Erp.Payroll` má v `.csproj` nastaveno `<CodePage>1250</CodePage>`, aby kompilátor četl
`StareKonstanty.cs` správně. Toto nastavení platí pro **všechny soubory projektu bez BOM**. Soubor
v UTF-8 bez BOM s českým textem v řetězci by kompilátor přečetl jako Windows-1250 a v programu by
byly rozsypané znaky. BOM říká kompilátoru „tohle je UTF-8“ a má přednost před `CodePage`.
Druhý důvod: Visual Studio na Windows ukládá C# soubory s BOM a týmy na Windows to tak mají.

## Proč ten jeden soubor zůstává ve Windows-1250

Texty z `StareKonstanty` se tisknou na výplatní pásky a historicky jdou do sestav ve Windows-1250.
Převod souboru do UTF-8 v minulosti rozbil tiskové výstupy. Soubor proto zůstává, jak je.

## Jak soubor upravit, aby diff obsahoval jen změněné řádky

- Editor musí soubor otevřít i uložit jako Windows-1250 (VS Code: „Reopen with Encoding“ a
  „Save with Encoding“ → Central European (Windows 1250)) a zachovat CRLF.
- Kontrola: `git diff --stat` ukazuje jen změněné řádky a `KodovaniTests` projde.
- Na macOS a Linuxu: `file src/Erp.Payroll/StareKonstanty.cs` má hlásit „with CRLF line terminators“
  a soubor se nesmí dát přečíst jako UTF-8 (`iconv -f UTF-8 -t UTF-8` selže).
