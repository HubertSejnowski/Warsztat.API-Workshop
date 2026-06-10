# Workshop Management System (Warsztat.API)

A robust, enterprise-ready backend REST API built with **.NET 10** and **Entity Framework Core** designed to streamline automotive workshop operations. The system automates vehicle reception, mechanic task assignments, real-time schedule collision validation, parts inventory tracking, and dynamic invoice/summary generation.

---

## 🚀 Key Features

* **Secure Authentication & Authorization:** Implements secure password hashing via **BCrypt** and stateless session management using **JWT (JSON Web Tokens)**.
* **Role-Based Access Control (RBAC):** Granular authorization mapping with distinct permissions for **Admin** (Workshop Owner), **Reception**, and **Mechanic** roles.
* **Smart Scheduling (Collision Prevention):** Algorithmic validation preventing double-booking of specific repair bays or diagnostic workstations at any given hour.
* **Inventory & Automated Billing:** Real-time logging of parts utilized during repairs (supporting OEM and aftermarket tracking) with dynamic calculation of final costs (parts matrix + standard labor fee).
* **Modern Tech Stack:** Leveraging the cutting-edge capabilities, features, and performance enhancements of **.NET 10**.

---

## 🛠️ Tech Stack & Architecture

* **Backend Framework:** .NET 10 (ASP.NET Core Web API)
* **Database ORM:** Entity Framework Core (Code-First Approach)
* **Database Engine:** Microsoft SQL Server
* **Security:** JWT Bearer Authentication, BCrypt.NET-Next
* **API Documentation:** Swagger / OpenAPI 

### Architectural Highlights
The codebase strictly adheres to clean coding practices and standard architectural separation of concerns:
* **Controllers:** Expose secure RESTful endpoints governed by strict HTTP verbs (`GET`, `POST`, `PUT`, `DELETE`).
* **Data Transfer Objects (DTOs):** Abstract database entities, ensuring input validation and decoupling the internal domain model from public contract endpoints.
* **Data Context:** Utilizes `DbContext` with custom model configurations and seamless relational mapping (One-to-Many, Many-to-Many).

---

## 🔒 Role Matrix & Endpoint Security

| Endpoint | HTTP Method | Allowed Roles | Description |
| :--- | :---: | :--- | :--- |
| `/api/Auth/register` | `POST` | Anonymous | Registers a new employee with a designated system role. |
| `/api/Auth/login` | `POST` | Anonymous | Authenticates credentials and returns a valid JWT token. |
| `/api/Customers` | `GET` / `POST` | Admin, Reception | View and manage client profiles. |
| `/api/Customers/{id}` | `DELETE` | Admin | Hard delete operation reserved strictly for management. |
| `/api/WorkOrders` | `POST` | Admin, Reception | Create new repair orders with time and bay validation. |
| `/api/WorkOrders/{id}/status` | `PUT` | Admin, Mechanic | Update workflow state (e.g., Planned -> In Progress -> Completed). |
| `/api/WorkOrders/{id}/parts` | `POST` | Admin, Mechanic | Link parts from inventory to an active repair ticket. |
| `/api/WorkOrders/{id}/summary` | `GET` | Admin, Reception | Generate complete dynamic balance sheets and invoices. |
| `/api/Workstations` | `POST` | Admin | Dynamically expand workshop infrastructure (bays, diagnostic areas). |

---

## ⚙️ Getting Started

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0)
* [SQL Server / LocalDB](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
* Visual Studio 2022 (Current preview or release supporting .NET 10)

### Configuration
1. Clone the repository:
   ```bash
   git clone [https://github.com/your-username/Warsztat.API.git](https://github.com/your-username/Warsztat.API.git)
   cd Warsztat.API

2. Open appsettings.json and adjust your connection string if needed:

JSON
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=WorkshopManagementDb;Trusted_Connection=True;"
}
Running Migrations & Database Setup
Execute the following commands in the Package Manager Console or your preferred CLI terminal to spin up the database schema and initialize seeding data:

PowerShell
Update-Database
Execution
Run the application via Visual Studio or via terminal command:

Bash
dotnet run
Once launched, navigate to http://localhost:your-port/swagger to explore the interactive API documentation and test endpoints.

🗺️ Roadmap
[x] Secure JWT Token Authorization with custom middleware integration.

[x] Intelligent scheduling and bay occupancy guardrails.

[x] Fully integrated parts inventory system linked to billing.

[ ] Containerization utilizing Docker and Docker-Compose for multi-service environments.

[ ] Front-end application utilizing React.js to build an interactive dashboard UI.

[ ] Automated PDF generator module for invoices.



   
