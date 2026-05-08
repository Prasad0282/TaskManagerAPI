# 🚀 Task Manager API

A RESTful API built with .NET 8 for managing tasks and projects with JWT authentication.

## 🛠️ Tech Stack

- **Backend:** .NET 8 Web API
- **Database:** SQLite with Entity Framework Core
- **Authentication:** JWT (JSON Web Tokens)
- **Documentation:** Swagger/OpenAPI

## 🏗️ Architecture

\`\`\`
Client → REST API → Controllers → Services → EF Core → SQLite
↓
JWT Authentication
\`\`\`

## 📊 Database Schema

![Database Schema](schema.png)

### Tables:

- **Users:** User accounts with credentials
- **Projects:** Project containers for tasks
- **Tasks:** Individual task items

## 🚀 Quick Start

### Prerequisites

- .NET 8 SDK
- Any OS (Windows/Mac/Linux)

### Installation

\`\`\`bash

# Clone repository

git clone https://github.com/YOUR_USERNAME/TaskManagerAPI.git

# Navigate to project

cd TaskManagerAPI/TaskManagerAPI

# Restore packages

dotnet restore

# Run the application

dotnet run
\`\`\`

### API Endpoints

Open `http://localhost:5252/swagger` to see all endpoints.

## 🔑 Authentication

1. **Register:** `POST /api/auth/register`
2. **Login:** `POST /api/auth/login`
3. Use token in header: `Authorization: Bearer <token>`

## 📝 API Endpoints

### Auth

- `POST /api/auth/register` - Create new account
- `POST /api/auth/login` - Login to account

### Projects (Requires Auth)

- `POST /api/projects` - Create project
- `GET /api/projects` - Get all projects
- `GET /api/projects/{id}` - Get project with tasks

### Tasks (Requires Auth)

- `POST /api/tasks` - Create task
- `GET /api/tasks` - Get all tasks
- `GET /api/tasks/{id}` - Get specific task
- `PATCH /api/tasks/{id}/status` - Update task status
- `DELETE /api/tasks/{id}` - Delete task

## 🎯 Features

- ✅ User registration & login
- ✅ JWT token authentication
- ✅ Project management
- ✅ Task CRUD operations
- ✅ Task status tracking (Todo, InProgress, Done)
- ✅ Swagger documentation
- ✅ SQLite database (no setup required)

## 📈 What I Learned

- Building RESTful APIs with .NET 8
- Entity Framework Core ORM
- JWT authentication implementation
- Clean Architecture patterns
- Swagger/OpenAPI documentation
- Dependency Injection in .NET
- Database design and relationships

## 🔜 Next Steps

- [ ] Add unit tests
- [ ] Docker containerization
- [ ] Deploy to cloud (AWS/Azure)
- [ ] Add user roles & permissions
- [ ] Implement pagination
- [ ] Add request validation
