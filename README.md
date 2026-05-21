# 🎸 Scatterbrain Music

> **Gear · Vinyl · Community · Since 2024**

A full-stack music store web application built with **React + TypeScript** (frontend) and **ASP.NET Core 8** (backend), with **NUnit** unit tests.

---

## 📐 Architecture

```
ScatterbrainMusic/
├── ScatterbrainMusic.sln
│
├── ScatterbrainMusic.API/          # .NET 8 Web API
│   ├── Controllers/
│   │   ├── ProductsController.cs   # GET /api/products, /api/products/{id}, /featured, /categories
│   │   └── CartController.cs       # GET/POST/PUT/DELETE /api/cart
│   ├── Models/
│   │   └── Models.cs               # Product, Cart, CartItem, request/response types
│   ├── Services/
│   │   ├── ProductService.cs       # In-memory product catalog (18 products)
│   │   └── CartService.cs          # Session-based cart management
│   └── Program.cs                  # DI, CORS, Swagger setup
│
├── ScatterbrainMusic.Tests/        # NUnit test project
│   ├── Services/
│   │   ├── ProductServiceTests.cs  # 18 unit tests for ProductService
│   │   └── CartServiceTests.cs     # 17 unit tests for CartService
│   └── Controllers/
│       └── ControllerTests.cs      # 14 controller tests with Moq mocks
│
└── ScatterbrainMusic.Client/       # React + TypeScript (Vite)
    ├── src/
    │   ├── assets/logo.png         # Scatterbrain Music logo
    │   ├── components/
    │   │   ├── Navbar.tsx/.css      # Sticky nav with search + cart badge
    │   │   ├── ProductCard.tsx/.css # Product grid card
    │   │   ├── CartDrawer.tsx/.css  # Slide-in cart panel
    │   │   └── Footer.tsx/.css      # Footer with newsletter
    │   ├── hooks/
    │   │   ├── useApi.ts            # Typed API client
    │   │   └── useCart.tsx          # Cart context + state
    │   ├── pages/
    │   │   ├── Home.tsx/.css        # Hero, categories, featured products
    │   │   ├── Shop.tsx/.css        # Filterable product catalog
    │   │   └── ProductDetail.tsx/.css # Single product view
    │   └── types/index.ts           # Shared TypeScript interfaces
    ├── index.html
    ├── package.json
    ├── tsconfig.json
    └── vite.config.ts
```

---

## 🚀 Getting Started

### Prerequisites

| Tool | Version |
|------|---------|
| .NET SDK | 8.0+ |
| Node.js | 18+ |
| npm | 9+ |

### 1. Clone & open

```bash
git clone <repo-url>
cd ScatterbrainMusic
```

### 2. Run the API

```bash
cd ScatterbrainMusic.API
dotnet run
```

The API will be available at:
- `https://localhost:7001` (HTTPS)
- `http://localhost:5001` (HTTP)
- **Swagger UI**: `https://localhost:7001/swagger`

### 3. Run the React frontend

```bash
cd ScatterbrainMusic.Client
npm install
npm run dev
```

Open **http://localhost:5173** in your browser.

> **Note**: The Vite dev server proxies `/api` requests to the .NET API automatically.

### 4. Run the tests

```bash
cd ScatterbrainMusic.Tests
dotnet test --verbosity normal
```

Or from the solution root:

```bash
dotnet test ScatterbrainMusic.sln
```

---

## 🧪 Test Coverage

### ProductServiceTests (18 tests)
- `GetProductsAsync` — no filter, category filter (case-insensitive), search, pagination, combined filters
- `GetProductByIdAsync` — valid/invalid IDs, required field validation
- `GetCategoriesAsync` — distinct, alphabetically sorted, expected values
- `GetFeaturedProductsAsync` — featured flag, max 6 results, valid data

### CartServiceTests (17 tests)
- `GetCartAsync` — new session, same-session identity
- `AddToCartAsync` — new item, quantity increment, multiple products, total calculation, auto-create session
- `UpdateCartItemAsync` — update, zero/negative quantity removal, non-existent product
- `RemoveFromCartAsync` — existing item, selective removal, missing item handling
- `ClearCartAsync` — clears all, empty cart no-throw

### ControllerTests (14 tests)
- `ProductsController` — pagination clamping, category/search pass-through, 404 for missing product
- `CartController` — add/remove/update/clear, bad request validation (qty, stock)

All tests use **FluentAssertions** for readable assertions and **Moq** for controller-layer mocking.

---

## 🎨 Design System

Colors extracted from the Scatterbrain Music logo:

| Token | Value | Usage |
|-------|-------|-------|
| `--navy` | `#1A2744` | Primary text, navbar, buttons |
| `--orange` | `#E8612A` | CTAs, accents, featured badges |
| `--teal` | `#2B8C82` | Secondary accents, brand labels |
| `--gold` | `#D4A843` | Star ratings, featured highlights |
| `--cream` | `#F5F0E8` | Page background |

Fonts: **Bebas Neue** (headings), **Permanent Marker** (accent), **Nunito** (body)

---

## 🛍️ Features

- **18 products** across Guitars, Vinyl, Gear, and Accessories
- Product filtering by category and free-text search
- Sort by price or rating
- Session-based shopping cart (cookie-persisted)
- Slide-in cart drawer with quantity management
- Product detail pages with stock indicators
- Responsive layout for mobile/tablet/desktop
- Swagger UI for API exploration

---

## 📦 Tech Stack

| Layer | Technology |
|-------|-----------|
| Frontend | React 18, TypeScript, Vite |
| Routing | React Router v6 |
| Styling | Pure CSS with custom properties |
| Backend | ASP.NET Core 8 Web API |
| Testing | NUnit 4, Moq, FluentAssertions |
| API Docs | Swagger / OpenAPI |
| State | In-memory (singleton services) |
