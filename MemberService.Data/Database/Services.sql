CREATE TABLE Services(
	Id INT IDENTITY (1, 1) PRIMARY KEY,
	Name NVARCHAR(255), --$5.00/$60.00
	Rate FLOAT, -- 5/60
	Status CHAR(1), -- A, I
	ServiceType NVARCHAR(20), -- OneTime/ Monthly
	AppliedDate datetime
)

