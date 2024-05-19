CREATE DATABASE VirtualVistaBase

USE VirtualVistaBase

CREATE TABLE Users (
	[UserId] INT PRIMARY KEY IDENTITY,
	[FirstName] VARCHAR(50) NOT NULL,
	[LastName] VARCHAR(50) NOT NULL,
	[Email] VARCHAR(100) NOT NULL,
	[Password] VARCHAR(1000) NOT NULL,
	[Deleted] BIT DEFAULT 0 NOT NULL
);

CREATE TABLE Staff (
	[UserId] INT FOREIGN KEY REFERENCES Users([UserId]) NOT NULL,
	[UserLevel] VARCHAR(50) NOT NULL,
	PRIMARY KEY([UserId])
);

CREATE TABLE Property (
	[PropertyId] INT PRIMARY KEY IDENTITY,
	[TypeOfProperty] VARCHAR(70) NOT NULL,
	[District] VARCHAR(150) NOT NULL,
	[Price] INT NOT NULL,
	[Area] INT NOT NULL,
	[TypeOfContrusction] VARCHAR(20) NOT NULL,
	[PhoneNumber] VARCHAR(50) NOT NULL,
	[AdditionalInformation] VARCHAR(2500) NOT NULL,
	[ApprovalStatus] VARCHAR(50) NOT NULL,
	[Deleted] BIT DEFAULT 0 NOT NULL,
	[Sold] BIT DEFAULT 0 NOT NULL,
	[UserId] INT FOREIGN KEY REFERENCES Users([UserId])
);

CREATE TABLE PropertyVisualDetails (
	[PropertyId] INT FOREIGN KEY REFERENCES Property([PropertyId]),
	[CoordinatesOfVTour] NVARCHAR NOT NULL,
	[Image] NVARCHAR NOT NULL,
	[Video] NVARCHAR NOT NULL,
	[UserId] INT FOREIGN KEY REFERENCES Users([UserId]) NOT NULL
	PRIMARY KEY([PropertyId])
);

/* 
make auto generate tables for every property
*/