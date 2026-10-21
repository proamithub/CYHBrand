-- ============================================================
-- CYHBrand Database – Tables + Stored Procedures
-- Server  : VRS-AMITPC\SQLEXPRESS
-- Database: CYHBrandDb
-- ============================================================

USE CYHBrandDb;
GO

-- ============================================================
-- TABLES
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'RoleMaster')
BEGIN
    CREATE TABLE RoleMaster (
        RoleId   INT IDENTITY(1,1) PRIMARY KEY,
        RoleName NVARCHAR(50)  NOT NULL,
        IsActive BIT           NOT NULL DEFAULT 1
    );
    INSERT INTO RoleMaster (RoleName) VALUES ('Doctor'), ('Admin'), ('SuperAdmin');
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'DoctorMaster')
BEGIN
    CREATE TABLE DoctorMaster (
        DoctorId           INT IDENTITY(1,1) PRIMARY KEY,
        FullName           NVARCHAR(150)  NULL,
        DisplayName        NVARCHAR(150)  NULL,
        Email              NVARCHAR(200)  NOT NULL UNIQUE,
        Password           NVARCHAR(500)  NOT NULL,
        MobileNo           NVARCHAR(20)   NULL,
        Specialty          NVARCHAR(100)  NULL,
        Qualifications     NVARCHAR(300)  NULL,
        ClinicName         NVARCHAR(200)  NULL,
        ClinicAddress      NVARCHAR(400)  NULL,
        City               NVARCHAR(100)  NULL,
        State              NVARCHAR(100)  NULL,
        PinCode            NVARCHAR(10)   NULL,
        WebsiteUrl         NVARCHAR(300)  NULL,
        Bio                NVARCHAR(1000) NULL,
        ProfilePic         NVARCHAR(500)  NULL,
        ClinicLogo         NVARCHAR(500)  NULL,
        PrimaryColorHex    NVARCHAR(10)   NULL DEFAULT '#2563eb',
        SecondaryColorHex  NVARCHAR(10)   NULL DEFAULT '#7c3aed',
        PreferredFont      NVARCHAR(100)  NULL DEFAULT 'Inter',
        IsActive           BIT            NOT NULL DEFAULT 1,
        IsVerified         BIT            NOT NULL DEFAULT 0,
        IsProfileComplete  BIT            NOT NULL DEFAULT 0,
        RoleId             INT            NOT NULL DEFAULT 1,
        CreatedDate        DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedDate        DATETIME       NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'TemplateCategoryMaster')
BEGIN
    CREATE TABLE TemplateCategoryMaster (
        CategoryId   INT IDENTITY(1,1) PRIMARY KEY,
        CategoryName NVARCHAR(100) NOT NULL,
        Slug         NVARCHAR(100) NULL,
        Description  NVARCHAR(500) NULL,
        SortOrder    INT           NOT NULL DEFAULT 0,
        IsActive     BIT           NOT NULL DEFAULT 1
    );
    INSERT INTO TemplateCategoryMaster (CategoryName, Slug, SortOrder)
    VALUES ('Health Tips','health-tips',1), ('Festival','festival',2),
           ('Awareness','awareness',3), ('Promotional','promotional',4);
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'BannerTemplate')
BEGIN
    CREATE TABLE BannerTemplate (
        TemplateId   INT IDENTITY(1,1) PRIMARY KEY,
        CategoryId   INT            NULL,
        TemplateName NVARCHAR(200)  NOT NULL,
        Description  NVARCHAR(500)  NULL,
        Tags         NVARCHAR(500)  NULL,
        ThumbnailPath NVARCHAR(500) NULL,
        TemplateJson NVARCHAR(MAX)  NULL,
        Orientation  NVARCHAR(20)   NOT NULL DEFAULT 'Square',
        WidthPx      INT            NULL DEFAULT 1080,
        HeightPx     INT            NULL DEFAULT 1080,
        IsPremium    BIT            NOT NULL DEFAULT 0,
        IsActive     BIT            NOT NULL DEFAULT 1,
        UsageCount   INT            NOT NULL DEFAULT 0,
        CreatedDate  DATETIME       NOT NULL DEFAULT GETDATE(),
        CreatedBy    INT            NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'BannerDesign')
