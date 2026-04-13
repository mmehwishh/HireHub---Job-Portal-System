# HireHub - Job Portal Management System 🚀

**HireHub** is a full-stack web application developed using **ASP.NET MVC** and **SQL Server**. It is designed to bridge the gap between job seekers and employers, providing a streamlined platform for job postings, applications, and profile management.
> **Status:** 🛠️ Work in Progress
## 🌟 Key Features
* **Job Seeker Module:** Create profiles, upload resumes, browse jobs, and track applications.
* **Employer Module:** Post new job openings, manage listings, and review applicant profiles.
* **Search & Filter:** Advanced search functionality to find jobs based on categories and roles.
* **Responsive UI:** Clean and modern interface built with Bootstrap for a seamless user experience.
* **Real-Time Bi-Directional Chat:** Instant messaging without page refreshes using SignalR WebSockets.
* **Interview Scheduler:** Employers can schedule interviews with custom notes and meeting links.
* **Status Tracking:** Real-time updates on application status (Pending, Approved, Rejected, Interview Scheduled).


Here’s your **clean, professional GitHub README.md** with proper formatting, sections, and markdown styling 👇

---

# 🛠 Tech Stack & Dependencies

## 🚀 Core Stack

* **Backend:** ASP.NET MVC 5 (C#)
* **Database:** MS SQL Server
* **Frontend:** HTML5, CSS3, JavaScript, Bootstrap 4/5
* **ORM:** Entity Framework (Database First Approach)

## 📦 Key Dependencies (NuGet Packages)

* **Microsoft.AspNet.SignalR** → Real-time chat functionality
* **EntityFramework** → Database operations
* **Newtonsoft.Json** → JSON handling in AJAX calls
* **jQuery** → DOM manipulation & SignalR client-side logic

---

# 💻 How to Run the Project Locally

## 🗄️ Database Setup

Since the database is locally connected, follow these steps:

1. Open **SQL Server Management Studio (SSMS)**
2. Locate the `HireHub_DB.sql` file in the repository
3. Drag & drop the file into SSMS
4. Click **Execute** to generate tables and sample data

---

## 🔗 Update Connection String

Open `web.config` and update the connection string to match your local SQL Server instance:

```xml
<connectionStrings>
  <add name="YourConnectionName" 
       connectionString="Data Source=YOUR_SERVER;Initial Catalog=HireHub_DB;Integrated Security=True" 
       providerName="System.Data.SqlClient" />
</connectionStrings>
```

---

## ▶️ Run the Application

1. Open the solution file (`.sln`) in **Visual Studio**
2. Restore NuGet Packages

   * Right-click Solution → **Restore NuGet Packages**
3. Build the solution

   * `Ctrl + Shift + B`
4. Press **F5** to run the project

---

# 🏗 Project Structure

* **Models** → Entity Framework database entities & data structures
* **ViewModels** → Custom models for complex views (e.g., `JobSeekerApplicationViewModel`)
* **Controllers** → Handle user requests, business logic, and database interaction
* **Views** → `.cshtml` UI files and dashboard layouts

---

# 👩‍💻 Developed By

**Mehwish Zehra**

