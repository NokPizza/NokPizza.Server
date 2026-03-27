# Azure Production Setup

## App Service configuration

Set the production database connection string in Azure App Service as an app setting:

- `ConnectionStrings__nok-pizza-db`

Use an Azure SQL connection string that relies on managed identity:

```text
Server=tcp:<server-name>.database.windows.net,1433;Database=<database-name>;Authentication=Active Directory Default;Encrypt=True;TrustServerCertificate=False;
```

The app also supports the Azure App Service connection-string environment variable name:

- `SQLCONNSTR_nok-pizza-db`

## Recommended production flow

1. Deploy `source/NokPizza.Server` to Azure App Service.
2. Enable the App Service system-assigned managed identity.
3. Set a Microsoft Entra admin on the Azure SQL logical server.
4. Create a database user for the App Service managed identity and grant the required roles.
5. Add the `ConnectionStrings__nok-pizza-db` setting in App Service configuration.

## Azure SQL permissions example

Run the following against the target database after the managed identity exists:

```sql
CREATE USER [<app-service-name>] FROM EXTERNAL PROVIDER;
ALTER ROLE db_datareader ADD MEMBER [<app-service-name>];
ALTER ROLE db_datawriter ADD MEMBER [<app-service-name>];
ALTER ROLE db_ddladmin ADD MEMBER [<app-service-name>];
```

`db_ddladmin` allows the app to apply EF Core migrations on startup. If you later move migrations into CI/CD, you can remove that role and grant fewer permissions to the runtime app.
