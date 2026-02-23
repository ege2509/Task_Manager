# Task Manager

A RESTful API built with ASP.NET Core and PostgreSQL for managing tasks with user authentication.

## Features

**User Authentication**: Register and login with JWT token-based authentication
**Task Management**: Create, read, update, and delete tasks 
**Secure**: Password hashing with BCrypt and JWT authorization
**Database**: PostgreSQL with Entity Framework Core ORM

## Tech Stack

**Framework**: ASP.NET Core 10.0
**Database**: PostgreSQL
**ORM**: Entity Framework Core
**Authentication**: JWT 
**Password Hashing**: BCrypt.Net

## To run this project on your device

### Prerequisites

- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [PostgreSQL](https://www.postgresql.org/download/)
- Optional: [Postman](https://www.postman.com/) for API testing

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/TaskManagementApi.git
   cd TaskManagementApi
   ```

2. **Install dependencies**
   ```bash
   dotnet restore
   ```

3. **Configure the database**
   
   Update `appsettings.json` with your PostgreSQL credentials:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=taskmanagementdb;Username=YOUR_USERNAME;Password=YOUR_PASSWORD"
     },
     "Jwt": {
       "Key": "YourSecretKeyHere-MustBeAtLeast16Characters!",
       "Issuer": "TaskManagementApi",
       "Audience": "TaskManagementApiUsers"
     }
   }
   ```

4. **Create the database**
   ```bash
   dotnet ef migrations add InitialCreate
   dotnet ef database update
   ```

5. **Run the application**
   ```bash
   dotnet run
   ```

   The API will be available at `http://localhost:5028`

## API Endpoints

### Authentication

#### Register a new user
```http
POST /api/auth/register
Content-Type: application/json

{
  "username": "johndoe",
  "email": "john@example.com",
  "password": "securepassword123"
}
```

**Response** (201 Created):
```json
{
  "id": 1,
  "username": "johndoe",
  "email": "john@example.com",
  "createdAt": "2026-02-23T10:30:00Z"
}
```

#### Login
```http
POST /api/auth/login
Content-Type: application/json

{
  "username": "johndoe",
  "password": "securepassword123"
}
```

**Response** (200 OK):
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": 1,
    "username": "johndoe",
    "email": "john@example.com",
    "createdAt": "2026-02-23T10:30:00Z"
  }
}
```

### Tasks 

- `GET /api/tasks` - Get all tasks for authenticated user
- `POST /api/tasks` - Create a new task
- `PUT /api/tasks/{id}` - Update a task
- `DELETE /api/tasks/{id}` - Delete a task
- `GET /api/tasks/{id}` - Get a task

## Project Structure

```
TaskManagementApi/
├── Controllers/
│   └── AuthController.cs       # Authentication endpoints
├── Data/
│   └── AppDbContext.cs         # Database context
├── DTOs/
│   ├── RegisterDto.cs          # Registration request
│   ├── LoginDto.cs             # Login request
│   ├── UserResponseDto.cs      # User response
│   └── LoginResponseDto.cs     # Login response with token
├── Models/
│   ├── User.cs                 # User entity
│   └── TaskItem.cs             # Task entity
├── Migrations/                 # EF Core migrations
├── appsettings.json           # Configuration
└── Program.cs                 # Application entry point
```




