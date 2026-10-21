# CYHBrand — UI Page Map & Component Inventory

> **Document Version:** 1.0  
> **Status:** Living Document  
> **Last Updated:** 2026-10-21  
> **Framework:** Blazor Web App (Interactive Server, .NET 8)

---

## Table of Contents

1. [Page Inventory](#1-page-inventory)
2. [Shared Components](#2-shared-components)
3. [Feature Components](#3-feature-components)
4. [Layout Components](#4-layout-components)
5. [Design System](#5-design-system)
6. [Navigation & Routing Notes](#6-navigation--routing-notes)

---

## 1. Page Inventory

### Legend

| Column | Meaning |
|--------|---------|
| Route | Blazor `@page` directive route |
| File Path | Path relative to `src/CYHBrand.Web/Components/Pages/` |
| Description | What the page does |
| Roles | Minimum required role(s) |
| Layout | Which layout component wraps this page |
| Interactive | Blazor render mode |

---

### 1.1 Public / Auth Pages

| Route | File Path | Description | Roles | Layout | Render Mode |
|-------|-----------|-------------|-------|--------|-------------|
| `/` | `Index.razor` | Root redirect — sends authenticated users to `/dashboard`, unauthenticated to `/login` | Any | `AuthLayout` | Static SSR |
| `/login` | `Auth/Login.razor` | Email + password login form; "Forgot password" link; "Register" link | Anonymous | `AuthLayout` | Interactive Server |
| `/register` | `Auth/Register.razor` | Registration form: first name, last name, email, password, confirm; T&C checkbox | Anonymous | `AuthLayout` | Interactive Server |
| `/forgot-password` | `Auth/ForgotPassword.razor` | Enter email to receive reset link; success confirmation shown | Anonymous | `AuthLayout` | Interactive Server |
| `/reset-password` | `Auth/ResetPassword.razor` | Form: new password + confirm; token + email passed as query params | Anonymous | `AuthLayout` | Interactive Server |
| `/verify-email` | `Auth/VerifyEmail.razor` | Handles email confirmation link; shows success or error | Anonymous | `AuthLayout` | Static SSR |
| `/access-denied` | `Auth/AccessDenied.razor` | Shown when user lacks required role; "Go to Dashboard" button | Any | `AuthLayout` | Static SSR |

---

### 1.2 Onboarding

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/onboarding` | `Onboarding/OnboardingWizard.razor` | Multi-step profile setup: (1) Basic info, (2) Clinic, (3) Brand colors/font, (4) Upload photo/logo, (5) Finish. Shown automatically after first login until complete. | Doctor+ | `AuthLayout` (no sidebar) |

---

### 1.3 Main App Pages

#### Dashboard

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/dashboard` | `Dashboard.razor` | Main home: profile completeness banner, quick actions, recent designs, upcoming scheduled posts, AI usage quota widget | Doctor+ | `MainLayout` |

---

#### Profile

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/profile` | `Profile/ProfileView.razor` | View-only doctor profile card: photo, name, specialty, clinic, contact, bio. Edit button. | Doctor+ | `MainLayout` |
| `/profile/edit` | `Profile/ProfileEdit.razor` | Full edit form for doctor profile + brand settings (colors, font, logo, photo upload) | Doctor+ | `MainLayout` |

---

#### Template Library

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/templates` | `Templates/TemplateLibrary.razor` | Paginated grid of banner templates. Left sidebar: category filter, orientation filter. Top: search bar, sort. Each card: thumbnail, name, category badge, "Use Template" CTA. | Doctor+ | `MainLayout` |
| `/templates/{id}/preview` | `Templates/TemplatePreview.razor` | Full-screen preview of a template. Shows larger preview image, name, description, tags, dimensions. CTA: "Use This Template" → navigates to `/editor?templateId={id}` | Doctor+ | `MainLayout` |

---

#### AI Banner Creator

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/ai-banner` | `AI/AiBannerCreator.razor` | AI banner creation form: prompt textarea, size selector (Square/Landscape/Portrait), brand context preview, usage quota indicator, "Generate" button. Validates quota before submitting. | Doctor+ | `MainLayout` |
| `/ai-banner/{jobId}/status` | `AI/AiBannerJobStatus.razor` | Live status polling page for a specific AI job. Shows: job status badge (Queued/Processing/Completed/Failed), progress indicator, preview of generated image when ready, CTA: "Open in Editor" / "Regenerate". | Doctor+ | `MainLayout` |

---

#### Banner Editor

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/editor` | `Editor/BannerEditor.razor` | New blank design editor | Doctor+ | `MainLayout` (full-width, no inner padding) |
| `/editor?templateId={id}` | `Editor/BannerEditor.razor` | Editor pre-loaded with template | Doctor+ | `MainLayout` |
| `/editor?jobId={id}` | `Editor/BannerEditor.razor` | Editor with AI-generated image as background layer | Doctor+ | `MainLayout` |
| `/editor/{designId}` | `Editor/BannerEditor.razor` | Editor with existing saved design | Doctor+ | `MainLayout` |

**Editor Layout:** Full viewport height. Toolbar top. Left panel: tool selector. Center: canvas. Right panel: context-sensitive properties. Bottom: layer list.

---

#### My Designs

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/designs` | `Designs/MyDesigns.razor` | Grid of saved designs. Filter bar: All / Draft / Saved / Published. Search by name. Cards: thumbnail, name, status badge, date, action menu (Edit, Duplicate, Download, Share, Delete). | Doctor+ | `MainLayout` |
| `/designs/{id}` | `Designs/DesignDetail.razor` | Detail view of a single design: large preview, name (editable inline), status, created/updated dates, version history list, export button, share button, open in editor button. | Doctor+ | `MainLayout` |

---

#### Content Calendar

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/calendar` | `Calendar/ContentCalendar.razor` | Monthly calendar grid. Each day cell shows scheduled posts as colored chips (color = status). Month nav. "New Post" button. Clicking a chip shows a side panel with post details. | Doctor+ | `MainLayout` |
| `/calendar/new` | `Calendar/NewScheduledPost.razor` | Create new scheduled post: design picker, platform selector (multi-select from connected accounts), caption input, date/time picker. | Doctor+ | `MainLayout` |
| `/calendar/edit/{postId}` | `Calendar/EditScheduledPost.razor` | Edit pending/scheduled post: change date/time, caption, platforms. | Doctor+ | `MainLayout` |

---

#### Share & Publish

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/share/{designId}` | `Share/ShareDesign.razor` | Share options for a specific design. Shows design preview. Tabs: "Publish Now" / "Schedule". Platform selector (shows connected accounts with status indicators). Caption input per platform. Publish / Schedule button. Download button. | Doctor+ | `MainLayout` |

---

#### Connected Accounts

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/connected-accounts` | `Accounts/ConnectedAccounts.razor` | List connected social accounts (platform icon, display name, connected date, status badge: Active/Expired/Disconnected). "Connect" buttons for each platform. "Disconnect" action. | Doctor+ | `MainLayout` |
| `/connected-accounts/callback/{platform}` | `Accounts/OAuthCallback.razor` | OAuth redirect handler; exchanges code for token; stores; redirects to `/connected-accounts` with result toast. | Doctor+ | `MainLayout` |

---

#### Usage & Subscription

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/usage` | `Usage/UsageDashboard.razor` | Detailed usage page: current plan banner, per-metric quota bars (AI Generations, Designs, Social Posts, Storage), monthly history table, "Upgrade Plan" CTA. | Doctor+ | `MainLayout` |

---

#### Settings

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/settings` | `Settings/UserSettings.razor` | Tabbed settings: Account (change email, change password), Notifications (email preferences), Danger Zone (request account deletion). Current plan badge + link to `/usage`. | Doctor+ | `MainLayout` |

---

#### Help & Support

| Route | File Path | Description | Roles | Layout |
|-------|-----------|-------------|-------|--------|
| `/help` | `Help/HelpCenter.razor` | FAQ accordion sections per module. Getting started section at top. Contact support form at bottom. | Doctor+ | `MainLayout` |

---

### 1.4 Admin Pages

All admin pages require `Policy = "IsAdminOrAbove"` and use `AdminLayout`.

| Route | File Path | Description |
|-------|-----------|-------------|
| `/admin` | `Admin/AdminDashboard.razor` | Admin overview: KPI stat cards (total users, active subscriptions, AI jobs today, posts published today, failed posts), recent audit log table, system health indicator |
| `/admin/doctors` | `Admin/Doctors/DoctorList.razor` | Paginated list of all doctor accounts. Columns: name, email, specialty, plan, status (active/locked), joined date. Search + filter. Click row → detail. |
| `/admin/doctors/{id}` | `Admin/Doctors/DoctorDetail.razor` | Full doctor detail: profile info, brand settings (read-only preview), subscription info, usage stats, AI jobs list, designs count. Actions: Lock Account, Unlock Account, Edit Profile. |
| `/admin/templates` | `Admin/Templates/AdminTemplateList.razor` | Manage templates: data grid with name, category, orientation, premium flag, active flag, usage count, dates. Actions: Edit, Activate/Deactivate, Delete. "New Template" button. |
| `/admin/templates/new` | `Admin/Templates/AdminTemplateNew.razor` | Create new template form: name, category dropdown, description, tags, orientation selector, dimension inputs, JSON editor (Monaco or textarea), thumbnail upload, preview image upload, premium toggle. |
| `/admin/templates/{id}/edit` | `Admin/Templates/AdminTemplateEdit.razor` | Edit existing template; same form as new. Shows usage count and list of designs using this template. |
| `/admin/categories` | `Admin/Categories/AdminCategories.razor` | Manage template categories: inline editable data grid. Add / reorder (drag handle) / delete categories. |
| `/admin/ai-config` | `Admin/AI/AdminAiConfig.razor` | AI configuration: active provider selector (OpenAI / AzureOpenAI / Stub), API key status indicator (masked; shows last 4 chars), test connection button, total AI generations this month, estimated cost. |
| `/admin/plans` | `Admin/Plans/AdminPlans.razor` | Manage subscription plans: list of plans with limits. Edit plan limits inline (not price in Phase 1). |
| `/admin/logs` | `Admin/Logs/AdminAuditLog.razor` | Searchable, filterable audit log table. Filters: user, action type, date range. Expandable row shows old/new values JSON diff. |
| `/admin/publishing-logs` | `Admin/Logs/AdminPublishingLog.razor` | Publishing log with filters: platform, outcome (Success/Failed), date range, user. |
| `/admin/settings` | `Admin/Settings/AdminSettings.razor` | System settings: maintenance mode toggle, default subscription plan selector, email sender name/address, feature flags toggles (AI enabled, premium templates, social publishing). |

---

## 2. Shared Components

### Location: `Components/Shared/`

---

#### `StatCard.razor`

Renders a KPI card with an icon, label, value, and optional trend indicator.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Title` | `string` | Card title (e.g., "AI Generations") |
| `Value` | `string` | Display value (e.g., "12/50") |
| `Icon` | `string` | Icon class or SVG path name |
| `TrendDirection` | `TrendDirection?` | Up/Down/Neutral enum |
| `TrendLabel` | `string?` | e.g., "+5 this week" |
| `ColorVariant` | `string` | "blue" \| "green" \| "red" \| "gray" |
| `LinkTo` | `string?` | Optional navigation URL on click |

---

#### `BannerCard.razor`

Renders a design tile in the My Designs grid.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Design` | `BannerDesignSummaryDto` | Design data |
| `OnEdit` | `EventCallback<Guid>` | Edit action |
| `OnDuplicate` | `EventCallback<Guid>` | Duplicate action |
| `OnDelete` | `EventCallback<Guid>` | Delete action (triggers confirm dialog) |
| `OnDownload` | `EventCallback<Guid>` | Download PNG |
| `OnShare` | `EventCallback<Guid>` | Navigate to share page |

**Renders:** Thumbnail image, design name, `StatusBadge`, date, 3-dot action menu.

---

#### `TemplateCard.razor`

Renders a template tile in the Template Library grid.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Template` | `BannerTemplateSummaryDto` | Template data |
| `OnUse` | `EventCallback<Guid>` | Opens editor with this template |
| `OnPreview` | `EventCallback<Guid>` | Opens preview modal or navigates to preview page |

**Renders:** Thumbnail, name, category badge, Premium badge (if applicable), "Use This Template" button on hover.

---

#### `StatusBadge.razor`

Renders a colored pill badge based on a status enum.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Status` | `object` | Any status enum value (JobStatus, PostStatus, DesignStatus) |
| `Size` | `string` | "sm" \| "md" (default "md") |

**Color mapping:**
- `Completed` / `Published` / `Active` → green
- `Queued` / `Scheduled` / `Pending` → blue
- `Processing` → indigo (animated pulse)
- `Failed` / `Expired` → red
- `Draft` / `Cancelled` → gray

---

#### `ModalDialog.razor`

Generic modal overlay with header, body, and footer slots.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `IsOpen` | `bool` | Visibility state |
| `Title` | `string` | Modal header title |
| `Size` | `string` | "sm" \| "md" \| "lg" \| "xl" (default "md") |
| `ChildContent` | `RenderFragment` | Modal body |
| `FooterContent` | `RenderFragment?` | Optional footer buttons |
| `OnClose` | `EventCallback` | Called when backdrop or X clicked |
| `PreventBackdropClose` | `bool` | Disable backdrop click close |

---

#### `ToastNotification.razor` + `ToastContainer.razor`

`ToastContainer` is placed once in `MainLayout`. Individual `ToastNotification` items are rendered inside it.

**ToastService (singleton):** `ShowSuccess()`, `ShowError()`, `ShowWarning()`, `ShowInfo()` — each accepts `message` and optional `durationMs` (default 4000).

**Parameters (ToastNotification):**
| Param | Type | Description |
|-------|------|-------------|
| `Message` | `string` | Toast text |
| `Type` | `ToastType` | Success, Error, Warning, Info |
| `DurationMs` | `int` | Auto-dismiss delay |
| `OnDismiss` | `EventCallback` | Called on dismiss |

---

#### `LoadingSpinner.razor`

Centered animated spinner with optional overlay.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `IsLoading` | `bool` | Show/hide |
| `FullPage` | `bool` | Full viewport overlay vs inline |
| `Message` | `string?` | Optional loading text below spinner |

---

#### `EmptyState.razor`

Displayed when a list or grid has zero items.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Icon` | `string` | Illustration/icon name |
| `Title` | `string` | Primary message (e.g., "No designs yet") |
| `Description` | `string?` | Secondary text |
| `ActionLabel` | `string?` | CTA button text |
| `OnAction` | `EventCallback?` | CTA callback |

---

#### `ErrorState.razor`

Displayed when a page or component encounters an error.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Title` | `string` | e.g., "Something went wrong" |
| `Message` | `string?` | Error detail |
| `OnRetry` | `EventCallback?` | Retry callback |
| `ShowHomeLink` | `bool` | Show "Go to Dashboard" link |

---

#### `ConfirmDialog.razor`

Modal confirmation dialog for destructive actions.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `IsOpen` | `bool` | Visibility |
| `Title` | `string` | Dialog title (e.g., "Delete Design") |
| `Message` | `string` | Confirmation message body |
| `ConfirmLabel` | `string` | Confirm button text (default "Delete") |
| `ConfirmVariant` | `string` | "danger" \| "primary" |
| `OnConfirm` | `EventCallback` | Confirm callback |
| `OnCancel` | `EventCallback` | Cancel callback |

---

#### `PageHeader.razor`

Consistent page title section with optional breadcrumb and action slot.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Title` | `string` | Page title |
| `Subtitle` | `string?` | Optional subtitle |
| `Breadcrumbs` | `BreadcrumbItem[]?` | Navigation breadcrumbs |
| `Actions` | `RenderFragment?` | Right-side buttons slot |

---

#### `SearchBar.razor`

Debounced search input field.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Placeholder` | `string` | Input placeholder |
| `Value` | `string` | Bound search value |
| `ValueChanged` | `EventCallback<string>` | Value change callback |
| `DebounceMs` | `int` | Debounce delay (default 300ms) |
| `OnSearch` | `EventCallback<string>` | Triggered after debounce |

---

#### `FileUploadControl.razor`

Drag-and-drop + click file upload control.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Accept` | `string` | MIME type filter (e.g., "image/png,image/jpeg") |
| `MaxSizeMb` | `int` | Max file size in MB |
| `OnFileSelected` | `EventCallback<IBrowserFile>` | Called with selected file |
| `PreviewUrl` | `string?` | Existing image preview URL |
| `Label` | `string` | Upload area label |
| `IsLoading` | `bool` | Shows upload progress |

---

#### `ColorPickerControl.razor`

Hex color input with visual swatch preview and preset color palette.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Value` | `string` | Hex color value (e.g., "#1A73E8") |
| `ValueChanged` | `EventCallback<string>` | Two-way binding |
| `Label` | `string` | Field label |
| `Presets` | `string[]?` | Optional preset colors |

---

#### `PaginationControl.razor`

Renders pagination controls (prev/next + page numbers).

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `CurrentPage` | `int` | Current page (1-based) |
| `TotalPages` | `int` | Total number of pages |
| `OnPageChanged` | `EventCallback<int>` | Page change callback |
| `PageSize` | `int` | Items per page (display only) |
| `TotalItems` | `int` | Total item count (display only) |

---

#### `DesignThumbnail.razor`

Renders a design's thumbnail with fallback placeholder.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `AssetUrl` | `string?` | Thumbnail URL |
| `AltText` | `string` | Image alt text |
| `AspectRatio` | `string` | "square" \| "landscape" \| "portrait" |
| `CssClass` | `string?` | Additional CSS classes |

---

#### `ProfileCompletionBanner.razor`

Banner shown on dashboard when profile is < 100% complete.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `CompletionPercent` | `int` | 0–100 |
| `MissingFields` | `string[]` | List of incomplete field names |

---

#### `UsageQuotaBar.razor`

Horizontal progress bar showing usage against limit.

**Parameters:**
| Param | Type | Description |
|-------|------|-------------|
| `Label` | `string` | Metric label |
| `Used` | `int` | Current usage |
| `Limit` | `int` | Max limit (-1 = unlimited) |
| `Unit` | `string` | e.g., "generations", "posts", "MB" |
| `WarningThreshold` | `double` | 0.0–1.0; bar turns orange above this (default 0.8) |

---

## 3. Feature Components

### 3.1 Editor Feature (`Components/Features/Editor/`)

| Component | Description |
|-----------|-------------|
| `CanvasComponent.razor` | Wraps the canvas HTML element; JS interop bridge to Fabric.js. Exposes: `LoadDesignAsync(json)`, `GetDesignJsonAsync()`, `ExportPngAsync()`. Raises `DesignChanged` event. |
| `LayerPanel.razor` | List of all layers in the design. Each row: visibility toggle (eye icon), lock toggle, layer name (editable), drag handle. Clicking a row selects that layer on canvas. |
| `TextToolPanel.razor` | Properties panel when a text layer is selected: font family dropdown, size input, bold/italic/underline toggles, color picker, alignment buttons, line height input. |
| `ImageToolPanel.razor` | Properties panel when an image layer is selected: opacity slider, replace image button, fit/fill/center options. |
| `BackgroundPanel.razor` | Background settings: solid color picker, upload background image, clear background. |
| `EditorToolbar.razor` | Top toolbar: Add Text button, Add Image button, Brand Preset button, Undo, Redo, Zoom controls, Save button (with spinner), Export button. |
| `ExportPanel.razor` | Export modal: format selector (PNG/JPEG), resolution (1x/2x), quality slider (JPEG only), download button, share button. |
| `BrandPresetPanel.razor` | Modal/panel showing doctor's brand colors and fonts. "Apply to Design" button sets text layers to brand font and recolors primary elements. |

### 3.2 Calendar Feature (`Components/Features/Calendar/`)

| Component | Description |
|-----------|-------------|
| `CalendarGrid.razor` | Renders the monthly grid. Receives `IEnumerable<ScheduledPostDto>`. Raises `DayClicked` and `PostClicked` events. |
| `CalendarDayCell.razor` | Individual day cell. Renders date number + post chips (truncated with "+N more" if overflow). |
| `PostDetailPanel.razor` | Slide-in side panel showing post details: design thumbnail, caption, platform badges, status badge, scheduled time, edit and cancel buttons. |

### 3.3 AI Feature (`Components/Features/AI/`)

| Component | Description |
|-----------|-------------|
| `PromptInput.razor` | Textarea with character counter (max 1000). Placeholder examples auto-cycling. |
| `SizeSelector.razor` | Three option cards: Square (1080×1080), Landscape (1920×1080), Portrait (1080×1920). Each shows a visual aspect-ratio preview. |
| `JobStatusTracker.razor` | Polls `IGeneratedImageJobRepository` (or SignalR in future). Shows animated status steps: Queued → Processing → Completed. On completion renders image preview. |

### 3.4 Template Feature (`Components/Features/Templates/`)

| Component | Description |
|-----------|-------------|
| `TemplateCategoryFilter.razor` | Left sidebar accordion filter. Category list with counts. "All" selected by default. Emits `CategorySelected` event. |
| `TemplateGrid.razor` | Responsive grid rendering `TemplateCard` items. Handles empty state. |

### 3.5 Designs Feature (`Components/Features/Designs/`)

| Component | Description |
|-----------|-------------|
| `DesignGrid.razor` | Responsive grid of `BannerCard` items. Handles empty state (different messages for "no designs" vs "no results"). |
| `DesignFilterBar.razor` | Tab bar (All / Draft / Saved / Published / Deleted) + search bar. Emits filter change events. |

### 3.6 Publishing Feature (`Components/Features/Publishing/`)

| Component | Description |
|-----------|-------------|
| `PlatformSelector.razor` | Grid of connected account cards with checkboxes. Shows platform icon, display name, account status badge. |
| `CaptionEditor.razor` | Textarea with character count. If multiple platforms selected, shows tab per platform for individual captions. Warns when over platform limit. |
| `ScheduleTimePicker.razor` | Date picker + time picker (doctor's local timezone). Shows "Publish Now" toggle. |

### 3.7 Dashboard Feature (`Components/Features/Dashboard/`)

| Component | Description |
|-----------|-------------|
| `UpcomingPostsWidget.razor` | List of next 7 days' scheduled posts. Platform icons, design name, scheduled time. "View Calendar" link. |
| `UsageSummaryWidget.razor` | Three `UsageQuotaBar` instances: AI Generations, Social Posts, Storage. Plan name badge. "View Details" link. |
| `RecentDesignsWidget.razor` | Horizontal scrollable row of last 5 saved designs as small `BannerCard` tiles. |
| `QuickActionsWidget.razor` | Four big action buttons: "Create from Template", "Use AI Banner", "Open Editor", "Schedule a Post". |

---

## 4. Layout Components

### Location: `Components/Layout/`

---

#### `MainLayout.razor`

Full-app layout for authenticated users.

**Structure:**
```
┌───────────────────────────────────────────────────────────┐
│                        TopNav                             │
├──────────────┬────────────────────────────────────────────┤
│              │                                            │
│   Sidebar    │          @Body (page content)              │
│  (fixed,     │          (scrollable)                      │
│   240px)     │                                            │
│              │                                            │
└──────────────┴────────────────────────────────────────────┘
```

Contains: `<ToastContainer />`, `<ConfirmDialog />` (global instance), `<ErrorBoundary>`.

---

#### `AdminLayout.razor`

Extends MainLayout with admin-specific sidebar section. Uses same structure but `Sidebar` shows admin navigation group.

---

#### `AuthLayout.razor`

Minimal layout for unauthenticated pages (login, register, etc.).

**Structure:**
```
┌─────────────────────────────────────┐
│           CYHBrand Logo             │
│         (centered, 120px)           │
├─────────────────────────────────────┤
│                                     │
│    Centered card (max-w-md)         │
│    @Body                            │
│                                     │
├─────────────────────────────────────┤
│    © 2026 CYHBrand | Help | Privacy │
└─────────────────────────────────────┘
```

---

#### `Sidebar.razor`

Left navigation panel. Collapsible on mobile.

**Navigation sections:**

| Section | Items |
|---------|-------|
| Main | Dashboard, My Designs, Template Library, AI Banner Creator |
| Publishing | Content Calendar, Share & Publish |
| Account | Connected Accounts, Usage & Subscription |
| Support | Help Center, Settings |
| Admin *(admin role only)* | Admin Dashboard, Doctors, Templates, Logs, Settings |

Active route highlighted. Tooltip labels when collapsed.

---

#### `TopNav.razor`

Top navigation bar.

**Contains:** Logo (left), breadcrumb (center, optional), `DoctorAvatarMenu` (right).

---

#### `DoctorAvatarMenu.razor`

Avatar dropdown in top-right corner.

**Shows:** Profile photo (or initials fallback), doctor display name, specialty.

**Dropdown items:** My Profile, Settings, Help, Divider, Logout.

---

## 5. Design System

### 5.1 Color Palette

| Token | Hex | Usage |
|-------|-----|-------|
| `brand-primary` | `#0D47A1` | Primary buttons, active nav, headings |
| `brand-primary-light` | `#1565C0` | Hover state for primary |
| `brand-primary-dark` | `#0A2E6F` | Active/pressed state |
| `brand-accent` | `#00ACC1` | Accent elements, badges, icons |
| `brand-accent-light` | `#4DD0E1` | Accent hover |
| `success` | `#2E7D32` | Success states, published status |
| `success-bg` | `#E8F5E9` | Success badge background |
| `warning` | `#F57F17` | Warning states, quota near-limit |
| `warning-bg` | `#FFF8E1` | Warning badge background |
| `danger` | `#C62828` | Error states, failed status, delete actions |
| `danger-bg` | `#FFEBEE` | Error badge background |
| `info` | `#1565C0` | Info states, scheduled status |
| `info-bg` | `#E3F2FD` | Info badge background |
| `surface-white` | `#FFFFFF` | Cards, modals, inputs |
| `surface-gray-50` | `#FAFAFA` | Page background |
| `surface-gray-100` | `#F5F5F5` | Sidebar background, table row alt |
| `border-gray-200` | `#EEEEEE` | Card borders, dividers |
| `border-gray-300` | `#E0E0E0` | Input borders |
| `text-primary` | `#212121` | Body text, headings |
| `text-secondary` | `#757575` | Muted labels, placeholders |
| `text-disabled` | `#BDBDBD` | Disabled text |
| `text-on-primary` | `#FFFFFF` | Text on primary-colored backgrounds |
| `sidebar-bg` | `#1A1F36` | Dark sidebar background |
| `sidebar-text` | `#C5CAE9` | Sidebar nav text |
| `sidebar-active` | `#3949AB` | Active sidebar item background |
| `sidebar-hover` | `#283593` | Sidebar item hover |

### 5.2 Typography

| Role | Font | Weight | Size | Line Height |
|------|------|--------|------|-------------|
| Display heading | Inter | 700 | 2.25rem (36px) | 1.2 |
| Page heading (H1) | Inter | 700 | 1.875rem (30px) | 1.25 |
| Section heading (H2) | Inter | 600 | 1.5rem (24px) | 1.3 |
| Card heading (H3) | Inter | 600 | 1.25rem (20px) | 1.4 |
| Sub-heading (H4) | Inter | 500 | 1rem (16px) | 1.5 |
| Body (default) | Inter | 400 | 0.875rem (14px) | 1.6 |
| Body large | Inter | 400 | 1rem (16px) | 1.6 |
| Small / caption | Inter | 400 | 0.75rem (12px) | 1.4 |
| Label | Inter | 500 | 0.75rem (12px) | 1.4 |
| Code / mono | JetBrains Mono | 400 | 0.875rem (14px) | 1.6 |

**Font loading:** Inter and JetBrains Mono loaded from Google Fonts or bundled as WOFF2.

### 5.3 Spacing Scale

Based on 4px base unit:

| Token | Value | Usage |
|-------|-------|-------|
| `space-1` | 4px | Tight spacing, icon gaps |
| `space-2` | 8px | Input padding, small gaps |
| `space-3` | 12px | Badge padding, compact cards |
| `space-4` | 16px | Standard padding, card gaps |
| `space-5` | 20px | Section gaps |
| `space-6` | 24px | Card padding, larger gaps |
| `space-8` | 32px | Section separation |
| `space-10` | 40px | Large section padding |
| `space-12` | 48px | Page top padding |
| `space-16` | 64px | Major section separation |

### 5.4 Border Radius

| Token | Value | Usage |
|-------|-------|-------|
| `radius-sm` | 4px | Badges, pills, small inputs |
| `radius-md` | 8px | Buttons, inputs |
| `radius-lg` | 12px | Cards |
| `radius-xl` | 16px | Modals, large cards |
| `radius-full` | 9999px | Avatar circles, toggle switches |

### 5.5 Shadows

| Token | CSS Value | Usage |
|-------|-----------|-------|
| `shadow-xs` | `0 1px 2px rgba(0,0,0,0.05)` | Subtle card elevation |
| `shadow-sm` | `0 1px 3px rgba(0,0,0,0.1), 0 1px 2px rgba(0,0,0,0.06)` | Default card |
| `shadow-md` | `0 4px 6px rgba(0,0,0,0.07), 0 2px 4px rgba(0,0,0,0.06)` | Hover card, dropdown |
| `shadow-lg` | `0 10px 15px rgba(0,0,0,0.1), 0 4px 6px rgba(0,0,0,0.05)` | Modal, toast |

### 5.6 Component States

| State | Visual Treatment |
|-------|-----------------|
| Default | Neutral styling per component |
| Hover | `shadow-md`, slight background lighten, cursor pointer |
| Active / Pressed | Slight scale down `scale(0.98)`, darker background |
| Focus | 2px `brand-primary` outline offset 2px (keyboard nav) |
| Disabled | 50% opacity, `cursor-not-allowed`, no hover/active effects |
| Loading | Spinner overlay or skeleton loader; buttons disabled |
| Error | `danger` border, `danger-bg` background, error text below |
| Success | `success` border or toast notification |

### 5.7 Breakpoints

| Token | Min Width | Description |
|-------|-----------|-------------|
| `sm` | 640px | Mobile landscape |
| `md` | 768px | Tablet |
| `lg` | 1024px | Small desktop |
| `xl` | 1280px | Standard desktop |
| `2xl` | 1536px | Wide desktop |

**Mobile strategy:** Sidebar collapses to bottom nav or hamburger menu on `< md`. Canvas editor degrades gracefully (limited tools on mobile; note to user that desktop is recommended for full experience).

---

## 6. Navigation & Routing Notes

### 6.1 Route Guards

All routes except `/login`, `/register`, `/forgot-password`, `/reset-password`, `/verify-email`, `/access-denied` require authentication via:

```razor
<AuthorizeRouteView RouteData="routeData" DefaultLayout="typeof(MainLayout)">
    <NotAuthorized>
        <RedirectToLogin />
    </NotAuthorized>
</AuthorizeRouteView>
```

`RedirectToLogin.razor` redirects to `/login?returnUrl={currentUrl}` and after successful login redirects back.

### 6.2 Profile Setup Guard

After login, if `ApplicationUser.ProfileSetupComplete == false`, redirect to `/onboarding` before allowing access to dashboard pages.

### 6.3 Admin Route Guard

Admin pages use `[Authorize(Policy = "IsAdminOrAbove")]`. Unauthorized access redirects to `/access-denied`.

### 6.4 Navigation Service

`NavigationService` (scoped) wraps `NavigationManager` and provides typed navigation methods:
- `GoToDashboard()`
- `GoToEditor(Guid? designId, Guid? templateId, Guid? jobId)`
- `GoToDesignShare(Guid designId)`
- `GoToAiBannerStatus(Guid jobId)`
