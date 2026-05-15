# Bunnings Sizzling Hot Products

A full-stack solution for Bunnings' "Sizzling Hot" feature — surfacing the
most popular product per day and over a rolling 3-day period, based on
order data.

---

## Stack

| Layer        | Technology                                  | Reason                                                                                       |
| ------------ | ------------------------------------------- | -------------------------------------------------------------------------------------------- |
| Backend      | ASP.NET Core Minimal API (.NET 8)           | Lightweight entry point suited to a small API surface area                                   |
| Architecture | Clean Architecture                          | Decouples business logic from infrastructure; makes the service layer independently testable |
| Frontend     | Vite + React 19 + TypeScript + Tailwind CSS | Fast dev experience, type safety, utility-first styling                                      |
| Testing      | xUnit + FluentAssertions                    | Familiar .NET testing stack; FluentAssertions improves test readability                      |

---

## Architecture Overview

The backend follows Clean Architecture with four layers:

```
Domain         → Core models (Order, Product). No dependencies.
Application    → Business logic (SizzlingHotService). Depends only on Domain interfaces.
Infrastructure → JSON file repositories. Implements Application interfaces.
API            → Minimal API endpoints. Wires everything together via DI.
```

This means the business rules in `SizzlingHotService` have zero knowledge of
how data is stored or served — swapping JSON files for a database requires
only a new Infrastructure implementation.

---

## Business Rules Implemented

1. A product is counted once per order regardless of quantity
2. Same customer buying the same product on the same day across multiple
   orders counts as one sale
3. A cancelled order (matched by `orderId`) removes that product's sale
   from the original order's date
4. Ties are broken alphabetically — the first product name in the list wins

---

## Assumptions

### Date & Scope

- Today's date is fixed at **23/04/2026** as per the brief
- The 3-day period is **21/04/2026 – 23/04/2026** inclusive
- Orders outside this date range are excluded from period calculations but
  still processed for cancellation matching

### Cancellation Behaviour

- A cancelled order is identified by `status: "cancelled"` and matched to
  its original completed order via `orderId`
- The cancellation record itself carries no `entries` — the entries are
  sourced from the original completed order
- If a cancelled `orderId` does not match any completed order, it is safely
  ignored with no effect on results
- If two completed orders share the same `orderId`, both are excluded when
  a cancellation for that `orderId` is encountered (defensive behaviour;
  documented as a data quality issue)

### Product & Order Data

- If a product ID appears in an order but does not exist in `products.json`,
  that entry is silently skipped — it cannot be named or ranked
- If `products.json` or `orders.json` is empty, the API returns an empty
  result set with a 200 OK response (no top product exists)
- If either file is missing or malformed, the API returns a 500 response
  with a descriptive error message
- Orders with an empty `entries` array are valid (e.g. cancelled orders)
  and are handled without errors

### Results Behaviour

- If all orders for a given day are cancelled, that day returns no top
  product — it is omitted from the daily results
- If all orders across the entire period are cancelled, the period result
  is empty — no top product is returned
- If a date range is queried with no orders at all, an empty result is
  returned rather than an error
- If every product ties on a given day, alphabetical ordering is applied
  across all tied products and the first is selected

---

## Getting Started

### Prerequisites

- .NET 8 SDK
- Node.js 18+

### Backend

```bash
cd backend
dotnet restore
dotnet run --project src/API
```

API runs at `http://localhost:5000`

### Frontend

```bash
cd frontend
npm install
npm run dev
```

Frontend runs at `http://localhost:5173`

---

## API Endpoints

| Method | Endpoint                                                          | Description                             |
| ------ | ----------------------------------------------------------------- | --------------------------------------- |
| GET    | `/api/products/sizzling-hot/daily`                                | Top product for each day in the dataset |
| GET    | `/api/products/sizzling-hot/period?from=21/04/2026&to=23/04/2026` | Top product over a date range           |

### Response Shape

**Daily**

```json
[
  {
    "date": "21/04/2026",
    "product": {
      "id": "P1",
      "name": "Ezy Storage 37L Flexi Laundry Basket - White"
    }
  }
]
```

**Period**

```json
{
  "from": "21/04/2026",
  "to": "23/04/2026",
  "product": {
    "id": "P3",
    "name": "Arlec 160W Crystalline Solar Foldable Charging Kit"
  }
}
```

---

## Testing

Unit tests cover `SizzlingHotService` in isolation using in-memory test
data — no file I/O involved. The repository interfaces are stubbed so tests
remain fast and deterministic.

### Scenarios Covered

**Core business rules**

- Basic top product selection across multiple products
- Quantity is ignored — five units in one order counts as one sale (rule 1)
- Customer deduplication within a day across separate orders (rule 2)
- Cancellation credits the sale back to the original order's date (rule 3)
- Tie-breaking selects the alphabetically first product name (rule 4)

**Edge cases**

- Order with empty entries array is handled without errors
- Cancelled `orderId` with no matching completed order is safely ignored
- All orders for a single day are cancelled — day is omitted from results
- All orders across all days are cancelled — period result is empty
- Product ID in order has no match in products list — entry is skipped
- Empty orders input — returns empty result
- Empty products input — returns empty result
- Single product in dataset — returned as winner with no tie-break needed
- Every product ties — alphabetical ordering applied correctly at scale

---

## Folder Structure

```
bunnings-sizzling-hot/
├── backend/
│   ├── src/
│   │   ├── API/                  # Minimal API, DI wiring, endpoints
│   │   ├── Application/          # Interfaces, SizzlingHotService
│   │   ├── Domain/               # Order, OrderEntry, Product models
│   │   └── Infrastructure/       # JSON repositories, inputs folder
│   └── tests/
│       └── Application.Tests/    # xUnit unit tests
└── frontend/
    └── src/
        ├── components/           # DailySizzle, PeriodSizzle
        ├── hooks/                # useProducts data fetching hook
        └── App.tsx
```

---

## What I'd Improve With More Time

- **Persistent storage** — replace JSON file repositories with a PostgreSQL
  database via EF Core; the repository interfaces make this a straight swap
  with no changes to the service layer
- **Caching** — cache aggregated results with a short TTL since the
  underlying order data changes infrequently; `IMemoryCache` or Redis
  depending on scale
- **Date flexibility** — make "today" injectable via `TimeProvider` (.NET 8)
  rather than hardcoded, enabling real-time rolling windows and easier
  testing of time-dependent logic
- **Pagination** — for the daily endpoint when the dataset grows large
- **Authentication** — secure endpoints with JWT bearer tokens if this were
  customer-facing in production
- **CI/CD** — GitHub Actions pipeline running tests and linting on every PR
- **Observability** — structured logging via Serilog and error tracking via
  Sentry to monitor failures in production
- **Frontend error boundaries** — graceful UI degradation if the API is
  unavailable, rather than a broken state
