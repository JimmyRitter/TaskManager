# TaskManager - SaaS Task Management Application

A full-stack task management application built with .NET Core API and Vue.js frontend, featuring user authentication, shared task lists, and a Pomodoro timer widget.

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) (includes dotnet CLI)
- [Node.js](https://nodejs.org/) (v18 or later)
- [npm](https://www.npmjs.com/) or [yarn](https://yarnpkg.com/)
- [Docker](https://www.docker.com/) (required for PostgreSQL database)

## Project Structure

```
TaskManager/
├── api/                    # .NET Core API
│   ├── src/               # Source code
│   ├── infrastructure/    # Docker & migrations
│   └── tests/            # Unit tests
└── web/                   # Vue.js frontend
    ├── src/              # Source code
    └── public/           # Static assets
```

## Backend Setup (.NET Core API)

### 1. Start PostgreSQL Database

```bash
cd api/infrastructure/docker
docker compose up -d
```

### 2. Install EF Core CLI Tool

```bash
dotnet tool install --global dotnet-ef
```

### 3. Navigate to API Directory

```bash
cd ../../
```

### 4. Restore Packages

```bash
dotnet restore
```

### 5. Run Database Migrations

```bash
cd infrastructure/migrations
dotnet ef database update
```

### 6. Run the API

```bash
cd ../../src/SaasTaskManager.Api
dotnet run
```

The API will be available at `https://localhost:5161` (or check the console output for the exact URL).

## Frontend Setup (Vue.js)

### 1. Navigate to Web Directory

```bash
cd web
```

### 2. Install Dependencies

```bash
npm install
# or
yarn install
```

### 3. Run Development Server

```bash
npm run dev
# or
yarn dev
```

The frontend will be available at `http://localhost:5173` (or check the console output for the exact URL).

## Running Both Projects

1. **Start the Database and API** (in one terminal):
   ```bash
   # Start PostgreSQL database
   cd api/infrastructure/docker
   docker compose up -d
   
   # Run the API
   cd ../../src/SaasTaskManager.Api
   dotnet run
   ```

2. **Start the Frontend** (in another terminal):
   ```bash
   cd web
   npm run dev
   ```

## Available Scripts

### Backend
- `dotnet run` - Run the API in development mode
- `dotnet test` - Run unit tests
- `dotnet build` - Build the project

### Frontend
- `npm run dev` - Start development server
- `npm run build` - Build for production
- `npm run preview` - Preview production build
- `npm run lint` - Run ESLint

## License

This project is licensed under the MIT License. 