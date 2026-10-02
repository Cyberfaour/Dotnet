# .NET application projects

A collection of focused application projects by **Ali Faour**, exploring business records, annual maintenance contracts, PDF generation, and account management with ASP.NET Core.

Start with **AMC_Generator** to inspect an owner → building → maintenance contract workflow. These are separate development projects, each with its own solution and configuration.

[Portfolio and current engineering work](https://cyberfaour.github.io/Portfolio/)

## Repository map

| Entry | Purpose | Start here |
| --- | --- | --- |
| `AMC_Generator/` | MVC application for owners, buildings, annual maintenance contracts, and generated Arabic contract PDFs. | [Project](AMC_Generator/AMC_Generator.csproj), [startup](AMC_Generator/Program.cs), [contract controller](AMC_Generator/Controllers/AMCsController.cs) |
| `IdentityService/` | MVC/Identity project with SQL Server storage, role support, and a confirmed-account sign-in requirement. | [Project](IdentityService/IdentityService.csproj), [startup](IdentityService/Program.cs), [database context](IdentityService/Data/ApplicationDbContext.cs) |
| `OrganizationManagementSystem` | A Git submodule reference (gitlink), rather than application source included in this checkout. There is no accompanying `.gitmodules` mapping, so a normal clone cannot initialize it from this repository alone. | Source location must be restored before this entry can be explored or built. |

## AMC Generator

### What the source implements

- Create, inspect, edit, and delete owner, building, and contract records.
- Associate buildings with owners and contracts with buildings.
- Search contracts by project number or building name, with active, expiring, expired, and terminated status filters.
- Generate or regenerate a contract PDF using QuestPDF and retain its relative path on the contract record.
- Store generated documents under `wwwroot/AMCs/BLD-{buildingId}/{year}/`.

The [contract model](AMC_Generator/Models/AMC.cs) contains date-based status calculations. The [PDF service](AMC_Generator/Services/AMCPDFService.cs) shows the Arabic document composition and output naming. The [building controller](AMC_Generator/Controllers/BuildingsController.cs) includes combined owner/building creation and lookup data used by the contract form.

### Source structure

| Folder | Responsibility |
| --- | --- |
| [Controllers](AMC_Generator/Controllers) and [Views](AMC_Generator/Views) | MVC requests, forms, registers, and record pages. |
| [Models](AMC_Generator/Models) and [ModelViews](AMC_Generator/ModelViews) | Stored entities and combined form data. |
| [Data](AMC_Generator/Data) and [Migrations](AMC_Generator/Migrations) | Entity Framework context and SQL Server schema migrations. |
| [Services](AMC_Generator/Services) | Contract PDF generation. |
| [UnitTest](AMC_Generator/UnitTest) | Model validation examples using xUnit and FluentAssertions. |

## Local setup reference

Both applications target **.NET 8** and use **SQL Server**. The checked-in project files pin different Entity Framework Core versions:

| Application | EF Core / matching `dotnet-ef` version | Connection-string key | DbContext |
| --- | --- | --- | --- |
| AMC Generator | `9.0.4` | `AMC_GeneratorContext` | `AMC_GeneratorContext` |
| Identity Service | `8.0.15` | `DefaultConnection` | `ApplicationDbContext` |

Install a .NET 8 SDK, provide your own development SQL Server databases, and use the matching EF Core CLI version for the project you are working on. Review the included migrations before applying them to those databases.

Set the connection string through local configuration or an environment variable:

- AMC Generator: `ConnectionStrings__AMC_GeneratorContext`.
- Identity Service: `ConnectionStrings__DefaultConnection`.

Keep machine-specific values and credentials outside committed configuration. The checked-in configuration refers to the original development setup and does not provision a local database.

From the repository root, restore and build the selected application:

```sh
dotnet restore AMC_Generator/AMC_Generator.csproj
dotnet build AMC_Generator/AMC_Generator.csproj --no-restore
```

After a successful build and database configuration, apply its migrations and launch it locally:

```sh
dotnet ef database update --project AMC_Generator --context AMC_GeneratorContext
dotnet run --project AMC_Generator
```

For Identity Service, use these corresponding commands with EF Core CLI `8.0.15`:

```sh
dotnet restore IdentityService/IdentityService.csproj
dotnet build IdentityService/IdentityService.csproj --no-restore
dotnet ef database update --project IdentityService --context ApplicationDbContext
dotnet run --project IdentityService
```

Use the local address printed by the application. AMC Generator starts at the contract register; Identity Service maps the home controller and Identity Razor Pages. The applications have separate startup pipelines; the repository does not configure Identity Service as authentication for AMC Generator.

## Reproduction notes

- This source snapshot has not been revalidated as a clean build or an end-to-end demo. The commands above describe the project structure and required setup, rather than a recorded successful run.
- `UnitTest/ModelTests.cs` is included in the AMC web project. There is no separate test project or configured test runner, so its presence is not evidence of a passing automated test suite.
- PDF output needs a writable `wwwroot/AMCs` directory and suitable fonts. The PDF service selects Times New Roman and optionally registers `wwwroot/fonts/Tajawal-Regular.ttf`; that optional font file is not included.
- Identity Service requires confirmed accounts. Configure the account-confirmation workflow and any roles for your local evaluation.
- Original IDE and build output files remain in the snapshot. Use the `.csproj` files and authored source as the review entry points.

My work represented here covers application models, MVC workflows, and document generation experiments. See the [portfolio](https://cyberfaour.github.io/Portfolio/) for the broader industrial software and automation context.
