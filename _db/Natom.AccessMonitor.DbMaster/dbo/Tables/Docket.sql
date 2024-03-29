﻿CREATE TABLE [dbo].[Docket] (
	DocketId INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
	DocketNumber NVARCHAR(20) NOT NULL,
	ClientId INT NOT NULL,
	PlaceId INT,
	FirstName NVARCHAR(50) NOT NULL,
	LastName NVARCHAR(50) NOT NULL,
	DNI NVARCHAR(15) NOT NULL,
	TitleId INT NOT NULL,
	HourValue DECIMAL(18,2),
	ExtraHourValue DECIMAL(18,2),
	Active BIT NOT NULL,
	ApplyInOutControl BIT NOT NULL
)