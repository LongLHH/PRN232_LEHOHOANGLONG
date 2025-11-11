# FU News Management System 📰

**Student:** Lê Hồ Hoàng Long  
**Student ID:** SE181754  
**Class:** SE1716  
**Subject:** PRN232 - Advanced Cross-Platform Application Programming with .NET

---

## 📋 Project Overview

FU News Management System is a full-stack web application for managing news articles, built with **ASP.NET Core Web API** (Backend) and **ASP.NET Core MVC** (Frontend). The system supports role-based access control with three user roles: Admin, Staff, and Lecturer.

---

## 🏗️ Project Structure

```
PRN232_LEHOHOANGLONG/
├── LEHOHOANGLONG_SE1716_A02_BE/          # Backend API
│   ├── Repository/                        # Data Access Layer
│   │   ├── Data/                         # DbContext
│   │   ├── Entities/                     # Entity Models
│   │   ├── Interfaces/                   # Repository Interfaces
│   │   └── Repositories/                 # Repository Implementations
│   ├── Service/                          # Business Logic Layer
│   │   ├── DTOs/                         # Data Transfer Objects
│   │   ├── Interfaces/                   # Service Interfaces
│   │   └── Services/                     # Service Implementations
│   └── WebAPI/                           # API Layer
│       ├── Controllers/                  # API Controllers
│       ├── Middleware/                   # Custom Middleware
│       └── Program.cs                    # API Configuration
│
├── LEHOHOANGLONG_SE1716_A02_FE/          # Frontend MVC
│   └── WebApp/                           # MVC Application
│       ├── Controllers/                  # MVC Controllers
│       ├── Models/                       # View Models
│       ├── Views/                        # Razor Views
│       ├── Services/                     # API Client Services
│       └── wwwroot/                      # Static Files
│
└── DatabaseScript.sql                    # Database Schema
```

---

## 🚀 Features

### 👤 User Roles & Permissions

| Role | Dashboard | Accounts | Categories | Tags | News Articles | Reports |
|------|-----------|----------|------------|------|---------------|---------|
| **Admin (0)** | ✅ | ✅ | ✅ Create/Edit/Delete | ✅ Create/Edit/Delete | ✅ Create/Edit/Delete All | ✅ |
| **Staff (1)** | ❌ | ❌ | ❌ | ✅ Create/Edit/Delete | ✅ Create/Edit/Delete | ❌ |
| **Lecturer (2)** | ❌ | ❌ | ❌ | ❌ | ✅ View/Edit Own | ❌ |
| **Guest** | ❌ | ❌ | ❌ | ❌ | ✅ View Active Only | ❌ |

### 📊 Admin Dashboard
- **Summary Cards:** Total Articles, Active Articles, Total Categories, Total Tags, Total Users
- **Charts (Chart.js):**
  - Pie Chart: Articles by Status (Active/Inactive)
  - Doughnut Chart: Users by Role (Admin/Staff/Lecturer)
  - Bar Chart: Articles by Category
- **Top Lists:**
  - Top 5 Categories (by article count)
  - Top 5 Tags (by usage count)
  - Top 5 Authors (by article count)
  - Recent 5 Articles (with status badges)

### 📝 News Article Management
- CRUD operations with role-based access
- Rich text editor support
- Category assignment (single)
- Tag assignment (multiple)
- Status management (Active/Inactive)
- Author tracking
- Search functionality
- "My Articles" view for Staff/Lecturer

### 🏷️ Category & Tag Management
- CRUD operations
- **Duplicate validation** (case-insensitive)
- **Delete protection** (prevents deletion if in use by articles)
- User-friendly error messages via AJAX

### 👥 System Account Management (Admin Only)
- User CRUD operations
- Role assignment (Admin/Staff/Lecturer)
- Password hashing with BCrypt
- User reports and statistics

### 🔐 Authentication & Authorization
- JWT-based authentication
- Session management
- Role-based access control
- Secure password storage

---

## 🛠️ Technology Stack

### Backend
- **Framework:** ASP.NET Core 8.0 Web API
- **Database:** SQL Server
- **ORM:** Entity Framework Core 8.0
- **Authentication:** JWT Bearer Tokens
- **Password Hashing:** BCrypt.Net
- **Architecture:** 3-Layer (Repository → Service → Controller)