BEGIN
    CREATE TABLE BannerDesign (
        DesignId         INT IDENTITY(1,1) PRIMARY KEY,
        DoctorId         INT            NOT NULL,
        DesignName       NVARCHAR(200)  NOT NULL DEFAULT 'Untitled Design',
        SourceTemplateId INT            NULL,
        DesignJson       NVARCHAR(MAX)  NULL,
        ThumbnailPath    NVARCHAR(500)  NULL,
        ExportedFilePath NVARCHAR(500)  NULL,
        Status           NVARCHAR(20)   NOT NULL DEFAULT 'Draft',
        Orientation      NVARCHAR(20)   NULL,
        WidthPx          INT            NULL,
        HeightPx         INT            NULL,
        IsDeleted        BIT            NOT NULL DEFAULT 0,
        CreatedDate      DATETIME       NOT NULL DEFAULT GETDATE(),
        UpdatedDate      DATETIME       NULL
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'ScheduledPost')
BEGIN
    CREATE TABLE ScheduledPost (
        PostId         INT IDENTITY(1,1) PRIMARY KEY,
        DoctorId       INT            NOT NULL,
        DesignId       INT            NULL,
        Platform       NVARCHAR(50)   NULL,
        Caption        NVARCHAR(2000) NULL,
        Status         NVARCHAR(20)   NOT NULL DEFAULT 'Pending',
        ScheduledAt    DATETIME       NULL,
        PublishedAt    DATETIME       NULL,
        PlatformPostId NVARCHAR(200)  NULL,
        ErrorMessage   NVARCHAR(1000) NULL,
        IsDeleted      BIT            NOT NULL DEFAULT 0,
        CreatedDate    DATETIME       NOT NULL DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'AuditLog')
BEGIN
    CREATE TABLE AuditLog (
        AuditId     BIGINT IDENTITY(1,1) PRIMARY KEY,
        DoctorId    INT            NULL,
        Action      NVARCHAR(100)  NULL,
        Description NVARCHAR(1000) NULL,
        IpAddress   NVARCHAR(50)   NULL,
        CreatedDate DATETIME       NOT NULL DEFAULT GETDATE()
    );
END
GO

-- ============================================================
-- STORED PROCEDURES
-- ============================================================

-- ── 1. AuthenticateDoctor_CYH ─────────────────────────────
IF OBJECT_ID('AuthenticateDoctor_CYH', 'P') IS NOT NULL DROP PROCEDURE AuthenticateDoctor_CYH;
GO
CREATE PROCEDURE AuthenticateDoctor_CYH
    @Flag     NVARCHAR(50),
    @Email    NVARCHAR(200) = NULL,
    @Password NVARCHAR(500) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Flag = 'LOGIN'
    BEGIN
        SELECT TOP 1
            DoctorId, FullName, DisplayName, Email,
            MobileNo, Specialty, Qualifications,
            ClinicName, ClinicAddress, City, State, PinCode,
            WebsiteUrl, Bio, ProfilePic, ClinicLogo,
            PrimaryColorHex, SecondaryColorHex, PreferredFont,
            IsActive, IsVerified, IsProfileComplete, RoleId, CreatedDate
        FROM DoctorMaster
        WHERE Email = @Email
          AND Password = @Password
          AND IsActive = 1;
    END
END
GO

-- ── 2. ManageDoctor_CYH ───────────────────────────────────
IF OBJECT_ID('ManageDoctor_CYH', 'P') IS NOT NULL DROP PROCEDURE ManageDoctor_CYH;
GO
CREATE PROCEDURE ManageDoctor_CYH
    @Flag             NVARCHAR(50),
    @DoctorId         INT            = NULL,
    @FullName         NVARCHAR(150)  = NULL,
    @DisplayName      NVARCHAR(150)  = NULL,
    @Email            NVARCHAR(200)  = NULL,
    @Password         NVARCHAR(500)  = NULL,
    @MobileNo         NVARCHAR(20)   = NULL,
    @Specialty        NVARCHAR(100)  = NULL,
    @Qualifications   NVARCHAR(300)  = NULL,
    @ClinicName       NVARCHAR(200)  = NULL,
    @ClinicAddress    NVARCHAR(400)  = NULL,
    @City             NVARCHAR(100)  = NULL,
    @State            NVARCHAR(100)  = NULL,
    @PinCode          NVARCHAR(10)   = NULL,
    @WebsiteUrl       NVARCHAR(300)  = NULL,
    @Bio              NVARCHAR(1000) = NULL,
    @ProfilePic       NVARCHAR(500)  = NULL,
    @ClinicLogo       NVARCHAR(500)  = NULL,
    @PrimaryColorHex  NVARCHAR(10)   = NULL,
    @SecondaryColorHex NVARCHAR(10)  = NULL,
    @PreferredFont    NVARCHAR(100)  = NULL,
    @IsActive         BIT            = NULL,
    @UpdatedBy        INT            = NULL
