# User Management React App

React application to interact with the User API.

## Features

✅ **Display Users in Table** - Paginated view of all users
✅ **Create User Form** - Submit new users via form
✅ **Create Bulk Users** - Generate 10,000 users with one click
✅ **Pagination** - Navigate through pages of users
✅ **Real-time Updates** - Auto-refresh after creating users
✅ **Responsive Design** - Works on desktop and mobile
✅ **Beautiful UI** - Modern gradient design with animations

## Prerequisites

- Node.js 14+ installed
- .NET API running on http://localhost:5000

## Setup Instructions

### 1. Install Dependencies

```bash
cd UserManagementReact
npm install
```

This will install:
- React 18.2.0
- React DOM 18.2.0
- Axios 1.6.0
- React Scripts 5.0.1

### 2. Start the API

Make sure your .NET API is running first:

```bash
cd ../UserApi
dotnet run
```

The API should be running on `http://localhost:5000`

### 3. Start React App

In a new terminal:

```bash
cd UserManagementReact
npm start
```

The app will open at `http://localhost:3000`

## Usage

### Create Single User

1. Fill in the form with:
   - Name (required)
   - Age (18-120, required)
   - Email (valid email, required)
2. Click "Create User"
3. User will be added and table will refresh

### Create Bulk Users

1. Click "Create 10,000 Users" button
2. Wait 10-15 seconds
3. Table will refresh with new users

### View Users

- Table shows 100 users per page
- Use pagination controls:
  - **First** - Go to first page
  - **Previous** - Previous page
  - **Next** - Next page
  - **Last** - Go to last page

### Refresh Users

Click "Refresh Users" to reload current page

## API Endpoints Used

| Endpoint | Method | Purpose |
|----------|--------|---------|
| `/api/fetch-users` | GET | Fetch users with pagination |
| `/api/create-users` | POST | Create single user |
| `/api/create-bulk-users` | POST | Create 10,000 users |

## Project Structure

```
UserManagementReact/
├── public/
│   └── index.html          # HTML template
├── src/
│   ├── App.js             # Main component
│   ├── App.css            # Styling
│   ├── index.js           # Entry point
│   └── index.css          # Global styles
├── package.json           # Dependencies
└── README.md             # This file
```

## Features Breakdown

### Form Validation
- Name: Required, text input
- Age: Required, number, min 18, max 120
- Email: Required, valid email format

### Error Handling
- Network errors displayed in red alert
- Form validation errors
- API error messages shown to user

### Loading States
- "Creating..." shown during form submission
- "Loading..." shown during data fetch
- Disabled buttons during operations

### Success Messages
- Green success message after user creation
- Alert with user ID confirmation
- Auto-clear form after successful submission

## Troubleshooting

### CORS Error
If you see CORS errors, make sure your API has CORS enabled:

```csharp
// In Program.cs
app.UseCors("AllowAll");
```

### API Not Running
Make sure API is running on port 5000:
```bash
dotnet run --urls "http://localhost:5000"
```

### Port Already in Use
If port 3000 is in use, React will ask to use another port. Type 'Y' to accept.

### Connection Refused
Check that:
1. API is running (`dotnet run` in UserApi folder)
2. API is on http://localhost:5000
3. No firewall blocking connections

## Build for Production

```bash
npm run build
```

Creates optimized production build in `build/` folder.

## Technologies Used

- **React 18** - UI library
- **Axios** - HTTP client
- **CSS3** - Styling with gradients and animations
- **React Hooks** - useState, useEffect

## Browser Support

- Chrome (latest)
- Firefox (latest)
- Safari (latest)
- Edge (latest)

## Screenshots

### Create User Form
Beautiful gradient form with validation

### Users Table
Paginated table with 100 users per page

### Responsive Design
Works perfectly on mobile and tablet

---

Enjoy using the User Management System! 🚀
