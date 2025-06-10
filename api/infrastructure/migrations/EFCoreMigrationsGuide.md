# Database Migrations Guide

## Project Structure
Migrations are managed in the dedicated project:
- Location: `./infrastructure/migrations/`
- Project: `SaasTaskManager.Migrations`

## Prerequisites
Install EF Core CLI tools (one-time setup):
```bash
dotnet tool install --global dotnet-ef
```

## Common Commands

### Create a New Migration
```bash
# From the migrations project directory
dotnet ef migrations add MigrationName

# Or with explicit project references (from any directory)
dotnet ef migrations add MigrationName \
    --project ./infrastructure/migrations/SaasTaskManager.Migrations.csproj \
    --startup-project ./src/SaasTaskManager.Api/SaasTaskManager.Api.csproj
```

### Apply Migrations
```bash
# From the migrations project directory
dotnet ef database update

# Or with explicit project references (from any directory)
dotnet ef database update \
    --project ./infrastructure/migrations/SaasTaskManager.Migrations.csproj \
    --startup-project ./src/SaasTaskManager.Api/SaasTaskManager.Api.csproj
```

### Remove Last Migration (if not applied)
```bash
dotnet ef migrations remove
```

### List Migrations
```bash
dotnet ef migrations list
```

### Generate SQL Script (without applying)
```bash
dotnet ef migrations script
```

## Best Practices

1. **Naming Conventions**
   - Use descriptive names: `AddUserTable`, `AddEmailVerificationFields`
   - Use present tense for actions: `AddUserPreferences` (not `Added...`)

2. **Migration Content**
   - Keep migrations focused and single-purpose
   - Review generated code before applying
   - Test migrations in development first

3. **Version Control**
   - Always commit migration files and snapshots
   - Include both `.cs` and `.Designer.cs` files

4. **Development Workflow**
   - Make model changes in entity classes
   - Create migration to capture changes
   - Review migration code
   - Test migration by applying
   - Commit changes

5. **Database Changes**
   - Always use migrations for database changes
   - Never modify the database schema directly
   - Keep migrations reversible when possible

## Troubleshooting

If you encounter errors:

1. Ensure you're in the correct directory
2. Verify connection string in `appsettings.json`
3. Check if database exists and is accessible
4. Ensure EF Core tools are installed globally