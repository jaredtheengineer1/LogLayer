# LogLayer

LogLayer is a multi-tenant logging and event management system designed to provide robust logging capabilities and event tracking. It features a RESTful API for logging events and retrieving logs, as well as a management page for monitoring and analyzing data.

## Features

- **Logs API**:
  - `GET /logs`: Retrieve all logs within a given time period with minimal filters.
  - `GET /logs/{id}`: Retrieve all logs for a specific session or tracking ID (GUID).
- **Events API**:
  - `GET /events`: Retrieve a list of all logged events.
  - `GET /events/{event}`: Retrieve event counts per hour/day (configurable).
- **Log Endpoint**:
  - `POST /log`: Allows frontend systems to log events.

## Getting Started

### Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/)
- PostgreSQL database
- Environment file (`.env`) for sensitive configuration

### Installation

1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd LogLayer
   ```

2. Install dependencies:
   ```bash
   dotnet restore
   ```

3. Create a `.env` file in the root directory and add the following:
   ```env
   DB_PASSWORD=your-secure-password
   ```

4. Update the `appsettings.json` file to include the database connection string:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=your-host;Port=5432;Database=your-database;Username=your-username;Password={DB_PASSWORD}"
     }
   }
   ```
   The `{DB_PASSWORD}` placeholder will be replaced with the value from the `.env` file at runtime.

5. Run the application:
   ```bash
   dotnet run
   ```

### Usage

#### Logs API

- **Retrieve Logs**:
  ```bash
  GET /logs?start=2026-04-01&end=2026-04-05
  ```
  Query Parameters:
  - `start`: Start date (ISO 8601 format)
  - `end`: End date (ISO 8601 format)

- **Retrieve Logs by ID**:
  ```bash
  GET /logs/{id}
  ```
  Path Parameters:
  - `id`: The session or tracking GUID

#### Events API

- **Retrieve Events**:
  ```bash
  GET /events
  ```

- **Retrieve Event Counts**:
  ```bash
  GET /events/{event}?interval=hour
  ```
  Path Parameters:
  - `event`: The name of the event
  Query Parameters:
  - `interval`: Time interval (`hour` or `day`)

#### Log Endpoint

- **Log an Event**:
  ```bash
  POST /log
  ```
  Request Body:
  ```json
  {
    "sessionId": "GUID",
    "trackingId": "GUID",
    "event": "event-name",
    "data": {
      "key": "value"
    }
  }
  ```

## Environment Variables

The `.env` file is used to store sensitive information such as the database password. This file is included in `.gitignore` to prevent it from being committed to version control.

### Example `.env` File
```env
DB_PASSWORD=your-secure-password
```

#### Updating `appsettings.json`
To use the `.env` file, update the `appsettings.json` file to include the database connection string with a placeholder for the password:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=your-host;Port=5432;Database=your-database;Username=your-username;Password={DB_PASSWORD}"
  }
}
```

The `{DB_PASSWORD}` placeholder will be replaced with the value from the `.env` file at runtime.

#### Setting Up Environment Variables

- On **Windows**:
  ```bash
  setx DB_PASSWORD "your-secure-password"
  ```
- On **Linux/Mac**:
  ```bash
  export DB_PASSWORD="your-secure-password"
  ```

Ensure the `.env` file is properly configured before running the application.