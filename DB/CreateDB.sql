-- Create tables

CREATE TABLE [dbo].[Accounts] (
    [UserId]   BIGINT        IDENTITY (1, 1) NOT NULL,
    [Login]    NVARCHAR (50) NOT NULL,
    [Password] NVARCHAR (50) NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC),
    UNIQUE NONCLUSTERED ([Login] ASC)
);

CREATE TABLE [dbo].[ProfileInfoTypes] (
    [Id]   SMALLINT     IDENTITY (1, 1) NOT NULL,
    [Type] VARCHAR (20) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[ProfileInfoEntries] (
    [UserId]   BIGINT         NOT NULL,
    [TypeId]   SMALLINT       NOT NULL,
    [Value]    NVARCHAR (MAX) NOT NULL,
    [IsPublic] BIT            DEFAULT ((0)) NOT NULL,
    PRIMARY KEY CLUSTERED ([UserId] ASC, [TypeId] ASC),
    CONSTRAINT [FK_ProfileInfoEntries_Accounts] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Accounts] ([UserId]) ON DELETE CASCADE,
    CONSTRAINT [FK_ProfileInfoEntries_ProfileInfoTypes] FOREIGN KEY ([TypeId]) REFERENCES [dbo].[ProfileInfoTypes] ([Id]) ON DELETE CASCADE
);


-- Fill data

SET IDENTITY_INSERT [dbo].[ProfileInfoTypes] ON
INSERT INTO [dbo].[ProfileInfoTypes] ([Id], [Type]) VALUES (1, N'firstname')
INSERT INTO [dbo].[ProfileInfoTypes] ([Id], [Type]) VALUES (2, N'lastname')
INSERT INTO [dbo].[ProfileInfoTypes] ([Id], [Type]) VALUES (3, N'middlename')
INSERT INTO [dbo].[ProfileInfoTypes] ([Id], [Type]) VALUES (5, N'description')
INSERT INTO [dbo].[ProfileInfoTypes] ([Id], [Type]) VALUES (6, N'city')
INSERT INTO [dbo].[ProfileInfoTypes] ([Id], [Type]) VALUES (7, N'region')
SET IDENTITY_INSERT [dbo].[ProfileInfoTypes] OFF
