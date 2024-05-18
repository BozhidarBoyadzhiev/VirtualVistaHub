CREATE DATABASE VirtualVistaBase

USE VirtualVistaBase

CREATE TABLE Users (
	[UserId] INT PRIMARY KEY IDENTITY,
	[FirstName] VARCHAR(50),
	[LastName] VARCHAR(50),
	[Email] VARCHAR(100),
	[Password] VARCHAR(100),
	[Deleted] BIT DEFAULT 0
);

CREATE TABLE Staff (
	[UserId] INT FOREIGN KEY REFERENCES Users([UserId]),
	[UserLevel] VARCHAR(50),
	PRIMARY KEY([UserId])
);

CREATE TABLE Propeties (
	[PropertyId] INT PRIMARY KEY IDENTITY,
	[TypeOfProperty] VARCHAR(70),
	[District] VARCHAR(150),
	[Price] INT,
	[Area] INT,
	[TypeOfContrusction] VARCHAR(20),
	[PhoneNumber] VARCHAR(50),
	[AdditionalInformation] VARCHAR(2500),
	[ApprovalStatus] VARCHAR(50),
	[Deleted] BIT DEFAULT 0,
	[Sold] BIT DEFAULT 0,
	[UserId] INT FOREIGN KEY REFERENCES Users([UserId])
);

/* 
make auto generate tables for every property
*/