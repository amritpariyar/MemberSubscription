USE master;
GO
CREATE DATABASE MemberService;
GO

USE [MemberService]
GO
/****** Object:  Table [dbo].[Member]    Script Date: 4/15/2019 8:02:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Member](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Email] [nvarchar](256) NULL,
	[EmailConfirmed] [bit] NOT NULL,
	[PasswordHash] [nvarchar](max) NULL,
	[SecurityStamp] [nvarchar](max) NULL,
	[PhoneNumber] [nvarchar](max) NULL,
	[PhoneNumberConfirmed] [bit] NOT NULL,
	[TwoFactorEnabled] [bit] NOT NULL,
	[LockoutEndDateUtc] [datetime] NULL,
	[LockoutEnabled] [bit] NOT NULL,
	[AccessFailedCount] [int] NOT NULL,
	[UserName] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.Member] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MemberClaim]    Script Date: 4/15/2019 8:02:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MemberClaim](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[MemberId] [int] NOT NULL,
	[ClaimType] [nvarchar](max) NULL,
	[ClaimValue] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.MemberClaim] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MemberLogin]    Script Date: 4/15/2019 8:02:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MemberLogin](
	[LoginProvider] [nvarchar](128) NOT NULL,
	[ProviderKey] [nvarchar](128) NOT NULL,
	[MemberId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.MemberLogin] PRIMARY KEY CLUSTERED 
(
	[LoginProvider] ASC,
	[ProviderKey] ASC,
	[MemberId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[MemberRole]    Script Date: 4/15/2019 8:02:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[MemberRole](
	[MemberId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.MemberRoles] PRIMARY KEY CLUSTERED 
(
	[MemberId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Role]    Script Date: 4/15/2019 8:02:09 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](256) NOT NULL,
 CONSTRAINT [PK_dbo.Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
ALTER TABLE [dbo].[MemberClaim]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MemberClaim_dbo.Member_MemberId] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MemberClaim] CHECK CONSTRAINT [FK_dbo.MemberClaim_dbo.Member_MemberId]
GO
ALTER TABLE [dbo].[MemberLogin]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MemberLogin_dbo.Member_MemberId] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MemberLogin] CHECK CONSTRAINT [FK_dbo.MemberLogin_dbo.Member_MemberId]
GO
ALTER TABLE [dbo].[MemberRole]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MemberRoles_dbo.Member_MemberId] FOREIGN KEY([MemberId])
REFERENCES [dbo].[Member] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MemberRole] CHECK CONSTRAINT [FK_dbo.MemberRoles_dbo.Member_MemberId]
GO
ALTER TABLE [dbo].[MemberRole]  WITH CHECK ADD  CONSTRAINT [FK_dbo.MemberRoles_dbo.Roles_RoleId] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[MemberRole] CHECK CONSTRAINT [FK_dbo.MemberRoles_dbo.Roles_RoleId]
GO


INSERT INTO ROLE(Name) VALUES('ADMIN')
GO 
INSERT INTO ROLE(Name) VALUES('MEMBER')
GO

CREATE TABLE Services(
	Id INT IDENTITY (1, 1) PRIMARY KEY,
	Name NVARCHAR(255), --$5.00/$60.00
	Rate FLOAT, -- 5/60
	Status CHAR(1), -- A, I
	ServiceType NVARCHAR(20), -- OneTime/ Monthly
	AppliedDate datetime
)
GO

SET IDENTITY_INSERT [dbo].[Services] ON 

GO
INSERT [dbo].[Services] ([Id], [Name], [Rate], [Status], [ServiceType], [AppliedDate]) VALUES (1, N'Installation', 60, N'I', N'OneTime', CAST(N'2019-04-14 00:00:00.000' AS DateTime))
GO
INSERT [dbo].[Services] ([Id], [Name], [Rate], [Status], [ServiceType], [AppliedDate]) VALUES (2, N'Monthly', 5, N'A', N'Monthly', CAST(N'2019-04-14 00:00:00.000' AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Services] OFF
GO


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