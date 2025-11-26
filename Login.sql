-- Create database
CREATE DATABASE ClaimsAppDb;
GO

USE ClaimsAppDb;
GO

-- Users table (simple demo). In production, store hashed passwords.
CREATE TABLE [User] (
    UserId INT IDENTITY(1,1) PRIMARY KEY,
    Username NVARCHAR(100) NOT NULL UNIQUE,
    Password NVARCHAR(200) NOT NULL, -- demo only
    Role NVARCHAR(50) NOT NULL -- 'Lecturer' or 'Coordinator'
);

-- Claims table
CREATE TABLE Claim (
    ClaimId INT IDENTITY(1,1) PRIMARY KEY,
    UserId INT NOT NULL,
    DateSubmitted DATETIME NOT NULL DEFAULT GETDATE(),
    HoursWorked DECIMAL(8,2) NOT NULL,
    HourlyRate DECIMAL(10,2) NOT NULL,
    Notes NVARCHAR(MAX) NULL,
    AttachmentPath NVARCHAR(500) NULL,
    Status NVARCHAR(50) NOT NULL DEFAULT 'Pending', -- Pending / Approved / Rejected
    ReviewedBy INT NULL,
    ReviewedOn DATETIME NULL,
    FOREIGN KEY (UserId) REFERENCES [User](UserId)
);

-- Sample users
INSERT INTO [User] (Username, Password, Role) VALUES
('lecturer1', 'password123', 'Lecturer'),
('coordinator1', 'coordpass', 'Coordinator');

-- sample claim (optional)
INSERT INTO Claim (UserId, HoursWorked, HourlyRate, Notes, AttachmentPath)
VALUES (1, 3.5, 200.00, 'Lecturing: Module A - 2 sessions', NULL);