AS
BEGIN
    SET NOCOUNT ON;

    -- REGISTER
    IF @Flag = 'REGISTER'
    BEGIN
        IF EXISTS (SELECT 1 FROM DoctorMaster WHERE Email = @Email)
        BEGIN
            SELECT 0 AS IsSuccess, 'Email already registered.' AS Msg, NULL AS Id;
            RETURN;
        END
        INSERT INTO DoctorMaster (FullName, Email, Password, MobileNo, Specialty, RoleId, CreatedDate)
        VALUES (@FullName, @Email, @Password, @MobileNo, @Specialty, 1, GETDATE());
        SELECT 1 AS IsSuccess, 'Registration successful!' AS Msg, SCOPE_IDENTITY() AS Id;
        RETURN;
    END

    -- EMAIL_CHECK
    IF @Flag = 'EMAIL_CHECK'
    BEGIN
        IF EXISTS (SELECT 1 FROM DoctorMaster WHERE Email = @Email)
            SELECT 1 AS IsSuccess, 'Email exists' AS Msg, NULL AS Id;
        ELSE
            SELECT 0 AS IsSuccess, 'Email not found' AS Msg, NULL AS Id;
        RETURN;
    END

    -- UPDATE_PROFILE
    IF @Flag = 'UPDATE_PROFILE'
    BEGIN
        UPDATE DoctorMaster SET
            FullName       = ISNULL(@FullName,       FullName),
            DisplayName    = ISNULL(@DisplayName,    DisplayName),
            MobileNo       = ISNULL(@MobileNo,       MobileNo),
            Specialty      = ISNULL(@Specialty,      Specialty),
            Qualifications = ISNULL(@Qualifications, Qualifications),
            ClinicName     = ISNULL(@ClinicName,     ClinicName),
            ClinicAddress  = ISNULL(@ClinicAddress,  ClinicAddress),
            City           = ISNULL(@City,           City),
            State          = ISNULL(@State,          State),
            PinCode        = ISNULL(@PinCode,        PinCode),
            WebsiteUrl     = ISNULL(@WebsiteUrl,     WebsiteUrl),
            Bio            = ISNULL(@Bio,            Bio),
            IsProfileComplete = 1,
            UpdatedDate    = GETDATE()
        WHERE DoctorId = @DoctorId;
        SELECT 1 AS IsSuccess, 'Profile updated successfully.' AS Msg, @DoctorId AS Id;
        RETURN;
    END

    -- UPDATE_BRAND
    IF @Flag = 'UPDATE_BRAND'
    BEGIN
        UPDATE DoctorMaster SET
            PrimaryColorHex   = ISNULL(@PrimaryColorHex,   PrimaryColorHex),
            SecondaryColorHex = ISNULL(@SecondaryColorHex, SecondaryColorHex),
            PreferredFont     = ISNULL(@PreferredFont,     PreferredFont),
            UpdatedDate       = GETDATE()
        WHERE DoctorId = @DoctorId;
        SELECT 1 AS IsSuccess, 'Brand settings updated.' AS Msg, @DoctorId AS Id;
        RETURN;
    END

    -- UPDATE_PHOTO
    IF @Flag = 'UPDATE_PHOTO'
    BEGIN
        UPDATE DoctorMaster SET ProfilePic = @ProfilePic, UpdatedDate = GETDATE()
        WHERE DoctorId = @DoctorId;
        SELECT 1 AS IsSuccess, 'Profile photo updated.' AS Msg, @DoctorId AS Id;
        RETURN;
    END

    -- UPDATE_LOGO
    IF @Flag = 'UPDATE_LOGO'
    BEGIN
        UPDATE DoctorMaster SET ClinicLogo = @ClinicLogo, UpdatedDate = GETDATE()
        WHERE DoctorId = @DoctorId;
        SELECT 1 AS IsSuccess, 'Clinic logo updated.' AS Msg, @DoctorId AS Id;
        RETURN;
    END

    -- TOGGLE_STATUS
    IF @Flag = 'TOGGLE_STATUS'
    BEGIN
        UPDATE DoctorMaster SET IsActive = @IsActive, UpdatedDate = GETDATE()
        WHERE DoctorId = @DoctorId;
        SELECT 1 AS IsSuccess, 'Status updated.' AS Msg, @DoctorId AS Id;
        RETURN;
    END
