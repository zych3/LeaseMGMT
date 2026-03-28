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
Commands:
  add-student      Add a new student
  add-employee     Add a new employee
  add-laptop       Register a new laptop
  add-projector    Register a new projector
  add-camera       Register a new camera
  list-devices     List all equipment
  list-available   List available equipment
  lease            Lease equipment to a user
  return           Return leased equipment
  set-unavailable  Mark equipment as unavailable
  user-leases      List active leases for a user
  overdue          List overdue leases
  report           Show summary report
  exit / :q        Quit
```