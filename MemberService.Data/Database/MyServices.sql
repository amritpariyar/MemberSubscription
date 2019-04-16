DROP TABLE MyServices;
GO
CREATE TABLE MyServices(
	Id INT IDENTITY (1, 1) PRIMARY KEY,
	MemberId INT FOREIGN KEY REFERENCES Member(Id),
	ServiceId INT FOREIGN KEY REFERENCES Services(Id),
	StartDate DATETIME, -- Service Start From
	ValidDate DATETIME, -- Service End Date
	Status NVARCHAR(20), -- Active/InActive/Cancelled
	CancelledDate DATETIME NULL, -- Date If Cancelled
	IsPaid BIT,
	Amount FLOAT NULL, -- amount cann be retrieved from respected serviceId,
	PaymentConfirmed BIT
)
GO