<div align="center">

<img src="VirtualVistaHub/VirtualVistaHub/Content/imgs/logo.png" alt="VirtualVistaHub"/>

<h1>Real Estate Platform with 3D Furnishing</h1>

<img alt="Version" src="https://img.shields.io/badge/version-1.0.0-blue" />
<img alt="License" src="https://img.shields.io/badge/license-MIT-green" />
<img alt="Framework" src="https://img.shields.io/badge/framework-ASP.NET%20MVC%20%2B%20.NET%204.7.2-purple" />
<img alt="Language" src="https://img.shields.io/badge/language-C%23-blueviolet" />
<img alt="Database" src="https://img.shields.io/badge/database-SQL%20Server%20%2B%20EF6-4479A1" />
<img alt="Status" src="https://img.shields.io/badge/status-active-success" />

</div>

## 🚀 Product Overview

VirtualVistaHub is a modern real estate web application built on the ASP.NET Framework 4.7.2. It provides a seamless property browsing and management experience with advanced search, rich property details, and curated content pages. The platform integrates FurnishUp, a 3D builder tool that enables interactive design and visualization of furnished properties directly from property pages.

### ✅ Why this solution
- **Real‑estate–ready content model**: properties, staff, rich pages, and media.
- **Interactive visualization**: integrate a 3D furnishing experience using FurnishUp.
- **Fast, responsive UI**: Bootstrap and jQuery on the front end.
- **Maintainable backend**: ASP.NET MVC + Entity Framework 6 with Razor views.

## ✨ Key Capabilities

### 🏠 Website Content
- Home, About, Contact and other marketing pages.
- Property listings with images, pricing, status, and key attributes.
- Property details page with gallery and optional FurnishUp 3D builder link/embed.
- Staff/team pages (profile cards, roles, contact info).

### 🔎 Search & Discovery
- Advanced filters (e.g., district/area, price, rooms, category).
- SEO-friendly routing and clean URLs.

### 👤 User Experience
- Mobile-first layout using Bootstrap.
- Client-side validation and unobtrusive forms.
- Session-aware flows (custom session authorization attribute).

### 🧩 Integrations
- **FurnishUp 3D builder**: explore furniture layouts for property interiors.
- Image assets and static content served from the app bundle.

## 🏗️ Technical Overview
- **Backend**: ASP.NET MVC, .NET Framework 4.7.2
- **Data Access**: Entity Framework
- **Views**: Razor views, layout partials, and shared components
- **Frontend**: Bootstrap, jQuery, jQuery Validate
- **Localization/Encoding**: UTF-8, `en-US` culture (configured in `Web.config`)

## 📦 Project Structure

```
VirtualVistaHub/
  VirtualVistaHub/                # ASP.NET MVC application (root project)
    Controllers/                  # Home, Property, Staff controllers
    Models/                       # EF6 EDMX, POCOs, templates
    Views/                        # Razor views for Home/Property/Staff
    Content/                      # Bootstrap, CSS, imgs
    Scripts/                      # jQuery, validation, helpers
    Filters/                      # Session authorization attribute
    App_Start/                    # MVC routing, bundles, filters
    Web.config                    # App config, bindings, connectionStrings
  README.md
  Demo video.mp4                  # Local demo video (see below)
```

## 🎥 Demo video showing the project

<video src="Demo%20video.mp4" controls preload="metadata" width="800">
  Your browser does not support the video tag. You can find the video at the repository root as "Demo video.mp4".
</video>

## 🔗 FurnishUp Integration
- Link or embed the FurnishUp 3D builder from property detail pages to allow users to visualize furniture layouts.
- The integration can be added as an external link, an iframe, or a modal launcher depending on FurnishUp’s embedding guidelines.

## 🧪 Testing & QA (suggested)
- Smoke test key routes: Home, Property listing, Property details, Staff, Contact.
- Validate search filters (districts/areas, price ranges, categories).
- Verify session-protected actions via `SessionAuthorizeAttribute`.

## 📜 License
This project is released under the MIT License. See `LICENSE` for details.

## 🪧 Attributions
- Front-end built with Bootstrap and jQuery; icons and CSS utilities from bundled assets.
- 3D furnishing experience powered by FurnishUp.