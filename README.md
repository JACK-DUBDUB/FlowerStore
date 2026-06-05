# FlowerStore - MVC Web Application with REST API

A modern **ASP.NET Core MVC** web application integrated with a **RESTful API** for managing a flower store inventory and customer-facing e-commerce features. 
Built as part of an academic assessment (AT2-MVC), the project demonstrates clean separation of concerns, authentication/authorization, and modern backend practices.

## Features

- **MVC Web Application** (Razor Views): User-friendly frontend for browsing products, categories, and store information.
- **RESTful API**: Backend services with full CRUD operations for products and related entities.
- **Authentication & Authorization**:
  - JWT Bearer token authentication.
  - MongoDB-backed Identity system (using AspNetCore.Identity.MongoDbCore).
  - Role-based policies (e.g., Admin, User).
- **Database**:
  - MongoDB for user/identity management.
  - In-memory database (EF Core) for products with seed data.
- **API Features**:
  - API Versioning (`FlowerStore-API-Version` query parameter).
  - Swagger/OpenAPI documentation with JWT support.
  - CORS configuration for frontend integration.
- **Additional**:
  - Seed data for flower products, categories, and Perth-area store locations.
  - Centralized site settings configuration.
  - Clean architecture with DTOs, Models, and Services.

## Project Structure

```
FlowerStore/
├── API_AND_WEB/
│   ├── Assessment2 MVC API/          # REST API project
│   └── Assessment2 MVC Web/          # MVC Web Application
├── data/
│   ├── Seed_data_1.txt               # Product seeding logic
│   └── Seed_data_2.txt
└── README.md
```

## Technologies Used

- **Backend**: ASP.NET Core 8 (C#)
- **Frontend**: Razor Pages/Views, HTML, CSS, JavaScript
- **Database**: MongoDB (Identity), EF Core In-Memory
- **Authentication**: JWT + MongoDB Identity
- **Documentation**: Swagger UI
- **Other**: API Versioning, CORS, Dependency Injection

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- MongoDB instance (local or cloud, e.g., MongoDB Atlas)
- Visual Studio 2022 (recommended) or VS Code

### Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/JACK-DUBDUB/FlowerStore.git
   cd FlowerStore
   ```

2. **Configure Secrets / Environment Variables**

   Copy `appsettings.example.json` to `appsettings.Development.json` (or use User Secrets) and update:

   ```json
   {
     "ConnectionStrings": {
       "DbConnection": "your_mongodb_connection_string_here"
     },
     "JwtConfig": {
       "Key": "your_super_secret_jwt_key_here_at_least_32_chars",
       "Issuer": "https://localhost:your-api-port",
       "Audience": "https://localhost:your-web-port"
     },
     "Site": {
       "Title": "FlowerStore",
       "DisplayName": "FlowerStore Perth"
     }
   }
   ```

   > **Security Note**: Never commit sensitive configuration files.

3. **Restore and Build**
   ```bash
   dotnet restore
   dotnet build
   ```

4. **Run the Projects**

   - **API** (typically on port 5xxx):
     ```bash
     cd API_AND_WEB/Assessment2\ MVC\ API
     dotnet run
     ```

   - **Web App** (typically on port 7xxx):
     ```bash
     cd API_AND_WEB/Assessment2\ MVC\ Web
     dotnet run
     ```

5. **Access the Application**
   - Web UI: `https://localhost:7272` (or configured port)
   - API + Swagger: `https://localhost:5xxx/swagger`

## Seeding Data

The project includes sample flower product data with Perth metropolitan store locations. The `StoreContext` will automatically seed on startup.

## API Endpoints (Example)

- `GET /api/products` – List products
- `GET /api/products/{id}` – Get product details
- Protected endpoints require JWT Bearer token with appropriate role.

Full documentation available via Swagger UI when running in Development mode.

## Contributing

This is primarily an academic/project portfolio repository. Feel free to fork and experiment. 

## License

This project is for educational/portfolio purposes. All rights reserved.
