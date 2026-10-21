# CYHBrand — Architecture Document

> **Document Version:** 1.0  
> **Status:** Living Document  
> **Last Updated:** 2026-10-21  
> **Audience:** Developers, Tech Lead, DevOps

---

## Table of Contents

1. [Architecture Overview](#1-architecture-overview)
2. [Project Layer Responsibilities](#2-project-layer-responsibilities)
3. [Dependency Flow](#3-dependency-flow)
4. [Technology Choices & Justification](#4-technology-choices--justification)
5. [CYHBrand.Core Contents](#5-cyhbrandcore-contents)
6. [CYHBrand.Infrastructure Contents](#6-cyhbrandinfrastructure-contents)
7. [CYHBrand.Web Contents](#7-cyhbrandweb-contents)
8. [Authentication & Authorization Strategy](#8-authentication--authorization-strategy)
9. [AI Integration Abstraction](#9-ai-integration-abstraction)
10. [Storage Abstraction](#10-storage-abstraction)
11. [Social Publishing Abstraction](#11-social-publishing-abstraction)
12. [Background Job Strategy](#12-background-job-strategy)
13. [Security Considerations](#13-security-considerations)
14. [Configuration Management](#14-configuration-management)
15. [Complete Folder Tree](#15-complete-folder-tree)

---

## 1. Architecture Overview

CYHBrand follows **Clean Architecture** (also known as Onion Architecture) with three primary projects arranged in concentric layers:

```
┌─────────────────────────────────────────────────────────┐
│                   CYHBrand.Web                          │
│         (Blazor UI, Pages, Components, Services)        │
├─────────────────────────────────────────────────────────┤
│              CYHBrand.Infrastructure                    │
│  (EF Core, Identity, Repositories, AI, Storage, Jobs)  │
├─────────────────────────────────────────────────────────┤
│                  CYHBrand.Core                          │
│     (Entities, Interfaces, DTOs, Enums, Exceptions)     │
└─────────────────────────────────────────────────────────┘
```

**Key Principles:**
- **Dependency Rule:** Outer layers depend on inner layers. `Core` has **zero** external dependencies (no EF Core, no ASP.NET, no third-party packages).
- **Interface Segregation:** All cross-boundary operations are defined as interfaces in `Core` and implemented in `Infrastructure`.
- **Testability:** Business logic in `Core` can be unit-tested without databases, HTTP, or file system.
- **Replaceable Infrastructure:** AI provider, storage backend, and social APIs can be swapped by changing the Infrastructure implementation registered in DI.

---

## 2. Project Layer Responsibilities

| Layer | Project | Responsibility | Can Reference |
|-------|---------|---------------|---------------|
| **Domain** | `CYHBrand.Core` | Entities, value objects, domain interfaces, DTOs, enums, domain exceptions, specification patterns | None (pure C#) |
| **Infrastructure** | `CYHBrand.Infrastructure` | EF Core DbContext, migrations, Identity, repository implementations, AI clients, storage clients, scheduling, social API clients, email | `CYHBrand.Core` only |
| **Presentation** | `CYHBrand.Web` | Blazor pages, Razor components, layouts, JavaScript interop, DI registration, middleware pipeline | `CYHBrand.Core` + `CYHBrand.Infrastructure` |
| **Tests** | `CYHBrand.UnitTests` | Unit tests for Core services, entities, specifications | `CYHBrand.Core` |
| **Tests** | `CYHBrand.IntegrationTests` | EF Core integration, HTTP integration tests (WebApplicationFactory) | All projects |

---

## 3. Dependency Flow

```
                           ┌─────────────────────┐
                           │   External World    │
                           │  (Browser / HTTP)   │
                           └──────────┬──────────┘
                                      │
                           ┌──────────▼──────────┐
                           │   CYHBrand.Web      │
                           │  (Blazor, Razor,    │
                           │   Middleware, DI)   │
                           └──────┬──────┬───────┘
                                  │      │
                   ┌──────────────┘      └──────────────┐
                   │                                     │
      ┌────────────▼──────────┐          ┌──────────────▼─────────────┐
      │  CYHBrand.Core        │◄─────────│ CYHBrand.Infrastructure    │
      │  (Interfaces,         │          │ (Implements interfaces from │
      │   Entities, DTOs)     │          │  Core using EF, AI, etc.)  │
      └───────────────────────┘          └────────────────────────────┘
               ▲                                        │
               │                         ┌─────────────▼──────────────┐
               └─────────────────────────│     External Services      │
                                         │  SQL Server, OpenAI, Meta, │
                                         │  LinkedIn, Twitter, SMTP   │
                                         └────────────────────────────┘

  Dependency arrows: ──► means "depends on" (references)
  Core has NO outward arrows (depends on nothing external)
```

---

## 4. Technology Choices & Justification

| Technology | Version | Choice & Justification |
|-----------|---------|------------------------|
| **Runtime** | .NET 8 LTS | Long-term support through Nov 2026; best performance in .NET history; great Blazor support |
| **UI Framework** | Blazor Web App (Interactive Server) | No separate API layer needed; C# full-stack; real-time UI updates via SignalR; good SSR for SEO on public pages |
| **ORM** | EF Core 8 | First-class .NET ORM; code-first migrations; LINQ; well-supported; no raw SQL needed for most operations |
| **Database** | SQL Server 2022 | Reliable RDBMS; JSON support for template data; full-text search; known operational characteristics |
| **Identity** | ASP.NET Core Identity | Built-in; well-audited; integrates with EF Core; supports role claims and external logins |
| **Logging** | Serilog | Structured logging; multiple sinks (file, console, seq); rich context enrichment |
| **Image Processing** | SixLabors.ImageSharp | Cross-platform; no GDI+ (safe for server); resize, crop, format convert |
| **CSS Framework** | Tailwind CSS (via CDN in Phase 1, local build Phase 2) | Utility-first; fast prototyping; purge-safe with Blazor |
| **Background Jobs** | .NET BackgroundService + DB queue | No extra infrastructure in Phase 1; predictable; can migrate to Hangfire or Azure Service Bus |
| **Rate Limiting** | ASP.NET Core built-in Rate Limiting (net8) + AspNetCoreRateLimit | Login endpoint protection |
| **Data Protection** | ASP.NET Core Data Protection | Encrypt OAuth tokens at rest; standard .NET feature |
| **Email** | MailKit / SmtpClient abstraction | Simple SMTP; can swap to SendGrid |
| **Testing** | xUnit + Moq + FluentAssertions | Industry standard .NET testing stack |
| **Integration Tests** | WebApplicationFactory + Testcontainers (SQL Server) | Full HTTP-level testing; ephemeral DB |

---

## 5. CYHBrand.Core Contents

### 5.1 Entities (`Core/Entities/`)

All entities inherit from `BaseEntity` which provides `Id (Guid)`, `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`.

```
Entities/
  BaseEntity.cs                   ← abstract: Id, CreatedAt, UpdatedAt, CreatedBy, UpdatedBy
  ApplicationUser.cs              ← extends IdentityUser<Guid>
  DoctorProfile.cs
  DoctorBrandSettings.cs
  BannerTemplateCategory.cs
  BannerTemplate.cs
  BannerDesign.cs
  BannerDesignVersion.cs
  GeneratedImageJob.cs
  StoredAsset.cs
  ScheduledPost.cs
  ScheduledPostAttempt.cs
  ConnectedSocialAccount.cs
  PublishingLog.cs
  UsageLedger.cs
  SubscriptionPlan.cs
  UserSubscription.cs
  AuditLog.cs
```

### 5.2 Interfaces (`Core/Interfaces/`)

```
Interfaces/
  Repositories/
    IRepository.cs                ← Generic: GetById, GetAll, Add, Update, Delete, SaveChanges
    IDoctorProfileRepository.cs
    IBannerTemplateRepository.cs
    IBannerDesignRepository.cs
    IGeneratedImageJobRepository.cs
    IScheduledPostRepository.cs
    IConnectedSocialAccountRepository.cs
    IUsageLedgerRepository.cs
    IUserSubscriptionRepository.cs
    IAuditLogRepository.cs
  Services/
    IImageGenerationService.cs    ← AI image generation abstraction
    IStorageService.cs            ← File storage abstraction
    ISocialPublishingService.cs   ← Social media publish abstraction
    IEmailService.cs              ← Email sending abstraction
    ICurrentUserService.cs        ← Gets current authenticated user info
    IUsageService.cs              ← Check/record usage quotas
    ISubscriptionService.cs       ← Plan query, limit checks
    IBackgroundJobQueue.cs        ← Enqueue/dequeue background jobs
    ITemplateRenderService.cs     ← Render template JSON to image
```

### 5.3 DTOs (`Core/DTOs/`)

```
DTOs/
  Auth/
    LoginRequest.cs
    RegisterRequest.cs
    ForgotPasswordRequest.cs
    ResetPasswordRequest.cs
  Doctor/
    DoctorProfileDto.cs
    DoctorProfileUpdateRequest.cs
    DoctorBrandSettingsDto.cs
  Template/
    BannerTemplateSummaryDto.cs
    BannerTemplateDetailDto.cs
    BannerTemplateCreateRequest.cs
    BannerTemplateCategoryDto.cs
  Design/
    BannerDesignDto.cs
    BannerDesignSummaryDto.cs
    BannerDesignSaveRequest.cs
    DesignVersionDto.cs
  AI/
    ImageGenerationRequest.cs
    ImageGenerationResult.cs
    GeneratedImageJobDto.cs
  Publishing/
    PublishRequest.cs
    SchedulePostRequest.cs
    ScheduledPostDto.cs
    PublishingLogDto.cs
  Account/
    ConnectedSocialAccountDto.cs
  Usage/
    UsageSummaryDto.cs
    UsageLedgerEntryDto.cs
  Subscription/
    SubscriptionPlanDto.cs
    UserSubscriptionDto.cs
  Admin/
    AdminDashboardDto.cs
    AdminUserListItemDto.cs
  Common/
    PagedResult.cs                ← PagedResult<T> with Items, TotalCount, Page, PageSize
    Result.cs                     ← Result<T>, Result with IsSuccess, Error
    SortDirection.cs
```

### 5.4 Enums (`Core/Enums/`)

```
Enums/
  UserRole.cs                     ← Doctor, ClinicAdmin, PlatformAdmin, SuperAdmin
  JobStatus.cs                    ← Queued, Processing, Completed, Failed, Cancelled
  DesignStatus.cs                 ← Draft, Saved, Published
  PostStatus.cs                   ← Pending, Scheduled, Published, Failed, Cancelled
  SocialPlatform.cs               ← Instagram, Facebook, LinkedIn, Twitter
  AssetType.cs                    ← ProfilePhoto, ClinicLogo, BannerExport, AIGenerated, TemplateThumb
  UsageActionType.cs              ← AIGeneration, BannerExport, SocialPost, StorageBytes
  SubscriptionTier.cs             ← Free, Pro, Enterprise
  BannerOrientation.cs            ← Square, Landscape, Portrait
  PublishOutcome.cs               ← Success, Failed, RateLimited, AuthError, NetworkError
```

### 5.5 Exceptions (`Core/Exceptions/`)

```
Exceptions/
  DomainException.cs              ← Base domain exception
  NotFoundException.cs            ← Entity not found
  UnauthorizedException.cs        ← Access denied at domain level
  UsageLimitExceededException.cs  ← Quota exceeded
  ValidationException.cs          ← Domain validation failure
```

### 5.6 Specifications (`Core/Specifications/`)

```
Specifications/
  BaseSpecification.cs            ← ISpecification<T> with Criteria, Includes, OrderBy, Paging
  DoctorProfileByUserIdSpec.cs
  ActiveTemplatesByCategorySpec.cs
  DesignsByUserIdSpec.cs
  DueScheduledPostsSpec.cs
  UsageByUserMonthSpec.cs
```

---

## 6. CYHBrand.Infrastructure Contents

### 6.1 Data (`Infrastructure/Data/`)

```
Data/
  AppDbContext.cs                 ← EF Core DbContext; all DbSets; model configuration calls
  Configurations/                 ← One IEntityTypeConfiguration<T> per entity
    ApplicationUserConfiguration.cs
    DoctorProfileConfiguration.cs
    DoctorBrandSettingsConfiguration.cs
    BannerTemplateCategoryConfiguration.cs
    BannerTemplateConfiguration.cs
    BannerDesignConfiguration.cs
    BannerDesignVersionConfiguration.cs
    GeneratedImageJobConfiguration.cs
    StoredAssetConfiguration.cs
    ScheduledPostConfiguration.cs
    ScheduledPostAttemptConfiguration.cs
    ConnectedSocialAccountConfiguration.cs
    PublishingLogConfiguration.cs
    UsageLedgerConfiguration.cs
    SubscriptionPlanConfiguration.cs
    UserSubscriptionConfiguration.cs
    AuditLogConfiguration.cs
  Migrations/                     ← EF Core generated migrations
  Seeds/
    RoleSeeder.cs                 ← Seeds default roles (Doctor, PlatformAdmin, etc.)
    SubscriptionPlanSeeder.cs     ← Seeds Free and Pro plans
    TemplateCategorySeeder.cs     ← Seeds default template categories
```

### 6.2 Repositories (`Infrastructure/Repositories/`)

```
Repositories/
  BaseRepository.cs               ← Generic EF Core implementation of IRepository<T>
  DoctorProfileRepository.cs
  BannerTemplateRepository.cs
  BannerDesignRepository.cs
  GeneratedImageJobRepository.cs
  ScheduledPostRepository.cs
  ConnectedSocialAccountRepository.cs
  UsageLedgerRepository.cs
  UserSubscriptionRepository.cs
  AuditLogRepository.cs
```

### 6.3 Identity (`Infrastructure/Identity/`)

```
Identity/
  ApplicationUserManager.cs       ← Thin extension of UserManager<ApplicationUser>
  RoleInitializer.cs              ← Ensures roles exist at startup
  CurrentUserService.cs           ← ICurrentUserService implementation via IHttpContextAccessor
```

### 6.4 AI (`Infrastructure/AI/`)

```
AI/
  IImageGenerationService.cs      ← (re-referenced from Core; implementations here)
  OpenAI/
    OpenAIImageGenerationService.cs   ← DALL-E 3 via HttpClient
    OpenAIOptions.cs                  ← Configuration POCO
  AzureOpenAI/
    AzureOpenAIImageGenerationService.cs
    AzureOpenAIOptions.cs
  Stubs/
    StubImageGenerationService.cs     ← Returns a placeholder image; no API call
  ImageGenerationServiceFactory.cs    ← Reads config to choose implementation
```

### 6.5 Storage (`Infrastructure/Storage/`)

```
Storage/
  LocalDisk/
    LocalDiskStorageService.cs    ← Stores files to wwwroot/uploads or configured path
    LocalDiskStorageOptions.cs
  AzureBlob/
    AzureBlobStorageService.cs    ← Azure.Storage.Blobs
    AzureBlobStorageOptions.cs
  StorageServiceFactory.cs        ← Selects implementation from config
```

### 6.6 Social Publishing (`Infrastructure/Social/`)

```
Social/
  Instagram/
    InstagramPublishingService.cs
    InstagramOAuthHandler.cs
  Facebook/
    FacebookPublishingService.cs
    FacebookOAuthHandler.cs
  LinkedIn/
    LinkedInPublishingService.cs
    LinkedInOAuthHandler.cs
  Twitter/
    TwitterPublishingService.cs
    TwitterOAuthHandler.cs
  SocialPublishingServiceFactory.cs  ← Returns ISocialPublishingService by SocialPlatform enum
  OAuthTokenEncryptionService.cs     ← Encrypt/decrypt tokens via ASP.NET Core Data Protection
```

### 6.7 Background Jobs (`Infrastructure/BackgroundJobs/`)

```
BackgroundJobs/
  BackgroundJobQueue.cs           ← IBackgroundJobQueue: in-memory Channel<> + DB persistence
  JobWorkerService.cs             ← BackgroundService; dequeues and dispatches jobs
  Handlers/
    ImageGenerationJobHandler.cs  ← Calls IImageGenerationService; updates DB
    SocialPublishJobHandler.cs    ← Calls ISocialPublishingService; updates PublishingLog
  ScheduledPostDispatchService.cs ← BackgroundService; polls ScheduledPost table every 60s
```

### 6.8 Email (`Infrastructure/Email/`)

```
Email/
  SmtpEmailService.cs             ← IEmailService implementation via MailKit
  EmailOptions.cs                 ← SMTP host, port, credentials
  Templates/
    PasswordReset.html
    EmailVerification.html
    PublishFailureNotification.html
```

### 6.9 Image Processing (`Infrastructure/Imaging/`)

```
Imaging/
  ImageSharpRenderService.cs      ← ITemplateRenderService: renders template JSON to PNG/JPEG
  ImageResizeService.cs           ← Profile photo / logo resize
```

### 6.10 DI Registration (`Infrastructure/Extensions/`)

```
Extensions/
  InfrastructureServiceExtensions.cs  ← AddInfrastructure(IServiceCollection, IConfiguration)
    registers: DbContext, Identity, Repositories, AI service, Storage service,
               Background services, Email, ImageSharp, Social services
```

---

## 7. CYHBrand.Web Contents

### 7.1 Structure

```
CYHBrand.Web/
  Program.cs                      ← App entry point; service registration; middleware pipeline
  appsettings.json
  appsettings.Development.json
  Components/
    App.razor                     ← Root component; router
    Routes.razor
    _Imports.razor                ← Global using directives
    Layout/
      MainLayout.razor
      AdminLayout.razor
      AuthLayout.razor            ← Minimal layout for login/register
      Sidebar.razor
      TopNav.razor
      DoctorAvatarMenu.razor
    Shared/                       ← Reusable UI components (see §7.2)
    Pages/                        ← Blazor route pages (see §7.3)
    Features/                     ← Feature-specific component groups (see §7.4)
  Services/                       ← Web-layer services (orchestrate Core + Infrastructure)
    AuthService.cs
    DoctorProfileService.cs
    TemplateService.cs
    DesignService.cs
    AiBannerService.cs
    CalendarService.cs
    PublishingService.cs
    ConnectedAccountService.cs
    UsageService.cs
    AdminService.cs
  Middleware/
    ErrorHandlingMiddleware.cs
    AuditMiddleware.cs
  Extensions/
    WebServiceExtensions.cs       ← AddWebServices(); registers Web-layer services
  wwwroot/
    css/
      app.css                     ← Tailwind output / custom CSS
    js/
      app.js                      ← Global JS helpers; editor interop stubs
      fabric-interop.js           ← Fabric.js canvas interop (Phase 2)
    images/
      logo.svg
      placeholder-banner.png
    lib/                          ← Vendor JS/CSS (Fabric.js, etc.)
    favicon.ico
```

### 7.2 Shared Components (`Components/Shared/`)

```
Shared/
  StatCard.razor
  BannerCard.razor
  TemplateCard.razor
  StatusBadge.razor
  ModalDialog.razor
  ToastNotification.razor
  ToastContainer.razor
  LoadingSpinner.razor
  EmptyState.razor
  ErrorState.razor
  ConfirmDialog.razor
  PageHeader.razor
  SearchBar.razor
  FileUploadControl.razor
  ColorPickerControl.razor
  PaginationControl.razor
  DesignThumbnail.razor
  ProfileCompletionBanner.razor
  UsageQuotaBar.razor
```

### 7.3 Pages (`Components/Pages/`)

```
Pages/
  Auth/
    Login.razor
    Register.razor
    ForgotPassword.razor
    ResetPassword.razor
    AccessDenied.razor
  Dashboard.razor
  Profile/
    ProfileView.razor
    ProfileEdit.razor
  Templates/
    TemplateLibrary.razor
    TemplatePreview.razor
  AI/
    AiBannerCreator.razor
    AiBannerJobStatus.razor
  Editor/
    BannerEditor.razor            ← Main editor page (designId optional)
  Designs/
    MyDesigns.razor
    DesignDetail.razor
  Calendar/
    ContentCalendar.razor
    NewScheduledPost.razor
  Share/
    ShareDesign.razor
  Accounts/
    ConnectedAccounts.razor
  Usage/
    UsageDashboard.razor
  Settings/
    UserSettings.razor
  Help/
    HelpCenter.razor
  Admin/
    AdminDashboard.razor
    Doctors/
      DoctorList.razor
      DoctorDetail.razor
    Templates/
      AdminTemplateList.razor
      AdminTemplateNew.razor
      AdminTemplateEdit.razor
    Categories/
      AdminCategories.razor
    AI/
      AdminAiConfig.razor
    Plans/
      AdminPlans.razor
    Logs/
      AdminAuditLog.razor
      AdminPublishingLog.razor
    Settings/
      AdminSettings.razor
```

### 7.4 Feature Components (`Components/Features/`)

```
Features/
  Editor/
    CanvasComponent.razor         ← JS interop wrapper for canvas
    LayerPanel.razor
    TextToolPanel.razor
    ImageToolPanel.razor
    BackgroundPanel.razor
    EditorToolbar.razor
    ExportPanel.razor
    BrandPresetPanel.razor
  Calendar/
    CalendarGrid.razor
    CalendarDayCell.razor
    PostDetailPanel.razor
  AI/
    PromptInput.razor
    SizeSelector.razor
    JobStatusTracker.razor
  Templates/
    TemplateCategoryFilter.razor
    TemplateGrid.razor
  Designs/
    DesignGrid.razor
    DesignFilterBar.razor
  Publishing/
    PlatformSelector.razor
    CaptionEditor.razor
    ScheduleTimePicker.razor
  Dashboard/
    UpcomingPostsWidget.razor
    UsageSummaryWidget.razor
    RecentDesignsWidget.razor
    QuickActionsWidget.razor
```

---

## 8. Authentication & Authorization Strategy

### 8.1 Authentication

- **Mechanism:** ASP.NET Core Identity with cookie authentication
- **Cookie settings:** `HttpOnly=true`, `Secure=true`, `SameSite=Strict`, sliding expiration 30 min, absolute 8 hours
- **Password policy:** Minimum 8 characters, requires uppercase, digit, non-alphanumeric
- **Email verification:** Required before full access; token generated by `UserManager.GenerateEmailConfirmationTokenAsync`
- **Password reset:** `UserManager.GeneratePasswordResetTokenAsync`; token URL-encoded; valid 1 hour (configurable)
- **Lockout:** Enabled after 5 failed attempts; lockout duration 15 minutes

### 8.2 Authorization Policies

Defined in `Program.cs` via `builder.Services.AddAuthorization(options => { ... })`:

```csharp
// Policies
"IsDoctorOrAbove"     → roles: Doctor, ClinicAdmin, PlatformAdmin, SuperAdmin
"IsClinicAdminOrAbove"→ roles: ClinicAdmin, PlatformAdmin, SuperAdmin
"IsAdminOrAbove"      → roles: PlatformAdmin, SuperAdmin
"IsSuperAdmin"        → roles: SuperAdmin
```

### 8.3 Blazor Authorization

- `<AuthorizeView>` and `<AuthorizeRouteView>` used throughout
- `[Authorize(Policy = "IsAdminOrAbove")]` on admin pages
- Route-level fallback to `/access-denied` for unauthorized access
- `ICurrentUserService` provides authenticated user ID and roles in services

### 8.4 Role Seeding

Roles seeded at application startup via `RoleInitializer.SeedRolesAsync()` using `RoleManager<IdentityRole<Guid>>`.

---

## 9. AI Integration Abstraction

### Interface Definition (`Core/Interfaces/Services/IImageGenerationService.cs`)

```csharp
public interface IImageGenerationService
{
    /// <summary>
    /// Generates an image from the given prompt.
    /// Returns the URL or local path of the generated image.
    /// </summary>
    Task<ImageGenerationResult> GenerateAsync(
        ImageGenerationRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the display name of the active provider (e.g., "OpenAI DALL-E 3").
    /// </summary>
    string ProviderName { get; }
}

public record ImageGenerationRequest(
    string Prompt,              // Enhanced prompt
    string RawUserPrompt,       // Original user input
    int WidthPx,
    int HeightPx,
    string StyleHint,           // "medical", "professional", etc.
    Guid UserId,
    Guid JobId);

public record ImageGenerationResult(
    bool Success,
    string? ImageUrl,           // If provider returns URL
    byte[]? ImageBytes,         // If provider returns bytes
    string? ErrorMessage,
    string? ProviderJobId,      // Provider's own reference ID
    int? EstimatedTokenCost);
```

### Provider Selection

Configured via `appsettings.json` → `AIImageGeneration:Provider` → values: `"OpenAI"`, `"AzureOpenAI"`, `"Stub"`.

`ImageGenerationServiceFactory` reads this config and registers the appropriate implementation as `IImageGenerationService` in DI.

### Prompt Enhancement

Before calling the AI API, the system enhances the user's raw prompt with doctor context:
```
"{UserRawPrompt}. Professional medical social media banner for Dr. {DoctorName}, 
{Specialty} specialist at {ClinicName}. Medical, trustworthy, clean aesthetic. 
Primary color: {HexColor}. No text overlays."
```

---

## 10. Storage Abstraction

### Interface Definition (`Core/Interfaces/Services/IStorageService.cs`)

```csharp
public interface IStorageService
{
    /// <summary>Saves a file and returns a StoredAsset record.</summary>
    Task<StoredAsset> SaveAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        AssetType assetType,
        Guid ownerId,
        CancellationToken cancellationToken = default);

    /// <summary>Returns a URL suitable for use in &lt;img&gt; src or download link.</summary>
    Task<string> GetUrlAsync(StoredAsset asset, CancellationToken cancellationToken = default);

    /// <summary>Returns the file bytes.</summary>
    Task<Stream> GetStreamAsync(StoredAsset asset, CancellationToken cancellationToken = default);

    /// <summary>Deletes the file from storage.</summary>
    Task DeleteAsync(StoredAsset asset, CancellationToken cancellationToken = default);

    /// <summary>Returns total bytes used by a given owner.</summary>
    Task<long> GetTotalBytesAsync(Guid ownerId, CancellationToken cancellationToken = default);
}
```

### Local Disk Implementation

- Base path: `{WebRootPath}/uploads/{ownerId}/`
- File names stored in DB as relative paths
- `GetUrlAsync` returns `/uploads/{ownerId}/{fileName}`

### Azure Blob Implementation

- Container per asset type or per owner
- SAS URLs for private assets (short expiry)
- CDN URL for public template thumbnails

---

## 11. Social Publishing Abstraction

### Interface Definition (`Core/Interfaces/Services/ISocialPublishingService.cs`)

```csharp
public interface ISocialPublishingService
{
    SocialPlatform Platform { get; }

    /// <summary>Publishes an image post immediately.</summary>
    Task<PublishResult> PublishImagePostAsync(
        PublishRequest request,
        CancellationToken cancellationToken = default);

    /// <summary>Returns the OAuth authorization URL to initiate account connection.</summary>
    string GetOAuthAuthorizationUrl(string state, string redirectUri);

    /// <summary>Exchanges authorization code for access/refresh tokens.</summary>
    Task<OAuthTokenSet> ExchangeCodeForTokenAsync(
        string code,
        string redirectUri,
        CancellationToken cancellationToken = default);

    /// <summary>Refreshes an expired access token.</summary>
    Task<OAuthTokenSet> RefreshTokenAsync(
        string refreshToken,
        CancellationToken cancellationToken = default);
}

public record PublishRequest(
    Guid ScheduledPostId,
    Guid ConnectedAccountId,
    string EncryptedAccessToken,
    byte[] ImageBytes,
    string Caption,
    SocialPlatform Platform);

public record PublishResult(
    bool Success,
    string? PlatformPostId,
    string? ErrorMessage,
    PublishOutcome Outcome);

public record OAuthTokenSet(
    string AccessToken,
    string? RefreshToken,
    DateTimeOffset ExpiresAt);
```

### Factory Pattern

`SocialPublishingServiceFactory.GetService(SocialPlatform platform)` returns the correct implementation.

---

## 12. Background Job Strategy

### Phase 1: .NET BackgroundService + DB-backed Channel

#### Job Types

| Job Type | Trigger | Handler |
|----------|---------|---------|
| `ImageGenerationJob` | User submits AI banner request | `ImageGenerationJobHandler` |
| `SocialPublishJob` | Immediate publish or scheduled dispatch | `SocialPublishJobHandler` |

#### Architecture

```
User Action
    │
    ▼
IBackgroundJobQueue.EnqueueAsync(jobDescriptor)
    │
    ├─► Writes JobDescriptor to DB (GeneratedImageJob / ScheduledPost) → Status = Queued
    └─► Pushes to in-memory Channel<JobDescriptor>
                              │
                              ▼
                     JobWorkerService (BackgroundService)
                              │
                     Reads from Channel (up to N concurrent)
                              │
                     Dispatches to appropriate Handler
                              │
                     Handler updates DB with result (Status = Completed/Failed)
                              │
                     Handler writes StoredAsset if image produced
```

#### Scheduled Post Dispatcher

`ScheduledPostDispatchService` runs every 60 seconds:
1. Queries `ScheduledPost` where `ScheduledAtUtc <= UtcNow` and `Status = Scheduled`
2. For each, enqueues a `SocialPublishJob`
3. Updates status to `Processing` to prevent double-dispatch

#### Resilience

- Failed jobs: exponential backoff retry, max 3 attempts, then `Status = Failed`
- Attempt history stored in `ScheduledPostAttempt`
- Crash recovery: on startup, jobs stuck in `Processing` state > 30 min are reset to `Queued`

#### Future Migration Path

The `IBackgroundJobQueue` interface makes it trivial to replace with:
- **Hangfire** (add persistence, dashboard, retry policies)
- **Azure Service Bus** (true distributed queue)

---

## 13. Security Considerations

| Area | Control | Implementation |
|------|---------|----------------|
| Transport | TLS 1.2+ only | IIS/Kestrel config + `UseHttpsRedirection` + HSTS |
| CSRF | Anti-forgery tokens | Blazor server handles this automatically via SignalR |
| SQL Injection | Parameterized queries only | EF Core — no raw SQL |
| XSS | Output encoding | Blazor auto-encodes; `MarkupString` never used with user input |
| Auth tokens | Encrypted at rest | ASP.NET Core Data Protection API |
| AI API keys | Never in source / DB | `dotnet user-secrets` + environment variables |
| File uploads | Type + size validation | Extension whitelist, MIME sniff, ImageSharp re-encode |
| Rate limiting | Login endpoint | `RateLimiter` middleware (fixed window: 5/15min) |
| Audit trail | All sensitive operations | `AuditLog` entity written by `AuditMiddleware` |
| PII in logs | Sensitive fields excluded | Serilog destructuring policies mask email, tokens |
| Admin access | Policy-gated | `[Authorize(Policy = "IsAdminOrAbove")]` |
| Data Protection keys | Persisted to DB | `dp.PersistKeysToDbContext<AppDbContext>()` |

---

## 14. Configuration Management

### Hierarchy (highest priority wins)

```
1. Environment Variables           ← Production secrets
2. dotnet user-secrets             ← Developer secrets (never committed)
3. appsettings.{Environment}.json ← Environment overrides
4. appsettings.json               ← Defaults (no secrets)
```

### appsettings.json Structure

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=VRS-AMITPC\\SQLEXPRESS;Database=CYHBrandDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
  },
  "AIImageGeneration": {
    "Provider": "Stub",
    "OpenAI": {
      "ApiKey": "",
      "Model": "dall-e-3",
      "TimeoutSeconds": 60
    },
    "AzureOpenAI": {
      "Endpoint": "",
      "ApiKey": "",
      "DeploymentName": ""
    }
  },
  "Storage": {
    "Provider": "LocalDisk",
    "LocalDisk": {
      "BasePath": "uploads"
    },
    "AzureBlob": {
      "ConnectionString": "",
      "ContainerName": "cyhbrand-assets"
    }
  },
  "Email": {
    "SmtpHost": "localhost",
    "SmtpPort": 587,
    "UseSsl": false,
    "Username": "",
    "Password": "",
    "FromAddress": "noreply@cyhbrand.com",
    "FromName": "CYHBrand"
  },
  "Social": {
    "Facebook": { "AppId": "", "AppSecret": "" },
    "Instagram": { "AppId": "", "AppSecret": "" },
    "LinkedIn": { "ClientId": "", "ClientSecret": "" },
    "Twitter": { "ClientId": "", "ClientSecret": "" }
  },
  "RateLimiting": {
    "Login": { "PermitLimit": 5, "WindowSeconds": 900 }
  },
  "Serilog": {
    "MinimumLevel": { "Default": "Information" },
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "File", "Args": { "path": "logs/cyhbrand-.log", "rollingInterval": "Day" } }
    ]
  },
  "Features": {
    "SocialPublishing": false,
    "AIBannerCreation": true,
    "PremiumTemplates": false
  }
}
```

### User Secrets (Development) — `dotnet user-secrets set`

```
AIImageGeneration:OpenAI:ApiKey = "sk-..."
Social:Facebook:AppSecret = "..."
Email:Password = "..."
```

### Environment Variables (Production)

Prefix: none required; ASP.NET Core reads `__` as `:` separator.

```
ConnectionStrings__DefaultConnection=...
AIImageGeneration__OpenAI__ApiKey=...
```

---

## 15. Complete Folder Tree

```
d:\Myd\CYHBrand\
│
├── CYHBrand.sln
│
├── src/
│   │
│   ├── CYHBrand.Core/
│   │   ├── CYHBrand.Core.csproj
│   │   ├── Entities/
│   │   │   ├── BaseEntity.cs
│   │   │   ├── ApplicationUser.cs
│   │   │   ├── DoctorProfile.cs
│   │   │   ├── DoctorBrandSettings.cs
│   │   │   ├── BannerTemplateCategory.cs
│   │   │   ├── BannerTemplate.cs
│   │   │   ├── BannerDesign.cs
│   │   │   ├── BannerDesignVersion.cs
│   │   │   ├── GeneratedImageJob.cs
│   │   │   ├── StoredAsset.cs
│   │   │   ├── ScheduledPost.cs
│   │   │   ├── ScheduledPostAttempt.cs
│   │   │   ├── ConnectedSocialAccount.cs
│   │   │   ├── PublishingLog.cs
│   │   │   ├── UsageLedger.cs
│   │   │   ├── SubscriptionPlan.cs
│   │   │   ├── UserSubscription.cs
│   │   │   └── AuditLog.cs
│   │   ├── Interfaces/
│   │   │   ├── Repositories/
│   │   │   │   ├── IRepository.cs
│   │   │   │   ├── IDoctorProfileRepository.cs
│   │   │   │   ├── IBannerTemplateRepository.cs
│   │   │   │   ├── IBannerDesignRepository.cs
│   │   │   │   ├── IGeneratedImageJobRepository.cs
│   │   │   │   ├── IScheduledPostRepository.cs
│   │   │   │   ├── IConnectedSocialAccountRepository.cs
│   │   │   │   ├── IUsageLedgerRepository.cs
│   │   │   │   ├── IUserSubscriptionRepository.cs
│   │   │   │   └── IAuditLogRepository.cs
│   │   │   └── Services/
│   │   │       ├── IImageGenerationService.cs
│   │   │       ├── IStorageService.cs
│   │   │       ├── ISocialPublishingService.cs
│   │   │       ├── IEmailService.cs
│   │   │       ├── ICurrentUserService.cs
│   │   │       ├── IUsageService.cs
│   │   │       ├── ISubscriptionService.cs
│   │   │       ├── IBackgroundJobQueue.cs
│   │   │       └── ITemplateRenderService.cs
│   │   ├── DTOs/
│   │   │   ├── Auth/
│   │   │   ├── Doctor/
│   │   │   ├── Template/
│   │   │   ├── Design/
│   │   │   ├── AI/
│   │   │   ├── Publishing/
│   │   │   ├── Account/
│   │   │   ├── Usage/
│   │   │   ├── Subscription/
│   │   │   ├── Admin/
│   │   │   └── Common/
│   │   ├── Enums/
│   │   │   ├── UserRole.cs
│   │   │   ├── JobStatus.cs
│   │   │   ├── DesignStatus.cs
│   │   │   ├── PostStatus.cs
│   │   │   ├── SocialPlatform.cs
│   │   │   ├── AssetType.cs
│   │   │   ├── UsageActionType.cs
│   │   │   ├── SubscriptionTier.cs
│   │   │   ├── BannerOrientation.cs
│   │   │   └── PublishOutcome.cs
│   │   ├── Exceptions/
│   │   │   ├── DomainException.cs
│   │   │   ├── NotFoundException.cs
│   │   │   ├── UnauthorizedException.cs
│   │   │   ├── UsageLimitExceededException.cs
│   │   │   └── ValidationException.cs
│   │   └── Specifications/
│   │       ├── BaseSpecification.cs
│   │       ├── DoctorProfileByUserIdSpec.cs
│   │       ├── ActiveTemplatesByCategorySpec.cs
│   │       ├── DesignsByUserIdSpec.cs
│   │       ├── DueScheduledPostsSpec.cs
│   │       └── UsageByUserMonthSpec.cs
│   │
│   ├── CYHBrand.Infrastructure/
│   │   ├── CYHBrand.Infrastructure.csproj
│   │   ├── Data/
│   │   │   ├── AppDbContext.cs
│   │   │   ├── Configurations/
│   │   │   ├── Migrations/
│   │   │   └── Seeds/
│   │   ├── Repositories/
│   │   ├── Identity/
│   │   ├── AI/
│   │   │   ├── OpenAI/
│   │   │   ├── AzureOpenAI/
│   │   │   └── Stubs/
│   │   ├── Storage/
│   │   │   ├── LocalDisk/
│   │   │   └── AzureBlob/
│   │   ├── Social/
│   │   │   ├── Instagram/
│   │   │   ├── Facebook/
│   │   │   ├── LinkedIn/
│   │   │   └── Twitter/
│   │   ├── BackgroundJobs/
│   │   │   └── Handlers/
│   │   ├── Email/
│   │   │   └── Templates/
│   │   ├── Imaging/
│   │   └── Extensions/
│   │       └── InfrastructureServiceExtensions.cs
│   │
│   └── CYHBrand.Web/
│       ├── CYHBrand.Web.csproj
│       ├── Program.cs
│       ├── appsettings.json
│       ├── appsettings.Development.json
│       ├── Components/
│       │   ├── App.razor
│       │   ├── Routes.razor
│       │   ├── _Imports.razor
│       │   ├── Layout/
│       │   │   ├── MainLayout.razor
│       │   │   ├── AdminLayout.razor
│       │   │   ├── AuthLayout.razor
│       │   │   ├── Sidebar.razor
│       │   │   ├── TopNav.razor
│       │   │   └── DoctorAvatarMenu.razor
│       │   ├── Shared/
│       │   ├── Pages/
│       │   │   ├── Auth/
│       │   │   ├── Dashboard.razor
│       │   │   ├── Profile/
│       │   │   ├── Templates/
│       │   │   ├── AI/
│       │   │   ├── Editor/
│       │   │   ├── Designs/
│       │   │   ├── Calendar/
│       │   │   ├── Share/
│       │   │   ├── Accounts/
│       │   │   ├── Usage/
│       │   │   ├── Settings/
│       │   │   ├── Help/
│       │   │   └── Admin/
│       │   └── Features/
│       │       ├── Editor/
│       │       ├── Calendar/
│       │       ├── AI/
│       │       ├── Templates/
│       │       ├── Designs/
│       │       ├── Publishing/
│       │       └── Dashboard/
│       ├── Services/
│       ├── Middleware/
│       ├── Extensions/
│       └── wwwroot/
│           ├── css/
│           ├── js/
│           ├── images/
│           ├── lib/
│           └── favicon.ico
│
├── tests/
│   ├── CYHBrand.UnitTests/
│   │   ├── CYHBrand.UnitTests.csproj
│   │   ├── Core/
│   │   │   ├── Entities/
│   │   │   └── Specifications/
│   │   └── Services/
│   │
│   └── CYHBrand.IntegrationTests/
│       ├── CYHBrand.IntegrationTests.csproj
│       ├── Fixtures/
│       │   └── WebApplicationFixture.cs
│       └── Tests/
│           ├── Auth/
│           ├── Templates/
│           └── Designs/
│
└── docs/
    ├── PROJECT_REQUIREMENTS.md
    ├── ARCHITECTURE.md
    ├── DATABASE_DESIGN.md
    ├── UI_PAGE_MAP.md
    └── IMPLEMENTATION_PLAN.md
```
