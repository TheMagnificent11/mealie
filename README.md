# Mealie Aspire Orchestration

This repository provides a .NET Aspire orchestration for [Mealie](https://mealie.io/), a self-hosted recipe manager and meal planner.

## Overview

This Aspire application orchestrates:
- **Mealie** - A self-hosted recipe manager and meal planner
- **PostgreSQL** - The data store used by Mealie

## Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
- [.NET Aspire workload](https://learn.microsoft.com/en-us/dotnet/aspire/fundamentals/setup-tooling?tabs=linux#install-net-aspire) (`dotnet workload install aspire`)
- [Docker Desktop](https://www.docker.com/products/docker-desktop/) (for PostgreSQL and Mealie containers)

## Running the Application

Execute the following in a terminal at the root of this Git repository:

```bash
dotnet run --project ./src/Mealie.AppHost/
```

This will start the .NET Aspire dashboard and orchestrate the PostgreSQL database and Mealie application.

Navigate to the Aspire dashboard to monitor services and view logs.

The Mealie application will be available at the URLs shown in the Aspire dashboard.

## Build

Build the solution:

```bash
dotnet build --configuration Release --nologo
```

## Structure

- `src/` - Source projects
  - `Mealie.AppHost/` - .NET Aspire orchestration project
- `tests/` - Test projects (empty initially)