### Frontend
- **Framework:** ASP.NET Core 8.0 MVC
- **UI Framework:** Bootstrap 5
- **Icons:** Bootstrap Icons
- **Charts:** Chart.js 4.x
- **AJAX:** Fetch API
- **Session:** ASP.NET Core Session

### Development Tools
- Visual Studio 2022 / VS Code
- SQL Server Management Studio (SSMS)
- Postman (API Testing)
- Git & GitHub

---

## 📦 Installation & Setup

### Prerequisites
- .NET 8.0 SDK
- SQL Server 2019+
- Visual Studio 2022 or VS Code

### Database Setup

1. **Create Database:**
```sql
-- Run DatabaseScript.sql in SSMS
-- This will create FUNewsManagement database with all tables and sample data
```

2. **Update Connection String:**

**Backend:** `LEHOHOANGLONG_SE1716_A02_BE/WebAPI/appsettings.json`
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=YOUR_SERVER;Database=FUNewsManagementSystem;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

**Frontend:** `LEHOHOANGLONG_SE1716_A02_FE/WebApp/appsettings.json`
```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5119"
  }
}
```

### Running the Application

#### Option 1: Using Terminal

**Backend API:**
```bash
cd LEHOHOANGLONG_SE1716_A02_BE/WebAPI
dotnet restore
dotnet build
dotnet run
# API will run on http://localhost:5119
```

**Frontend MVC:**
```bash
cd LEHOHOANGLONG_SE1716_A02_FE/WebApp
dotnet restore
dotnet build
dotnet run
# Web app will run on http://localhost:5XXX (check terminal output)
```

#### Option 2: Using Visual Studio
1. Open solution file `LEHOHOANGLONG_SE1716_A02_BE.sln`
2. Set multiple startup projects (WebAPI + WebApp)
3. Press F5 to run

---

## 🔑 Default Login Credentials

| Role | Email | Password |
|------|-------|----------|
| Admin | admin@FUNewsManagementSystem.org | @@abc123@@ |
| Staff | staff1@funews.edu.vn | 123456 |
| Lecturer | lecturer1@funews.edu.vn | 123456 |

---

## 🌟 Key Implementations

### 1. Duplicate Validation
- **Tags & Categories:** Case-insensitive duplicate name detection
- **Error Handling:** User-friendly alerts on create/update
- **Implementation:** Service layer validation with `StringComparison.OrdinalIgnoreCase`

### 2. Delete Protection
- **Tags:** Cannot delete if used by any news articles
- **Categories:** Cannot delete if has news articles
- **User Experience:** AJAX-based deletion with clear error messages

### 3. Chart Legends & Tooltips
- **Custom Labels:** Show data counts in legends (e.g., "Active: 12 articles")
- **Percentage Tooltips:** Hover to see percentages in pie/doughnut charts
- **Axis Labels:** Clear X and Y axis titles in bar charts

### 4. Role-Based Navigation
- **Dynamic Navbar:** Menu items displayed based on user role
- **Admin:** Full access to all features
- **Staff:** Limited to Tags, News, My Articles
- **Lecturer:** News viewing and My Articles only

### 5. JWT Authentication Flow
```
Login → API validates credentials → Generate JWT token 
→ Store in session → Include in API requests via Authorization header
```

---

## 📡 API Endpoints

### Authentication
- `POST /api/Auth/login` - User login
- `POST /api/Auth/logout` - User logout

### News Articles
- `GET /api/NewsArticles` - Get all active articles (public)
- `GET /api/NewsArticles/{id}` - Get article by ID
- `GET /api/NewsArticles/search?searchTerm=...` - Search articles
- `POST /api/NewsArticles` - Create article (Admin/Staff)
- `PUT /api/NewsArticles/{id}` - Update article
- `DELETE /api/NewsArticles/{id}` - Delete article (Admin/Staff)

### Categories
- `GET /api/Categories` - Get all categories
- `GET /api/Categories/{id}` - Get category by ID
- `POST /api/Categories` - Create category (Admin only)
- `PUT /api/Categories/{id}` - Update category (Admin only)
- `DELETE /api/Categories/{id}` - Delete category (Admin only)

### Tags
- `GET /api/Tags` - Get all tags
- `GET /api/Tags/{id}` - Get tag by ID
- `POST /api/Tags` - Create tag (Admin/Staff)
- `PUT /api/Tags/{id}` - Update tag (Admin/Staff)
- `DELETE /api/Tags/{id}` - Delete tag (Admin/Staff)

