-- Uloží výpočet mzdy jednoho zaměstnance za období. Volá MzdyRepository.UlozVypocet.
-- Předchozí výpočet téhož zaměstnance a období smaže (přepočet).
-- Zdravotní pojištění si procedura počítá sama, protože stejnou proceduru volá import
-- mezd z externího zpracovatele (XLS), který zdravotní neposílá. (2014, M. K.)

CREATE OR ALTER PROCEDURE dbo.usp_Mzdy_Vypocet_Uloz
    @ZamId       INT,
    @Obdobi      INT,
    @Hruba       DECIMAL(12,2),
    @SocZam      DECIMAL(12,2),
    @SocFirma    DECIMAL(12,2),
    @ZakladDane  DECIMAL(12,2),
    @Zaloha      DECIMAL(12,2),
    @Srazkova    DECIMAL(12,2),
    @Bonus       DECIMAL(12,2),
    @Cista       DECIMAL(12,2)
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Rok INT = @Obdobi / 100;
    DECLARE @SazbaZdrZam DECIMAL(12,4) =
        (SELECT PAR_HODNOTA FROM dbo.SYS_PARAMETR WHERE PAR_ROK = @Rok AND PAR_KOD = 'SAZBA_ZDR_ZAM');
    DECLARE @SazbaZdrCelkem DECIMAL(12,4) =
        (SELECT PAR_HODNOTA FROM dbo.SYS_PARAMETR WHERE PAR_ROK = @Rok AND PAR_KOD = 'SAZBA_ZDR_CELKEM');

    IF @SazbaZdrZam IS NULL OR @SazbaZdrCelkem IS NULL
    BEGIN
        RAISERROR('Chybí parametry zdravotního pojištění pro rok %d.', 16, 1, @Rok);
        RETURN;
    END

    -- zdravotní pojištění zaměstnance a firmy
    DECLARE @ZdrZam   DECIMAL(12,2) = CEILING(@Hruba * @SazbaZdrZam);
    DECLARE @ZdrFirma DECIMAL(12,2) = CEILING(@Hruba * @SazbaZdrCelkem) - @ZdrZam;

    DECLARE @TypUvazku VARCHAR(3) = (SELECT ZAM_TYP_UVAZKU FROM dbo.MZD_ZAMESTNANEC WHERE ZAM_ID = @ZamId);

    -- DPP a DPČ bez účasti na pojištění: C# poslal SocZam = 0, zdravotní taky nulujeme
    IF @SocZam = 0 AND @TypUvazku IN ('DPP', 'DPC')
    BEGIN
        SET @ZdrZam = 0;
        SET @ZdrFirma = 0;
    END

    BEGIN TRAN;

    DELETE FROM dbo.MZD_VYPOCET WHERE MZD_ZAM_ID = @ZamId AND MZD_OBDOBI = @Obdobi;

    INSERT INTO dbo.MZD_VYPOCET (
        MZD_ZAM_ID, MZD_OBDOBI, MZD_TYP_UVAZKU, MZD_HRUBA, MZD_SOC_ZAM, MZD_ZDR_ZAM, MZD_SOC_FIRMA, MZD_ZDR_FIRMA,
        MZD_ZAKLAD_DANE, MZD_BONUS, MZD_ZALOHA, MZD_SRAZKOVA, MZD_CISTA)
    VALUES (
        @ZamId, @Obdobi, @TypUvazku, @Hruba, @SocZam, @ZdrZam, @SocFirma, @ZdrFirma,
        @ZakladDane, @Bonus, @Zaloha, @Srazkova, @Cista);

    DECLARE @NoveId INT = SCOPE_IDENTITY();

    COMMIT;

    SELECT @NoveId AS MZD_ID;
END
GO
