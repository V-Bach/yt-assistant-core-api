# YT Learning Assistant - Core Management API (C#)

This is the central hub of the project. It handles user data, security, and serves the main Learning Dashboard.

## How to Run (Step-by-Step)
1. **Database Setup:** Ensure you have **MySQL** running. Update the connection string in `appsettings.json`.
2. **Restore Packages:** Run `dotnet restore` to download all necessary libraries.
3. **Database Migration:** Run `dotnet ef database update` to create your tables.
4. **Launch:** Run `dotnet run`.

## How It Works & Architecture
* **Dashboard Hosting:** It serves `dashboard.html` from the `wwwroot` folder. This is the main page where you see your study history.
* **Data Management:** It uses **Entity Framework Core** to save video metadata and user notes into MySQL.
* **API Endpoints:** It provides endpoints for the Extension to save video progress and for the Dashboard to retrieve saved data.

## The "Golden Triangle" Connection
* **With Python (AI Backend):** When you request a summary on the Dashboard, the frontend makes a call to the Python server. C# acts as the host for the UI that displays this AI data.
* **With Chrome Extension:** The Extension sends "Save" requests to your localhost server (C#) to store video progress and notes.
* **With MySQL:** C# manages the persistent storage, ensuring your data survives even if the servers are restarted.