# 🚀 Task Manager

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![React](https://img.shields.io/badge/React-18-61DAFB?logo=react)](https://react.dev/)
[![TypeScript](https://img.shields.io/badge/TypeScript-5-3178C6?logo=typescript)](https://www.typescriptlang.org/)
[![Tailwind CSS](https://img.shields.io/badge/Tailwind-3-06B6D4?logo=tailwindcss)](https://tailwindcss.com/)
[![SQLite](https://img.shields.io/badge/SQLite-3-003B57?logo=sqlite)](https://sqlite.org/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

A full‑stack task management application with a **.NET 8 Web API** backend and a **React + TypeScript** frontend.  
**Authenticate**, manage **projects**, **tasks**, **categories**, and **tags** with powerful **filtering**, **sorting**, and **pagination** – all in one slick interface.

---

## 🏗️ Architecture

```mermaid
flowchart TB
    subgraph Frontend["Frontend (React + Vite)"]
        Login["Login / Register"]
        Dashboard["Dashboard"]
        TaskList["Task List\n(with filters, pagination)"]
        TaskForm["Task Form (Create / Edit)"]
    end

    subgraph Backend["Backend (.NET 8 Web API)"]
        AuthController["Auth Controller\n(JWT Authentication)"]
        ProjectsController["Projects Controller"]
        TasksController["Tasks Controller"]
        CategoriesController["Categories Controller"]
    end

    subgraph Database["Database"]
        SQLite["SQLite\n(Users, Projects, Tasks, Categories)"]
    end

    Frontend -- "REST API (JSON)" --> Backend
    Backend -- "Entity Framework Core" --> Database

    AuthController -- "Validates / Generates JWT" --> SQLite
    ProjectsController -- "CRUD on Projects" --> SQLite
    TasksController -- "CRUD on Tasks\n+ Filtering / Pagination" --> SQLite
    CategoriesController -- "CRUD on Categories" --> SQLite
✨ Features
User Authentication – Register & login with JWT tokens, protected routes

Project Management – Create, list, and delete projects

Task Management – Full CRUD with status flow (Todo → InProgress → Done)

Priorities & Tags – Critical / High / Medium / Low priority; comma‑separated tags

Categories – Organize tasks into custom categories

Advanced Filtering – By status, priority, project, category, or text search

Pagination & Sorting – Browse large task lists efficiently

Swagger UI – Explore and test the API directly from the browser

Unit Tests – xUnit tests for core services (TaskService)

Responsive Frontend – Tailwind CSS, works on desktop & mobile

📁 Project Structure
text
TaskManagerAPI/
├── frontend/                     # React + TypeScript frontend
│   ├── src/
│   │   ├── components/           # Layout, ProtectedRoute, Pagination
│   │   ├── config/               # Axios API client
│   │   ├── pages/                # Login, Register, Dashboard, TaskList, TaskForm
│   │   ├── services/             # auth, task, project, category API calls
│   │   └── types/                # TypeScript interfaces
│   └── ...
├── TaskManagerAPI/               # .NET 8 Web API backend
│   ├── Controllers/              # Auth, Projects, Tasks, Categories
│   ├── Data/                     # AppDbContext (EF Core)
│   ├── Middleware/                # Error handling middleware
│   ├── Models/                   # Entities and DTOs
│   └── Services/                 # Business logic (AuthService, TaskService)
├── TaskManagerAPI.Tests/         # xUnit tests
└── TaskManagerAPI.sln
## 🐳 Docker & CI/CD

- **Containerized** with Docker (multi-stage builds for .NET and React + Nginx)
- **Docker Compose** for local full-stack development
- **GitHub Actions** CI/CD pipeline automatically builds and pushes images to GitHub Container Registry on every push to `main`
- **Azure-ready / AWS-ready** – images can be deployed to any container service

### Run with Docker

\`\`\`bash
docker compose up --build
\`\`\`

Frontend: `http://localhost:3000`
Backend Swagger: `http://localhost:5252/swagger`

### Public Container Images

- Backend: `ghcr.io/Prasad0282/taskmanager-backend:latest`
- Frontend: `ghcr.io/Prasad0282/taskmanager-frontend:latest`
🚀 Quick Start
Prerequisites
.NET 8 SDK

Node.js (v18+)

npm

1. Clone the repository
bash
git clone https://github.com/Prasad0282/TaskManagerAPI.git
cd TaskManagerAPI
2. Run the backend
bash
cd TaskManagerAPI
dotnet restore
dotnet run
The API starts at http://localhost:5252.
Open http://localhost:5252/swagger to see interactive documentation.

3. Run the frontend
Open a new terminal in the root folder:

bash
cd frontend
npm install
npm run dev
The React app starts at http://localhost:5173.
It automatically connects to the backend API.

🔑 Usage Guide
Register a new account (or login with existing credentials)

Create a project (via Dashboard or Swagger)

Create categories (optional) to organize tasks

Add tasks with priorities, tags, and due dates

Use the task list to filter, sort, and change statuses

🧪 Testing
Run the backend unit tests:

bash
cd TaskManagerAPI.Tests
dotnet test
These tests cover the TaskService logic using an in‑memory database.

🌐 API Endpoints (Backend)
Method	Endpoint	Auth	Description
POST	/api/auth/register	No	Register a new user
POST	/api/auth/login	No	Login and receive JWT
GET	/api/projects	Yes	List user projects
POST	/api/projects	Yes	Create a project
DELETE	/api/projects/{id}	Yes	Delete a project
GET	/api/categories	Yes	List user categories
POST	/api/categories	Yes	Create a category
DELETE	/api/categories/{id}	Yes	Delete a category
GET	/api/tasks	Yes	List tasks (with filters)
GET	/api/tasks/{id}	Yes	Get a single task
POST	/api/tasks	Yes	Create a task
PUT	/api/tasks/{id}	Yes	Update a task
PATCH	/api/tasks/{id}/status	Yes	Update task status only
DELETE	/api/tasks/{id}	Yes	Delete a task
Query parameters for GET /api/tasks:
status, priority, searchTerm, tag, categoryId, projectId, sortBy, sortOrder, pageNumber, pageSize

🛠️ Tech Stack
Layer	Technology
Frontend	React 18, TypeScript, Vite, Tailwind CSS, Axios
Backend	.NET 8 Web API, Entity Framework Core, SQLite
Auth	JWT (JSON Web Tokens)
Testing	xUnit, Moq, FluentAssertions
Documentation	Swagger / OpenAPI
Versioning	Git, GitHub
📈 What I Learned
Building a complete REST API from scratch with .NET 8

Entity Framework Core (SQLite) – migrations, relationships, seed data

JWT authentication & authorization with middleware

Clean Architecture (Controllers → Services → Data)

Frontend development with React, TypeScript, and Tailwind

State management with React hooks and Axios interceptors

Advanced filtering, sorting, and pagination patterns

Writing unit tests with xUnit and Moq
```
