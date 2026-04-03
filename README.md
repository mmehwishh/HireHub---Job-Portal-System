# HireHub - Job Portal Management System 🚀

**HireHub** is a full-stack web application developed using **ASP.NET MVC** and **SQL Server**. It is designed to bridge the gap between job seekers and employers, providing a streamlined platform for job postings, applications, and profile management.
> **Status:** 🛠️ Work in Progress
## 🌟 Key Features
* **Job Seeker Module:** Create profiles, upload resumes, browse jobs, and track applications.
* **Employer Module:** Post new job openings, manage listings, and review applicant profiles.
* **Search & Filter:** Advanced search functionality to find jobs based on categories and roles.
* **Responsive UI:** Clean and modern interface built with Bootstrap for a seamless user experience.

## 🛠️ Tech Stack
* **Backend:** ASP.NET MVC, C#
* **Database:** SQL Server
* **Frontend:** HTML5, CSS3, JavaScript, Bootstrap
* **Data Access:** Entity Framework / ADO.NET

## 🚀 How to Run the Project Locally

### 1. Database Setup
Since the database is locally connected, follow these steps:
1. Open **SQL Server Management Studio (SSMS)**.
2. Locate the `HireHub_DB.sql` file provided in the repository.
3. Drag and drop the file into SSMS and click **Execute** to generate the tables and sample data.

### 2. Update Connection String
Open `appsettings.json` (or `web.config`) and ensure the connection string points to your local server:
```json
"DefaultConnection": "Server=.;Database=JobDB;Trusted_Connection=True;TrustServerCertificate=True;"

### 3. Run the Application
Open the solution file (.sln) in Visual Studio.

Build the solution (Ctrl + Shift + B).

Press F5 or click the IIS Express button to launch the application in your browser.

Project Structure

Models: Data structures and database entities.
ViewModels: Specialized models for handling complex View data.
Views: CSHTML files for the user interface.

Developed by Mehwish Zehra 
Controllers: Logic for handling user requests and database interaction.
