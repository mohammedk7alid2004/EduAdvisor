<div align="center">

<img src="https://img.shields.io/badge/EduAdvisor-Intelligent%20Academic%20Platform-1a3c6e?style=for-the-badge&logo=graduation-cap&logoColor=white" alt="EduAdvisor"/>

# 🎓 EduAdvisor
### Intelligent Academic Advising & Recommendation Platform

> Bridging the gap between students and academic excellence through AI-powered guidance.

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet)
[![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?style=flat-square&logo=dotnet)](https://docs.microsoft.com/ef/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-Database-CC2927?style=flat-square&logo=microsoftsqlserver)](https://www.microsoft.com/sql-server)
[![SignalR](https://img.shields.io/badge/SignalR-Real--Time-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/apps/aspnet/signalr)
[![Flutter](https://img.shields.io/badge/Flutter-Mobile-02569B?style=flat-square&logo=flutter)](https://flutter.dev/)
[![React](https://img.shields.io/badge/React-Web-61DAFB?style=flat-square&logo=react)](https://reactjs.org/)

**Fayoum University — Faculty of Computers & Artificial Intelligence — 2026**

</div>

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Features](#-features)
- [Tech Stack](#-tech-stack)
- [Architecture](#-architecture)
- [Project Structure](#-project-structure)
- [Getting Started](#-getting-started)
- [Configuration](#-configuration)
- [API Endpoints](#-api-endpoints)
- [Team](#-team)

---

## 🌟 Overview

**EduAdvisor** is a comprehensive intelligent academic advising platform designed to facilitate effective academic guidance between students and academic mentors at Fayoum University. The system leverages AI-driven recommendations, real-time communication, and smart analytics to support students in their academic journey.

### The Problem We Solve

| Challenge | Our Solution |
|-----------|-------------|
| Students struggle to select suitable courses | AI-powered Course Recommendation Engine |
| Limited access to academic advisors | 24/7 Intelligent Chatbot Assistant |
| Manual and fragmented advising processes | Centralized Digital Platform |
| No real-time communication tools | SignalR-powered Live Chat |
| Lack of academic performance insights | Interactive Analytics Dashboard |

---

## ✨ Features

### 👨‍🎓 For Students
- 🤖 **AI Course Recommendations** — Personalized course suggestions based on GPA, completed credits, and prerequisites
- 📊 **Academic Dashboard** — Visual GPA trends, progress tracking, and risk detection
- 💬 **Chatbot Support** — Instant answers about courses, prerequisites, and registration
- 🗺️ **Track Suggestion** — Specialized academic pathway recommendations
- 🔔 **Smart Notifications** — Real-time alerts for registration status and advisor messages

### 👨‍🏫 For Advisors
- 👥 **Student Management** — View academic history, status, and enrolled courses
- ✅ **Course Approval** — Approve or reject student course registration requests
- 📝 **Custom Recommendations** — Override or enhance AI-generated suggestions
- 📈 **Performance Analytics** — Monitor student progress and generate reports

### 🔧 For Administrators
- 🏛️ **System Management** — Manage universities, faculties, departments, and semesters
- 👤 **User Management** — Create and manage student and advisor accounts
- ⚙️ **Recommendation Engine Config** — Configure AI rules and course prerequisites
- 🔗 **Database Integration** — Sync with university's official academic database

---

## 🛠️ Tech Stack

### Backend
```
ASP.NET Core 9.0          — Web API Framework
Entity Framework Core 9   — ORM & Database Access
CQRS + MediatR            — Clean Architecture Pattern
SignalR                   — Real-Time Communication
JWT Bearer                — Authentication & Authorization
MailKit / MimeKit         — Email Service
FluentValidation          — Request Validation
SQL Server                — Primary Database
```

### Frontend
```
Flutter (Dart)            — Cross-Platform Mobile App
React.js                  — Web Application
MVVM + Bloc (Cubit)       — State Management
REST APIs                 — Backend Integration
```

### AI & ML
```
Recommendation Engine     — Hybrid Rule-Based + ML Model
NLP Chatbot               — Natural Language Processing
Python ML Libraries       — Model Training & Evaluation
```

---

## 🏗️ Architecture

The backend follows **Clean Architecture** with **CQRS Pattern**:

```
EduAdvisor/
├── EduAdvisor.API              # Presentation Layer (Controllers, Middlewares)
├── EduAdvisor.Application      # Application Layer (Commands, Queries, Handlers)
├── EduAdvisor.Domain           # Domain Layer (Entities, Value Objects)
└── EduAdvisor.Infrastructure   # Infrastructure Layer (DB, Email, File Storage)
```

### CQRS Flow
```
Request → Controller → MediatR → Command/Query Handler → Repository → Database
                                        ↓
                               Validation (FluentValidation)
                                        ↓
                               Response → DTO → Client
```

---

## 📁 Project Structure

```
EduAdvisor.API/
├── Controllers/
│   └── AuthModules/
├── Middlewares/
│   └── ExceptionHandlingMiddleware.cs
└── EmailTemplates/
    ├── EmailConfirmation.html
    └── ForgetPassword.html

EduAdvisor.Application/
├── Commands/
│   └── AuthModules/
├── Queries/
├── Handlers/
│   └── AuthModules/
├── Behaviors/
│   └── ValidationBehavior.cs
├── Interfaces/
└── DTO/

EduAdvisor.Domain/
├── Entities/
│   └── AuthModule/
│       ├── User.cs
│       └── Student.cs
└── Common/

EduAdvisor.Infrastructure/
├── Persistence/
│   └── ApplicationDbContext.cs
├── Services/
│   ├── Email/
│   │   ├── EmailService.cs
│   │   ├── EmailBodyBuilder.cs
│   │   └── MailSettings.cs
│   ├── AuthModules/
│   ├── File/
│   └── Hasher/
├── Repositories/
├── Migrations/
└── EmailTemplates/
    ├── EmailConfirmation.html
    └── ForgetPassword.html
```

---

## 🚀 Getting Started

### Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- [SQL Server](https://www.microsoft.com/sql-server) (LocalDB or full instance)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

### Installation

**1. Clone the repository**
```bash
git clone https://github.com/mohammedk7alid2004/EduAdvisor.git
cd EduAdvisor
```

**2. Restore dependencies**
```bash
dotnet restore
```

**3. Configure `appsettings.json`** *(see [Configuration](#-configuration))*

**4. Apply database migrations**
```bash
cd EduAdvisor.API
dotnet ef database update
```

**5. Run the application**
```bash
dotnet run
```

**6. Open Swagger UI**
```
https://localhost:{port}/swagger
```

---

## ⚙️ Configuration

Add the following to `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.;Database=EduAdvisorDb;Trusted_Connection=True;TrustServerCertificate=True"
  },
  "MailSettings": {
    "Mail": "your-email@gmail.com",
    "DisplayName": "EduAdvisor",
    "Password": "your-app-password",
    "Host": "smtp.gmail.com",
    "Port": 587
  },
  "JwtSettings": {
    "Key": "your-secret-key-here",
    "Issuer": "EduAdvisor",
    "Audience": "EduAdvisorUsers",
    "ExpiryInDays": 7
  }
}
```

> ⚠️ **Important:** For Gmail, use an [App Password](https://myaccount.google.com/apppasswords), not your regular password. Never commit real credentials to Git.

---

## 📡 API Endpoints

### Auth Module
| Method | Endpoint | Description |
|--------|----------|-------------|
| `POST` | `/api/auth/register/student` | Register new student |
| `POST` | `/api/auth/login` | User login |
| `POST` | `/api/auth/confirm-email` | Confirm email with OTP |
| `POST` | `/api/auth/resend-confirmation` | Resend confirmation OTP |
| `POST` | `/api/auth/forget-password` | Request password reset |
| `POST` | `/api/auth/reset-password` | Reset password with OTP |

---

## 👥 Team

| Name | Role |
|------|------|
| **Mohammed Khaled Mohamed Farag** | Team Leader & Backend Developer |
| **Mahmoud Waleed Mahmoud** | Backend Developer |
| **Mohamed Ahmed Gamal** | Frontend Developer |
| **Ahmed Ragb** | Frontend Developer |
| **Mostafa Ata** | Machine Learning Engineer |
| **Eman Ramadan Abdelzaher** | Database Administrator |
| **Aliaa Mohamed Hamady** | Project Reporter & UI/UX |

### Supervised By
- **Dr. Hebatulla M. Nabil**
- **Eng. Tasneem Mohammed**

---

## 📄 License

This project was developed as a graduation project at **Fayoum University — Faculty of Computers & Artificial Intelligence — 2026**.

---

<div align="center">

Made with ❤️ by the EduAdvisor Team — Fayoum University 2026

</div>
