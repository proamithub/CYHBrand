# CYHBrand — Database Design Document

> **Document Version:** 1.0  
> **Status:** Living Document  
> **Last Updated:** 2026-10-21  
> **Database:** SQL Server 2022  
> **ORM:** Entity Framework Core 8 (Code-First)

---

## Table of Contents

1. [Entity List & Field Definitions](#1-entity-list--field-definitions)
2. [Relationships](#2-relationships)
3. [Index Strategy](#3-index-strategy)
4. [Constraints](#4-constraints)
5. [Sensitive Data Handling](#5-sensitive-data-handling)
6. [EF Core Migration Strategy](#6-ef-core-migration-strategy)

---

## 1. Entity List & Field Definitions

---

### 1.1 ApplicationUser

> Extends `IdentityUser<Guid>`. Identity columns (`Id`, `UserName`, `NormalizedUserName`, `Email`, `NormalizedEmail`, `EmailConfirmed`, `PasswordHash`, `SecurityStamp`, `ConcurrencyStamp`, `PhoneNumber`, `PhoneNumberConfirmed`, `TwoFactorEnabled`, `LockoutEnd`, `LockoutEnabled`, `AccessFailedCount`) are inherited from IdentityUser. Only additional columns listed.

**Table:** `AspNetUsers` (ASP.NET Core Identity default, extended)

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | Primary key (GUID) | PK; default `NEWID()` |
| `Email` | `nvarchar(256)` | NOT NULL | Login email | UNIQUE; inherited |
| `FirstName` | `nvarchar(100)` | NOT NULL | User's first name | |
| `LastName` | `nvarchar(100)` | NOT NULL | User's last name | |
| `IsActive` | `bit` | NOT NULL | Account active flag; admin can set false | DEFAULT 1 |
| `CreatedAt` | `datetimeoffset` | NOT NULL | Account creation timestamp | DEFAULT SYSUTCDATETIME() |
| `LastLoginAt` | `datetimeoffset` | NULL | Timestamp of most recent login | |
| `ProfileSetupComplete` | `bit` | NOT NULL | True after onboarding wizard completed | DEFAULT 0 |
| `AccountDeleteRequestedAt` | `datetimeoffset` | NULL | Set when user requests account deletion | |

---

### 1.2 DoctorProfile

**Table:** `DoctorProfiles`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `UserId` | `uniqueidentifier` | NOT NULL | FK → AspNetUsers.Id | FK; UNIQUE (one profile per user) |
| `DisplayName` | `nvarchar(150)` | NOT NULL | Name shown on banners (e.g., "Dr. Ravi Sharma") | |
| `Specialty` | `nvarchar(150)` | NOT NULL | Medical specialty (e.g., "Cardiologist") | |
| `Qualifications` | `nvarchar(300)` | NULL | Degrees/credentials (e.g., "MBBS, MD, FACC") | |
| `ClinicName` | `nvarchar(200)` | NULL | Name of clinic or hospital | |
| `ClinicAddressLine1` | `nvarchar(250)` | NULL | Address line 1 | |
| `ClinicAddressLine2` | `nvarchar(250)` | NULL | Address line 2 | |
| `City` | `nvarchar(100)` | NULL | City | |
| `State` | `nvarchar(100)` | NULL | State | |
| `PinCode` | `nvarchar(10)` | NULL | Postal / PIN code | |
| `PhoneNumber` | `nvarchar(20)` | NULL | Contact phone for banners | |
| `WebsiteUrl` | `nvarchar(500)` | NULL | Clinic/personal website | |
| `Bio` | `nvarchar(500)` | NULL | Short bio for profile display | Max 500 chars |
| `ProfilePhotoAssetId` | `uniqueidentifier` | NULL | FK → StoredAssets.Id | |
| `ClinicLogoAssetId` | `uniqueidentifier` | NULL | FK → StoredAssets.Id | |
| `CompletionPercent` | `tinyint` | NOT NULL | Calculated profile completeness 0–100 | DEFAULT 0 |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | DEFAULT SYSUTCDATETIME() |
| `UpdatedAt` | `datetimeoffset` | NOT NULL | | DEFAULT SYSUTCDATETIME() |
| `CreatedBy` | `uniqueidentifier` | NOT NULL | UserId who created | |
| `UpdatedBy` | `uniqueidentifier` | NOT NULL | UserId of last updater | |

---

### 1.3 DoctorBrandSettings

**Table:** `DoctorBrandSettings`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `DoctorProfileId` | `uniqueidentifier` | NOT NULL | FK → DoctorProfiles.Id | FK; UNIQUE (1-1 with profile) |
| `PrimaryColorHex` | `char(7)` | NOT NULL | Brand primary color e.g., `#1A73E8` | CHECK LIKE '#[0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f]' |
| `SecondaryColorHex` | `char(7)` | NOT NULL | Brand secondary color | Same CHECK |
| `AccentColorHex` | `char(7)` | NULL | Optional accent color | Same CHECK |
| `PreferredFontFamily` | `nvarchar(100)` | NOT NULL | Font name from allowed list | DEFAULT 'Inter' |
| `DefaultBannerOrientation` | `tinyint` | NOT NULL | 0=Square,1=Landscape,2=Portrait | DEFAULT 0 |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | |
| `UpdatedAt` | `datetimeoffset` | NOT NULL | | |

---

### 1.4 BannerTemplateCategory

**Table:** `BannerTemplateCategories`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `Name` | `nvarchar(100)` | NOT NULL | Category name e.g., "Health Tips" | UNIQUE |
| `Slug` | `nvarchar(120)` | NOT NULL | URL-safe slug e.g., "health-tips" | UNIQUE |
| `Description` | `nvarchar(500)` | NULL | Category description | |
| `IconName` | `nvarchar(100)` | NULL | Icon identifier (CSS class or name) | |
| `SortOrder` | `int` | NOT NULL | Display order | DEFAULT 0 |
| `IsActive` | `bit` | NOT NULL | Visible to users | DEFAULT 1 |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | |
| `UpdatedAt` | `datetimeoffset` | NOT NULL | | |

---

### 1.5 BannerTemplate

**Table:** `BannerTemplates`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `CategoryId` | `uniqueidentifier` | NOT NULL | FK → BannerTemplateCategories.Id | FK |
| `Name` | `nvarchar(200)` | NOT NULL | Template display name | |
| `Description` | `nvarchar(1000)` | NULL | Template description | |
| `Tags` | `nvarchar(500)` | NULL | Comma-separated tags for search | |
| `Orientation` | `tinyint` | NOT NULL | 0=Square,1=Landscape,2=Portrait | |
| `WidthPx` | `int` | NOT NULL | Canvas width in pixels | CHECK > 0 |
| `HeightPx` | `int` | NOT NULL | Canvas height in pixels | CHECK > 0 |
| `ThumbnailAssetId` | `uniqueidentifier` | NULL | FK → StoredAssets.Id (thumbnail image) | |
| `PreviewAssetId` | `uniqueidentifier` | NULL | FK → StoredAssets.Id (full preview) | |
| `TemplateJson` | `nvarchar(max)` | NOT NULL | JSON definition of layers, fonts, placeholders | |
| `IsPremium` | `bit` | NOT NULL | Requires paid subscription | DEFAULT 0 |
| `IsActive` | `bit` | NOT NULL | Visible in library | DEFAULT 1 |
| `UsageCount` | `int` | NOT NULL | Times used; incremented on "Use Template" | DEFAULT 0 |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | |
| `UpdatedAt` | `datetimeoffset` | NOT NULL | | |
| `CreatedBy` | `uniqueidentifier` | NOT NULL | Admin user who created | |
| `UpdatedBy` | `uniqueidentifier` | NOT NULL | Admin user who last edited | |

---

### 1.6 BannerDesign

**Table:** `BannerDesigns`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `OwnerId` | `uniqueidentifier` | NOT NULL | FK → AspNetUsers.Id | FK |
| `Name` | `nvarchar(200)` | NOT NULL | User-given name for the design | DEFAULT 'Untitled Design' |
| `SourceTemplateId` | `uniqueidentifier` | NULL | FK → BannerTemplates.Id (if started from template) | FK; nullable |
| `SourceJobId` | `uniqueidentifier` | NULL | FK → GeneratedImageJobs.Id (if started from AI) | FK; nullable |
| `Status` | `tinyint` | NOT NULL | 0=Draft,1=Saved,2=Published | DEFAULT 0 |
| `WidthPx` | `int` | NOT NULL | Canvas width in pixels | CHECK > 0 |
| `HeightPx` | `int` | NOT NULL | Canvas height in pixels | CHECK > 0 |
| `Orientation` | `tinyint` | NOT NULL | 0=Square,1=Landscape,2=Portrait | |
| `DesignJson` | `nvarchar(max)` | NOT NULL | Current canvas state as JSON | |
| `ThumbnailAssetId` | `uniqueidentifier` | NULL | FK → StoredAssets.Id (last saved thumbnail) | |
| `ExportedAssetId` | `uniqueidentifier` | NULL | FK → StoredAssets.Id (last exported PNG/JPEG) | |
| `IsDeleted` | `bit` | NOT NULL | Soft delete flag | DEFAULT 0 |
| `DeletedAt` | `datetimeoffset` | NULL | Timestamp of soft delete | |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | |
| `UpdatedAt` | `datetimeoffset` | NOT NULL | | |
| `LastAutoSavedAt` | `datetimeoffset` | NULL | Timestamp of last auto-save | |

---

### 1.7 BannerDesignVersion

**Table:** `BannerDesignVersions`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `BannerDesignId` | `uniqueidentifier` | NOT NULL | FK → BannerDesigns.Id | FK |
| `VersionNumber` | `int` | NOT NULL | Sequential version number | |
| `DesignJson` | `nvarchar(max)` | NOT NULL | Snapshot of design JSON at this version | |
| `SaveType` | `tinyint` | NOT NULL | 0=Manual,1=AutoSave | |
| `CreatedAt` | `datetimeoffset` | NOT NULL | When this version was saved | |
| `CreatedBy` | `uniqueidentifier` | NOT NULL | UserId | |

> **Composite UNIQUE:** (`BannerDesignId`, `VersionNumber`)

---

### 1.8 GeneratedImageJob

**Table:** `GeneratedImageJobs`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `UserId` | `uniqueidentifier` | NOT NULL | FK → AspNetUsers.Id | FK |
| `RawPrompt` | `nvarchar(1000)` | NOT NULL | User's original prompt text | |
| `EnhancedPrompt` | `nvarchar(2000)` | NULL | System-enhanced prompt sent to AI | |
| `WidthPx` | `int` | NOT NULL | Requested output width | |
| `HeightPx` | `int` | NOT NULL | Requested output height | |
| `Status` | `tinyint` | NOT NULL | 0=Queued,1=Processing,2=Completed,3=Failed,4=Cancelled | |
| `Provider` | `nvarchar(50)` | NOT NULL | AI provider used e.g., "OpenAI" | |
| `ProviderJobId` | `nvarchar(200)` | NULL | Provider's reference ID | |
| `ResultAssetId` | `uniqueidentifier` | NULL | FK → StoredAssets.Id (generated image) | |
| `ErrorMessage` | `nvarchar(1000)` | NULL | Error detail if failed | |
| `AttemptCount` | `tinyint` | NOT NULL | Number of attempts made | DEFAULT 0 |
| `EstimatedCost` | `decimal(10,6)` | NULL | Estimated API cost in USD | |
| `QueuedAt` | `datetimeoffset` | NOT NULL | When job was queued | DEFAULT SYSUTCDATETIME() |
| `ProcessingStartedAt` | `datetimeoffset` | NULL | When processing began | |
| `CompletedAt` | `datetimeoffset` | NULL | When job completed or failed | |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | |

---

### 1.9 StoredAsset

**Table:** `StoredAssets`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `OwnerId` | `uniqueidentifier` | NOT NULL | FK → AspNetUsers.Id | FK |
| `AssetType` | `tinyint` | NOT NULL | 0=ProfilePhoto,1=ClinicLogo,2=BannerExport,3=AIGenerated,4=TemplateThumb | |
| `FileName` | `nvarchar(500)` | NOT NULL | Storage path / key / relative URL | |
| `OriginalFileName` | `nvarchar(500)` | NULL | Original upload filename | |
| `ContentType` | `nvarchar(100)` | NOT NULL | MIME type e.g., "image/png" | |
| `FileSizeBytes` | `bigint` | NOT NULL | File size in bytes | CHECK >= 0 |
| `StorageProvider` | `nvarchar(50)` | NOT NULL | e.g., "LocalDisk", "AzureBlob" | |
| `IsPublic` | `bit` | NOT NULL | Public URL accessible without auth | DEFAULT 0 |
| `IsDeleted` | `bit` | NOT NULL | Soft delete | DEFAULT 0 |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | |
| `DeletedAt` | `datetimeoffset` | NULL | | |

---

### 1.10 ScheduledPost

**Table:** `ScheduledPosts`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `UserId` | `uniqueidentifier` | NOT NULL | FK → AspNetUsers.Id | FK |
| `BannerDesignId` | `uniqueidentifier` | NOT NULL | FK → BannerDesigns.Id | FK |
| `ConnectedAccountId` | `uniqueidentifier` | NOT NULL | FK → ConnectedSocialAccounts.Id | FK |
| `Platform` | `tinyint` | NOT NULL | 0=Instagram,1=Facebook,2=LinkedIn,3=Twitter | |
| `Status` | `tinyint` | NOT NULL | 0=Pending,1=Scheduled,2=Published,3=Failed,4=Cancelled | |
| `ScheduledAtUtc` | `datetimeoffset` | NOT NULL | When to publish (UTC) | |
| `PublishedAtUtc` | `datetimeoffset` | NULL | When actually published | |
| `Caption` | `nvarchar(3000)` | NULL | Post caption / text | |
| `PlatformPostId` | `nvarchar(200)` | NULL | Platform's post ID after publish | |
| `LastAttemptAt` | `datetimeoffset` | NULL | Timestamp of last publish attempt | |
| `AttemptCount` | `tinyint` | NOT NULL | Total publish attempts | DEFAULT 0 |
| `ErrorMessage` | `nvarchar(1000)` | NULL | Last error detail | |
| `ExportedAssetId` | `uniqueidentifier` | NULL | FK → StoredAssets.Id (image sent to platform) | |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | |
| `UpdatedAt` | `datetimeoffset` | NOT NULL | | |
| `IsDeleted` | `bit` | NOT NULL | | DEFAULT 0 |

---

### 1.11 ScheduledPostAttempt

**Table:** `ScheduledPostAttempts`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `ScheduledPostId` | `uniqueidentifier` | NOT NULL | FK → ScheduledPosts.Id | FK |
| `AttemptNumber` | `tinyint` | NOT NULL | 1-based attempt number | CHECK > 0 |
| `AttemptedAt` | `datetimeoffset` | NOT NULL | When this attempt was made | |
| `Outcome` | `tinyint` | NOT NULL | 0=Success,1=Failed,2=RateLimited,3=AuthError,4=NetworkError | |
| `PlatformResponseCode` | `nvarchar(20)` | NULL | HTTP status or platform error code | |
| `PlatformResponseBody` | `nvarchar(2000)` | NULL | Raw platform response (truncated) | |
| `ErrorMessage` | `nvarchar(1000)` | NULL | Human-readable error | |

---

### 1.12 ConnectedSocialAccount

**Table:** `ConnectedSocialAccounts`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `UserId` | `uniqueidentifier` | NOT NULL | FK → AspNetUsers.Id | FK |
| `Platform` | `tinyint` | NOT NULL | 0=Instagram,1=Facebook,2=LinkedIn,3=Twitter | |
| `PlatformAccountId` | `nvarchar(200)` | NOT NULL | Platform's user/page ID | |
| `PlatformDisplayName` | `nvarchar(200)` | NULL | e.g., "@dr_ravi_sharma" | |
| `PlatformAvatarUrl` | `nvarchar(500)` | NULL | URL to platform profile picture | |
| `EncryptedAccessToken` | `nvarchar(max)` | NOT NULL | Access token encrypted via Data Protection | |
| `EncryptedRefreshToken` | `nvarchar(max)` | NULL | Refresh token encrypted | |
| `TokenExpiresAt` | `datetimeoffset` | NULL | When access token expires | |
| `Scopes` | `nvarchar(500)` | NULL | Granted OAuth scopes | |
| `IsActive` | `bit` | NOT NULL | Connection active (false = needs reconnect) | DEFAULT 1 |
| `ConnectedAt` | `datetimeoffset` | NOT NULL | When account was connected | |
| `LastUsedAt` | `datetimeoffset` | NULL | Last time a post was published via this account | |
| `DisconnectedAt` | `datetimeoffset` | NULL | When disconnected | |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | |

> **Composite UNIQUE:** (`UserId`, `Platform`, `PlatformAccountId`) — prevent duplicate connections

---

### 1.13 PublishingLog

**Table:** `PublishingLogs`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `ScheduledPostId` | `uniqueidentifier` | NOT NULL | FK → ScheduledPosts.Id | FK |
| `UserId` | `uniqueidentifier` | NOT NULL | FK → AspNetUsers.Id | FK (denormalized for query perf) |
| `Platform` | `tinyint` | NOT NULL | Platform published to | |
| `Outcome` | `tinyint` | NOT NULL | 0=Success,1=Failed,2=RateLimited,3=AuthError,4=NetworkError | |
| `PlatformPostId` | `nvarchar(200)` | NULL | Platform post ID (if success) | |
| `ErrorMessage` | `nvarchar(1000)` | NULL | | |
| `PublishedAt` | `datetimeoffset` | NOT NULL | Timestamp of this log entry | |
| `DurationMs` | `int` | NULL | How long the API call took | |

---

### 1.14 UsageLedger

**Table:** `UsageLedger`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `bigint` IDENTITY | NOT NULL | PK (bigint for volume) | PK |
| `UserId` | `uniqueidentifier` | NOT NULL | FK → AspNetUsers.Id | FK |
| `ActionType` | `tinyint` | NOT NULL | 0=AIGeneration,1=BannerExport,2=SocialPost,3=StorageBytes | |
| `Quantity` | `int` | NOT NULL | Units consumed (1 for single actions; bytes for storage) | DEFAULT 1 |
| `PeriodYear` | `smallint` | NOT NULL | Year of action | |
| `PeriodMonth` | `tinyint` | NOT NULL | Month (1–12) | CHECK BETWEEN 1 AND 12 |
| `ReferenceId` | `uniqueidentifier` | NULL | FK to related entity (JobId, PostId, etc.) | |
| `Notes` | `nvarchar(200)` | NULL | Additional context | |
| `RecordedAt` | `datetimeoffset` | NOT NULL | | DEFAULT SYSUTCDATETIME() |

---

### 1.15 SubscriptionPlan

**Table:** `SubscriptionPlans`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `Name` | `nvarchar(100)` | NOT NULL | e.g., "Free", "Pro", "Enterprise" | UNIQUE |
| `Tier` | `tinyint` | NOT NULL | 0=Free,1=Pro,2=Enterprise | |
| `PriceMonthly` | `decimal(10,2)` | NOT NULL | Monthly price in INR (0 for Free) | CHECK >= 0 |
| `PriceAnnual` | `decimal(10,2)` | NOT NULL | Annual price in INR | CHECK >= 0 |
| `MaxAIGenerationsPerMonth` | `int` | NOT NULL | -1 = unlimited | |
| `MaxDesigns` | `int` | NOT NULL | -1 = unlimited | |
| `MaxSocialPostsPerMonth` | `int` | NOT NULL | -1 = unlimited | |
| `MaxStorageMb` | `int` | NOT NULL | Max storage in MB; -1 = unlimited | |
| `CanAccessPremiumTemplates` | `bit` | NOT NULL | | DEFAULT 0 |
| `CanConnectSocialAccounts` | `bit` | NOT NULL | | DEFAULT 0 |
| `SupportLevel` | `nvarchar(50)` | NOT NULL | e.g., "Community", "Email", "Priority" | |
| `IsActive` | `bit` | NOT NULL | Visible for new subscriptions | DEFAULT 1 |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | |
| `UpdatedAt` | `datetimeoffset` | NOT NULL | | |

---

### 1.16 UserSubscription

**Table:** `UserSubscriptions`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `uniqueidentifier` | NOT NULL | PK | PK |
| `UserId` | `uniqueidentifier` | NOT NULL | FK → AspNetUsers.Id | FK |
| `PlanId` | `uniqueidentifier` | NOT NULL | FK → SubscriptionPlans.Id | FK |
| `Status` | `tinyint` | NOT NULL | 0=Active,1=Expired,2=Cancelled,3=Trial | |
| `StartedAt` | `datetimeoffset` | NOT NULL | Subscription start | |
| `ExpiresAt` | `datetimeoffset` | NULL | Subscription end; NULL = no expiry (Free) | |
| `CancelledAt` | `datetimeoffset` | NULL | When user cancelled | |
| `PaymentReference` | `nvarchar(200)` | NULL | Payment gateway transaction ID (Phase 6+) | |
| `BillingCycle` | `tinyint` | NULL | 0=Monthly,1=Annual | |
| `CreatedAt` | `datetimeoffset` | NOT NULL | | |
| `UpdatedAt` | `datetimeoffset` | NOT NULL | | |

> Only one `Active` subscription per user enforced at application layer + partial unique index.

---

### 1.17 AuditLog

**Table:** `AuditLogs`

| Field | Type | Nullable | Description | Constraints |
|-------|------|----------|-------------|-------------|
| `Id` | `bigint` IDENTITY | NOT NULL | PK | PK |
| `UserId` | `uniqueidentifier` | NULL | FK → AspNetUsers.Id (null for anonymous) | |
| `UserEmail` | `nvarchar(256)` | NULL | Snapshot of email at time of action | |
| `Action` | `nvarchar(100)` | NOT NULL | e.g., "User.Login", "Design.Delete", "Admin.LockUser" | |
| `EntityType` | `nvarchar(100)` | NULL | e.g., "BannerDesign" | |
| `EntityId` | `nvarchar(100)` | NULL | Entity primary key (as string for flexibility) | |
| `OldValues` | `nvarchar(max)` | NULL | JSON snapshot of old state | |
| `NewValues` | `nvarchar(max)` | NULL | JSON snapshot of new state | |
| `IpAddress` | `nvarchar(50)` | NULL | Client IP | |
| `UserAgent` | `nvarchar(500)` | NULL | Browser user agent | |
| `CorrelationId` | `nvarchar(50)` | NULL | Request correlation ID | |
| `OccurredAt` | `datetimeoffset` | NOT NULL | | DEFAULT SYSUTCDATETIME() |

---

## 2. Relationships

### 2.1 One-to-One Relationships

| Parent | Child | FK Column | Notes |
|--------|-------|-----------|-------|
| `ApplicationUser` | `DoctorProfile` | `DoctorProfile.UserId` | One account → one profile |
| `DoctorProfile` | `DoctorBrandSettings` | `DoctorBrandSettings.DoctorProfileId` | One profile → one brand settings |

### 2.2 One-to-Many Relationships

| Parent | Child | FK Column | Notes |
|--------|-------|-----------|-------|
| `ApplicationUser` | `BannerDesign[]` | `BannerDesign.OwnerId` | Doctor owns many designs |
| `BannerDesign` | `BannerDesignVersion[]` | `BannerDesignVersion.BannerDesignId` | Design has many versions |
| `ApplicationUser` | `GeneratedImageJob[]` | `GeneratedImageJob.UserId` | User has many AI jobs |
| `ApplicationUser` | `ScheduledPost[]` | `ScheduledPost.UserId` | User has many scheduled posts |
| `ScheduledPost` | `ScheduledPostAttempt[]` | `ScheduledPostAttempt.ScheduledPostId` | Post has many publish attempts |
| `ScheduledPost` | `PublishingLog[]` | `PublishingLog.ScheduledPostId` | Post has many log entries |
| `ApplicationUser` | `ConnectedSocialAccount[]` | `ConnectedSocialAccount.UserId` | User has many social connections |
| `ApplicationUser` | `UsageLedger[]` | `UsageLedger.UserId` | User has many usage records |
| `ApplicationUser` | `UserSubscription[]` | `UserSubscription.UserId` | User has many subscriptions (history) |
| `SubscriptionPlan` | `UserSubscription[]` | `UserSubscription.PlanId` | Plan has many subscribers |
| `BannerTemplateCategory` | `BannerTemplate[]` | `BannerTemplate.CategoryId` | Category has many templates |
| `BannerTemplate` | `BannerDesign[]` | `BannerDesign.SourceTemplateId` | Template can source many designs |
| `GeneratedImageJob` | `BannerDesign[]` | `BannerDesign.SourceJobId` | AI job can source many designs |
| `ApplicationUser` | `StoredAsset[]` | `StoredAsset.OwnerId` | User owns many assets |

### 2.3 Cascade Behavior

| Relationship | On Delete | Rationale |
|-------------|-----------|-----------|
| `User → DoctorProfile` | Restrict | Protect data; require explicit cleanup |
| `User → BannerDesign` | Restrict | Soft-delete designs before user deletion |
| `Design → DesignVersion` | Cascade | Versions meaningless without parent design |
| `ScheduledPost → Attempt` | Cascade | Attempts meaningless without parent post |
| `ScheduledPost → PublishingLog` | Restrict | Logs needed for audit even after post deleted |
| `User → StoredAsset` | Restrict | Assets may be referenced; cleanup explicitly |

---

## 3. Index Strategy

### 3.1 Primary Indexes (clustered on PK)

All tables use GUID PKs. For high-volume tables (`UsageLedger`, `AuditLogs`) with `bigint IDENTITY` PKs, the clustered index is on the identity column; GUIDs in other tables are non-clustered PKs.

### 3.2 Non-Clustered Indexes

| Table | Index Name | Columns | Purpose |
|-------|-----------|---------|---------|
| `AspNetUsers` | `IX_Users_Email` | `NormalizedEmail` | Login lookup |
| `DoctorProfiles` | `IX_DoctorProfile_UserId` | `UserId` | User → profile lookup |
| `BannerTemplates` | `IX_Template_Category` | `CategoryId, IsActive` | Library filter |
| `BannerTemplates` | `IX_Template_Tags` | `Tags` (FTS) | Full-text search |
| `BannerDesigns` | `IX_Design_Owner` | `OwnerId, IsDeleted` | My Designs page |
| `BannerDesigns` | `IX_Design_Owner_Status` | `OwnerId, Status, IsDeleted` | Filter by status |
| `BannerDesignVersions` | `IX_Version_Design` | `BannerDesignId, VersionNumber DESC` | Latest version lookup |
| `GeneratedImageJobs` | `IX_Job_User` | `UserId, Status` | User's job list |
| `GeneratedImageJobs` | `IX_Job_Status_Queued` | `Status, QueuedAt` | Job worker dispatch |
| `ScheduledPosts` | `IX_Post_User` | `UserId, Status, IsDeleted` | Calendar page |
| `ScheduledPosts` | `IX_Post_Due` | `Status, ScheduledAtUtc` | Dispatcher poll |
| `ConnectedSocialAccounts` | `IX_Social_User_Platform` | `UserId, Platform, IsActive` | Connected accounts list |
| `UsageLedger` | `IX_Usage_User_Period` | `UserId, PeriodYear, PeriodMonth, ActionType` | Monthly usage totals |
| `UserSubscriptions` | `IX_Sub_User_Active` | `UserId, Status` | Current subscription lookup |
| `AuditLogs` | `IX_Audit_User` | `UserId, OccurredAt DESC` | User audit trail |
| `AuditLogs` | `IX_Audit_Action` | `Action, OccurredAt DESC` | Filter by action type |
| `PublishingLogs` | `IX_PubLog_User` | `UserId, PublishedAt DESC` | User's publish history |

### 3.3 Full-Text Search

Enable SQL Server Full-Text Search on:
- `BannerTemplates.Tags` and `BannerTemplates.Name` → template search
- `BannerDesigns.Name` → design search

---

## 4. Constraints

### 4.1 Unique Constraints

| Table | Constraint | Columns |
|-------|-----------|---------|
| `DoctorProfiles` | `UQ_DoctorProfile_UserId` | `UserId` |
| `DoctorBrandSettings` | `UQ_BrandSettings_ProfileId` | `DoctorProfileId` |
| `BannerTemplateCategories` | `UQ_Category_Name` | `Name` |
| `BannerTemplateCategories` | `UQ_Category_Slug` | `Slug` |
| `BannerDesignVersions` | `UQ_Version_Design_Number` | `BannerDesignId, VersionNumber` |
| `ConnectedSocialAccounts` | `UQ_Social_User_Platform_Account` | `UserId, Platform, PlatformAccountId` |
| `SubscriptionPlans` | `UQ_Plan_Name` | `Name` |

### 4.2 Check Constraints

| Table | Constraint | Expression |
|-------|-----------|------------|
| `DoctorBrandSettings` | `CHK_PrimaryColor` | `PrimaryColorHex LIKE '#[0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f][0-9A-Fa-f]'` |
| `BannerTemplates` | `CHK_Template_Dims` | `WidthPx > 0 AND HeightPx > 0` |
| `BannerDesigns` | `CHK_Design_Dims` | `WidthPx > 0 AND HeightPx > 0` |
| `StoredAssets` | `CHK_Asset_Size` | `FileSizeBytes >= 0` |
| `SubscriptionPlans` | `CHK_Plan_Price` | `PriceMonthly >= 0 AND PriceAnnual >= 0` |
| `UsageLedger` | `CHK_Usage_Month` | `PeriodMonth BETWEEN 1 AND 12` |
| `ScheduledPostAttempts` | `CHK_Attempt_Number` | `AttemptNumber > 0` |

### 4.3 Partial Unique Index (Active Subscription)

```sql
CREATE UNIQUE INDEX UQ_User_ActiveSubscription
ON UserSubscriptions (UserId)
WHERE Status = 0; -- 0 = Active
```

---

## 5. Sensitive Data Handling

### 5.1 Social OAuth Tokens

The `EncryptedAccessToken` and `EncryptedRefreshToken` columns in `ConnectedSocialAccounts` store tokens encrypted using **ASP.NET Core Data Protection API**.

```csharp
// OAuthTokenEncryptionService.cs
public class OAuthTokenEncryptionService(IDataProtectionProvider provider)
{
    private readonly IDataProtector _protector = 
        provider.CreateProtector("CYHBrand.SocialTokens.v1");

    public string Encrypt(string plaintext) => _protector.Protect(plaintext);
    public string Decrypt(string ciphertext) => _protector.Unprotect(ciphertext);
}
```

**Data Protection key storage:** Keys persisted to `DataProtectionKeys` table in SQL Server via:

```csharp
services.AddDataProtection()
    .PersistKeysToDbContext<AppDbContext>()
    .SetApplicationName("CYHBrand");
```

This requires the `Microsoft.AspNetCore.DataProtection.EntityFrameworkCore` NuGet package.

### 5.2 What Is NOT Stored in the Database

| Sensitive Item | Where It Lives |
|---------------|----------------|
| AI API keys | `appsettings` / User Secrets / environment variable |
| Social OAuth App Secret | `appsettings` / environment variable |
| Email SMTP password | `appsettings` / environment variable |
| User passwords | Hashed by ASP.NET Core Identity (PBKDF2) |

### 5.3 PII Considerations

| Data | Classification | Handling |
|------|---------------|---------|
| Email address | PII | Stored normalized; excluded from logs |
| Doctor name | PII | Stored in profile; not logged raw |
| Phone number | PII | Optional; not indexed |
| Clinic address | PII/Business | Optional |
| Audit log entries | PII possible | `OldValues`/`NewValues` exclude passwords; admin-only access |
| AI prompts | PII possible | Stored for 90 days, then anonymized |

---

## 6. EF Core Migration Strategy

### 6.1 Workflow

```
1. Make entity / configuration changes in CYHBrand.Core / CYHBrand.Infrastructure
2. Add migration:
   dotnet ef migrations add <MigrationName> --project CYHBrand.Infrastructure --startup-project CYHBrand.Web
3. Review generated migration .cs file BEFORE applying
4. Apply to dev DB:
   dotnet ef database update --project CYHBrand.Infrastructure --startup-project CYHBrand.Web
5. For staging/production: generate SQL script and apply via DBA review
   dotnet ef migrations script --idempotent -o migrations/deploy_<version>.sql --project CYHBrand.Infrastructure --startup-project CYHBrand.Web
```

### 6.2 Initial Migration Checklist

The first migration (`InitialCreate`) must include:
- [ ] All ASP.NET Core Identity tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens`, `AspNetRoleClaims`)
- [ ] `DataProtectionKeys` table (from `Microsoft.AspNetCore.DataProtection.EntityFrameworkCore`)
- [ ] All CYHBrand domain tables listed in §1
- [ ] All indexes listed in §3
- [ ] All check constraints listed in §4
- [ ] Seed data: default roles, Free and Pro subscription plans, initial template categories

### 6.3 Startup Migration Application

In `Program.cs` (development only):

```csharp
if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync(); // Apply pending migrations
    await SeedData.SeedAsync(scope.ServiceProvider); // Run seeders
}
```

### 6.4 Production Deployment

- Never use `EnsureCreated()` in production
- Use idempotent SQL script generated by `dotnet ef migrations script --idempotent`
- Run as part of deployment pipeline (GitHub Actions / Azure DevOps) before starting new app version
- Migration history tracked in `__EFMigrationsHistory` table

### 6.5 Conventions Configured in AppDbContext

```csharp
protected override void OnModelCreating(ModelBuilder builder)
{
    base.OnModelCreating(builder); // Must call for Identity

    // Apply all IEntityTypeConfiguration<T> from Infrastructure assembly
    builder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

    // Global query filter: soft-delete
    builder.Entity<BannerDesign>().HasQueryFilter(d => !d.IsDeleted);
    builder.Entity<StoredAsset>().HasQueryFilter(a => !a.IsDeleted);
    builder.Entity<ScheduledPost>().HasQueryFilter(p => !p.IsDeleted);
}
```

### 6.6 Naming Conventions

| Item | Convention |
|------|-----------|
| Tables | PascalCase plural (e.g., `BannerDesigns`) |
| Columns | PascalCase |
| Identity tables | Default ASP.NET Core names (`AspNetUsers`, etc.) |
| FKs | `{EntityName}Id` (e.g., `OwnerId`, `CategoryId`) |
| Indexes | `IX_{Table}_{Column(s)}` |
| Check constraints | `CHK_{Table}_{Description}` |
| Unique constraints | `UQ_{Table}_{Column(s)}` |
