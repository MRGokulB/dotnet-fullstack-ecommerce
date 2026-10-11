# AuraTech Gear - Enterprise Fullstack .NET E-Commerce Platform

Production-grade e-commerce application built with modern .NET 7/8 Clean Architecture, Entity Framework Core, Stripe multi-item checkout, and a React 18 storefront.

## Solution Architecture

```
DotnetEcommerce/
├── src/
│   ├── DotnetEcommerce.Domain/         # Product, Category, Cart, CartItem, Order, OrderItem
│   ├── DotnetEcommerce.Application/    # DTOs, FluentValidation, Interfaces (IPaymentService, IApplicationDbContext)
│   ├── DotnetEcommerce.Infrastructure/ # EF Core (ApplicationDbContext), Multi-Provider (SQLite/Postgres/SQL Server), Stripe.net
│   └── DotnetEcommerce.WebApi/         # Kestrel server, Controllers (Products, Cart, Orders, Payments), RFC 7807 Middleware
├── client/                             # Vite + React 18 + Tailwind CSS Storefront
├── Dockerfile                          # Multi-stage production container build
└── docker-compose.yml                  # Fullstack orchestration (PostgreSQL + API)
```

## Quick Start (Zero Config Local Run)

### 1. Start the .NET Web API
```bash
cd src/DotnetEcommerce.WebApi
dotnet run
```
- Swagger UI: `http://localhost:5000/swagger`
- Health check: `http://localhost:5000/health`
- The application automatically initializes SQLite (`ecommerce.db`) with catalog seed data on first run.

### 2. Start the React Storefront
```bash
cd client
npm install
npm run dev
```
- Open `http://localhost:5173` to explore the catalog, add products to the cart, and test checkout.

## Author & Repository Details

- **Author:** MRGokulB
- **Email:** rdxdarth80@gmail.com
- **Repository:** https://github.com/MRGokulB/dotnet-fullstack-ecommerce
