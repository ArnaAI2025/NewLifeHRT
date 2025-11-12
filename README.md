# NewLifeHRT - Hospital Management System
NewLifeHRT is a full-stack hospital management system designed using Angular 20 and .NET 8. The solution is split into a **frontend (Angular SPA)** and a **backend (ASP.NET Core Web API + Background Jobs)** architecture, fully aligned with clean architecture principles and HIPAA-compliant design.


## Prerequisites
- Make sure the following tools are installed before starting.

### Backend
- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download)
- [Visual Studio 2022](https://visualstudio.microsoft.com) with **ASP.NET and web development** workload
- [SQL Server 2022 Developer Edition] (https://www.microsoft.com/en-us/sql-server/sql-server-downloads) 

### Frontend
- [Node.js v22.17.0](https://nodejs.org/)
- [Angular CLI v20.0.5]
    ```bash
     npm install -g @angular/cli@20
    ```

## Running the Application

###  1. Backend (API + Background Jobs)
1. For API project. 
    - Open `Backend` folder. 
    - Open `NewLifeHRT.sln` in Visual Studio
    - Set `NewLifeHRT.API.Controllers` as startup project
    - Press **F5** or use terminal from root folder:
         ```bash
         cd Backend/Web API/NewLifeHRT.API.Controllers
         dotnet run
         ```
4. Swagger is available at:
     ```
     https://localhost:7055/swagger/index.html
5. For background Jobs:
    - Open `Backend` folder. 
    - Open `NewLifeHRT.sln` in Visual Studio
    - Set `NewLifeHRT.Background.Console` as startup project
    - Press **F5** or use terminal from root folder:
         ```bash
         cd Backend/Background Jobs/NewLifeHRT.Background.Console
         dotnet run
         ```
         
###  2. Frontend (Angular App)
    ```bash
    cd NewLifeHRT.Web.Client
    npm install
    ng serve