END
GO

-- ── 3. GetDoctor_CYH ──────────────────────────────────────
IF OBJECT_ID('GetDoctor_CYH', 'P') IS NOT NULL DROP PROCEDURE GetDoctor_CYH;
GO
CREATE PROCEDURE GetDoctor_CYH
    @Flag      NVARCHAR(50),
    @DoctorId  INT            = NULL,
    @SearchKey NVARCHAR(200)  = NULL,
    @PageNo    INT            = 1,
    @PageSize  INT            = 10
AS
BEGIN
    SET NOCOUNT ON;

    IF @Flag = 'GET_BY_ID'
    BEGIN
        SELECT DoctorId, FullName, DisplayName, Email, MobileNo,
               Specialty, Qualifications, ClinicName, ClinicAddress,
               City, State, PinCode, WebsiteUrl, Bio,
               ProfilePic, ClinicLogo, PrimaryColorHex, SecondaryColorHex,
               PreferredFont, IsActive, IsVerified, IsProfileComplete,
               RoleId, CreatedDate
        FROM DoctorMaster
        WHERE DoctorId = @DoctorId;
        RETURN;
    END

    IF @Flag = 'LIST'
    BEGIN
        DECLARE @Offset INT = (@PageNo - 1) * @PageSize;
        SELECT DoctorId, FullName, DisplayName, Email, MobileNo,
               Specialty, ClinicName, IsActive, IsVerified, CreatedDate
        FROM DoctorMaster
        WHERE (@SearchKey IS NULL
               OR FullName LIKE '%' + @SearchKey + '%'
               OR Email    LIKE '%' + @SearchKey + '%')
        ORDER BY CreatedDate DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        RETURN;
    END
END
GO

-- ── 4. GetTemplates_CYH ───────────────────────────────────
IF OBJECT_ID('GetTemplates_CYH', 'P') IS NOT NULL DROP PROCEDURE GetTemplates_CYH;
GO
CREATE PROCEDURE GetTemplates_CYH
    @Flag        NVARCHAR(50),
    @TemplateId  INT           = NULL,
    @CategoryId  INT           = NULL,
    @Orientation NVARCHAR(20)  = NULL,
    @SearchKey   NVARCHAR(200) = NULL,
    @PageNo      INT           = 1,
    @PageSize    INT           = 12
AS
BEGIN
    SET NOCOUNT ON;

    IF @Flag = 'GET_BY_ID'
    BEGIN
        SELECT t.*, c.CategoryName
        FROM BannerTemplate t
        LEFT JOIN TemplateCategoryMaster c ON c.CategoryId = t.CategoryId
        WHERE t.TemplateId = @TemplateId;
        RETURN;
    END

    IF @Flag = 'CATEGORIES'
    BEGIN
        SELECT CategoryId, CategoryName, Slug, SortOrder, IsActive
        FROM TemplateCategoryMaster
        WHERE IsActive = 1
        ORDER BY SortOrder;
        RETURN;
    END

    -- LIST / ACTIVE
    BEGIN
        DECLARE @Offset INT = (@PageNo - 1) * @PageSize;
        SELECT t.TemplateId, t.CategoryId, c.CategoryName,
               t.TemplateName, t.Description, t.Tags,
               t.ThumbnailPath, t.Orientation,
               t.WidthPx, t.HeightPx, t.IsPremium, t.IsActive,
               t.UsageCount, t.CreatedDate
        FROM BannerTemplate t
        LEFT JOIN TemplateCategoryMaster c ON c.CategoryId = t.CategoryId
        WHERE (@Flag = 'ACTIVE' AND t.IsActive = 1 OR @Flag = 'ALL')
          AND (@CategoryId  IS NULL OR t.CategoryId  = @CategoryId)
          AND (@Orientation IS NULL OR t.Orientation = @Orientation)
          AND (@SearchKey   IS NULL OR t.TemplateName LIKE '%' + @SearchKey + '%'
                                    OR t.Tags         LIKE '%' + @SearchKey + '%')
        ORDER BY t.CreatedDate DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
    END
