# Quick Start Guide

## Prerequisites
Install .NET 10.0 SDK from: https://dotnet.microsoft.com/download

## Running the Projects

### Option 1: Using Visual Studio
1. Open `CSharpChallenge.sln` in Visual Studio
2. Right-click on each project and select "Set as Startup Project"
3. Press F5 to run

### Option 2: Using Command Line

#### Run User API:
```bash
cd UserApi
dotnet restore
dotnet run
```
Then open browser to: http://localhost:5000/swagger

#### Run System Health Monitor:
```bash
cd SystemHealthMonitor
dotnet restore
dotnet run
```

## Testing the User API

### Using Swagger UI (Recommended)
1. Navigate to http://localhost:5000/swagger
2. Click on each endpoint to expand
3. Click "Try it out"
4. Click "Execute"

### Using Command Line (Windows PowerShell)
```powershell
# Create 10,000 users
Invoke-WebRequest -Method POST -Uri "http://localhost:5000/api/create-bulk-users"

# Fetch all users (will be cached)
Invoke-WebRequest -Uri "http://localhost:5000/api/fetch-users"

# Get user count
Invoke-WebRequest -Uri "http://localhost:5000/api/users/count"
```

### Using Command Line (Bash/Linux/Mac)
```bash
# Create 10,000 users
curl -X POST http://localhost:5000/api/create-bulk-users

# Fetch all users (will be cached)
curl http://localhost:5000/api/fetch-users

# Get user count
curl http://localhost:5000/api/users/count
```

## Expected Results

### User API
- First POST to `/api/create-bulk-users` creates 10,000 users (~10-15 seconds)
- First GET to `/api/fetch-users` reads from database (cache miss)
- Second GET to `/api/fetch-users` reads from cache (much faster)
- Check console logs to see cache hit/miss messages

### System Health Monitor
- Console displays metrics every 10 seconds
- Logs are written to `logs/system-health-YYYYMMDD.txt`
- Press Ctrl+C to stop

## Files Created

### User API
- `users.db` - SQLite database file (created automatically)

### System Health Monitor
- `logs/system-health-YYYYMMDD.txt` - Daily log files

## Troubleshooting

**If port 5000 is in use:**
```bash
dotnet run --urls "http://localhost:5555"
```

**If you see SSL/HTTPS errors:**
```bash
dotnet dev-certs https --trust
```

**To rebuild from scratch:**
```bash
dotnet clean
dotnet restore
dotnet build
dotnet run
```
