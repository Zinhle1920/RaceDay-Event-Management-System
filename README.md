# RaceDay Event Management System

## Project Description

The RaceDay Event Management System is a planned system for managing race events, participants, organisers, categories, enrolments, and race results.

The system provides a structured way for organisers to manage race events and for participants to register for events and access their race information and results.

This repository contains the planning and database work completed for **PROG6212 Programming 2B – Portfolio of Evidence Part 1**.

## User Roles

### Organiser

Organisers are responsible for managing the RaceDay system. They can manage events, categories, participant enrolments, and race results.

### Participant

Participants can manage their profile, view available events and categories, enrol in events, and access their race results.

## Part 1 – System Planning and Database

Part 1 focuses on planning the RaceDay system before developing the RESTful API in Part 2.

The completed planning documents include:

* Entity Relationship Diagram (ERD)
* API Endpoint Plan
* SQL Server database script

## Entity Relationship Diagram

The ERD represents the database structure of the RaceDay system.

The ERD contains the following entities:

* User
* Event
* Event Type
* Category
* Enrolment
* Results

The ERD identifies the attributes, primary keys, foreign keys, and relationships between the entities.

The ERD is available in the `/docs` folder as:

**Race Day ERD.pdf**

## API Endpoint Plan

The API Endpoint Plan defines the RESTful API that will be developed in Part 2.

The planned endpoints cover:

* Authentication
* User Profiles
* Events
* Categories
* Event Enrolments
* Results

Each endpoint includes the HTTP method, route, description, required role, request body, and expected response.

The complete endpoint plan is available in the `/docs` folder as:

**ST10470323-API Endpoint.pdf**

## SQL Database

The SQL script is used to create and populate the RaceDay database using SQL Server.

The script includes:

* Database tables
* Primary keys
* Foreign keys
* Constraints
* Organiser sample data
* Participant sample data
* Event sample data
* Event categories
* Sample enrolments

The SQL script is available in the `/docs` folder as:

**Race Day_Database.sql**

### Running the SQL Script

1. Open SQL Server Management Studio (SSMS).
2. Connect to a SQL Server instance.
3. Open `Race Day_Database.sql`.
4. Execute the complete script.
5. Confirm that the database and tables are created successfully.
6. Check the sample data.

## Repository Structure

```text
RaceDay Event Management System/
│
├── docs/
│   ├── Race Day ERD.pdf
│   ├── Race Day_Database.sql
│   └── ST10470323-API Endpoint.pdf
│
└── README.md
```

## CI/CD

GitHub Actions is used to validate the repository structure.

The workflow checks that the `/docs` folder and the required Part 1 planning documents are present.

### Successful Build

The successful GitHub Actions build screenshot will be added here after the workflow has been created and successfully executed.
<img width="1366" height="689" alt="Screenshot (37)" src="https://github.com/user-attachments/assets/d8e42139-99c0-444a-ad67-25a45607e9bb" />



## Part 1 Demonstration Video

The Part 1 demonstration video covers:

* ERD design decisions
* Database entities and relationships
* API endpoint planning
* SQL database design
* Running the SQL script in SQL Server Management Studio

**YouTube Video:**
The Unlisted YouTube link will be added after the demonstration video has been recorded.
https://www.youtube.com/watch?v=Iu5wyQMvVDI

## Technologies Used

* Microsoft SQL Server
* SQL Server Management Studio (SSMS)
* GitHub
* GitHub Actions
* RESTful API planning
* Entity Relationship Diagram (ERD)


# Part 2- API Development 
## Part 2 — R API Development

# Overview

Part 2 extends the RaceDay project from the planning and database design completed in Part 1 to the development of an ASP.NET Core API using C#.

* The API is intended to support user authentication, profile management, race event management, categories, participant enrolments and race results.

Part 1 and Part 2
Part 1 focused on planning and designing the RaceDay system, including the database design and API endpoint plan.
Part 2 focuses on implementing the API and its supporting database models, endpoint groups, authentication and role-based access control.

## Technologies and Tools
* C#
* ASP.NET Core Web API
* Visual Studio 2022
* SQL Server
* Swagger / OpenAPI
* Entity Framework Core, where configured in the project
* Git and GitHub
* Database

## The existing database is named RaceDayDb.

The database tables currently visible in SQL Server Object Explorer are:
*Categories
*Enrollment
*Event
*EventType
*Result
*User
*Data Models

## The existing Models folder contains:
AppUser.cs
Category.cs
Enrollment.cs
EventType.cs
RaceEvent.cs
RaceResults.cs

## These classes represent the main data entities used by the RaceDay application.
API Endpoint Groups
The Swagger interface displays the following endpoint groups:
Auth: Registration, login and logout.
Profile: Viewing and updating a user's profile.
Events: Browsing and managing race events.

Categories: Managing event categories.

Enrolments: Managing participant entries.

Results: Recording and viewing race results.

The successful operation of each endpoint must be verified through testing.

User Roles

Organiser: Intended to manage their own race events and associated categories, enrolments and results.

Participant: Intended to browse events, enrol in events and view personal race results.

Role-based access control should prevent users from performing operations that are not permitted for their role.

Running and Testing the API

Open the RaceDay solution in Visual Studio 2022.

Select RaceDay.Api as the startup project.

Run the API.

Open the Swagger page provided by the running application.

Test the available endpoints and inspect their responses.

The API's database connection, authentication flows and endpoint functionality must be verified in the local environment.

Unit Testing and CI/CD

Automated tests should cover successful operations and failure scenarios, including authentication, event management, role restrictions and event enrolments.

The GitHub Actions workflow should restore the project dependencies, build the solution and run the automated tests.

# Video Presentation

## Video link: [Insert the URL of the unlisted YouTube presentation.]

The video explains the Part 2 implementation, including the project structure, data models, database, Swagger endpoint groups, authentication design, testing and current implementation status.