END
GO

-- ── 5. ManageTemplate_CYH ─────────────────────────────────
IF OBJECT_ID('ManageTemplate_CYH', 'P') IS NOT NULL DROP PROCEDURE ManageTemplate_CYH;
GO
CREATE PROCEDURE ManageTemplate_CYH
    @Flag         NVARCHAR(50),
    @TemplateId   INT            = NULL,
    @CategoryId   INT            = NULL,
    @TemplateName NVARCHAR(200)  = NULL,
    @Description  NVARCHAR(500)  = NULL,
    @Tags         NVARCHAR(500)  = NULL,
    @ThumbnailPath NVARCHAR(500) = NULL,
    @TemplateJson NVARCHAR(MAX)  = NULL,
    @Orientation  NVARCHAR(20)   = NULL,
    @WidthPx      INT            = NULL,
    @HeightPx     INT            = NULL,
    @IsPremium    BIT            = NULL,
    @IsActive     BIT            = NULL,
    @CreatedBy    INT            = NULL,
    @UpdatedBy    INT            = NULL,
    @DeletedBy    INT            = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Flag = 'INSERT'
    BEGIN
        INSERT INTO BannerTemplate
            (CategoryId, TemplateName, Description, Tags,
             ThumbnailPath, TemplateJson, Orientation, WidthPx, HeightPx,
             IsPremium, IsActive, CreatedDate, CreatedBy)
        VALUES
            (@CategoryId, @TemplateName, @Description, @Tags,
             @ThumbnailPath, @TemplateJson, @Orientation, @WidthPx, @HeightPx,
             ISNULL(@IsPremium,0), 1, GETDATE(), @CreatedBy);
        SELECT 1 AS IsSuccess, 'Template created.' AS Msg, SCOPE_IDENTITY() AS Id;
        RETURN;
    END

    IF @Flag = 'UPDATE'
    BEGIN
        UPDATE BannerTemplate SET
            CategoryId    = ISNULL(@CategoryId,    CategoryId),
            TemplateName  = ISNULL(@TemplateName,  TemplateName),
            Description   = ISNULL(@Description,   Description),
            Tags          = ISNULL(@Tags,          Tags),
            ThumbnailPath = ISNULL(@ThumbnailPath, ThumbnailPath),
            TemplateJson  = ISNULL(@TemplateJson,  TemplateJson),
            Orientation   = ISNULL(@Orientation,   Orientation),
            WidthPx       = ISNULL(@WidthPx,       WidthPx),
            HeightPx      = ISNULL(@HeightPx,      HeightPx),
            IsPremium     = ISNULL(@IsPremium,     IsPremium)
        WHERE TemplateId = @TemplateId;
        SELECT 1 AS IsSuccess, 'Template updated.' AS Msg, @TemplateId AS Id;
        RETURN;
    END

    IF @Flag = 'TOGGLE_STATUS'
    BEGIN
        UPDATE BannerTemplate SET IsActive = @IsActive WHERE TemplateId = @TemplateId;
        SELECT 1 AS IsSuccess, 'Status updated.' AS Msg, @TemplateId AS Id;
        RETURN;
    END

    IF @Flag = 'DELETE'
    BEGIN
        DELETE FROM BannerTemplate WHERE TemplateId = @TemplateId;
        SELECT 1 AS IsSuccess, 'Template deleted.' AS Msg, @TemplateId AS Id;
        RETURN;
    END

    IF @Flag = 'INCREMENT_USAGE'
    BEGIN
        UPDATE BannerTemplate SET UsageCount = UsageCount + 1
        WHERE TemplateId = @TemplateId;
        RETURN;
    END
END
GO

-- ── 6. GetDesigns_CYH ─────────────────────────────────────
IF OBJECT_ID('GetDesigns_CYH', 'P') IS NOT NULL DROP PROCEDURE GetDesigns_CYH;
GO
CREATE PROCEDURE GetDesigns_CYH
    @Flag      NVARCHAR(50),
    @DesignId  INT            = NULL,
    @DoctorId  INT            = NULL,
    @Status    NVARCHAR(20)   = NULL,
    @SearchKey NVARCHAR(200)  = NULL,
    @PageNo    INT            = 1,
    @PageSize  INT            = 12
