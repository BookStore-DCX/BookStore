# BookStore

## Local Configuration

Since the `appsettings.json` file is ignored from Git to keep credentials secure, you must create it manually to run the project.

1. **Create the file:** `appsettings.json` in the project root.
2. **Setup Database:** To perform **CRUD operations**, add your local database connection string by creating a `ConnectionStrings` section within the file.
3. **Paste this configuration:**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
