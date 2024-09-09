This is a short tutorial of how to run and demostrate the project
After downloading the project .. 
1.	Open with Visual Studio. Make sure that you have an MS SqlServer instance installed on your machine. Make sure that .Net 8 is available on your development environment.
2.	Create a new Solution and Pull/Fetch From GitHub or Assign the Downloaded Project
3.	Install the missing NuGet packages (latest version). These are:
  •	  Microsoft.EntityFrameworkCore" Version="8.0.8"
  •	  Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.8"
  •	  Microsoft.EntityFrameworkCore.Tools" Version="8.0.8"
  •	  Newtonsoft.Json" Version="13.0.3"
  •	  Swashbuckle.AspNetCore" Version="6.4.0"
4.	Check the appsettings.json and find the group “ConnectionStrings”. Find the “DbConnection” and replace the value according to your needs. Note the Database attribute to be the name you prefer. The default value for this is StealTheCatsDB
5.	Make sure that the Migration Folder is Downloaded and includes the migration files.
	a.	If Is not Included then open the Package Manager Console and run the command: Add-Migration
	b.	If is included run the command: Update-Database
6.	Build and Run the project. The Swagger should be loaded on the port assigned.
7.	For Unit Test there is the project "StealAllTheCats.nUnitTests". Assign the Project "StealAllTheCats" as Reference. All add "Existing Item" appsettings.json file from StealAllTheCats project. Make sure tha the ConnectionString is valid according to your DB.

