# C# Coding Challenge Solution

This solution contains two projects:
1. **UserApi** - Web API with SQLite database and caching
2. **SystemHealthMonitor** - Console application for system health monitoring

## Prerequisites

- .NET 10.0 SDK or later
- Windows/Linux/macOS

## Project 1: User API

### Features

- **SQLite Database** with Entity Framework Core
- **Three API Endpoints:**
  - `POST /api/create-users` - Creates a single user with random data
  - `POST /api/create-bulk-users` - Creates 10,000 users with random data
  - `GET /api/fetch-users` - Fetches all users (with caching)
- **Memory Caching** for improved performance
- **Bogus (Faker)** library for generating realistic random data
- **Swagger UI** for API testing

### Setup and Run

```bash
cd UserApi
dotnet restore
dotnet run
```

The API will start on `http://localhost:5000` (or `https://localhost:5001`)

### API Endpoints

#### 1. Create Single User
```bash
POST http://localhost:5000/api/create-users
```
Creates one user with randomly generated data.

#### 2. Create Bulk Users
```bash
POST http://localhost:5000/api/create-bulk-users
```
Creates 10,000 users with the following attributes:
- **Id** (auto-generated)
- **Name** (random full name)
- **Age** (random between 18-80)
- **Email** (random email)
- **TimeStamp** (current UTC time)

Response:
```json
{
  "message": "Successfully created 10,000 users",
  "count": 10000,
  "durationSeconds": 12.34
}
```

#### 3. Fetch Users
```bash
GET http://localhost:5000/api/fetch-users
```
Fetches all users from the database. Results are cached for 5 minutes (sliding) or 30 minutes (absolute).

Response:
```json
[
  {
    "id": 1,
    "name": "John Doe",
    "age": 35,
    "email": "john.doe@example.com",
    "timeStamp": "2026-02-11T15:30:00Z"
  },
  ...
]
```

#### Additional Helper Endpoints

**Get User Count:**
```bash
GET http://localhost:5000/api/users/count
```

**Clear Cache (for testing):**
```bash
DELETE http://localhost:5000/api/clear-cache
```

### Testing with Swagger

1. Navigate to `http://localhost:5000/swagger`
2. Use the interactive UI to test endpoints
3. View request/response schemas

### Testing with cURL

```bash
# Create bulk users
curl -X POST http://localhost:5000/api/create-bulk-users

# Fetch all users (first call - from DB)
curl http://localhost:5000/api/fetch-users

# Fetch all users (second call - from cache)
curl http://localhost:5000/api/fetch-users

# Check user count
curl http://localhost:5000/api/users/count
```

### Database

The SQLite database (`users.db`) will be created automatically in the project directory when you first run the application.

---

## Project 2: System Health Monitor

### Features

- **System Metrics Collection** every 10 seconds
- **CPU Usage** monitoring
- **Memory Usage** monitoring (process and system)
- **Serilog Logging** to both console and file
- **Timestamped Logs** with indexed time
- **Cross-platform** support (Windows/Linux/macOS)

### Setup and Run

```bash
cd SystemHealthMonitor
dotnet restore
dotnet run
```

### Output

The application will:
1. Display metrics in the console every 10 seconds
2. Log metrics to `logs/system-health-YYYYMMDD.txt`

**Console Output:**
```
[2026-02-11 15:30:00 INF] System Metrics - Timestamp: 02/11/2026 15:30:00, CPU Usage: 12.45%, Memory Used: 125.67 MB, Total Memory: 8192.00 MB, Memory Usage: 1.53%
[2026-02-11 15:30:00] CPU: 12.45%, Memory: 125.67MB / 8192.00MB (1.53%)
```

**Log File (`logs/system-health-20260211.txt`):**
```
[2026-02-11 15:30:00 INF] System Health Monitor started
[2026-02-11 15:30:00 INF] Monitoring system metrics every 10 seconds...
[2026-02-11 15:30:00 INF] System Metrics - Timestamp: 02/11/2026 15:30:00, CPU Usage: 12.45%, Memory Used: 125.67 MB, Total Memory: 8192.00 MB, Memory Usage: 1.53%
[2026-02-11 15:30:10 INF] System Metrics - Timestamp: 02/11/2026 15:30:10, CPU Usage: 15.23%, Memory Used: 126.12 MB, Total Memory: 8192.00 MB, Memory Usage: 1.54%
```

### Stopping the Monitor

