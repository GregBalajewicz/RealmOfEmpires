<picture>
  <img src="https://static.realmofempires.com/images/D2test/RoEBanner.png">
</picture>

# About
Realm of Empires MMORTS Game engine, is an open source version of a game by the same name - [RealmOfEmpires.com](https://www.realmofempires.com/)

Realm of Empires was launch in 2008, has had over 2 million players and is live today.

[BDA Entertainment](https://www.bdaentertainment.com/) decide to open source the engine to allow the community to create other games based on the code. 

In the last 16 years, Realm of Empires has gone through a lot of changes, technology upgrades and is provided as-is, without any warranties.

References
- [Official Blog](https://realmofempires.blogspot.com/)
- [Video Dev Logs](https://www.youtube.com/@realmofempires9110)
- ["Unofficial" Blog](https://bubaribaman.blogspot.com/)

# License
The code is provided with AGLP-3.0 License but this does not include images, videos, sounds, music etc, most which are sourced from original game (https://static.realmofempires.com). These are not provided for general use and are copyright. You may use them for development and experimentation purposes only.

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
#### B.1 - Setup ASP.NET Membership
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

### Step D - Compile and run the game
- Open the solution with Visual Studio
- Re-build the entire solution
- Set fbg.web as Startup Project
- Run / debug the project 


### Step E - Run Event-Handler
- Compile and run project Fbg.EventHandlerWin2
- Compile and run project Fbg.EventHandlerWin2

The best way to run it, is to navigate to the directory where Fbg.EventHandlerWin2 is placed, then navigate to subdirectory `bin\debug` and execute `Fbg.EventHandlerWin2.exe`

- Once running, click the "Start / ReInit ALL" button. 

This process will handle completion of events such as building uggrades, troop movements etc. 

# How to deploy a game for public use 
This is a big topic, and we are happy to help with more detailed guidance (please create a discussion for this), here is a very brief and high level guide.

Realm of Empires was meant to run on bear-metal or on a virtual machine (it was created before the days of cloud computing as it is now). One way is to create a Windows Server virtual machine, install SQL Server and configurate all that is needed to run a website under Windows’ Internet Information Services. 

You can also release on Azure Cloud in various configurations, but SQL Databases have to be on Azure SQL Instances or on SQL on VM.  You cannot use Azure SQL Databases as the solution uses cross-database queries. 

You will of course need your own URL and art. You can use our art assets for testing purposes only. 

# Creator's comments
### About the code
Please note. The code is a product of 16 years of intense work (some code 20+ years). It is  not up to date with latest patters and methods software development. It was written in a different time - before cloud, 
before mobile devices, before Faceook matured, before hardware was as fast and as cheap as now. Code also went through a number of upgrades, so it contains both old and new technology. If you have question, or want 
more explanations, please open an issue or start a discussion

### About the engine
The code was originally designed to be a game engine. There are a lot of options for configuring a world, and an ability to skin the game to look completely differently. 