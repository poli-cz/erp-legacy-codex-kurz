-- Podklady kontrolního hlášení DPH za období: ke každému dokladu oddíl A4, A5, B2 nebo B3.
-- Používá tisková sestava KH (Reporting Services). Obrazovka KH počítá totéž v C#
-- (KontrolniHlaseniRozdelovac). DOK_CELKEM_S_DPH plní trigger TR_DPH_DOKLAD_RADEK_SUMA.

CREATE OR ALTER PROCEDURE dbo.usp_KH_Podklady
    @Obdobi INT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Limit DECIMAL(12,2) =
        ISNULL((SELECT PAR_HODNOTA FROM dbo.SYS_PARAMETR WHERE PAR_ROK = @Obdobi / 100 AND PAR_KOD = 'KH_LIMIT'), 10000);

    SELECT
        d.DOK_ID,
        d.DOK_CISLO,
        d.DOK_DRUH,
        d.DOK_DIC_PARTNERA,
        d.DOK_CELKEM_S_DPH,
        CASE
            WHEN d.DOK_DRUH = 'FV' AND ISNULL(d.DOK_DIC_PARTNERA, '') <> '' AND d.DOK_CELKEM_S_DPH >= @Limit THEN 'A4'
            WHEN d.DOK_DRUH = 'FV' THEN 'A5'
            WHEN d.DOK_DRUH = 'FP' AND ISNULL(d.DOK_DIC_PARTNERA, '') <> '' AND d.DOK_CELKEM_S_DPH >= @Limit THEN 'B2'
            WHEN d.DOK_DRUH = 'FP' AND ISNULL(d.DOK_DIC_PARTNERA, '') <> '' THEN 'B3'
            ELSE 'Nezahrnovat'
        END AS ODDIL
    FROM dbo.DPH_DOKLAD d
    WHERE d.DOK_OBDOBI = @Obdobi
      AND d.DOK_STORNO = 0
    ORDER BY d.DOK_DRUH, d.DOK_CISLO;
END
GO
