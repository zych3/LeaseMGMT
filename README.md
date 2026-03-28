# Lease MGMT

## About
A C# console app to handle leasing university devices to students and employees. Helps keep track of the devices, 
users and, last but not least, leases themselves. 

## Features 
* Command-line REPL interface for interacting with the app
* DB support (by default SQLite) allowing for persistency

## Domain model
* Device - abstract base for all devices 
* Laptop, Projector, Camera - device types 
* User - abstract base for all users
* Student, Employee - User types

## Technical Breakdown 
```shell
Data 
  |-> Models # Classes for data structure 
  |-> AppDbContext* # Files related to Entity Framework setup and operations
Repositories
  |-> DeviceRepository  # All Device-related operations
  |-> UserRepository   # All User-related operations
  |-> LeaseRepository # All Lease-related operations
```

## Usage 
```shell
git clone https://github.com/zych3/LeaseMGMT.git
cd LeaseMGMT/APBD1
dotnet ef migrations add "Create" 
dotnet ef database update
dotnet run 
```