Press `Ctrl+C` to gracefully stop the monitoring service.

### Log Files

- Logs are stored in the `logs/` directory
- One file per day with rolling interval
- Retained for 30 days by default
- Format: `system-health-YYYYMMDD.txt`

---

## Architecture & Design Decisions

### User API

1. **Entity Framework Core** - Used as ORM for type-safe database operations
2. **SQLite** - Lightweight, serverless database perfect for standalone applications
3. **Memory Cache** - Improves performance for frequently accessed data
4. **Bogus Library** - Generates realistic fake data for testing
5. **Batch Processing** - Bulk insert done in batches of 1000 for optimal performance

### System Health Monitor

1. **Serilog** - Structured logging framework
   - File sink for persistent logs
   - Console sink for real-time feedback
   - Rolling interval for log management
2. **Performance Counters** - Native Windows CPU monitoring
3. **Cross-platform CPU Monitoring** - Fallback method for Linux/Mac
4. **Timer-based Collection** - Precise 10-second intervals
5. **Graceful Shutdown** - Handles Ctrl+C for clean exit

---

## Solution Structure

```
CSharpChallenge/
│
├── UserApi/
│   ├── Controllers/
│   │   └── UsersController.cs
│   ├── Data/
│   │   └── UserDbContext.cs
│   ├── Models/
│   │   └── User.cs
│   ├── UserApi.csproj
│   ├── Program.cs
│   └── appsettings.json
│
└── SystemHealthMonitor/
    ├── Models/
    │   └── SystemMetrics.cs
    ├── Services/
    │   └── HealthMonitorService.cs
    ├── SystemHealthMonitor.csproj
    └── Program.cs
```

---

## Performance Considerations

### User API
- Batch insertions reduce database round-trips
- Memory caching prevents repeated database queries
- Sliding expiration keeps cache fresh
- Connection pooling managed by EF Core

### System Health Monitor
- Minimal CPU overhead from monitoring
- Efficient logging with Serilog
- Automatic log rotation prevents disk bloat
- Non-blocking asynchronous operations

---

## Testing Scenarios

### User API Test Sequence

1. **Initial State:**
   ```bash
   curl http://localhost:5000/api/users/count
   # Should return {"totalUsers": 0}
   ```

2. **Create Bulk Users:**
   ```bash
   curl -X POST http://localhost:5000/api/create-bulk-users
   # Watch console logs for progress
   ```

3. **First Fetch (Cache Miss):**
   ```bash
   curl http://localhost:5000/api/fetch-users > users.json
   # Check logs: "Cache miss - fetching from database"
   ```

4. **Second Fetch (Cache Hit):**
   ```bash
   curl http://localhost:5000/api/fetch-users
   # Check logs: "Cache hit - returning 10000 cached users"
   ```

5. **Verify Data:**
   ```bash
   curl http://localhost:5000/api/users/count
   # Should return {"totalUsers": 10000}
   ```

### System Health Monitor Test

1. **Run the monitor:**
   ```bash
   cd SystemHealthMonitor
   dotnet run
   ```

2. **Observe output** in console every 10 seconds

3. **Check log file:**
   ```bash
   cat logs/system-health-*.txt
   ```

4. **Generate load** (optional):
   - Open multiple applications
   - Run CPU-intensive tasks
   - Observe metrics change

---

## Troubleshooting

### User API

**Issue:** Database not created
- **Solution:** Ensure write permissions in the project directory

**Issue:** Port already in use
- **Solution:** Change port in `Properties/launchSettings.json` or use:
  ```bash
  dotnet run --urls "http://localhost:5555"
  ```

**Issue:** Cache not working
- **Solution:** Check logs for cache hit/miss messages

### System Health Monitor

**Issue:** Permission denied for performance counters (Windows)
- **Solution:** Run as Administrator or use the Linux fallback method

**Issue:** Logs directory not created
- **Solution:** Ensure write permissions; Serilog creates it automatically

---

## Extensions & Improvements

Potential enhancements:

### User API
- Add pagination to `fetch-users` endpoint
- Implement Redis for distributed caching
- Add filtering and search capabilities
- Implement user update/delete operations
- Add authentication and authorization

### System Health Monitor
- Add disk usage monitoring
- Network bandwidth tracking
- Alert thresholds with notifications
- Export metrics to Prometheus/Grafana
- Database storage for historical metrics
- Dashboard UI for visualization

---

## License

This is a coding challenge solution for educational purposes.
