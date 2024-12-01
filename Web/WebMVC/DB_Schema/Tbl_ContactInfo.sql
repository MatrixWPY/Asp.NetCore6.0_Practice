CREATE TABLE [dbo].[Tbl_ContactInfo](
	[ContactInfoID] [bigint] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](10) NOT NULL,
	[Nickname] [nvarchar](10) NULL,
	[Gender] [tinyint] NULL,
	[Age] [tinyint] NULL,
	[PhoneNo] [varchar](20) NOT NULL,
	[Address] [nvarchar](100) NOT NULL,
	[IsEnable] [bit] NOT NULL,
	[CreateTime] [datetime] NOT NULL,
	[UpdateTime] [datetime] NULL,
	[RowVersion] [timestamp] NOT NULL,
 CONSTRAINT [PK_Tbl_ContactInfo] PRIMARY KEY CLUSTERED 
(
	[ContactInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tbl_ContactInfo] ADD  DEFAULT (CONVERT([bit],(1))) FOR [IsEnable]
GO
ALTER TABLE [dbo].[Tbl_ContactInfo] ADD  DEFAULT (getdate()) FOR [CreateTime]
GO