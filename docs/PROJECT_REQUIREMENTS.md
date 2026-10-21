# CYHBrand — Software Requirements Specification (SRS)

> **Document Version:** 1.0  
> **Status:** Living Document  
> **Last Updated:** 2026-10-21  
> **Audience:** Product Owner, Developers, QA, Designers

---

## Table of Contents

1. [Executive Summary](#1-executive-summary)
2. [Product Vision & Goals](#2-product-vision--goals)
3. [Target Users](#3-target-users)
4. [Functional Requirements](#4-functional-requirements)
5. [Non-Functional Requirements](#5-non-functional-requirements)
6. [Out of Scope](#6-out-of-scope)
7. [External Integrations Required](#7-external-integrations-required)
8. [Assumptions](#8-assumptions)
9. [Open Decisions / Risks](#9-open-decisions--risks)

---

## 1. Executive Summary

**CYHBrand** (Care Your Health Brand) is a SaaS web application built specifically for healthcare professionals — primarily independent and clinic-based doctors — who need to maintain a consistent, professional, and compliant digital brand presence across social media platforms without requiring graphic design skills or hiring an agency.

The platform provides:
- A curated library of medical-themed banner and social post templates
- An AI-powered banner generation engine (text-to-image)
- A drag-and-drop / guided banner editor with branding presets
- A content calendar for scheduling posts
- Direct publishing to connected social accounts (Instagram, Facebook, LinkedIn, Twitter/X)
- Usage tracking and tiered subscription management
- An admin back-office for platform management

CYHBrand is implemented as a **.NET 8 Blazor Web App** (Interactive Server) backed by **SQL Server 2022**, with a clean layered architecture.

---

## 2. Product Vision & Goals

### Vision Statement

> *Empower every doctor to be their own brand — professionally, compliantly, and effortlessly.*

### Strategic Goals

| # | Goal | Success Metric |
|---|------|----------------|
| G-01 | Reduce the time doctors spend on social media content creation | < 5 minutes from idea to ready-to-publish post |
| G-02 | Ensure every published asset looks professional and medically appropriate | 95%+ user satisfaction with output quality |
| G-03 | Provide a zero-design-skill required experience | New user onboarding → first banner < 10 minutes |
| G-04 | Enable multi-platform publishing from a single tool | Publish to ≥ 3 platforms from one place |
| G-05 | Monetize through tiered subscriptions | Convert ≥ 20% of free users to paid within 30 days |
| G-06 | Maintain healthcare content compliance guardrails | Zero regulatory violations from generated content |

---

## 3. Target Users

### Primary User — Independent Doctor / Clinic Owner

- **Description:** MBBS/MD/BDS/Specialist physician running a solo practice or small clinic
- **Technical Level:** Low to moderate; comfortable with smartphones and basic web apps
- **Pain Points:** No time or budget for designers; doesn't know what to post; worried about looking unprofessional; inconsistent brand presence
- **Goals:** Build trust with patients online, announce services, share health tips, grow social following

### Secondary User — Clinic Admin / Social Media Manager

- **Description:** Staff member managing the doctor's digital presence on their behalf
- **Technical Level:** Moderate; familiar with social platforms
- **Pain Points:** Lacks medically appropriate content; needs approval workflows
- **Goals:** Queue content in advance, schedule posts, maintain brand consistency

### Tertiary User — Platform Administrator (Internal)

- **Description:** CYHBrand operations team
- **Technical Level:** High
- **Goals:** Manage user accounts, monitor AI usage, publish new templates, configure subscription plans, view audit logs

### User Roles

| Role | Description |
|------|-------------|
| `Doctor` | Standard registered user with a doctor profile |
| `ClinicAdmin` | Elevated user who can manage multiple sub-profiles (future) |
| `PlatformAdmin` | Full back-office access |
| `SuperAdmin` | Root access including system configuration |

---

## 4. Functional Requirements

### 4.1 Authentication & Account Management (AUTH)

| ID | Requirement | Priority |
|----|-------------|----------|
| AUTH-01 | Users can register with email + password | Must Have |
| AUTH-02 | Users can log in with email + password | Must Have |
| AUTH-03 | Passwords must meet minimum complexity (8+ chars, uppercase, digit, special) | Must Have |
| AUTH-04 | Forgot password flow sends email with secure reset link (token valid 1 hour) | Must Have |
| AUTH-05 | Reset password with token validation | Must Have |
| AUTH-06 | Email address verification on registration | Must Have |
| AUTH-07 | Login rate limiting (max 5 attempts, then 15-minute lockout) | Must Have |
| AUTH-08 | Session maintained via secure cookie (sliding expiry 30 min idle, absolute 8 h) | Must Have |
| AUTH-09 | Logout clears session and cookie | Must Have |
| AUTH-10 | Role-based access: Doctor, ClinicAdmin, PlatformAdmin, SuperAdmin | Must Have |
| AUTH-11 | Admin can manually lock/unlock user accounts | Must Have |
| AUTH-12 | All authentication events written to AuditLog | Must Have |
| AUTH-13 | HTTPS enforced; HSTS enabled | Must Have |
| AUTH-14 | Optional 2FA via TOTP authenticator app (Phase 3+) | Should Have |
| AUTH-15 | Google OAuth login (Phase 4+) | Could Have |

---

### 4.2 Doctor Profile (PROF)

| ID | Requirement | Priority |
|----|-------------|----------|
| PROF-01 | After registration, user completes doctor profile setup wizard | Must Have |
| PROF-02 | Profile fields: Full name, display name, specialty, qualifications, clinic name, clinic address, city, phone, website URL, bio (500 chars max) | Must Have |
| PROF-03 | Upload profile photo (JPEG/PNG, max 5 MB, auto-resized to 400×400) | Must Have |
| PROF-04 | Upload clinic logo (JPEG/PNG/SVG, max 5 MB, auto-resized) | Must Have |
| PROF-05 | Brand color picker — primary and secondary colors stored as hex | Must Have |
| PROF-06 | Brand font preference (select from curated list of 6 web-safe / Google Fonts) | Must Have |
| PROF-07 | Profile completeness indicator (%) shown on dashboard | Should Have |
| PROF-08 | Profile data auto-applied as defaults in editor and AI prompts | Must Have |
| PROF-09 | Admin can view and edit any doctor's profile | Must Have |
| PROF-10 | Profile changes are audited | Must Have |

---

### 4.3 Template Library (TMPL)

| ID | Requirement | Priority |
|----|-------------|----------|
| TMPL-01 | Browse paginated gallery of banner templates (20/page) | Must Have |
| TMPL-02 | Templates organized into categories (e.g., Health Tips, Clinic Announcement, Festival, Awareness Day, Appointment Reminder) | Must Have |
| TMPL-03 | Filter templates by category, orientation (landscape/portrait/square), color tone | Must Have |
| TMPL-04 | Search templates by keyword | Must Have |
| TMPL-05 | Preview template in a full-screen modal | Must Have |
| TMPL-06 | "Use This Template" → opens editor pre-loaded with template | Must Have |
| TMPL-07 | Templates have metadata: name, category, description, tags, orientation, dimensions, thumbnail URL, preview URL, template JSON data | Must Have |
| TMPL-08 | Templates marked as active/inactive by admin | Must Have |
| TMPL-09 | Templates marked as Premium (requires paid subscription) | Should Have |
| TMPL-10 | Admin can create, edit, and delete templates | Must Have |
| TMPL-11 | Template upload accepts a structured JSON definition + preview image | Must Have |
| TMPL-12 | Most-used templates surfaced on dashboard | Could Have |

---

### 4.4 AI Banner Creator (AIBANNER)

| ID | Requirement | Priority |
|----|-------------|----------|
| AIBN-01 | Dedicated AI Banner creation page with prompt input | Must Have |
| AIBN-02 | User enters a text description of the banner they want | Must Have |
| AIBN-03 | System enhances the raw prompt with doctor's brand context (name, specialty, colors) before calling AI API | Must Have |
| AIBN-04 | User selects output size: Square (1080×1080), Landscape (1920×1080), Portrait (1080×1920) | Must Have |
| AIBN-05 | System calls AI image generation API asynchronously; job created in DB | Must Have |
| AIBN-06 | User sees real-time job status (Queued → Processing → Completed / Failed) | Must Have |
| AIBN-07 | On completion, generated image stored in StoredAssets and linked to BannerDesign | Must Have |
| AIBN-08 | User can open result in editor for further refinement | Must Have |
| AIBN-09 | User can regenerate (new job) from same prompt or modified prompt | Must Have |
| AIBN-10 | AI usage counted against monthly quota (per subscription tier) | Must Have |
| AIBN-11 | Prompt content filtered for non-medical / inappropriate content (basic keyword filter + AI moderation) | Must Have |
| AIBN-12 | Failed jobs show user-friendly error and allow retry | Must Have |
| AIBN-13 | Admin can view all AI generation jobs (status, prompt, cost estimate) | Must Have |
| AIBN-14 | Pluggable AI backend: OpenAI DALL-E, Azure OpenAI, or stub for dev/test | Must Have |

---

### 4.5 Banner Editor (EDITOR)

| ID | Requirement | Priority |
|----|-------------|----------|
| EDIT-01 | Canvas-based editor loaded from template JSON or AI-generated image | Must Have |
| EDIT-02 | Text layers: add/edit/delete; font, size, color, bold, italic, alignment | Must Have |
| EDIT-03 | Image layers: add/replace from uploads or asset library | Must Have |
| EDIT-04 | Move and resize layers on canvas | Must Have |
| EDIT-05 | Background color or image background change | Must Have |
| EDIT-06 | Insert doctor name, clinic name, phone, website as quick-insert tokens | Must Have |
| EDIT-07 | Undo / Redo (min 20 levels) | Must Have |
| EDIT-08 | Save design (creates BannerDesign + BannerDesignVersion in DB) | Must Have |
| EDIT-09 | Auto-save every 2 minutes when changes detected | Should Have |
| EDIT-10 | Export as PNG (1x, 2x) via server-side rendering | Must Have |
| EDIT-11 | Export as JPEG (configurable quality 80–95%) | Should Have |
| EDIT-12 | Design name editable inline | Must Have |
| EDIT-13 | Brand preset button applies doctor's colors and fonts to current design | Should Have |
| EDIT-14 | Layer panel showing all layers with show/hide toggle | Should Have |
| EDIT-15 | Zoom in/out on canvas | Should Have |

---

### 4.6 My Designs (DESIGNS)

| ID | Requirement | Priority |
|----|-------------|----------|
| DSGN-01 | Gallery of all saved designs (cards with thumbnail, name, date, status) | Must Have |
| DSGN-02 | Filter designs by status: Draft, Published, Scheduled | Must Have |
| DSGN-03 | Search designs by name | Must Have |
| DSGN-04 | Open design in editor | Must Have |
| DSGN-05 | Duplicate design | Must Have |
| DSGN-06 | Delete design (soft delete; admin can hard-delete) | Must Have |
| DSGN-07 | View version history of a design | Should Have |
| DSGN-08 | Download design as PNG/JPEG from this page | Must Have |
| DSGN-09 | Quick-share design from card | Should Have |
| DSGN-10 | Designs owned by a doctor; admin can view all | Must Have |

---

### 4.7 Content Calendar (CALENDAR)

| ID | Requirement | Priority |
|----|-------------|----------|
| CAL-01 | Monthly calendar view showing scheduled posts | Must Have |
| CAL-02 | Week view | Should Have |
| CAL-03 | Day view | Could Have |
| CAL-04 | Create a new scheduled post: select design, pick platforms, pick date/time | Must Have |
| CAL-05 | Edit scheduled post date/time (if not yet published) | Must Have |
| CAL-06 | Cancel/delete a scheduled post | Must Have |
| CAL-07 | Posts colored by status: Scheduled (blue), Published (green), Failed (red), Draft (gray) | Must Have |
| CAL-08 | Click post on calendar → detail panel with design preview and platform badges | Should Have |
| CAL-09 | Upcoming posts listed on dashboard (next 7 days) | Must Have |
| CAL-10 | System background job polls ScheduledPost table and publishes due posts | Must Have |

---

### 4.8 Share & Publish (SHARE)

| ID | Requirement | Priority |
|----|-------------|----------|
| SHAR-01 | Share page for a specific design showing publish options | Must Have |
| SHAR-02 | Publish immediately to one or more connected social accounts | Must Have |
| SHAR-03 | Schedule post for future date/time | Must Have |
| SHAR-04 | Custom caption / text for each platform | Should Have |
| SHAR-05 | Platform-specific character limits enforced (Twitter 280, LinkedIn 3000, etc.) | Should Have |
| SHAR-06 | PublishingLog record created for each publish attempt | Must Have |
| SHAR-07 | Success/failure notification to user | Must Have |
| SHAR-08 | Re-publish / retry failed posts | Should Have |
| SHAR-09 | Generate shareable link (public URL) for design (Phase 5+) | Could Have |
| SHAR-10 | Download design for manual sharing | Must Have |

---

### 4.9 Connected Accounts (CONNECT)

| ID | Requirement | Priority |
|----|-------------|----------|
| CONN-01 | List connected social accounts (platform, display name, connected date, status) | Must Have |
| CONN-02 | Connect Instagram Business account via OAuth | Should Have |
| CONN-03 | Connect Facebook Page via OAuth | Should Have |
| CONN-04 | Connect LinkedIn Profile/Page via OAuth | Should Have |
| CONN-05 | Connect Twitter/X account via OAuth | Could Have |
| CONN-06 | Disconnect a social account | Must Have |
| CONN-07 | OAuth tokens stored encrypted (ASP.NET Core Data Protection) | Must Have |
| CONN-08 | Token expiry handled gracefully; prompt user to reconnect | Must Have |
| CONN-09 | Admin can view all connected accounts (not tokens) | Must Have |

---

### 4.10 Usage & Subscription (USAGE)

| ID | Requirement | Priority |
|----|-------------|----------|
| USAG-01 | Dashboard widget showing current month's AI generation usage (used / limit) | Must Have |
| USAG-02 | Dashboard widget showing stored assets size | Should Have |
| USAG-03 | Detailed usage page: per-day breakdown, by type (AI generations, exports, posts) | Should Have |
| USAG-04 | Every countable action writes a UsageLedger record | Must Have |
| USAG-05 | Free plan limits: 5 AI generations/month, 10 designs, 3 social posts/month | Must Have |
| USAG-06 | Pro plan limits: 50 AI generations/month, unlimited designs, 100 posts/month | Must Have |
| USAG-07 | When limit reached, user shown upgrade prompt | Must Have |
| USAG-08 | Current subscription shown on usage page and settings | Must Have |
| USAG-09 | Subscription plans managed by admin | Must Have |
| USAG-10 | Payment integration (Stripe or Razorpay — Phase 6+) | Should Have |

---

### 4.11 Admin Module (ADMIN)

| ID | Requirement | Priority |
|----|-------------|----------|
| ADMN-01 | Admin-only section at /admin (role-gated) | Must Have |
| ADMN-02 | Dashboard: total users, active subscriptions, AI jobs today, failed posts | Must Have |
| ADMN-03 | List all doctors with search, filter, pagination | Must Have |
| ADMN-04 | View/edit doctor profile; lock/unlock account | Must Have |
| ADMN-05 | Manage templates: list, create, edit, delete, activate/deactivate | Must Have |
| ADMN-06 | Manage template categories | Must Have |
| ADMN-07 | AI configuration page: active provider, API key status (masked), usage stats | Must Have |
| ADMN-08 | Manage subscription plans: name, limits, price placeholder | Must Have |
| ADMN-09 | View AuditLog with search/filter by user, action, date range | Must Have |
| ADMN-10 | View PublishingLog with status filter | Should Have |
| ADMN-11 | View all AI generation jobs with status filter | Should Have |
| ADMN-12 | System settings: maintenance mode toggle, default plan, email sender config | Must Have |

---

### 4.12 Settings (SETTINGS)

| ID | Requirement | Priority |
|----|-------------|----------|
| SETT-01 | Change email (requires verification) | Must Have |
| SETT-02 | Change password | Must Have |
| SETT-03 | Notification preferences (email on publish success/failure) | Should Have |
| SETT-04 | Delete account request (soft delete, 30-day grace period) | Should Have |
| SETT-05 | Download my data (GDPR data export — Phase 7+) | Could Have |
| SETT-06 | Language preference (en-IN default; future localization) | Could Have |

---

### 4.13 Help & Support (HELP)

| ID | Requirement | Priority |
|----|-------------|----------|
| HELP-01 | Static help/FAQ page with sections for each module | Must Have |
| HELP-02 | Link to external knowledge base / documentation site | Could Have |
| HELP-03 | Contact support form (emails support inbox) | Should Have |
| HELP-04 | In-app tooltips on key actions | Should Have |
| HELP-05 | Getting started checklist shown to new users | Should Have |

---

## 5. Non-Functional Requirements

### 5.1 Performance

| ID | Requirement | Target |
|----|-------------|--------|
| PERF-01 | Page load time (server-rendered) | < 2 seconds (95th percentile) |
| PERF-02 | Banner editor load time | < 3 seconds |
| PERF-03 | AI generation job enqueue → response | < 500 ms (enqueue only; async processing separate) |
| PERF-04 | Image export (server-side) | < 5 seconds for 1080×1080 PNG |
| PERF-05 | Database queries | No query > 200 ms in normal operation |
| PERF-06 | Concurrent users | Support 200 concurrent interactive users on single server initially |

### 5.2 Security

| ID | Requirement |
|----|-------------|
| SEC-01 | All traffic over HTTPS / TLS 1.2+ |
| SEC-02 | HSTS enabled with min 1-year max-age |
| SEC-03 | CSRF protection on all state-changing operations (Blazor anti-forgery) |
| SEC-04 | SQL injection prevention via EF Core parameterized queries only |
| SEC-05 | XSS prevention — Blazor output encoding by default; no raw HTML injection |
| SEC-06 | Social OAuth tokens encrypted at rest (ASP.NET Core Data Protection) |
| SEC-07 | AI API keys stored in User Secrets / environment variables only, never in source |
| SEC-08 | Login rate limiting (AspNetCoreRateLimit middleware) |
| SEC-09 | File uploads validated: MIME type, extension, max size, malware scan (ClamAV or stub) |
| SEC-10 | Role-based authorization on all admin endpoints/pages |
| SEC-11 | Sensitive data masked in logs (tokens, passwords) |
| SEC-12 | Regular dependency vulnerability scanning (NuGet audit) |
| SEC-13 | Account lockout after 5 failed login attempts |

### 5.3 Scalability

| ID | Requirement |
|----|-------------|
| SCAL-01 | Stateless application tier (session via cookie; no in-memory session affinity requirement in Phase 1) |
| SCAL-02 | Storage abstraction (IStorageService) allows switching from local disk to Azure Blob / S3 without code changes |
| SCAL-03 | AI service abstraction allows switching providers without code changes |
| SCAL-04 | Background job queue DB-backed (allows future migration to Hangfire / Azure Service Bus) |
| SCAL-05 | Database connection pooling via EF Core defaults |

### 5.4 Accessibility

| ID | Requirement |
|----|-------------|
| ACC-01 | WCAG 2.1 Level AA compliance target |
| ACC-02 | All interactive elements keyboard-navigable |
| ACC-03 | Color contrast ratio ≥ 4.5:1 for normal text |
| ACC-04 | ARIA labels on all icon-only buttons |
| ACC-05 | Screen reader tested with NVDA (Windows) |

### 5.5 Maintainability

| ID | Requirement |
|----|-------------|
| MAIN-01 | Minimum 70% unit test coverage on Core layer |
| MAIN-02 | All public APIs and services documented with XML doc comments |
| MAIN-03 | Structured logging via Serilog to file and console |
| MAIN-04 | EF Core migrations used for all schema changes; no manual SQL DDL |
| MAIN-05 | Health check endpoint at /health for monitoring |
| MAIN-06 | Exception handling middleware with correlation IDs |

---

## 6. Out of Scope

The following are explicitly **not** part of CYHBrand v1:

- ❌ Patient appointment booking or management
- ❌ Electronic Medical Records (EMR) or clinical workflows
- ❌ Hospital / multi-branch management
- ❌ Telemedicine or video consultation
- ❌ Prescription management
- ❌ Medical billing or insurance claims
- ❌ Native iOS or Android mobile apps (web-responsive only)
- ❌ Real-time collaboration / multi-user editing
- ❌ WhatsApp or SMS publishing
- ❌ Analytics / insights from social platforms (Phase 8+ consideration)
- ❌ Marketplace for third-party designers to sell templates
- ❌ Video content creation or editing
- ❌ Payment processing in Phase 1 (subscription plans defined but not charged)

---

## 7. External Integrations Required

### 7.1 AI Image Generation

| Provider | Use Case | Priority |
|----------|----------|----------|
| OpenAI DALL-E 3 (via API) | Primary AI banner generation | Must Have |
| Azure OpenAI Service | Alternative / enterprise option | Should Have |
| Stub/Mock service | Dev/test without API keys | Must Have |

**Notes:** API key stored in environment variable `AIImageGeneration__OpenAI__ApiKey`. Usage tracked per user per month.

### 7.2 Social Platform APIs

| Platform | OAuth Version | API Used | Priority |
|----------|---------------|----------|----------|
| Meta (Instagram/Facebook) | OAuth 2.0 | Graph API v18+ | Should Have |
| LinkedIn | OAuth 2.0 | Share API v2 | Should Have |
| Twitter/X | OAuth 2.0 (v2) | X API v2 | Could Have |

### 7.3 Email

| Provider | Use Case |
|----------|----------|
| SMTP (configurable) | Password reset, verification, notifications |
| SendGrid (optional) | Production transactional email |

### 7.4 Storage

| Provider | Use Case | Phase |
|----------|----------|-------|
| Local disk | Dev / Phase 1 | Phase 1 |
| Azure Blob Storage | Production | Phase 5+ |
| AWS S3 | Alternative production | Phase 5+ |

---

## 8. Assumptions

1. Doctors are the primary account owners; one account = one doctor (multi-doctor clinics deferred).
2. All content is in English (India) with en-IN locale settings; full i18n deferred.
3. The application will initially be deployed to a single Windows Server IIS instance.
4. Internet connectivity is required; no offline mode.
5. AI-generated images are stored by CYHBrand and usage rights belong to the subscribing doctor.
6. Social platform API terms of service are acceptable for the intended use case.
7. No real-time notifications in Phase 1 (polling used for job status).
8. Medical disclaimer: CYHBrand does not review or validate the clinical accuracy of content created by users.

---

## 9. Open Decisions / Risks

| ID | Category | Issue | Options | Owner | Status |
|----|----------|-------|---------|-------|--------|
| OD-01 | Technology | Banner editor implementation | A) Custom Blazor canvas component, B) Fabric.js interop, C) 3rd party SaaS embed | Tech Lead | Open |
| OD-02 | AI | Primary AI provider selection | OpenAI DALL-E 3 vs Azure OpenAI | Product | Open |
| OD-03 | Payments | Payment gateway for India | Razorpay vs Stripe | Business | Deferred Phase 6 |
| OD-04 | Storage | Phase 1 storage location | Local disk vs Azure Blob from day 1 | Tech Lead | Open |
| OD-05 | Compliance | Indian IT Act / PDPA compliance review | Legal review needed | Product/Legal | Open |
| OD-06 | Email | SMTP provider for production | SendGrid vs AWS SES vs Postmark | DevOps | Open |
| OD-07 | Risk | Meta API approval for Instagram publishing | Meta review process can take weeks | Tech Lead | At Risk |
| OD-08 | Risk | AI content moderation for medical compliance | Need guardrails to prevent inappropriate health claims | Product | Open |
| OD-09 | Risk | Editor complexity | Canvas-based editor is complex; risk of scope creep | Tech Lead | Mitigate with phased features |
| OD-10 | Architecture | Blazor Interactive Server vs WASM | Server chosen for simplicity; revisit if scale needed | Tech Lead | Decided: Server |
