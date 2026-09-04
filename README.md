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

**[Student Number] API Endpoint.pdf**

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
│   └── [Student Number] API Endpoint.pdf
│
└── README.md
```

## CI/CD

GitHub Actions is used to validate the repository structure.

The workflow checks that the `/docs` folder and the required Part 1 planning documents are present.

### Successful Build

The successful GitHub Actions build screenshot will be added here after the workflow has been created and successfully executed.

## Part 1 Demonstration Video

The Part 1 demonstration video covers:

* ERD design decisions
* Database entities and relationships
* API endpoint planning
* SQL database design
* Running the SQL script in SQL Server Management Studio

**YouTube Video:**
The Unlisted YouTube link will be added after the demonstration video has been recorded.

## Technologies Used

* Microsoft SQL Server
* SQL Server Management Studio (SSMS)
* GitHub
* GitHub Actions
* RESTful API planning
* Entity Relationship Diagram (ERD)
