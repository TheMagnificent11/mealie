---
applyTo: "**/*"
---

# Repository Structure

## Project Organization

```
mealie/
│
├── src/                          # Source projects
│   └── Mealie.AppHost/           # .NET Aspire orchestration entry point
│
├── tests/                        # Test projects (empty initially)
│
├── Directory.Build.props         # Root build properties (all projects)
├── Directory.Packages.props      # Central Package Management
└── .editorconfig                 # Root code style rules
```

## Configuration Files

| File | Purpose | When to Edit |
|------|---------|-------------|
| `mealie.slnx` | Solution with all projects | Adding/removing projects |
| `Directory.Build.props` | Global MSBuild properties (targets .NET 10.0) | Changing global build settings |
| `Directory.Packages.props` | Central Package Management (CPM) | Adding/updating NuGet packages |
| `.editorconfig` | Global code style rules | Adjusting global code formatting |
| `src/Mealie.AppHost/AppHost.cs` | Aspire orchestration | Configuring services |
| `.github/workflows/ci.yml` | CI/CD pipeline | Modifying build/test process |

## Common Development Tasks

| Task | Command | Use Case |
|------|---------|----------|
| Clean artifacts | `dotnet clean` | Remove build outputs |
| Full rebuild | `dotnet build --configuration Release --no-incremental` | After major changes |
| Run Aspire app | `dotnet run --project src/Mealie.AppHost/` | Start orchestrated services |
| Format code | `dotnet format` | Fix style issues |
