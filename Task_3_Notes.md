Task 3: Entity Framework Core Implementation

Added SQLite database using Entity Framework Core.

Technical Decisions

- Chose SQLite over In-Memory to demonstrate real persistence and migrations
- Repository pattern with Unit of Work for clean data access
- Created proper migrations with `dotnet ef migrations add InitialCreate`
- Added `Veterinarian` and `Owner` entities (normalized from Animal model)
- Minimal model changes - only added navigation properties required by EF Core

Implementation Details

- Generic `Repository<T>` base class for CRUD operations
- Specialized repositories only for complex queries (e.g., date range filtering)
- Fluent API configurations with indexes on foreign keys and query fields
- Auto-apply migrations on startup via `MigrateAsync()`

Navigation Property Fix

- POST endpoints broke due to model validation expecting navigation properties
- Created DTOs: `CreateAnimalRequest` and `CreateAppointmentRequest`
- Controllers map DTO → Entity → Save → Reload with navigation properties
- Provides clean API contracts without exposing domain model details

Configuration

- Moved EF Core SQL logging to Development environment only
- Connection string in appsettings.json: `"Data Source=veterinary.db"`

Assumptions

- Keeping models minimal aligns with existing codebase
- DTOs for create operations are standard practice
