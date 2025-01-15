# PhoneBook API Backend

## Tech Stack
- ASP.NET Core Web API (.NET 8.0)
- Entity Framework Core with SQLite
- AutoMapper for object mapping
- Swagger/OpenAPI for API documentation
- xUnit and Moq for unit testing

## Project Structure
```
PhoneBookAPI/
├── Controllers/
│   └── ContactsController.cs      # API endpoints implementation
├── Models/
│   ├── Contact.cs                 # Domain model
│   └── PhoneBookDbContext.cs      # EF Core DbContext
├── DTOs/
│   ├── ContactCreateDto.cs        # DTO for creating contacts
│   ├── ContactUpdateDto.cs        # DTO for updating contacts
│   ├── ContactResponseDto.cs      # DTO for API responses
│   └── ContactSearchDto.cs        # DTO for search operations
├── Infrastructure/
│   ├── IRepository.cs             # Generic repository interface
│   └── Repository.cs              # Generic repository implementation
├── Mappings/
│   └── MappingProfile.cs          # AutoMapper configuration
└── Tests/
    └── ContactsControllerTests.cs  # Unit tests
```

## Prerequisites
- .NET SDK 8.0
- Visual Studio 2022 or VS Code
- SQLite

## Required NuGet Packages
```xml
<PackageReference Include="AutoMapper.Extensions.Microsoft.DependencyInjection" Version="12.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Sqlite" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.5.0" />
```

## Setup Instructions

1. Clone the repository:
```bash
git clone [repository-url]
cd PhoneBookAPI
```

2. Install EF Core tools (if not already installed):
```bash
dotnet tool install --global dotnet-ef
```

3. Update database connection string in appsettings.json:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=PhoneBook.db"
  }
}
```

4. Apply database migrations:
```bash
dotnet ef migrations add InitialCreate
dotnet ef database update
```

5. Run the application:
```bash
dotnet run
```

The API will be available at:
- https://localhost:7xxx/swagger - Swagger UI
- https://localhost:7xxx/api/contacts - API endpoints

## API Endpoints Documentation

### 1. Get All Contacts
```http
GET /api/contacts
```
Returns all contacts in the system.

### 2. Get Contact by ID
```http
GET /api/contacts/{id}
```
Returns a specific contact by ID.

### 3. Create Contact
```http
POST /api/contacts
Content-Type: application/json

{
    "name": "John Doe",
    "phoneNumber": "1234567890",
    "email": "john.doe@example.com"
}
```

### 4. Update Contact
```http
PUT /api/contacts/{id}
Content-Type: application/json

{
    "name": "John Doe Updated",
    "phoneNumber": "0987654321",
    "email": "john.updated@example.com"
}
```

### 5. Delete Contact
```http
DELETE /api/contacts/{id}
```

### 6. Search Contacts
```http
GET /api/contacts/search?searchTerm={term}
```

## Data Validation Rules
- Name: Required, max length 100 characters
- Phone Number: Required, max length 20 characters, must be unique
- Email: Optional, must be valid email format

## Testing
Run the unit tests using:
```bash
dotnet test
```

The test suite covers:
- CRUD operations
- Input validation
- Business rules
- Error handling
- Search functionality

## Error Handling
Global exception handling middleware provides consistent error responses:

```json
{
    "status": 400,
    "message": "Error description",
    "detailedMessage": "Detailed error information"
}
```

## Configuration Options

### CORS Configuration
CORS is configured to allow requests from the frontend application:
```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        builder =>
        {
            builder.WithOrigins("http://localhost:3000")
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});
```

### Swagger Configuration
Swagger is configured with XML documentation:
```xml
<PropertyGroup>
    <GenerateDocumentationFile>true</GenerateDocumentationFile>
    <NoWarn>$(NoWarn);1591</NoWarn>
</PropertyGroup>
```

## Database Management

### Create New Migration
```bash
dotnet ef migrations add MigrationName
```

### Update Database
```bash
dotnet ef database update
```

### Remove Last Migration
```bash
dotnet ef migrations remove
```

## Troubleshooting

### Common Issues

1. Database Connection Issues
```bash
# Verify connection string in appsettings.json
# Ensure SQLite file exists and has proper permissions
```

2. CORS Issues
```bash
# Check CORS policy configuration
# Verify allowed origins
```

3. Migration Issues
```bash
# Remove the database file
# Remove the Migrations folder
# Re-run migrations commands
```

## Performance Considerations
- Generic Repository pattern for data access
- Async/await for all I/O operations
- Proper indexing on PhoneNumber field
- AutoMapper for efficient object mapping
