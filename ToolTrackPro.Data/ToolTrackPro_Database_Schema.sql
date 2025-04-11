-- Create the database if it doesn't exist
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'ToolTrackPro')
BEGIN
    CREATE DATABASE ToolTrackPro;
END
GO

USE ToolTrackPro;
GO

-- Drop tables in FK-safe order
IF OBJECT_ID('Returns', 'U') IS NOT NULL DROP TABLE Returns;
IF OBJECT_ID('Borrows', 'U') IS NOT NULL DROP TABLE Borrows;
IF OBJECT_ID('ToolAvailability', 'U') IS NOT NULL DROP TABLE ToolAvailability;
IF OBJECT_ID('Tools', 'U') IS NOT NULL DROP TABLE Tools;
IF OBJECT_ID('Users', 'U') IS NOT NULL DROP TABLE Users;
IF OBJECT_ID('EmailQueue', 'U') IS NOT NULL DROP TABLE EmailQueue;
GO

-- Create Users table
CREATE TABLE Users (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(50) UNIQUE NOT NULL,
    PasswordHash VARBINARY(MAX) NOT NULL   
);
GO

-- Create Tools table
CREATE TABLE Tools (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Name NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);
GO

-- Create Borrows table
CREATE TABLE Borrows (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ToolId INT NOT NULL,
    UserId INT NOT NULL,
    BorrowDate DATETIME NOT NULL,
    EstimatedReturnDate DATETIME NOT NULL,
    FOREIGN KEY (ToolId) REFERENCES Tools(Id),
    FOREIGN KEY (UserId) REFERENCES Users(Id)
);
GO

-- Create Returns table
CREATE TABLE Returns (
    Id INT PRIMARY KEY IDENTITY(1,1),
    BorrowId INT NOT NULL,
    ReturnDate DATETIME NOT NULL,
    FOREIGN KEY (BorrowId) REFERENCES Borrows(Id)
);
GO

-- Create ToolAvailability table
CREATE TABLE ToolAvailability (
    ToolId INT UNIQUE NOT NULL,
    IsAvailable BIT NOT NULL,
    CONSTRAINT FK_ToolAvailability_Tools FOREIGN KEY (ToolId) REFERENCES Tools(Id)
);
GO

-- Create EmailQueue table
CREATE TABLE EmailQueue (
    Id INT PRIMARY KEY IDENTITY(1,1),
    ToEmail NVARCHAR(100) NOT NULL,
    MailSubject NVARCHAR(255) NOT NULL,
    Body NVARCHAR(MAX) NOT NULL,
    CreatedAt DATETIME NOT NULL DEFAULT GETDATE(),
    Sent BIT NOT NULL DEFAULT 0
);
GO

-- Create indexes for performance
CREATE UNIQUE INDEX IX_Users_Email ON Users(Email);
CREATE INDEX IX_Borrows_ToolId ON Borrows(ToolId);
CREATE INDEX IX_Borrows_UserId ON Borrows(UserId);
CREATE INDEX IX_Returns_BorrowId ON Returns(BorrowId);
CREATE INDEX IX_ToolAvailability_ToolId ON ToolAvailability(ToolId);
CREATE INDEX IX_EmailQueue_Sent ON EmailQueue(Sent);
GO

-- Reset identity columns
 DBCC CHECKIDENT ('Users', RESEED, 1);
 DBCC CHECKIDENT ('Tools', RESEED, 1);
 DBCC CHECKIDENT ('Borrows', RESEED, 1);
 DBCC CHECKIDENT ('Returns', RESEED, 1);
 DBCC CHECKIDENT ('EmailQueue', RESEED, 1);
GO

