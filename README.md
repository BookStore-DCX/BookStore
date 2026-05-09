# BookStore

## Local Configuration

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

```

---

## Password Hashing Update

This project uses `BCrypt.Net-Next` for password hashing, so you must update the password column size before running the project.

### Run this SQL query

```sql
ALTER TABLE [book].[dbo].[user]
ALTER COLUMN [Password] VARCHAR(255);

```

### Update `BookContext`

Change the password property configuration to:

```csharp
entity.Property(e => e.Password)
    .HasMaxLength(255)
    .IsUnicode(false);
```

---

- No additional migrations are required for this change.
