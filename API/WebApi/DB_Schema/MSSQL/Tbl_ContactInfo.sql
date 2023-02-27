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
PRIMARY KEY CLUSTERED 
(
	[ContactInfoID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Tbl_ContactInfo] ADD  CONSTRAINT [DF_Tbl_ContactInfo_IsEnable]  DEFAULT ((1)) FOR [IsEnable]
GO
ALTER TABLE [dbo].[Tbl_ContactInfo] ADD  CONSTRAINT [DF_Tbl_ContactInfo_CreateTime]  DEFAULT (getdate()) FOR [CreateTime]
GO