
GO

/****** Object:  Table [dbo].[K33P_LOG_SAPSF]    Script Date: 15/12/2023 13:56:35 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[K33P_LOG_SAPSF](
	[ID] [bigint] IDENTITY(1,1) NOT NULL,
	[LOGDATE] [nvarchar](50) NOT NULL,
	[LOGHOUR] [nvarchar](50) NOT NULL,
	[COMPANY] [nvarchar](50) NOT NULL,
	[MESSAGE] [ntext] NOT NULL,
	[KEY_PARC] [nvarchar](50) NOT NULL,
	[KEY_SAP] [nvarchar](50) NOT NULL,
	[REQUESTOBJECT] [ntext] NOT NULL,
	[RESPONSEOBJECT] [ntext] NOT NULL,
	[OWNER] [nvarchar](50) NOT NULL,
	[METHOD] [nvarchar](50) NOT NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO


