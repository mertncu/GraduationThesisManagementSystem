# Graduation Thesis Management System (GTMS)

GTMS is a comprehensive web-based platform designed to streamline and digitize the graduation thesis process for universities. It facilitates the entire lifecycle of a thesis, from proposal submission and advisor assignment to jury selection, defense scheduling, and final grading.

Built with **.NET 8** and adhering to **Clean Architecture** principles, this project prioritizes maintainability, scalability, and loose coupling between components.

---

## 🏗 Architecture

The solution follows the **Onion Architecture** (Clean Architecture) pattern, ensuring a clear separation of concerns:

- **GTMS.Domain:** The core of the system. Contains enterprise-wide business rules, Entities (e.g., `Thesis`, `Defense`), Enums, and Value Objects. It has no dependencies on other projects.
- **GTMS.Application:** Contains application business rules. It implements Use Cases using **CQRS** (Command Query Responsibility Segregation) via **MediatR**. This layer depends only on the Domain layer.
- **GTMS.Infrastructure:** Implements interfaces defined in Application/Domain. Handles external concerns like Identity (Auth), Email (SMTP), File Storage, and other cross-cutting concerns.
- **GTMS.Persistence:** Handles data access. Implements repositories and the **Entity Framework Core** DbContext.
- **DTMS.Web:** The Presentation layer. An ASP.NET Core MVC application that interacts with users across different roles (Students, Advisors, Heads of Department).

### 📐 Logical Flow
Request (`DTMS.Web`) ➡️ Controller ➡️ MediatR Command/Query (`GTMS.Application`) ➡️ Domain Logic / Repository Interface ➡️ Implementation (`GTMS.Persistence`/`GTMS.Infrastructure`) ➡️ Database / External Service

---

## 🛠 Technology Stack

- **Framework:** .NET 8.0
- **Web App:** ASP.NET Core MVC
- **Data Access:** Entity Framework Core 8 (SQL Server)
- **Messaging/CQRS:** MediatR
- **Mapping:** AutoMapper
- **Validation:** FluentValidation
- **Authentication:** Cookie-based Auth & JWT Support
- **Security:** BCrypt for password hashing

---

## 📦 Key Features & Modules

The system is organized into several key modular features:

### 1. Identity & Access Management
- **Role-based Access Control (RBAC):** Supports Student, Advisor, Jury Member, Head of Department, and Admin roles.
- **User Management:** Secure registration, login, and profile management.

### 2. Academic Terms
- Management of active semesters and academic periods.
- Configuration of system-wide deadlines and rules per term.

### 3. Thesis Management
- **Proposals:** Students can submit thesis proposals; Advisors can review and approve them.
- **Milestones:** Tracking of thesis progress specific to department rules.
- **Monthly Reports:** Submission and review of progress reports.

### 4. Defense & Jury
- **Jury Selection:** Advisors can propose jury members; Department Heads review and appoint them.
- **Scheduling:** Coordination of defense dates and locations.
- **Grading:** Digital scoring sheets for jury members.
- **Events:** Management of defense events.

### 5. Notifications
- Automated email notifications for status changes (e.g., "Proposal Approved", "Defense Scheduled").

---

## 📂 Project Structure

```bash
GraduationThesisMS
├── GTMS.Domain          # Entities, Enums, Exceptions (Core)
├── GTMS.Application     # Features (CQRS), Interfaces, DTOs, Mappings
├── GTMS.Infrastructure  # Services (Email, FileStorage, Identity)
├── GTMS.Persistence     # DbContext, Migrations, Repositories, Seeding
└── DTMS.Web            # MVC Controllers, Views, ViewModels, Middleware
```

---

## 🚀 Getting Started

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server](https://www.microsoft.com/sql-server/) (LocalDB or Docker or Standalone)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

1.  **Clone the repository**
    ```bash
    git clone https://github.com/your-repo/GraduationThesisMS.git
    cd GraduationThesisMS
    ```

2.  **Configure Database**
    Open `DTMS.Web/appsettings.json` and update the `ConnectionStrings:DefaultConnection` with your SQL Server connection string.
    ```json
    "ConnectionStrings": {
      "DefaultConnection": "Server=localhost;Database=GTMS_DB;User Id=sa;Password=YourStrong!Passw0rd;..."
    }
    ```

3.  **Apply Migrations**
    Navigate to the root directory and run:
    ```bash
    dotnet ef database update --project GTMS.Persistence --startup-project DTMS.Web
    ```
    *Note: This will also seed initial data if configured in the `DbSeeder`.*

4.  **Run the Application**
    ```bash
    dotnet run --project DTMS.Web
    ```
    Or simply press `F5` in Visual Studio.

5.  **Access the App**
    Open your browser and navigate to `https://localhost:7087` (or the port indicated in the output).

---

## ⚙️ Configuration

- **Email Settings:** Configured in `appsettings.json` under `EmailSettings`.
- **JWT Settings:** Configured under `Jwt` section (for API purposes).
- **Global Exception Handling:** Implemented via middleware to ensure consistent error responses.

---

## 🤝 Contributing

1.  Fork the Project
2.  Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3.  Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4.  Push to the Branch (`git push origin feature/AmazingFeature`)
5.  Open a Pull Request

---

## 📄 License

Distributed under the MIT License. See `LICENSE` for more information.
