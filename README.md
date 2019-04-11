# Studio-Metrics

## What is Studio Metrics?
This is a project management app directed toward sound engineering studios as the intended audience. The application is intended
to be an internal application for the employees at the studio. Studio Metrics allow the studio to create and store their data for
their Projects, Clients, Artists and In-House Players. The Projects tab of the application breaks the projects down into tabs regarding
the status of the project allowing the user to easily access their projects and move them as needed.

The deployed application can be seen [here](https://studiometrics20190409102759.azurewebsites.net/)

## Technologies Used
* C #
* .NET
* Entity
* Identity
* [Canva](https://www.canva.com/) - Design site used to make the Logo
* Azure - It was deployed using Azure

## How to Start
To run Studio Metrics on your local computer
1. First you will need to clone or fork this repository
2. Next you will need to create an appsetting.json file at the root of the StudioMetrics folder. Additonally you will need to add the correct connection string for your local machine.
3. Then you must open the Package Manager Console and run `Add-Migration Initial`
4. After this you must run `Update-Database` in the Package Manager Console

Once you have done this you can start up the application locally. You can either make an account for yourself or use the premade account with the login "admin@admin.com" and password "Admin8*". The Admin account has some data associated with it to see how the application works.

## Data Structure
![ERD](StudioMetrics/wwwroot/images/Backend-Diagram.png)