### System Accounts
- `GET /api/SystemAccounts` - Get all accounts (Admin only)
- `GET /api/SystemAccounts/{id}` - Get account by ID
- `POST /api/SystemAccounts` - Create account (Admin only)
- `PUT /api/SystemAccounts/{id}` - Update account (Admin only)
- `DELETE /api/SystemAccounts/{id}` - Delete account (Admin only)

### Statistics (Admin Only)
- `GET /api/Statistics/dashboard` - Get dashboard statistics

---

## 📸 Screenshots

### Admin Dashboard
- Summary cards with key metrics
- Interactive charts with legends
- Top performers and recent activity

### News Articles Management
- Grid view with search
- Create/Edit modals
- Tag and category assignment

### Role-Based Access
- Different navigation menus per role
- Conditional button visibility
- Access denied pages

---

## 🧪 Testing

### Manual Testing Checklist
- ✅ User login/logout
- ✅ Role-based navigation
- ✅ CRUD operations for all entities
- ✅ Duplicate validation (Tags/Categories)
- ✅ Delete protection (Tags/Categories)
- ✅ Dashboard charts rendering
- ✅ Search functionality
- ✅ Error handling and user feedback

### Test Accounts
Use the default credentials provided above to test different role permissions.

---

## 📚 Documentation

Additional documentation files in the project:
- `DASHBOARD_AND_VALIDATION_COMPLETED.md` - Dashboard implementation details
- `DELETE_VALIDATION_COMPLETED.md` - Delete validation implementation
- `TESTING_GUIDE.md` - Comprehensive testing guide
- `API_RESPONSE_GUIDE.md` - API response format standards

---

## 🐛 Known Issues & Limitations

1. **Single Category per Article:** Current design allows only one category per article
2. **No Image Upload:** News articles don't support image uploads yet
3. **No Pagination:** Large datasets may affect performance
4. **Session Timeout:** JWT tokens expire after configured time (default: 60 minutes)

---

## 🔮 Future Enhancements

1. **Image Upload:** Support for article featured images
2. **Pagination:** Implement paging for large datasets
3. **Rich Text Editor:** WYSIWYG editor for article content
4. **Email Notifications:** Notify users of article status changes
5. **Article Comments:** Reader comments and feedback
6. **Article Versioning:** Track article revision history
7. **Export Features:** Export data to Excel/PDF
8. **Soft Delete:** Implement soft delete instead of hard delete

---

## 👨‍💻 Development Notes

### Code Structure
- **Backend:** Clean architecture with separation of concerns
- **Frontend:** MVC pattern with service layer for API calls
- **Error Handling:** Centralized error handling middleware
- **Validation:** Both client-side and server-side validation

### Best Practices Followed
- ✅ Dependency Injection
- ✅ Repository Pattern
- ✅ Service Layer Pattern
- ✅ DTOs for data transfer
- ✅ Async/Await throughout
- ✅ JWT for authentication
- ✅ Password hashing (BCrypt)
- ✅ CORS configuration
- ✅ Input validation
- ✅ Error logging

---

## 📞 Contact

**Student:** Lê Hồ Hoàng Long  
**Email:** longlhhse181754@fpt.edu.vn  
**GitHub:** https://github.com/LongLHH/PRN232_LEHOHOANGLONG

---

## 📄 License

This project is developed for educational purposes as part of the PRN232 course at FPT University.

---

## 🙏 Acknowledgments

- FPT University - PRN232 Course
- Instructors and Teaching Assistants
- ASP.NET Core Documentation
- Bootstrap & Chart.js Communities

---

**Last Updated:** January 2025  
**Version:** 1.0.0

---

## 🚀 Quick Start Guide

```bash
# 1. Clone repository
git clone https://github.com/LongLHH/PRN232_LEHOHOANGLONG.git
cd PRN232_LEHOHOANGLONG

# 2. Setup database (run DatabaseScript.sql in SSMS)

# 3. Update connection strings in appsettings.json

# 4. Run Backend
cd LEHOHOANGLONG_SE1716_A02_BE/WebAPI
dotnet run

# 5. Run Frontend (new terminal)
cd LEHOHOANGLONG_SE1716_A02_FE/WebApp
dotnet run

# 6. Access application
# Frontend: http://localhost:5XXX
# Backend API: http://localhost:5119
# Swagger: http://localhost:5119/swagger

# 7. Login with default credentials
# Admin: admin@FUNewsManagementSystem.org / @@abc123@@
```

---

**Happy Coding! 🎉**
