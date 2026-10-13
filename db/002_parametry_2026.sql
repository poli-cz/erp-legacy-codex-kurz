-- Parametry mezd 2026 pro procedury. C# má vlastní kopii v ParametryMezd2026 a StareKonstanty.
-- Při změně parametru upravit OBĚ místa.

DELETE FROM dbo.SYS_PARAMETR WHERE PAR_ROK = 2026;
INSERT INTO dbo.SYS_PARAMETR (PAR_ROK, PAR_KOD, PAR_HODNOTA) VALUES
    (2026, 'PRUMERNA_MZDA',      48967),
    (2026, 'MINIMALNI_MZDA',     22400),
    (2026, 'SLEVA_POPLATNIK',     2570),
    (2026, 'SAZBA_SOC_ZAM',      0.071),
    (2026, 'SAZBA_ZDR_ZAM',      0.045),
    (2026, 'SAZBA_ZDR_CELKEM',   0.135),
    (2026, 'KH_LIMIT',           10000);
GO
