# BookStore

## 🛠️ Local Configuration

Since the `appsettings.json` file is ignored by Git to keep credentials secure, you must create it manually to run the project locally.

1. **Create the file:** `appsettings.json` in the project root directory.
2. **Setup Database:** Update the `DefaultConnection` with your local SQL Server instances.
3. **JWT Authentication:** 
   - The `Secret` must be at least 32 characters long.
   - The `ExpiryMinutes` is set to 30 minutes by default.
4. **Paste this configuration:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_DB_CONNECTION;Database=YOUR_DB_NAME;Trusted_Connection=True;TrustServerCertificate=True;"
  },
  "JwtSettings": {
    "Secret": "YourSuperSecretKeyThatIsAtLeast32CharactersLong!!",
    "Issuer": "BookStoreAPI",
    "Audience": "BookStoreClient",
    "ExpiryMinutes": 30
  }
}
