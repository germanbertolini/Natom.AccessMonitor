CREATE TABLE [dbo].[Config] (
    [Key]         NVARCHAR (50)  NOT NULL,
    [Value]       NVARCHAR (255) NULL,
    [Description] NVARCHAR (300) NULL,
    CONSTRAINT [PK_Config] PRIMARY KEY CLUSTERED ([Key] ASC)
);

