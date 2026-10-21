# CYHBrand — Implementation Plan

> **Document Version:** 1.0  
> **Status:** Living Document  
> **Last Updated:** 2026-10-21  
> **Target:** .NET 8 / SQL Server 2022 / Blazor Web App (Interactive Server)

---

## Table of Contents

- [NuGet Packages Reference](#nuget-packages-reference)
- [External Configuration Reference](#external-configuration-reference)
- [Phase 0 — Environment Setup](#phase-0--environment-setup-completed)
- [Phase 1 — Solution Scaffold & Core Domain](#phase-1--solution-scaffold--core-domain)
- [Phase 2 — Infrastructure: Data Layer](#phase-2--infrastructure-data-layer)
- [Phase 3 — Authentication & User Management](#phase-3--authentication--user-management)
- [Phase 4 — Doctor Profile & Brand Settings](#phase-4--doctor-profile--brand-settings)
- [Phase 5 — Template Library](#phase-5--template-library)
- [Phase 6 — Banner Editor](#phase-6--banner-editor)
- [Phase 7 — AI Banner Creator](#phase-7--ai-banner-creator)
- [Phase 8 — My Designs & Version History](#phase-8--my-designs--version-history)
- [Phase 9 — Content Calendar & Scheduling](#phase-9--content-calendar--scheduling)
- [Phase 10 — Social Publishing & Connected Accounts](#phase-10--social-publishing--connected-accounts)
- [Phase 11 — Admin Module, Usage & Subscription](#phase-11--admin-module-usage--subscription)

---

## NuGet Packages Reference

### CYHBrand.Core

> No external NuGet packages — pure .NET 8 class library.

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

---

### CYHBrand.Infrastructure

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.AspNetCore.Identity.EntityFrameworkCore` | `8.0.*` | ASP.NET Core Identity + EF Core integration |
| `Microsoft.EntityFrameworkCore.SqlServer` | `8.0.*` | SQL Server EF Core provider |
| `Microsoft.EntityFrameworkCore.Tools` | `8.0.*` | CLI tools for migrations (dev dependency) |
| `Microsoft.EntityFrameworkCore.Design` | `8.0.*` | EF Core design-time tools |
| `Microsoft.AspNetCore.DataProtection.EntityFrameworkCore` | `8.0.*` | Persist Data Protection keys to DB |
| `SixLabors.ImageSharp` | `3.1.*` | Cross-platform image processing (resize, crop, export) |
| `SixLabors.ImageSharp.Drawing` | `2.1.*` | Drawing primitives for template rendering |
| `MailKit` | `4.7.*` | SMTP email sending |
| `MimeKit` | `4.7.*` | Email message building (MailKit dependency) |
| `Microsoft.Extensions.Http` | `8.0.*` | HttpClientFactory for AI API calls |
| `System.Text.Json` | `8.0.*` (built-in) | JSON serialization of template/design data |
| `Serilog` | `4.1.*` | Structured logging core |
| `Serilog.AspNetCore` | `8.0.*` | ASP.NET Core integration |
| `Serilog.Sinks.Console` | `6.0.*` | Console log output |
| `Serilog.Sinks.File` | `6.0.*` | File log output with rolling |
| `Serilog.Sinks.Seq` | `8.0.*` | Optional: Seq structured log server |

**Optional for future phases:**

| Package | Version | Purpose |
|---------|---------|---------|
| `Azure.Storage.Blobs` | `12.21.*` | Azure Blob Storage (Phase 5+) |
| `Azure.AI.OpenAI` | `2.1.*` | Azure OpenAI client (Phase 7+) |

```xml
<!-- Example: CYHBrand.Infrastructure.csproj -->
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\CYHBrand.Core\CYHBrand.Core.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.*" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.*" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.Tools" Version="8.0.*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.*">
      <PrivateAssets>all</PrivateAssets>
      <IncludeAssets>runtime; build; native; contentfiles; analyzers; buildtransitive</IncludeAssets>
    </PackageReference>
    <PackageReference Include="Microsoft.AspNetCore.DataProtection.EntityFrameworkCore" Version="8.0.*" />
    <PackageReference Include="SixLabors.ImageSharp" Version="3.1.*" />
    <PackageReference Include="SixLabors.ImageSharp.Drawing" Version="2.1.*" />
    <PackageReference Include="MailKit" Version="4.7.*" />
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.*" />
    <PackageReference Include="Serilog.Sinks.Console" Version="6.0.*" />
    <PackageReference Include="Serilog.Sinks.File" Version="6.0.*" />
  </ItemGroup>
</Project>
```

---

### CYHBrand.Web

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.AspNetCore.Components.WebAssembly.Server` | N/A (built-in with SDK) | Blazor Web App hosting |
| `AspNetCoreRateLimit` | `5.0.*` | Rate limiting for login endpoint |
| `Microsoft.AspNetCore.Authentication.Cookies` | Built-in (`net8.0`) | Cookie auth (part of ASP.NET Core) |
| `Serilog.AspNetCore` | `8.0.*` | Logging pipeline |
| `Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore` | `8.0.*` | EF Core error page (dev only) |

```xml
<!-- Example: CYHBrand.Web.csproj -->
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\CYHBrand.Core\CYHBrand.Core.csproj" />
    <ProjectReference Include="..\CYHBrand.Infrastructure\CYHBrand.Infrastructure.csproj" />
  </ItemGroup>
  <ItemGroup>
    <PackageReference Include="AspNetCoreRateLimit" Version="5.0.*" />
    <PackageReference Include="Serilog.AspNetCore" Version="8.0.*" />
    <PackageReference Include="Microsoft.AspNetCore.Diagnostics.EntityFrameworkCore" Version="8.0.*" />
  </ItemGroup>
</Project>
```

---

### Test Projects

| Package | Version | Purpose |
|---------|---------|---------|
| `Microsoft.NET.Test.Sdk` | `17.11.*` | Test host |
| `xunit` | `2.9.*` | Test framework |
| `xunit.runner.visualstudio` | `2.8.*` | VS runner |
| `Moq` | `4.20.*` | Mocking |
| `FluentAssertions` | `6.12.*` | Readable assertions |
| `Microsoft.AspNetCore.Mvc.Testing` | `8.0.*` | Integration test factory |
| `Testcontainers.MsSql` | `3.10.*` | SQL Server container for integration tests |

---

## External Configuration Reference

### Required Configuration Keys

| Key | Where Set | Phase Needed |
|-----|-----------|-------------|
| `ConnectionStrings:DefaultConnection` | `appsettings.Development.json` / env var | Phase 1 |
| `AIImageGeneration:Provider` | `appsettings.json` | Phase 7 |
| `AIImageGeneration:OpenAI:ApiKey` | User Secrets / env var | Phase 7 |
| `AIImageGeneration:OpenAI:Model` | `appsettings.json` | Phase 7 |
| `AIImageGeneration:AzureOpenAI:Endpoint` | User Secrets / env var | Phase 7 (alt) |
| `AIImageGeneration:AzureOpenAI:ApiKey` | User Secrets / env var | Phase 7 (alt) |
| `Storage:Provider` | `appsettings.json` | Phase 1 |
| `Storage:LocalDisk:BasePath` | `appsettings.json` | Phase 1 |
| `Storage:AzureBlob:ConnectionString` | User Secrets / env var | Phase 10 |
| `Email:SmtpHost` | `appsettings.json` | Phase 3 |
| `Email:SmtpPort` | `appsettings.json` | Phase 3 |
| `Email:Username` | User Secrets / env var | Phase 3 |
| `Email:Password` | User Secrets / env var | Phase 3 |
| `Email:FromAddress` | `appsettings.json` | Phase 3 |
| `Social:Facebook:AppId` | `appsettings.json` | Phase 10 |
| `Social:Facebook:AppSecret` | User Secrets / env var | Phase 10 |
| `Social:Instagram:AppId` | `appsettings.json` | Phase 10 |
| `Social:Instagram:AppSecret` | User Secrets / env var | Phase 10 |
| `Social:LinkedIn:ClientId` | `appsettings.json` | Phase 10 |
| `Social:LinkedIn:ClientSecret` | User Secrets / env var | Phase 10 |
| `DataProtection:ApplicationName` | `appsettings.json` | Phase 1 |

### Developer Setup Commands

```powershell
# Initialize user secrets (run in CYHBrand.Web directory)
dotnet user-secrets init

# Set development secrets
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=VRS-AMITPC\SQLEXPRESS;Database=CYHBrandDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
dotnet user-secrets set "Email:Username" "dev-email@example.com"
dotnet user-secrets set "Email:Password" "dev-password"
dotnet user-secrets set "AIImageGeneration:OpenAI:ApiKey" "sk-your-key-here"
```

---

## Phase 0 — Environment Setup `COMPLETED`

**Goal:** Verify development environment is ready.

**Complexity:** Low  
**Dependencies:** None

### Checklist

- [x] .NET 8 SDK installed and verified (`dotnet --version`)
- [x] SQL Server 2022 RTM installed and running
- [x] SQL Server Express local instance accessible
- [x] Visual Studio 2022 or VS Code with C# extensions installed
- [x] Git repository initialized at `d:\Myd\CYHBrand`
- [x] Workspace folder structure confirmed
- [x] `docs/` directory created

### Deliverables

- [x] Working development environment
- [x] `docs/` directory with planning documents

### Success Criteria

- `dotnet --version` outputs `8.0.x`
- SQL Server reachable via `Server=localhost\SQLEXPRESS`
- Workspace confirmed at `d:\Myd\CYHBrand`

---

## Phase 1 — Solution Scaffold & Core Domain

**Goal:** Create the solution file, three projects with correct references, and all Core layer entities, interfaces, DTOs, and enums. Set up Blazor Web App shell and verify it runs.

**Complexity:** Medium  
**Dependencies:** Phase 0

### Checklist

#### 1.1 Solution & Project Creation

- [ ] Create solution: `dotnet new sln -n CYHBrand -o d:\Myd\CYHBrand`
- [ ] Create `src/` and `tests/` directories
- [ ] Create Core library: `dotnet new classlib -n CYHBrand.Core -o src/CYHBrand.Core --framework net8.0`
- [ ] Create Infrastructure library: `dotnet new classlib -n CYHBrand.Infrastructure -o src/CYHBrand.Infrastructure --framework net8.0`
- [ ] Create Web app: `dotnet new blazorweb -n CYHBrand.Web -o src/CYHBrand.Web --interactivity Server --framework net8.0`
- [ ] Create UnitTests: `dotnet new xunit -n CYHBrand.UnitTests -o tests/CYHBrand.UnitTests --framework net8.0`
- [ ] Create IntegrationTests: `dotnet new xunit -n CYHBrand.IntegrationTests -o tests/CYHBrand.IntegrationTests --framework net8.0`
- [ ] Add all projects to solution: `dotnet sln add` for each `.csproj`
- [ ] Add project references: Infrastructure → Core; Web → Core + Infrastructure; Tests → respective projects
- [ ] Verify solution builds: `dotnet build CYHBrand.sln`

#### 1.2 Core: Entities

- [ ] Create `BaseEntity.cs` with `Id (Guid)`, `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`
- [ ] Create `ApplicationUser.cs` extending `IdentityUser<Guid>` with additional fields
- [ ] Create `DoctorProfile.cs` with all fields per DATABASE_DESIGN.md §1.2
- [ ] Create `DoctorBrandSettings.cs` per §1.3
- [ ] Create `BannerTemplateCategory.cs` per §1.4
- [ ] Create `BannerTemplate.cs` per §1.5
- [ ] Create `BannerDesign.cs` per §1.6
- [ ] Create `BannerDesignVersion.cs` per §1.7
- [ ] Create `GeneratedImageJob.cs` per §1.8
- [ ] Create `StoredAsset.cs` per §1.9
- [ ] Create `ScheduledPost.cs` per §1.10
- [ ] Create `ScheduledPostAttempt.cs` per §1.11
- [ ] Create `ConnectedSocialAccount.cs` per §1.12
- [ ] Create `PublishingLog.cs` per §1.13
- [ ] Create `UsageLedger.cs` per §1.14
- [ ] Create `SubscriptionPlan.cs` per §1.15
- [ ] Create `UserSubscription.cs` per §1.16
- [ ] Create `AuditLog.cs` per §1.17

#### 1.3 Core: Enums

- [ ] `UserRole.cs` — Doctor, ClinicAdmin, PlatformAdmin, SuperAdmin
- [ ] `JobStatus.cs` — Queued, Processing, Completed, Failed, Cancelled
- [ ] `DesignStatus.cs` — Draft, Saved, Published
- [ ] `PostStatus.cs` — Pending, Scheduled, Published, Failed, Cancelled
- [ ] `SocialPlatform.cs` — Instagram, Facebook, LinkedIn, Twitter
- [ ] `AssetType.cs` — ProfilePhoto, ClinicLogo, BannerExport, AIGenerated, TemplateThumb
- [ ] `UsageActionType.cs` — AIGeneration, BannerExport, SocialPost, StorageBytes
- [ ] `SubscriptionTier.cs` — Free, Pro, Enterprise
- [ ] `BannerOrientation.cs` — Square, Landscape, Portrait
- [ ] `PublishOutcome.cs` — Success, Failed, RateLimited, AuthError, NetworkError

#### 1.4 Core: Interfaces

- [ ] `IRepository<T>.cs` — generic repository interface
- [ ] All domain repository interfaces (one per entity, per ARCHITECTURE.md §5.2)
- [ ] `IImageGenerationService.cs` with `ImageGenerationRequest` and `ImageGenerationResult` records
- [ ] `IStorageService.cs` with all methods
- [ ] `ISocialPublishingService.cs` with all methods and supporting records
- [ ] `IEmailService.cs` — `SendAsync(to, subject, htmlBody)`
- [ ] `ICurrentUserService.cs` — `UserId`, `UserEmail`, `IsInRole()`
- [ ] `IUsageService.cs` — `RecordAsync()`, `GetSummaryAsync()`, `IsWithinLimitAsync()`
- [ ] `ISubscriptionService.cs` — `GetActivePlanAsync()`, `CheckLimitAsync()`
- [ ] `IBackgroundJobQueue.cs` — `EnqueueAsync()`, `DequeueAsync()`
- [ ] `ITemplateRenderService.cs` — `RenderToPngAsync(json, widthPx, heightPx)`

#### 1.5 Core: DTOs

- [ ] Create all DTO records/classes per ARCHITECTURE.md §5.3 (all folders and files)
- [ ] `PagedResult<T>.cs` — `Items`, `TotalCount`, `Page`, `PageSize`, `TotalPages`
- [ ] `Result<T>.cs` and `Result.cs` — `IsSuccess`, `Error`, `Value`

#### 1.6 Core: Exceptions

- [ ] `DomainException.cs` (base)
- [ ] `NotFoundException.cs`
- [ ] `UnauthorizedException.cs`
- [ ] `UsageLimitExceededException.cs`
- [ ] `ValidationException.cs`

#### 1.7 Core: Specifications

- [ ] `BaseSpecification<T>.cs` implementing `ISpecification<T>`
- [ ] All concrete specification classes per ARCHITECTURE.md §5.6

#### 1.8 Web: Blazor Shell

- [ ] Remove default Blazor template boilerplate pages (Counter, Weather)
- [ ] Configure `App.razor` with `AuthorizeRouteView`
- [ ] Create `AuthLayout.razor` — minimal centered card layout
- [ ] Create `MainLayout.razor` — sidebar + topnav + body layout (skeleton only)
- [ ] Create `AdminLayout.razor` — extends MainLayout
- [ ] Create placeholder `Sidebar.razor` with nav links
- [ ] Create placeholder `TopNav.razor`
- [ ] Create `DoctorAvatarMenu.razor` placeholder
- [ ] Create `_Imports.razor` with global using directives
- [ ] Create `Routes.razor`

#### 1.9 Web: Shared Components (Skeletons)

- [ ] Create skeleton versions of all shared components from UI_PAGE_MAP.md §2 (render minimal markup, params defined)
- [ ] Create `ToastService.cs` singleton for in-app toast notifications
- [ ] Register `ToastService` in DI

#### 1.10 Configuration

- [ ] Set up `appsettings.json` with all configuration keys (empty/default values)
- [ ] Set up `appsettings.Development.json`
- [ ] Initialize user secrets: `dotnet user-secrets init`
- [ ] Document required secrets in `docs/DEVELOPER_SETUP.md`

#### 1.11 Logging

- [ ] Install Serilog NuGet packages
- [ ] Configure Serilog in `Program.cs` with Console and File sinks
- [ ] Set minimum log level per environment
- [ ] Add correlation ID enricher

#### 1.12 Health Check

- [ ] Add `/health` endpoint via `builder.Services.AddHealthChecks()`
- [ ] Add DB health check (once DB is wired)

#### 1.13 Verify

- [ ] `dotnet build` — zero errors, zero warnings
- [ ] `dotnet run --project src/CYHBrand.Web` — app starts and shows default Blazor page at localhost

### Deliverables

- Buildable solution with 5 projects
- All Core entities, enums, interfaces, DTOs, exceptions, specifications
- Blazor shell with layouts and skeleton components
- Serilog logging configured

### Success Criteria

- `dotnet build CYHBrand.sln` exits 0
- `dotnet run` starts Blazor app on `https://localhost:5001`
- No unused using warnings

---

## Phase 2 — Infrastructure: Data Layer

**Goal:** Implement EF Core DbContext, entity configurations, all repository implementations, Data Protection key persistence, initial migration, and seed data.

**Complexity:** High  
**Dependencies:** Phase 1

### Checklist

#### 2.1 NuGet Packages

- [ ] Add all Infrastructure NuGet packages to `CYHBrand.Infrastructure.csproj`
- [ ] Verify `dotnet restore` succeeds

#### 2.2 AppDbContext

- [ ] Create `AppDbContext.cs` extending `IdentityDbContext<ApplicationUser, IdentityRole<Guid>, Guid>`
- [ ] Add `DbSet<T>` property for every entity
- [ ] Add `DbSet<DataProtectionKey>` for Data Protection
- [ ] Override `OnModelCreating` to call `ApplyConfigurationsFromAssembly`
- [ ] Configure global query filters for soft-deleted entities
- [ ] Set up `SaveChangesAsync` override to auto-set `CreatedAt`/`UpdatedAt`

#### 2.3 Entity Configurations

Create one `IEntityTypeConfiguration<T>` class per entity in `Data/Configurations/`:

- [ ] `ApplicationUserConfiguration.cs` — extra columns mapping
- [ ] `DoctorProfileConfiguration.cs` — FK to user (UNIQUE), FK to StoredAssets, all column mappings
- [ ] `DoctorBrandSettingsConfiguration.cs` — FK to profile (UNIQUE), CHECK constraint for color hex
- [ ] `BannerTemplateCategoryConfiguration.cs` — UNIQUE on Name and Slug
- [ ] `BannerTemplateConfiguration.cs` — FK to category, FK to StoredAssets (x2), `TemplateJson` as `nvarchar(max)`
- [ ] `BannerDesignConfiguration.cs` — FK to user, optional FK to template, optional FK to job, soft-delete query filter
- [ ] `BannerDesignVersionConfiguration.cs` — UNIQUE (`BannerDesignId`, `VersionNumber`), cascade delete
- [ ] `GeneratedImageJobConfiguration.cs` — FK to user, FK to StoredAssets (optional)
- [ ] `StoredAssetConfiguration.cs` — FK to user, soft-delete filter
- [ ] `ScheduledPostConfiguration.cs` — FK to user, design, connected account, soft-delete filter
- [ ] `ScheduledPostAttemptConfiguration.cs` — FK to post, cascade delete
- [ ] `ConnectedSocialAccountConfiguration.cs` — UNIQUE (`UserId`, `Platform`, `PlatformAccountId`); encrypted token columns as `nvarchar(max)`
- [ ] `PublishingLogConfiguration.cs` — FK to post, FK to user
- [ ] `UsageLedgerConfiguration.cs` — bigint PK, index on `(UserId, PeriodYear, PeriodMonth, ActionType)`
- [ ] `SubscriptionPlanConfiguration.cs` — UNIQUE on Name
- [ ] `UserSubscriptionConfiguration.cs` — FK to user and plan; partial unique index for active subscription
- [ ] `AuditLogConfiguration.cs` — bigint PK, indexes on UserId+OccurredAt and Action+OccurredAt

#### 2.4 Repositories

- [ ] `BaseRepository<T>.cs` — EF Core implementation of `IRepository<T>` with specification support
- [ ] `DoctorProfileRepository.cs`
- [ ] `BannerTemplateRepository.cs` — search by tags (LIKE or FTS)
- [ ] `BannerDesignRepository.cs`
- [ ] `GeneratedImageJobRepository.cs`
- [ ] `ScheduledPostRepository.cs` — `GetDuePostsAsync(DateTime utcNow)` method
- [ ] `ConnectedSocialAccountRepository.cs`
- [ ] `UsageLedgerRepository.cs` — `GetMonthlyTotalAsync(userId, year, month, actionType)`
- [ ] `UserSubscriptionRepository.cs` — `GetActiveAsync(userId)`
- [ ] `AuditLogRepository.cs` — paginated query with filters

#### 2.5 Identity Setup

- [ ] `CurrentUserService.cs` implementing `ICurrentUserService` via `IHttpContextAccessor`
- [ ] `RoleInitializer.cs` — ensures all roles exist at startup

#### 2.6 Data Protection

- [ ] Install `Microsoft.AspNetCore.DataProtection.EntityFrameworkCore`
- [ ] Add `DataProtectionKeys` table to `AppDbContext` (via `IDataProtectionKeyContext`)
- [ ] Configure in `InfrastructureServiceExtensions.cs`:
  ```csharp
  services.AddDataProtection()
      .PersistKeysToDbContext<AppDbContext>()
      .SetApplicationName("CYHBrand");
  ```
- [ ] Implement `OAuthTokenEncryptionService.cs`

#### 2.7 DI Registration

- [ ] Create `InfrastructureServiceExtensions.cs` with `AddInfrastructure(services, config)` method
- [ ] Register `AppDbContext` with SQL Server connection string
- [ ] Register ASP.NET Core Identity with role support
- [ ] Register all repository implementations
- [ ] Register `ICurrentUserService`
- [ ] Register `IHttpContextAccessor`
- [ ] Register Data Protection
- [ ] Call `AddInfrastructure()` from `Program.cs`

#### 2.8 Initial Migration

- [ ] Create initial migration:
  ```powershell
  dotnet ef migrations add InitialCreate `
    --project src/CYHBrand.Infrastructure `
    --startup-project src/CYHBrand.Web
  ```
- [ ] Review generated migration code — verify all tables, columns, indexes, constraints
- [ ] Apply migration:
  ```powershell
  dotnet ef database update `
    --project src/CYHBrand.Infrastructure `
    --startup-project src/CYHBrand.Web
  ```
- [ ] Verify all tables created in SQL Server Management Studio / sqlcmd

#### 2.9 Seed Data

- [ ] `RoleSeeder.cs` — seeds Doctor, ClinicAdmin, PlatformAdmin, SuperAdmin roles
- [ ] `SubscriptionPlanSeeder.cs` — seeds Free and Pro plans with limits
- [ ] `TemplateCategorySeeder.cs` — seeds: Health Tips, Clinic Announcement, Festival Greetings, Awareness Day, Appointment Reminder, Service Highlight
- [ ] Wire seeders to run on startup (development only initially; production flag-gated)
- [ ] Create initial SuperAdmin user via seeder (credentials from user secrets)

#### 2.10 Storage Service

- [ ] `LocalDiskStorageService.cs` implementing `IStorageService`
- [ ] Create `uploads/` directory under `wwwroot` or configured path
- [ ] Register storage service in DI
- [ ] Test: save a test file, retrieve URL, delete

#### 2.11 Email Service

- [ ] `SmtpEmailService.cs` implementing `IEmailService` via MailKit
- [ ] `EmailOptions.cs` bound from configuration
- [ ] Register in DI
- [ ] Test: send a test email in development (to local Mailhog or Papercut)

#### 2.12 Tests

- [ ] Write unit tests for `BaseSpecification` filter logic
- [ ] Write integration tests for `DoctorProfileRepository` (CRUD against test DB)
- [ ] Write integration test verifying migration applies cleanly

### Deliverables

- EF Core DbContext with all configurations
- All repositories implemented
- Data Protection configured
- Initial migration applied to SQL Server
- Seed data working
- Storage and email services working

### Success Criteria

- `dotnet ef database update` applies with zero errors
- All entities visible in SSMS with correct schema
- `dotnet test` passes all Phase 2 tests
- Seed data visible in DB after startup

---

## Phase 3 — Authentication & User Management

**Goal:** Full registration, login, email verification, forgot/reset password, logout, and account lockout. Blazor auth state configured.

**Complexity:** High  
**Dependencies:** Phase 2

### Checklist

#### 3.1 Identity Configuration

- [ ] Configure `AddIdentity<ApplicationUser, IdentityRole<Guid>>()` with password policy:
  - RequireDigit, RequireLowercase, RequireUppercase, RequireNonAlphanumeric: all true
  - MinimumLength: 8
- [ ] Configure lockout: 5 attempts, 15-minute window
- [ ] Configure cookie auth: HttpOnly, Secure, SameSite=Strict, sliding 30min, absolute 8h
- [ ] Add `CascadeAuthorizationMiddleware` to pipeline

#### 3.2 Auth Pages

- [ ] `Login.razor` — email/password form, validation, error display, forgot password link
- [ ] `Register.razor` — first name, last name, email, password, confirm password, T&C, validation
- [ ] `ForgotPassword.razor` — email input, success confirmation step
- [ ] `ResetPassword.razor` — new password + confirm, token + email from query params
- [ ] `VerifyEmail.razor` — handle confirmation link, success/error states
- [ ] `AccessDenied.razor` — message + dashboard link

#### 3.3 Auth Service Layer

- [ ] `AuthService.cs` (Web layer) with:
  - `RegisterAsync(RegisterRequest)` → calls `UserManager.CreateAsync`, send verification email
  - `LoginAsync(LoginRequest)` → calls `SignInManager.PasswordSignInAsync`, checks lockout, checks email confirmed
  - `LogoutAsync()` → `SignInManager.SignOutAsync`
  - `ForgotPasswordAsync(email)` → generate token, send reset email
  - `ResetPasswordAsync(ResetPasswordRequest)` → `UserManager.ResetPasswordAsync`
  - `ConfirmEmailAsync(userId, token)` → `UserManager.ConfirmEmailAsync`

#### 3.4 Email Templates

- [ ] `PasswordReset.html` — reset link email template
- [ ] `EmailVerification.html` — verification link email template
- [ ] Wire `IEmailService` to send from `AuthService`

#### 3.5 Rate Limiting

- [ ] Install `AspNetCoreRateLimit` (or use built-in .NET 8 rate limiting)
- [ ] Apply fixed window rate limit to login endpoint: 5 requests per 15 minutes per IP
- [ ] Configure in `Program.cs`

#### 3.6 Authorization Policies

- [ ] Define all authorization policies in `Program.cs` per ARCHITECTURE.md §8.2
- [ ] Configure `AuthorizeRouteView` in `Routes.razor`
- [ ] Create `RedirectToLogin.razor` component

#### 3.7 Onboarding Guard

- [ ] `ProfileSetupGuard.razor` — if user logged in but `ProfileSetupComplete == false`, redirect to `/onboarding`
- [ ] Wire into main layout or route check

#### 3.8 Onboarding Wizard Skeleton

- [ ] `OnboardingWizard.razor` — multi-step component (empty steps; content in Phase 4)
- [ ] Step navigation (prev/next) with step indicator

#### 3.9 Role-Based Sidebar Rendering

- [ ] Show admin section in `Sidebar.razor` only when user has `PlatformAdmin` or `SuperAdmin` role

#### 3.10 Audit Logging for Auth Events

- [ ] Middleware or event handlers to write `AuditLog` entries for:
  - Login success / failure
  - Registration
  - Password reset request
  - Password changed
  - Email confirmed
  - Account locked

#### 3.11 Tests

- [ ] Unit test `AuthService.RegisterAsync` (mock UserManager)
- [ ] Integration test: register → confirm email → login → logout
- [ ] Integration test: failed login lockout after 5 attempts
- [ ] Integration test: forgot password → reset password

### Deliverables

- Full auth flow (register, verify, login, logout, forgot/reset password)
- Cookie auth with secure settings
- Rate limiting on login
- Role-based policies
- Auth audit logging

### Success Criteria

- User can register, verify email, log in, and log out
- Failed logins lock account after 5 attempts
- `/dashboard` redirects to `/login` when unauthenticated
- `/admin` returns 403 for Doctor role
- Audit log has login events

---

## Phase 4 — Doctor Profile & Brand Settings

**Goal:** Doctor profile creation (onboarding wizard), profile view/edit, brand color/font settings, photo/logo upload.

**Complexity:** Medium  
**Dependencies:** Phase 3

### Checklist

#### 4.1 Services

- [ ] `DoctorProfileService.cs` (Web layer):
  - `GetProfileAsync(userId)`
  - `CreateProfileAsync(userId, DoctorProfileUpdateRequest)`
  - `UpdateProfileAsync(userId, DoctorProfileUpdateRequest)`
  - `GetBrandSettingsAsync(doctorProfileId)`
  - `UpdateBrandSettingsAsync(doctorProfileId, DoctorBrandSettingsDto)`
  - `UploadPhotoAsync(userId, IBrowserFile)` → validate, resize via ImageSharp, save via IStorageService
  - `UploadLogoAsync(userId, IBrowserFile)` → validate, save via IStorageService
  - `CalculateCompletionPercent(profile)` → returns 0–100

#### 4.2 Onboarding Wizard (full implementation)

- [ ] **Step 1 — Basic Info:** Display name, specialty, qualifications, bio
- [ ] **Step 2 — Clinic Details:** Clinic name, address lines, city, state, PIN, phone, website URL
- [ ] **Step 3 — Brand Colors & Font:** Primary color picker, secondary color picker, accent color picker, font family dropdown (6 options)
- [ ] **Step 4 — Photos:** Profile photo upload (FileUploadControl, with preview), clinic logo upload
- [ ] **Step 5 — Finish:** Summary of entered data + confetti/animation, "Go to Dashboard" button, sets `ProfileSetupComplete = true`
- [ ] Validate each step before allowing next
- [ ] Save progress on each "Next" click (partial save allowed)

#### 4.3 Profile View Page

- [ ] `ProfileView.razor` — read-only profile card: avatar, name, specialty, qualifications, clinic details, contact info, bio, brand color swatches, "Edit Profile" button

#### 4.4 Profile Edit Page

- [ ] `ProfileEdit.razor` — full edit form matching all profile + brand settings fields
- [ ] Real-time color preview as user changes brand colors
- [ ] Font preview with selected font applied to sample text
- [ ] Save button with loading state and success toast
- [ ] Cancel with unsaved changes confirmation

#### 4.5 Image Processing

- [ ] `ImageResizeService.cs`:
  - Profile photo: resize and crop to 400×400 JPEG
  - Clinic logo: resize to max 800px wide, preserve aspect ratio, PNG
- [ ] File validation: max 5 MB, JPEG/PNG/SVG only, re-encode through ImageSharp (strip metadata)

#### 4.6 Dashboard Profile Completeness

- [ ] `ProfileCompletionBanner.razor` shown on dashboard when `< 100%`
- [ ] List missing fields with links to edit

#### 4.7 Tests

- [ ] Unit test `CalculateCompletionPercent` for various field combinations
- [ ] Integration test: create profile → upload photo → verify StoredAsset created
- [ ] Integration test: update brand settings → verify persisted

### Deliverables

- Working onboarding wizard
- Profile view and edit pages
- Photo and logo upload with resize
- Dashboard completeness indicator

### Success Criteria

- New user completes onboarding in < 5 minutes
- Profile photo visible on TopNav avatar
- `ProfileSetupComplete` flag set after wizard
- Profile edit saves and reflects immediately

---

## Phase 5 — Template Library

**Goal:** Admin can create/manage templates. Doctors can browse, filter, search, and preview templates.

**Complexity:** Medium  
**Dependencies:** Phase 4

### Checklist

#### 5.1 Services

- [ ] `TemplateService.cs`:
  - `GetCategoriesAsync()` → active categories
  - `GetTemplatesAsync(filter, paging)` → paginated, filtered
  - `GetTemplateByIdAsync(id)` → with category
  - `SearchTemplatesAsync(query, filter, paging)`
  - `IncrementUsageCountAsync(templateId)`
  - `CreateTemplateAsync(request)` — admin only
  - `UpdateTemplateAsync(id, request)` — admin only
  - `ToggleActiveAsync(id)` — admin only
  - `DeleteTemplateAsync(id)` — admin only

#### 5.2 Template Library Page

- [ ] `TemplateLibrary.razor`:
  - Sidebar: `TemplateCategoryFilter` component
  - Top: `SearchBar`, orientation filter chips (All/Square/Landscape/Portrait), sort dropdown
  - Grid: `TemplateGrid` → `TemplateCard` for each result
  - `PaginationControl` at bottom
  - Empty state for no results
  - Loading skeleton cards while fetching

#### 5.3 Template Preview Page

- [ ] `TemplatePreview.razor`:
  - Full-size preview image
  - Name, description, tags, dimensions, category
  - "Use This Template" button → navigates to `/editor?templateId={id}`

#### 5.4 Template Seeding

- [ ] Create 5–10 sample templates with placeholder images and valid template JSON
- [ ] Seed in `TemplateCategorySeeder` or separate seeder
- [ ] Template JSON schema documented (layer types: text, image, background; each with position, size, properties)

#### 5.5 Admin Template Management Pages

- [ ] `AdminTemplateList.razor` — data grid with actions
- [ ] `AdminTemplateNew.razor` — creation form
- [ ] `AdminTemplateEdit.razor` — edit form (pre-populated)
- [ ] `AdminCategories.razor` — category CRUD (inline editable grid or modal-based)

#### 5.6 Tests

- [ ] Unit test template search specification
- [ ] Integration test: create template → query library → verify appears in filtered results
- [ ] Integration test: filter by category returns only matching templates

### Deliverables

- Template library with browse, filter, search, preview
- Admin template management (CRUD)
- 5+ seeded sample templates

### Success Criteria

- Template grid loads in < 2 seconds with 50 templates
- Category filter and search work correctly
- "Use This Template" lands in editor with template loaded

---

## Phase 6 — Banner Editor

**Goal:** Implement the banner editor page with canvas-based editing using JavaScript interop.

**Complexity:** High  
**Dependencies:** Phase 5

### Checklist

#### 6.1 Technology Decision

- [ ] Decide and document editor approach (Fabric.js via JS interop recommended)
- [ ] Install Fabric.js via npm or CDN link in `wwwroot/lib/`

#### 6.2 Editor Infrastructure

- [ ] `fabric-interop.js` — JS module: `initCanvas(elementId, options)`, `loadFromJson(json)`, `toJson()`, `toPng(scale)`, `addText(options)`, `addImage(url)`, `setBackground(color|url)`, `undo()`, `redo()`, `getActiveObject()`, etc.
- [ ] `CanvasComponent.razor` — Blazor wrapper with `IJSObjectReference` to Fabric.js
- [ ] Implement `DotNetObjectReference` callbacks for canvas events (`DesignChanged`, `ObjectSelected`)

#### 6.3 Editor Pages

- [ ] `BannerEditor.razor` — route handling for all variants:
  - Blank: `/editor`
  - From template: `/editor?templateId={id}` → loads template JSON
  - From AI job: `/editor?jobId={id}` → loads AI image as background
  - From existing design: `/editor/{designId}` → loads design JSON

#### 6.4 Editor Layout & Components

- [ ] `EditorToolbar.razor` — top bar with add text, add image, brand preset, undo, redo, zoom, save, export
- [ ] `LayerPanel.razor` — layer list with visibility, lock, delete, reorder
- [ ] `TextToolPanel.razor` — text properties (font, size, color, alignment, bold, italic)
- [ ] `ImageToolPanel.razor` — image properties (opacity, fit, replace)
- [ ] `BackgroundPanel.razor` — background color and image
- [ ] `BrandPresetPanel.razor` — apply doctor's brand colors/font to design
- [ ] `ExportPanel.razor` — export modal (PNG/JPEG, 1x/2x)

#### 6.5 Design Service

- [ ] `DesignService.cs`:
  - `CreateDesignAsync(userId, BannerDesignSaveRequest)` → creates `BannerDesign` + version 1
  - `SaveDesignAsync(designId, BannerDesignSaveRequest)` → updates design + creates new version
  - `AutoSaveAsync(designId, json)` → saves version with `SaveType = AutoSave`
  - `GetDesignAsync(designId, userId)` → validates ownership
  - `ExportDesignAsync(designId, format, scale)` → calls `ITemplateRenderService`, saves asset
  - `DuplicateDesignAsync(designId, userId)`
  - `DeleteDesignAsync(designId, userId)` → soft delete

#### 6.6 Server-Side Export

- [ ] `ImageSharpRenderService.cs` implementing `ITemplateRenderService`:
  - Parse design JSON
  - Render text layers (load font, draw text)
  - Render image layers
  - Apply background
  - Output PNG or JPEG bytes
- [ ] Save exported image via `IStorageService` → `StoredAsset` created
- [ ] Record export in `UsageLedger`

#### 6.7 Auto-Save

- [ ] Timer in `BannerEditor.razor` — fires every 2 minutes
- [ ] Dirty flag tracks unsaved changes
- [ ] Auto-save calls `DesignService.AutoSaveAsync`
- [ ] "Unsaved changes" indicator in toolbar

#### 6.8 Quick-Insert Tokens

- [ ] Toolbar button: "Insert Doctor Info" → dropdown: Name, Specialty, Clinic, Phone, Website
- [ ] Clicking inserts a text layer with the doctor's actual data pre-filled

#### 6.9 Tests

- [ ] Unit test `DesignService.DuplicateDesignAsync`
- [ ] Unit test `ImageSharpRenderService` — render a minimal JSON → verify PNG bytes
- [ ] Integration test: create design → save → load → verify JSON round-trips

### Deliverables

- Working canvas-based banner editor
- Server-side image export (PNG/JPEG)
- Design save with versioning
- Auto-save

### Success Criteria

- Editor loads with template in < 3 seconds
- Can add text, change font/color, move layers
- Save creates new version in DB
- Export downloads valid PNG file
- Undo/redo works for 20 levels

---

## Phase 7 — AI Banner Creator

**Goal:** AI-powered banner generation with async job processing, real-time status, and quota enforcement.

**Complexity:** High  
**Dependencies:** Phase 6

### Checklist

#### 7.1 AI Service Implementations

- [ ] `StubImageGenerationService.cs` — returns a placeholder image from `wwwroot/images/`; no API call; immediate completion
- [ ] `OpenAIImageGenerationService.cs`:
  - `HttpClient` registered with `IHttpClientFactory`
  - POST to `https://api.openai.com/v1/images/generations`
  - Handle response: `data[0].url` or `data[0].b64_json`
  - Download image bytes if URL returned
  - Map to `ImageGenerationResult`
- [ ] `AzureOpenAIImageGenerationService.cs` (optional; can defer)
- [ ] `ImageGenerationServiceFactory.cs` — reads `AIImageGeneration:Provider` config key

#### 7.2 Prompt Enhancement

- [ ] `PromptEnhancerService.cs`:
  - Takes raw user prompt + `DoctorProfileDto` + `DoctorBrandSettingsDto`
  - Returns enhanced prompt string
  - Applies content guardrails (block disallowed keywords)

#### 7.3 Background Job Infrastructure

- [ ] `IBackgroundJobQueue.cs` interface
- [ ] `BackgroundJobQueue.cs` — `System.Threading.Channels.Channel<T>` with bounded capacity 100
- [ ] `JobWorkerService.cs` (BackgroundService) — reads from channel, dispatches to handlers
- [ ] `ImageGenerationJobHandler.cs`:
  1. Update `GeneratedImageJob.Status = Processing`
  2. Call `IImageGenerationService.GenerateAsync()`
  3. Save result image via `IStorageService`
  4. Create/link `StoredAsset`
  5. Update `GeneratedImageJob.Status = Completed` + `ResultAssetId`
  6. Record in `UsageLedger`
  7. On failure: update status to Failed, write error message, increment attempt count

#### 7.4 AI Banner Service (Web Layer)

- [ ] `AiBannerService.cs`:
  - `EnqueueJobAsync(userId, ImageGenerationRequest)` → validates quota, creates `GeneratedImageJob` (Queued), enqueues to `IBackgroundJobQueue`, returns job ID
  - `GetJobStatusAsync(jobId, userId)` → returns `GeneratedImageJobDto`
  - `GetUserJobsAsync(userId, paging)` → list of jobs

#### 7.5 AI Banner Pages

- [ ] `AiBannerCreator.razor`:
  - `PromptInput` component (max 1000 chars)
  - `SizeSelector` component (Square/Landscape/Portrait)
  - Usage quota bar showing remaining AI generations
  - "Generate" button (disabled if quota exhausted)
  - On submit: calls `AiBannerService.EnqueueJobAsync` → navigates to `/ai-banner/{jobId}/status`

- [ ] `AiBannerJobStatus.razor`:
  - `JobStatusTracker` component
  - Polls `AiBannerService.GetJobStatusAsync` every 3 seconds
  - Shows animated progress
  - On completion: preview of generated image
  - CTA buttons: "Open in Editor", "Regenerate"
  - On failure: error message + "Try Again" button

#### 7.6 Quota Enforcement

- [ ] `IUsageService.IsWithinLimitAsync(userId, UsageActionType.AIGeneration)` check in `AiBannerService.EnqueueJobAsync`
- [ ] User shown quota bar and upgrade prompt when limit reached
- [ ] Quota check also runs in `JobWorkerService` before calling AI API (second gate)

#### 7.7 Tests

- [ ] Unit test `PromptEnhancerService` — verify doctor context injected in output
- [ ] Unit test quota enforcement — mock `IUsageService` returns over-limit
- [ ] Integration test with StubImageGenerationService: enqueue job → process → verify Completed status and StoredAsset created
- [ ] Integration test: quota exceeded → enqueue rejected

### Deliverables

- AI banner generation with stub and OpenAI providers
- Async job queue with background worker
- Real-time job status page
- Quota enforcement

### Success Criteria

- Stub generation completes in < 1 second
- OpenAI generation completes and saves image
- Job status page shows correct state in real-time
- Exceeding quota shows upgrade prompt

---

## Phase 8 — My Designs & Version History

**Goal:** Complete My Designs gallery, design detail page, version history, duplicate, delete, and download.

**Complexity:** Low-Medium  
**Dependencies:** Phase 6

### Checklist

- [ ] `MyDesigns.razor` — full implementation with `DesignGrid`, `DesignFilterBar`, `SearchBar`, `PaginationControl`
- [ ] `DesignDetail.razor` — preview, version history list, name edit, export, share button, open editor button
- [ ] `BannerCard.razor` — full implementation with 3-dot menu (Edit, Duplicate, Download, Share, Delete)
- [ ] `DesignService.DuplicateDesignAsync` — tested and working
- [ ] `DesignService.DeleteDesignAsync` — soft delete with `ConfirmDialog`
- [ ] Version history: list versions by date, "Restore this version" → copies JSON to current design
- [ ] Download from My Designs: triggers export → downloads file
- [ ] Tests: list designs with filters, paginated results, duplicate design

### Deliverables

- My Designs gallery with CRUD operations
- Version history with restore

---

## Phase 9 — Content Calendar & Scheduling

**Goal:** Content calendar with month view, scheduled post creation, dispatch background service.

**Complexity:** High  
**Dependencies:** Phase 8

### Checklist

- [ ] `ContentCalendar.razor` — month grid with `CalendarGrid` and `CalendarDayCell` components
- [ ] `NewScheduledPost.razor` — design picker, date/time picker, caption input (caption wire-up ready, platform select deferred to Phase 10)
- [ ] `EditScheduledPost.razor` — edit pending posts
- [ ] `CalendarService.cs`:
  - `GetMonthPostsAsync(userId, year, month)`
  - `CreateScheduledPostAsync(userId, SchedulePostRequest)` → validates design ownership, creates `ScheduledPost`
  - `UpdateScheduledPostAsync(postId, userId, updates)`
  - `CancelScheduledPostAsync(postId, userId)`
- [ ] `ScheduledPostDispatchService.cs` (BackgroundService):
  - Polls every 60 seconds
  - Queries posts where `ScheduledAtUtc <= UtcNow AND Status = Scheduled`
  - For each: updates to `Processing`, enqueues `SocialPublishJob`
  - On startup crash recovery: reset `Processing` posts stuck > 30 min to `Scheduled`
- [ ] `SocialPublishJobHandler.cs` skeleton (full implementation in Phase 10)
- [ ] Upcoming posts widget on dashboard — next 7 days' scheduled posts
- [ ] Tests: dispatch service unit test with mocked time, calendar query tests

### Deliverables

- Working content calendar
- Scheduled post creation and management
- Background dispatch service (functional with stub publish)

---

## Phase 10 — Social Publishing & Connected Accounts

**Goal:** OAuth account connection for Instagram, Facebook, LinkedIn. Real publishing via social APIs.

**Complexity:** High  
**Dependencies:** Phase 9

> ⚠️ **Risk:** Meta API approval required. Start OAuth app registration in parallel with Phase 9.

### Checklist

#### 10.1 OAuth Infrastructure

- [ ] `OAuthTokenEncryptionService.cs` — encrypt/decrypt via Data Protection
- [ ] `ConnectedAccountService.cs`:
  - `GetConnectedAccountsAsync(userId)`
  - `GetOAuthUrlAsync(platform, userId)` → state = JWT with userId + platform
  - `HandleOAuthCallbackAsync(platform, code, state)` → exchange code, encrypt tokens, save
  - `DisconnectAsync(accountId, userId)` → soft delete
  - `RefreshTokenIfExpiredAsync(accountId)` → calls platform refresh endpoint

#### 10.2 Platform Implementations

- [ ] `FacebookPublishingService.cs` — Graph API `/{page-id}/photos` endpoint
- [ ] `InstagramPublishingService.cs` — Instagram Graph API via Facebook (Business account required)
- [ ] `LinkedInPublishingService.cs` — LinkedIn Share API v2
- [ ] `TwitterPublishingService.cs` — X API v2 (optional / later)

#### 10.3 Connected Accounts Page

- [ ] `ConnectedAccounts.razor` — list accounts, connect buttons, disconnect action
- [ ] `OAuthCallback.razor` — exchange code, store, redirect with toast
- [ ] Show token expiry warning (< 7 days)

#### 10.4 Social Publish Job Handler (Full)

- [ ] `SocialPublishJobHandler.cs` full implementation:
  1. Load `ScheduledPost` with connected account
  2. Decrypt access token
  3. Export design image via `ITemplateRenderService`
  4. Call `ISocialPublishingService.PublishImagePostAsync`
  5. On success: update `ScheduledPost.Status = Published`, write `PublishingLog`, record `UsageLedger`
  6. On failure: write `ScheduledPostAttempt`, if attempts < 3 reschedule for +5 min, else set Failed
  7. Notify user on failure via email

#### 10.5 Share Page (Full)

- [ ] `ShareDesign.razor` full implementation with `PlatformSelector`, `CaptionEditor`, `ScheduleTimePicker`

#### 10.6 Tests

- [ ] Integration test: mock platform service → immediate publish → verify PublishingLog
- [ ] Integration test: 3 failed attempts → ScheduledPost.Status = Failed
- [ ] Unit test token encryption/decryption round-trip

---

## Phase 11 — Admin Module, Usage & Subscription

**Goal:** Complete admin back-office, usage dashboard, subscription plan management.

**Complexity:** Medium  
**Dependencies:** Phase 10

### Checklist

#### 11.1 Admin Pages (Full Implementation)

- [ ] `AdminDashboard.razor` — KPI stat cards, recent audit log, system health
- [ ] `DoctorList.razor` — searchable, paginated, with lock/unlock actions
- [ ] `DoctorDetail.razor` — full profile view, usage stats, AI job list, action buttons
- [ ] `AdminAiConfig.razor` — provider status, key indicator, usage stats
- [ ] `AdminPlans.razor` — plan limits edit
- [ ] `AdminAuditLog.razor` — filterable log table with expandable rows
- [ ] `AdminPublishingLog.razor` — filterable publish log
- [ ] `AdminSettings.razor` — maintenance mode, feature flags, email config

#### 11.2 Usage Dashboard

- [ ] `UsageDashboard.razor` — per-metric quota bars, monthly history table, current plan badge
- [ ] `UsageSummaryWidget.razor` on main dashboard
- [ ] `UsageService.cs`:
  - `GetSummaryAsync(userId, year, month)`
  - `RecordAsync(userId, actionType, quantity, referenceId)`

#### 11.3 Subscription Management

- [ ] `UserSettings.razor` — current plan display, upgrade CTA (links to usage page)
- [ ] `SubscriptionService.cs`:
  - `GetActivePlanAsync(userId)` → `SubscriptionPlanDto`
  - `GetLimitsAsync(userId)` → plan limits
  - `CheckLimitAsync(userId, actionType)` → bool within limit
- [ ] On registration: assign Free plan subscription automatically

#### 11.4 Audit Middleware

- [ ] `AuditMiddleware.cs` — captures action, entity, old/new values for key operations
- [ ] Or: `AuditService.cs` called explicitly from service methods

#### 11.5 Health Check & Monitoring

- [ ] DB health check
- [ ] Storage health check (can write/read a test file)
- [ ] AI service health check (ping provider API)
- [ ] `/health` endpoint returns all checks

#### 11.6 Final Polish

- [ ] Error handling middleware with correlation IDs
- [ ] 404 custom page
- [ ] 500 custom page
- [ ] Accessibility review: ARIA labels, keyboard navigation
- [ ] Mobile responsiveness review
- [ ] Performance review: check all DB queries < 200ms
- [ ] Security review: HTTPS, HSTS, headers audit
- [ ] Run `dotnet list package --vulnerable` → fix any vulnerabilities

### Deliverables

- Complete admin module
- Usage tracking and quota enforcement end-to-end
- Subscription plan assignment on registration
- Health endpoints
- Final production-readiness review

### Success Criteria

- All admin pages load and function for PlatformAdmin role
- Usage ledger accurately reflects actions taken
- New user automatically gets Free plan
- `/health` returns 200 with all checks passing
- `dotnet test` passes all test suites

---

## Implementation Notes

### Git Branching Strategy

```
main          ← production-ready
develop       ← integration branch
feature/*     ← per-phase or per-task feature branches
hotfix/*      ← urgent production fixes
```

### Commit Convention

```
feat(auth): add email verification flow
fix(editor): canvas not loading template JSON
chore(deps): upgrade EF Core to 8.0.8
docs(readme): update developer setup instructions
```

### PR Checklist (Template)

- [ ] Unit tests added/updated
- [ ] Integration tests added if new DB schema or service
- [ ] XML doc comments on all public methods
- [ ] No secrets committed (check with `git-secrets` or `truffleHog`)
- [ ] `dotnet build` exits 0 with no warnings
- [ ] `dotnet test` passes

### Database Migration Naming Convention

```
InitialCreate
AddDoctorProfileTable         ← avoid these in Phase 1; all in InitialCreate
AddBannerEditorTables
AddSocialPublishingTables
AddAuditLogIndex_UserId
```
