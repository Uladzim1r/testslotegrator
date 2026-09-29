# ApiIntegratorTests

.NET integration test suite for the Slotegrator test API. The project covers the full player-management flow: authentication, player creation, profile retrieval, listing, and cleanup.

## Technology stack

- **.NET 10**
- **xUnit v3** with `Microsoft.Testing.Platform` runner
- **Refit** for typed HTTP clients
- **Shouldly** for assertions
- **Bogus** for fake request data
- **NSwag** for OpenAPI/DTO generation
- **xUnit.DependencyInjection** for DI container support

## Project structure

```
ApiIntegratorTests/
├── Converters/                     # Custom System.Text.Json converters
│   └── PlayerResponseDTOJsonConverter.cs
├── Extensions/
│   ├── TestCaseOrderer.cs          # xUnit test-ordering helpers
│   └── ValidationExtentions.cs     # Shouldly assertions for player DTOs
├── Fakers/
│   └── PlayerCreateRequestFaker.cs # Bogus rules for PlayerRequestDTO
├── Fixtures/
│   ├── CreateResourcesCollection.cs
│   └── CreateResoucesFixture.cs    # Tracks created players and deletes them on dispose
├── Generated/                      # Partial classes extending generated DTOs
│   └── PlayerResponseDTO.cs
├── Interfaces/
│   ├── IAuthApi.cs
│   └── IIntegratorAutomationApi.cs # Refit contracts
├── Services/
│   ├── AuthorizationHandler.cs     # Fetches and attaches Bearer token
│   └── LoggingHandler.cs           # Optional HTTP request/response logger
├── Specification/
│   └── IntegratorOpenApi.json      # OpenAPI spec used by NSwag
├── Tests/
│   └── IntegratorTests.cs          # Main test class
├── appsettings.json                # API base URL and credentials
└── Startup.cs                      # DI registration
```

## Covered test scenarios

The tests run in a fixed order (`TestOrder`) because later tests rely on state produced by earlier ones.

1. **Login** (`/api/tester/login`)  
   Verifies that the login endpoint returns a token.

2. **Create 12 players** (`/api/automationTask/create`)  
   Creates 12 distinct players and asserts HTTP 201 plus that each response matches the request.

3. **Get player by email** (`/api/automationTask/getOne`)  
   Creates a player, fetches the profile by email, and asserts the profile matches. Accepts HTTP 200 or 201 because the live API returns 201.

4. **Get all players sorted by name** (`/api/automationTask/getAll`)  
   Creates three named players (Alice, Bob, Charlie), fetches the full list, and verifies that the created players appear in the expected sorted order.

5. **Delete all players** (`/api/automationTask/deleteOne/{id}`)  
   Deletes every player returned by `getAll` and verifies the list is empty afterwards.

## Configuration

Copy or edit `appsettings.json` with your API credentials:

```json
{
  "IntegratorApiConfig": {
    "BaseUrl": "https://testslotegrator.com/",
    "Email": "your-email@example.com",
    "Password": "your-password"
  }
}
```

For local overrides you can also add an `appsettings.local.json` file (it is ignored by source control if added to `.gitignore`).

## Running the tests

The project uses the Microsoft.TestingPlatform runner, so the executable is built as a console app. Run it directly:

```powershell
dotnet build
.\ApiIntegratorTests\bin\Debug\net10.0\ApiIntegratorTests.exe
```

Or run a single test by filter:

```powershell
.\ApiIntegratorTests\bin\Debug\net10.0\ApiIntegratorTests.exe --filter "CreatePlayers_Returns201_AndMatchesSpec"
```

## Notes on API/spec mismatches

The live API deviates from the original OpenAPI spec in a few places. The project handles these via:

- A custom `PlayerResponseDTOJsonConverter` that maps both `_id` (create response) and `id` (get responses) to the same `Id` property.
- `PlayerResponseDTO.Id` modeled as `string` because the API returns MongoDB ObjectIds.
- `DeletePlayerAsync` accepts a `string id` for the same reason.
- `AuthorizationHandler` reads `accessToken` from the login response (camelCase) rather than `access_token`.

## Optional HTTP logging

`LoggingHandler` buffers request/response content into memory so downstream handlers and Refit can still read it. It is registered in DI but commented out in `Startup.cs` by default. Uncomment the `AddHttpMessageHandler<LoggingHandler>()` lines to enable verbose HTTP logs.
