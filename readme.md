# Full-Stack Application with .NET Backend, Vue.js Frontend, and SQL Server Database

This project is a full-stack application consisting of a .NET backend API with audits, a Vue.js frontend, a SQL Server database, scheduled tasks, and database migrations using DbUp.

## Components

1. Backend API (.NET)
2. Frontend (Vue.js)
3. SQL Server Database
4. Scheduled Tasks
5. Database Migrations (DbUp)

## Prerequisites

- Docker
- Docker Compose

## Getting Started

1. Clone the repository:
   ```
   git clone <repository-url>
   cd <project-directory>
   ```

2. Start the application:
   ```
   docker-compose up --build
   ```

3. Access the application:
   - Frontend: http://localhost:9000
   - Backend API: http://localhost:5000

## Services

### Backend API

- Built with .NET
- Exposed on port 5000
- Audit logs are stored in a volume mounted at `./audits`

### Frontend

- Built with Vue.js
- Exposed on port 9000
- Depends on the backend service

### Scheduled Tasks

- .NET application for running scheduled tasks
- Depends on the backend and database services

### Database Migrations (DbUp)

- Handles database schema migrations and seeding
- Runs before the application starts to ensure the database is up-to-date

### SQL Server Database

- SQL Server 2019
- Exposed on port 1433
- Data persisted in a named volume `sqlserver_data`
- Credentials:
  - User: sa
  - Password: YourStrong@Passw0rd

## Configuration

The connection string for the database is set in the environment variables for the backend, scheduled tasks, and DbUp services. If you need to modify it, update the `ConnectionStrings__DefaultConnection` in the docker-compose.yml file.