AS
BEGIN
    SET NOCOUNT ON;

    IF @Flag = 'GET_BY_ID'
    BEGIN
        SELECT d.*, t.TemplateName
        FROM BannerDesign d
        LEFT JOIN BannerTemplate t ON t.TemplateId = d.SourceTemplateId
        WHERE d.DesignId = @DesignId
          AND d.DoctorId = @DoctorId
          AND d.IsDeleted = 0;
        RETURN;
    END

    IF @Flag = 'LIST'
    BEGIN
        DECLARE @Offset INT = (@PageNo - 1) * @PageSize;
        SELECT d.DesignId, d.DoctorId, d.DesignName,
               d.SourceTemplateId, t.TemplateName,
               d.ThumbnailPath, d.ExportedFilePath,
               d.Status, d.Orientation, d.WidthPx, d.HeightPx,
               d.CreatedDate, d.UpdatedDate
        FROM BannerDesign d
        LEFT JOIN BannerTemplate t ON t.TemplateId = d.SourceTemplateId
        WHERE d.DoctorId  = @DoctorId
          AND d.IsDeleted = 0
          AND (@Status    IS NULL OR d.Status     = @Status)
          AND (@SearchKey IS NULL OR d.DesignName LIKE '%' + @SearchKey + '%')
        ORDER BY d.UpdatedDate DESC, d.CreatedDate DESC
        OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY;
        RETURN;
    END
END
GO

-- ── 7. ManageDesign_CYH ───────────────────────────────────
IF OBJECT_ID('ManageDesign_CYH', 'P') IS NOT NULL DROP PROCEDURE ManageDesign_CYH;
GO
CREATE PROCEDURE ManageDesign_CYH
    @Flag            NVARCHAR(50),
    @DesignId        INT            = NULL,
    @DoctorId        INT            = NULL,
    @DesignName      NVARCHAR(200)  = NULL,
    @SourceTemplateId INT           = NULL,
    @DesignJson      NVARCHAR(MAX)  = NULL,
    @ThumbnailPath   NVARCHAR(500)  = NULL,
    @Status          NVARCHAR(20)   = NULL,
    @Orientation     NVARCHAR(20)   = NULL,
    @WidthPx         INT            = NULL,
    @HeightPx        INT            = NULL
AS
BEGIN
    SET NOCOUNT ON;

    IF @Flag = 'INSERT'
    BEGIN
        INSERT INTO BannerDesign
            (DoctorId, DesignName, SourceTemplateId, DesignJson,
             ThumbnailPath, Status, Orientation, WidthPx, HeightPx, CreatedDate)
        VALUES
            (@DoctorId, ISNULL(@DesignName,'Untitled Design'), @SourceTemplateId, @DesignJson,
             @ThumbnailPath, ISNULL(@Status,'Draft'), @Orientation, @WidthPx, @HeightPx, GETDATE());
        SELECT 1 AS IsSuccess, 'Design saved.' AS Msg, SCOPE_IDENTITY() AS Id;
        RETURN;
    END

    IF @Flag = 'UPDATE'
    BEGIN
        UPDATE BannerDesign SET
            DesignName    = ISNULL(@DesignName,    DesignName),
            DesignJson    = ISNULL(@DesignJson,    DesignJson),
            ThumbnailPath = ISNULL(@ThumbnailPath, ThumbnailPath),
            Status        = ISNULL(@Status,        Status),
            UpdatedDate   = GETDATE()
        WHERE DesignId = @DesignId AND DoctorId = @DoctorId;
        SELECT 1 AS IsSuccess, 'Design updated.' AS Msg, @DesignId AS Id;
        RETURN;
    END

    IF @Flag = 'UPDATE_STATUS'
    BEGIN
        UPDATE BannerDesign SET Status = @Status, UpdatedDate = GETDATE()
        WHERE DesignId = @DesignId AND DoctorId = @DoctorId;
        SELECT 1 AS IsSuccess, 'Status updated.' AS Msg, @DesignId AS Id;
        RETURN;
    END

    IF @Flag = 'DELETE'
    BEGIN
        UPDATE BannerDesign SET IsDeleted = 1, UpdatedDate = GETDATE()
        WHERE DesignId = @DesignId AND DoctorId = @DoctorId;
        SELECT 1 AS IsSuccess, 'Design deleted.' AS Msg, @DesignId AS Id;
        RETURN;
    END
END
GO

PRINT 'CYHBrand database setup complete!';
GO
