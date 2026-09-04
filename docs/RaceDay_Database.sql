CREATE DATABASE RaceDayDB;
GO

USE RaceDayDB;
GO

CREATE TABLE [User] (
    UserID INT IDENTITY(1,1) PRIMARY KEY,
    FirstName VARCHAR(50) NOT NULL,
    LastName VARCHAR(50) NOT NULL,
    Email VARCHAR(100) NOT NULL UNIQUE,
    Password VARCHAR(255) NOT NULL,
    Phone VARCHAR(20),
    Role VARCHAR(20) NOT NULL
        CHECK (Role IN ('Organiser', 'Participant'))
);

CREATE TABLE EventType (
    EventTypeID INT IDENTITY(1,1) PRIMARY KEY,
    TypeName VARCHAR(50) NOT NULL UNIQUE,
    Description VARCHAR(255)
);

CREATE TABLE Event (
    EventID INT IDENTITY(1,1) PRIMARY KEY,
    EventName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),
    EventDate DATE NOT NULL,
    Location VARCHAR(150) NOT NULL,
    Distance DECIMAL(6,2) NOT NULL,
    EventTypeID INT NOT NULL,
    OrganiserID INT NOT NULL,

    CONSTRAINT FK_Event_EventType
        FOREIGN KEY (EventTypeID)
        REFERENCES EventType(EventTypeID),

    CONSTRAINT FK_Event_Organiser
        FOREIGN KEY (OrganiserID)
        REFERENCES [User](UserID)
);

CREATE TABLE Category (
    CategoryID INT IDENTITY(1,1) PRIMARY KEY,
    EventID INT NOT NULL,
    CategoryName VARCHAR(100) NOT NULL,
    Description VARCHAR(255),

    CONSTRAINT FK_Category_Event
        FOREIGN KEY (EventID)
        REFERENCES Event(EventID)
);

CREATE TABLE Enrolment (
    EnrolmentID INT IDENTITY(1,1) PRIMARY KEY,
    ParticipantID INT NOT NULL,
    EventID INT NOT NULL,
    CategoryID INT NOT NULL,
    EnrolmentDate DATE NOT NULL DEFAULT GETDATE(),
    Status VARCHAR(20) NOT NULL DEFAULT 'Active',

    CONSTRAINT FK_Enrolment_Participant
        FOREIGN KEY (ParticipantID)
        REFERENCES [User](UserID),

    CONSTRAINT FK_Enrolment_Event
        FOREIGN KEY (EventID)
        REFERENCES Event(EventID),

    CONSTRAINT FK_Enrolment_Category
        FOREIGN KEY (CategoryID)
        REFERENCES Category(CategoryID)
);

CREATE TABLE Result (
    ResultID INT IDENTITY(1,1) PRIMARY KEY,
    EnrolmentID INT NOT NULL UNIQUE,
    FinishTime TIME NOT NULL,
    FinishPosition INT NOT NULL,

    CONSTRAINT FK_Result_Enrolment
        FOREIGN KEY (EnrolmentID)
        REFERENCES Enrolment(EnrolmentID)
);

-- Event Types
INSERT INTO EventType (TypeName, Description)
VALUES
('Running', 'Road running events'),
('Walking', 'Road walking events'),
('Cycling', 'Road cycling events');

-- Users
INSERT INTO [User] (FirstName, LastName, Email, Password, Phone, Role)
VALUES
('Thabo', 'Mokoena', 'thabo.mokoena@email.com', 'Password123', '0712345678', 'Organiser'),
('Lerato', 'Dlamini', 'lerato.dlamini@email.com', 'Password123', '0723456789', 'Organiser'),
('Sipho', 'Ndlovu', 'sipho.ndlovu@email.com', 'Password123', '0734567890', 'Participant'),
('Nomsa', 'Mthembu', 'nomsa.mthembu@email.com', 'Password123', '0745678901', 'Participant');

-- Events
INSERT INTO Event
(EventName, Description, EventDate, Location, Distance, EventTypeID, OrganiserID)
VALUES
('Johannesburg City Run', 'Annual Johannesburg road running event',
 '2026-10-10', 'Johannesburg', 10.00, 1, 1),

('Soweto Community Walk', 'Community road walking event',
 '2026-11-07', 'Soweto', 8.00, 2, 2),

('Pretoria Cycle Challenge', 'Road cycling challenge',
 '2026-12-05', 'Pretoria', 40.00, 3, 1);

-- Categories
INSERT INTO Category (EventID, CategoryName, Description)
VALUES
(1, '10 KM Open', '10 kilometre open category'),
(1, '10 KM Junior', '10 kilometre junior category'),
(2, '8 KM Open', '8 kilometre open walking category'),
(2, '8 KM Senior', '8 kilometre senior walking category'),
(3, '40 KM Open', '40 kilometre open cycling category'),
(3, '40 KM Veteran', '40 kilometre veteran cycling category');

-- Enrolments
INSERT INTO Enrolment
(ParticipantID, EventID, CategoryID, Status)
VALUES
(3, 1, 1, 'Active'),
(3, 3, 5, 'Active'),
(4, 1, 2, 'Active'),
(4, 2, 3, 'Active');

-- Results
INSERT INTO Result
(EnrolmentID, FinishTime, FinishPosition)
VALUES
(1, '00:52:34', 12),
(3, '01:04:21', 27);
