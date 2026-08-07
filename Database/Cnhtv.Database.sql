/*
  CNH TV - Banco de dados SQL Server
  Compatível com SQL Server 2016 ou superior.
*/
IF DB_ID(N'CNHTV') IS NULL
BEGIN
    CREATE DATABASE [CNHTV];
END;
GO

USE [CNHTV];
GO

IF OBJECT_ID(N'dbo.TvDevice', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TvDevice
    (
        Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TvDevice PRIMARY KEY,
        DeviceKey NVARCHAR(100) NOT NULL,
        Name NVARCHAR(100) NOT NULL,
        Model NVARCHAR(100) NULL,
        WebOsVersion NVARCHAR(50) NULL,
        AppVersion NVARCHAR(30) NULL,
        IpAddress NVARCHAR(64) NULL,
        PresentationUrl NVARCHAR(2048) NOT NULL
            CONSTRAINT DF_TvDevice_PresentationUrl DEFAULT (N'https://itm.curitiba.cnh.com/cnhtv/presentation'),
        RefreshEnabled BIT NOT NULL CONSTRAINT DF_TvDevice_RefreshEnabled DEFAULT (0),
        RefreshIntervalSeconds INT NOT NULL CONSTRAINT DF_TvDevice_RefreshInterval DEFAULT (300),
        IsOnline BIT NOT NULL CONSTRAINT DF_TvDevice_IsOnline DEFAULT (0),
        CreatedAtUtc DATETIME2(3) NOT NULL CONSTRAINT DF_TvDevice_CreatedAtUtc DEFAULT (SYSUTCDATETIME()),
        LastSeenAtUtc DATETIME2(3) NOT NULL CONSTRAINT DF_TvDevice_LastSeenAtUtc DEFAULT (SYSUTCDATETIME()),
        ConfigurationUpdatedAtUtc DATETIME2(3) NOT NULL CONSTRAINT DF_TvDevice_ConfigUpdatedAtUtc DEFAULT (SYSUTCDATETIME()),
        CONSTRAINT CK_TvDevice_RefreshInterval CHECK (RefreshIntervalSeconds BETWEEN 30 AND 86400)
    );
    CREATE UNIQUE INDEX UX_TvDevice_DeviceKey ON dbo.TvDevice(DeviceKey);
END;
GO

IF OBJECT_ID(N'dbo.TvConnectionHistory', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.TvConnectionHistory
    (
        Id BIGINT IDENTITY(1,1) NOT NULL CONSTRAINT PK_TvConnectionHistory PRIMARY KEY,
        TvDeviceId BIGINT NOT NULL,
        ConnectedAtUtc DATETIME2(3) NOT NULL,
        LastHeartbeatAtUtc DATETIME2(3) NOT NULL,
        DisconnectedAtUtc DATETIME2(3) NULL,
        IpAddress NVARCHAR(64) NULL,
        CONSTRAINT FK_TvConnectionHistory_TvDevice FOREIGN KEY (TvDeviceId)
            REFERENCES dbo.TvDevice(Id) ON DELETE CASCADE
    );
    CREATE INDEX IX_TvConnectionHistory_Device_Open
        ON dbo.TvConnectionHistory(TvDeviceId, DisconnectedAtUtc);
END;
GO
