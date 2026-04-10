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


🛠 Tech Stack & Dependencies
Core Stack
Backend: ASP.NET MVC 5 (C#)

Database: MS SQL Server

Frontend: HTML5, CSS3, JavaScript, Bootstrap 4/5

ORM: Entity Framework (Database First Approach)

Key Dependencies (NuGet Packages)
Microsoft.AspNet.SignalR - Powering the real-time chat functionality.

EntityFramework - For seamless database interactions.

Newtonsoft.Json - For handling JSON data in AJAX calls.

jQuery - For DOM manipulation and SignalR client logic.

💻 How to Run the Project Locally
1. Database Setup
Since the database is locally connected, follow these steps:

Open SQL Server Management Studio (SSMS).

Locate the HireHub_DB.sql file provided in the repository.

Drag and drop the file into SSMS and click Execute to generate the tables and sample data.

2. Update Connection String
Open web.config and ensure the connection string points to your local server instance:

XML
<add name="HireHubEntities" connectionString="metadata=res://*/Models.Model1.csdl...;provider=System.Data.SqlClient;provider connection string=&quot;data source=.;initial catalog=HireHub_DB;integrated security=True;App=EntityFramework&quot;" providerName="System.Data.EntityClient" />
3. Run the Application
Open the solution file (.sln) in Visual Studio.

Restore NuGet Packages (Right-click Solution > Restore NuGet Packages).

Build the solution (Ctrl + Shift + B).

Press F5 to launch the application.

🏗 Project Structure
Models: Data structures and Entity Framework database entities.

ViewModels: Specialized models for handling complex View data (e.g., JobSeekerApplicationViewModel).

Controllers: Logic for handling user requests, chat hubs, and DB interaction.

Views: CSHTML files for the user interface and dashboard layouts.

Developed by: Mehwish Zehra 👩‍💻
