# 🚀 HireHub — Job Portal System

![.NET](https://img.shields.io/badge/.NET-ASP.NET%20MVC%205-blueviolet)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-4.0-blue)
![Database](https://img.shields.io/badge/Database-First%20Approach-orange)
![License](https://img.shields.io/badge/License-Educational-lightgrey)
![Status](https://img.shields.io/badge/Status-Active-success)

---

## 📌 Overview

**HireHub** is a full-featured **ASP.NET MVC 5 job portal system** designed to simulate real-world recruitment workflows.  
It supports job posting, applications, candidate management, real-time chat, notifications, and external job integration.

Built using:
- ASP.NET MVC 5
- Entity Framework 6 (Database First)
- SignalR (Real-time communication)
- Adzuna API (External job data)
- Custom Dark Blue UI Theme

---

## Live view : http://hirehubjobportal.runasp.net/

## Test credentials:
Role          | Email Address        | Password
--------------|----------------------|----------
Job Seeker 👤 | kazim@example.com    | pass123
Employer 💼   | mehwish@example.com  | pass123

## 🎯 Key Features

### 💼 Job Management
- Post, update, and manage jobs (Employer side)
- Browse, apply, and save jobs (Job Seeker side)
- Skill tagging system (Many-to-Many relationship)
- Duplicate prevention in skills mapping
- External job fetching via **Adzuna API**

---

### 🏢 Employer Dashboard
- Real-time analytics:
  - Active Jobs
  - Total Applicants
  - Unread Messages
- Candidate resume viewing & secure PDF download
- Interview scheduling with **transaction-safe operations**

---

### 👤 Job Seeker Module
- Profile management (skills, resume upload)
- Application tracking:
  - PENDING
  - APPROVED
  - REJECTED
- Job bookmarking system (Saved Jobs)
- Dynamic UI updates (notification badges globally)

---

### 📩 Notifications & Chat System
- AJAX-based notification system (mark read/unread)
- Real-time messaging using **SignalR (ChatHub)**
- Seamless SPA-like experience

---

### 🔐 Security & Authentication
- Session-based authentication (UserId + Role)
- Strict server-side validation
- Ownership verification for sensitive operations
- Protection against unauthorized access

---

## 🛠️ Technical Architecture

### 🗄️ Database Layer
- Entity Framework 6 (Database First)
- EDMX Model: `JobDBEntities3`
- Eager loading using `.Include()` to avoid N+1 queries

### ⚙️ Performance & Reliability
- Database transactions for critical workflows
- Optimized query handling
- Scalable design for large datasets

---

## 📂 Project Structure

```text
├── /Controllers      # Business Logic (MVC Controllers)
├── /Models           # Entity Framework Generated Models
├── /Views/Shared     # Shared Layouts & Admin Dashboards
├── /Content          # Custom CSS Themes (Dark Blue UI)
└── JobPortal.edmx    # Database Schema & EF Mapping
```

---

## ⚙️ Setup Instructions

### 1️⃣ Clone Repository

```bash
git clone https://github.com/your-username/hirehub.git
```

### 2️⃣ Configure Database

* Rename: `web.config.example` → `web.config`
* Update connection string: `JobDBEntities3` → Your SQL Server configuration

### 3️⃣ Run Project

* Open in **Visual Studio 2022**
* Build solution
* Run using **IIS Express**

---

## 💡 Highlights

* Real-world recruitment workflow simulation
* Clean MVC architecture
* Hybrid system (Internal + External job sources)
* Real-time communication layer
* Scalable and modular design

---

## 📜 License

This project is intended for educational and academic purposes.













