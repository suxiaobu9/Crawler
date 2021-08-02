USE [master]
GO

/****** Object:  Database [JobCrawler]    Script Date: 2021/8/2 上午 11:37:25 ******/
CREATE DATABASE [JobCrawler]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'OneOFourCrawler', FILENAME = N'/var/opt/mssql/data/OneOFourCrawler.mdf' , SIZE = 73728KB , MAXSIZE = UNLIMITED, FILEGROWTH = 65536KB )
 LOG ON 
( NAME = N'OneOFourCrawler_log', FILENAME = N'/var/opt/mssql/data/OneOFourCrawler_log.ldf' , SIZE = 73728KB , MAXSIZE = 2048GB , FILEGROWTH = 65536KB )
 WITH CATALOG_COLLATION = DATABASE_DEFAULT
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [JobCrawler].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO

ALTER DATABASE [JobCrawler] SET ANSI_NULL_DEFAULT OFF 
GO

ALTER DATABASE [JobCrawler] SET ANSI_NULLS OFF 
GO

ALTER DATABASE [JobCrawler] SET ANSI_PADDING OFF 
GO

ALTER DATABASE [JobCrawler] SET ANSI_WARNINGS OFF 
GO

ALTER DATABASE [JobCrawler] SET ARITHABORT OFF 
GO

ALTER DATABASE [JobCrawler] SET AUTO_CLOSE OFF 
GO

ALTER DATABASE [JobCrawler] SET AUTO_SHRINK OFF 
GO

ALTER DATABASE [JobCrawler] SET AUTO_UPDATE_STATISTICS ON 
GO

ALTER DATABASE [JobCrawler] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO

ALTER DATABASE [JobCrawler] SET CURSOR_DEFAULT  GLOBAL 
GO

ALTER DATABASE [JobCrawler] SET CONCAT_NULL_YIELDS_NULL OFF 
GO

ALTER DATABASE [JobCrawler] SET NUMERIC_ROUNDABORT OFF 
GO

ALTER DATABASE [JobCrawler] SET QUOTED_IDENTIFIER OFF 
GO

ALTER DATABASE [JobCrawler] SET RECURSIVE_TRIGGERS OFF 
GO

ALTER DATABASE [JobCrawler] SET  DISABLE_BROKER 
GO

ALTER DATABASE [JobCrawler] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO

ALTER DATABASE [JobCrawler] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO

ALTER DATABASE [JobCrawler] SET TRUSTWORTHY OFF 
GO

ALTER DATABASE [JobCrawler] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO

ALTER DATABASE [JobCrawler] SET PARAMETERIZATION SIMPLE 
GO

ALTER DATABASE [JobCrawler] SET READ_COMMITTED_SNAPSHOT OFF 
GO

ALTER DATABASE [JobCrawler] SET HONOR_BROKER_PRIORITY OFF 
GO

ALTER DATABASE [JobCrawler] SET RECOVERY FULL 
GO

ALTER DATABASE [JobCrawler] SET  MULTI_USER 
GO

ALTER DATABASE [JobCrawler] SET PAGE_VERIFY CHECKSUM  
GO

ALTER DATABASE [JobCrawler] SET DB_CHAINING OFF 
GO

ALTER DATABASE [JobCrawler] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO

ALTER DATABASE [JobCrawler] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO

ALTER DATABASE [JobCrawler] SET DELAYED_DURABILITY = DISABLED 
GO

ALTER DATABASE [JobCrawler] SET ACCELERATED_DATABASE_RECOVERY = OFF  
GO

ALTER DATABASE [JobCrawler] SET QUERY_STORE = OFF
GO

ALTER DATABASE [JobCrawler] SET  READ_WRITE 
GO


USE [JobCrawler]
GO

/****** Object:  Table [dbo].[Company]    Script Date: 2021/8/2 上午 11:38:26 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Company](
	[No] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[SourceFrom] [nvarchar](200) NOT NULL,
	[Link] [nvarchar](max) NOT NULL,
	[Welfare] [nvarchar](max) NULL,
	[LatestCheckDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Company_1] PRIMARY KEY CLUSTERED 
(
	[No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Company] ADD  CONSTRAINT [DF_Company_LatestCheckDate]  DEFAULT (getdate()) FOR [LatestCheckDate]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'公司編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Company', @level2type=N'COLUMN',@level2name=N'No'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'公司名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Company', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'資料來源' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Company', @level2type=N'COLUMN',@level2name=N'SourceFrom'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'公司連結' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Company', @level2type=N'COLUMN',@level2name=N'Link'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'福利' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Company', @level2type=N'COLUMN',@level2name=N'Welfare'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後確認資料時間' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Company', @level2type=N'COLUMN',@level2name=N'LatestCheckDate'
GO


USE [JobCrawler]
GO

/****** Object:  Table [dbo].[Vacancy]    Script Date: 2021/8/2 上午 11:38:38 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Vacancy](
	[CompanyNo] [nvarchar](100) NOT NULL,
	[No] [nvarchar](100) NOT NULL,
	[Name] [nvarchar](200) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Required] [nvarchar](max) NULL,
	[Seniority] [int] NULL,
	[SalaryMin] [int] NOT NULL,
	[SalaryMax] [int] NOT NULL,
	[AppearDate] [datetime] NOT NULL,
	[Link] [nvarchar](max) NOT NULL,
	[CreatedDate] [datetime] NOT NULL,
	[IsDelete] [bit] NOT NULL,
 CONSTRAINT [PK_Vacancy_1] PRIMARY KEY CLUSTERED 
(
	[No] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO

ALTER TABLE [dbo].[Vacancy] ADD  CONSTRAINT [DF_Vacancy_CreatedDate]  DEFAULT (getdate()) FOR [CreatedDate]
GO

ALTER TABLE [dbo].[Vacancy] ADD  CONSTRAINT [DF_Vacancy_IsDelete]  DEFAULT ((0)) FOR [IsDelete]
GO

ALTER TABLE [dbo].[Vacancy]  WITH CHECK ADD  CONSTRAINT [FK_Vacancy_Company] FOREIGN KEY([CompanyNo])
REFERENCES [dbo].[Company] ([No])
GO

ALTER TABLE [dbo].[Vacancy] CHECK CONSTRAINT [FK_Vacancy_Company]
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'公司Id' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'CompanyNo'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'職缺編號' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'No'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'職缺名稱' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'Name'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'職缺描述' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'Description'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'條件要求' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'Required'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'資歷' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'Seniority'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'低薪' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'SalaryMin'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'高薪' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'SalaryMax'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'最後更新日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'AppearDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'職缺連結' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'Link'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'系統建立日期' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'CreatedDate'
GO

EXEC sys.sp_addextendedproperty @name=N'MS_Description', @value=N'是否刪除' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'TABLE',@level1name=N'Vacancy', @level2type=N'COLUMN',@level2name=N'IsDelete'
GO


