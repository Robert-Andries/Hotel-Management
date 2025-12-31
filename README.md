 Hotel Management Application

A robust Clean Architecture solution for managing hotel operations, featuring both a Desktop (WPF) and Web (ASP.NET Core MVC) interface.

## Overview

This application manages the core functions of a hotel, including:
- **User Management**: Guest and Staff administration.
- **Bookings**: Reservation creation, confirmation, and management.
- **Room Management**: Searching for rooms based on criteria (date, price, features).

The solution is built following **Domain-Driven Design (DDD)** and **Clean Architecture** principles to ensure maintainability, testability, and scalability.

## Technical Architecture

The solution (`Hotel Management App.sln`) is structured into four main layers:

1.  **HM.Domain**: The core of the system. Contains entities (`User`, `Room`, `Booking`), value objects, and domain events. Dependent on nothing.
2.  **HM.Application**: Application business logic. Contains CQRS commands/queries (`CreateBookingForGuestCommand`, `GetUsersQuery`), validators, and interfaces.
3.  **HM.Infrastructure**: External concerns. Contains EF Core `DbContext`, Repositories, email services, and file system access.
4.  **HM.Presentation**:
    - **HM.Presentation.WPF**: Desktop client for administrative staff.
    - **HM.Presentation.WebUI**: Web interface for guests.

### Tech Stack
- **Framework**: .NET 9.0
- **Database**: SQL Server
- **ORM**: Entity Framework Core 9
- **Architecture**: CQRS (Mediator Pattern), Clean Architecture
- **Testing**: xUnit, FluentAssertions, Moq, Coverlet
- **CI Pipeline**: GitHub Actions (Build, Test, Snyk Scan, SonarQube)

## Setup Instructions

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- SQL Server
- A preferred IDE (Visual Studio 2022, JetBrains Rider, or VS Code)

### Installation
1.  **Clone the repository**:
    ```bash
    git clone https://github.com/your-username/hotel-management.git
    cd hotel-management
    ```

2.  **Configure Database**:
    Update the connection string in `appsettings.json` (from Presentation.WebUI) and `appsettings.json` (from Presentation.WPF) .

3.  **Apply Migrations**:
    Navigate to the Infrastructure project and update the database:
    ```powershell
    dotnet ef database update --project "HM\Hotel Management App\HM.Infrastructure" --startup-project "HM\Hotel Management App\HM.Presentation.WebUI"
    ```

4.  **Run the Application**:
    - **Web**: `dotnet run --project "HM\Hotel Management App\HM.Presentation.WebUI"`
    - **Desktop**: Open the solution in Visual Studio and set `HM.Presentation.WPF` as the startup project.

## Testing

The solution emphasizes strong testing coverage:

- **Unit Tests**: `dotnet test "HM\Hotel Management App\HM.Tests.UnitTests"`
- **Integration Tests**: `dotnet test "HM\Hotel Management App\HM.Tests.IntegrationTests"`
- **Architecture Tests**: Ensures dependency rules are respected.
- **End to End Testing**: Using Playwright

### Code Coverage
We enforce a strict code coverage policy.
- **Run Coverage**:
  ```powershell
  dotnet test --collect:"XPlat Code Coverage"
  ```
- **Generate Report**:
  ```powershell
  reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coverage" -reporttypes:Html
  ```
  *Current CI Limit: 50% coverage required (excluding UI/Infra).*

## CI Pipeline

The project uses GitHub Actions for Continuous Integration:
- Triggers on `push` and `pull_request` to `main`.
- Builds the solution in `Debug` and `Release`.
- Runs full test suite with coverage checks.
- Scans for vulnerabilities using **Snyk**.
- Analyze code using **SonarQube**
