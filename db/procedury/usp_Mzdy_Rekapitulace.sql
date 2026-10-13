-- Rekapitulace mezd za období (sestava pro účetní a podklad pro platby pojistného a daně).

CREATE OR ALTER PROCEDURE dbo.usp_Mzdy_Rekapitulace
    @Obdobi INT
AS
BEGIN
    SET NOCOUNT ON;

    SELECT
        v.MZD_TYP_UVAZKU                             AS TYP_UVAZKU,
        COUNT(*)                                     AS POCET,
        SUM(v.MZD_HRUBA)                             AS HRUBA,
        SUM(v.MZD_SOC_ZAM)                           AS SOC_ZAM,
        SUM(v.MZD_SOC_FIRMA)                         AS SOC_FIRMA,
        SUM(v.MZD_ZDR_ZAM)                           AS ZDR_ZAM,
        SUM(v.MZD_ZDR_FIRMA)                         AS ZDR_FIRMA,
        SUM(v.MZD_ZALOHA)                            AS ZALOHY,
        SUM(v.MZD_SRAZKOVA)                          AS SRAZKOVA,
        SUM(v.MZD_BONUS)                             AS BONUSY,
        SUM(v.MZD_CISTA)                             AS CISTA,
        -- odvod na FÚ: zálohy minus vyplacené bonusy (bonus si plátce odečte z odvodu)
        SUM(v.MZD_ZALOHA) - SUM(v.MZD_BONUS)         AS ODVOD_ZALOHY_FU
    FROM dbo.MZD_VYPOCET v
    WHERE v.MZD_OBDOBI = @Obdobi
    GROUP BY v.MZD_TYP_UVAZKU
    ORDER BY v.MZD_TYP_UVAZKU;
END
GO
