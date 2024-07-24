<picture>
  <img src="https://static.realmofempires.com/images/D2test/RoEBanner.png">
</picture>

# About
Realm of Empires MMORTS Game engine, is an open source version of a game by the same name - [RealmOfEmpires.com](https://www.realmofempires.com/)

# License
The code is provided with AGLP-3.0 License but this does not include images, videos, sounds, music etc, most which are sourced from original game (https://static.realmofempires.com). These are not provided for use, are copy right and you may use them for development and experimentation purposes only. 

# How to run on your local development machine

## Perequisites 
- Windows based machine
- Latest [Visual Studio](https://visualstudio.microsoft.com/downloads/)
- Latest [.NET frameworks](https://dotnet.microsoft.com/en-us/download/dotnet-framework)
- [SQL Server](https://www.microsoft.com/en-ca/sql-server/sql-server-downloads) 
<details>
<summary>Hits on setting up SQL Server</summary>

SQL Server Developer addition is a free to use version of SQL Server that you can download from [here](https://www.microsoft.com/en-ca/sql-server/sql-server-downloads)

You can also easily [setup SQL Sever via Docker](https://www.linkedin.com/pulse/how-setup-local-database-server-developer-greg-bala-m69tc/)

Please note that you cannot use Azre SQL Databases as Realm of Empires utalizes multiple databases with cross database queries. [Azure SQL Managed Instance](https://learn.microsoft.com/en-us/azure/azure-sql/managed-instance/sql-managed-instance-paas-overview?view=azuresql) can be used 
</details>

<details>
<summary>Download of older .NET Frameworks maybe needed</summary>

We have not yet fully tested this on a machine with only the latest .net framework. As part of the setup, older version may be neccessary. Please log an issue so that we can update this guide
</details>

## Setup

### Step A - Create Databases

Create two Databases, FBGCommon and Fbg1. You can use this script

```
create database fbgcommon

create database fbg1
```
<details>
<summary>Notes</summary>

These databases can be named differently, however, you would need to edit a lot of script files if you do name them differently. Leave the names as they are to start. 

FBGCommon database holds data related to the player of the game, that is not specific to any world (realm) 
Fbg1 database represents one world (realm). Game engine supports multiple realms, with one database for each realm. 

Trivia: Term "fbg" stands for, "face-book-game" since Realm of Empires started as a game on facebook and did not have a name when development started. The term "fbg" stuck and is used througout the code
</details>

### Step B - Setup FBGCommon Database
#### B.1 - Setup ASP.NET Membership on FBGCommon DB
run tool `Fbg.Database.Common/Create Scripts/Step 1 - register aspnet membership/Aspnet_regsql.exe)` and follow the wizard to setup .NET Framework Membership, which is [an old](https://learn.microsoft.com/en-us/aspnet/identity/overview/migrations/migrating-an-existing-website-from-sql-membership-to-aspnet-identity) 
tool to add email based login and registration functionality


#### B.2 - Populate DB

- run this script `Fbg.Database.Common/Create Scripts/Step 2 - FbgCommon DDL - creates.sql` to create the necessary tables and alike
- run this script `Fbg.Database.Common/Create Scripts/Step 3 - Populate DB - FbgCommon.sql` to seed the database

#### B.3 - Add stored procedures etc to DB

All objects in folder `Fbg.Database.Common/SP` need to be created in the database. You can run the scripts one by one, or you can use a provided batch file `Fbg.Database.Common/SP/_runs.bat` 
which applies all files to the database. Just run the file using command or powershell window


<details>
<summary>Important note!</summary>

Running `_runs.bat` will generate a lot of warnings that one object depends on another, which is not yet created. It is safe to run this batch file multiple times 
and the 2nd time you run it, no warnings should be generated. 

Run the file at least twice, and ensure no errors are generated to be sure all files applied successfully

</details>

### Step C - Setup FBG1 Database

#### C.1 Create tables 
run this script `Fbg.Database/Create Scripts/Step 2 - Fbg DDL - creates.sql` to create the necessary tables and alike

#### C.2 Create a linked server (to access FBGCommon from Fbg1) 
run this script `Fbg.Database/Create Scripts/Step 2.2 - add linked server.sql` 

#### C.3 - Add stored procedures etc 

All objects in folder `Fbg.Database/SP` need to be created in the database. You can run the scripts one by one, or you can use a provided batch file `Fbg.Database/SP/_runs.bat` 
which applies all files to the database. Just run the file using command or powershell window


<details>
<summary>Important note!</summary>

Running `_runs.bat` will generate a lot of warnings that one object depends on another, which is not yet created. It is safe to run this batch file multiple times 
and the 2nd time you run it, no warnings should be generated. 

Run the file at least twice, and ensure no errors are generated to be sure all files applied successfully

</details>

#### C.4 - Seed the DB (creates a realm)

- run this script `Fbg.Database/Create Scripts/Step 3.5 - Populate DB.sql` to seed the database and actually create a realm, it will be Realm 1

This script could take a few minutes to run

<details>
<summary>note</summary>

You can create many different types of realms (worlds). You will edit this script to change the parameters of those realms. For now, just setup a default realm as is. 

</details>

### Step D - Compile and run the application
- Re-build the entire solution
- Set frb.web as Startup Project
- Run / debug the project 
