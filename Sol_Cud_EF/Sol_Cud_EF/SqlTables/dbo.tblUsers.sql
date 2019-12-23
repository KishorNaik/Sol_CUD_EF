CREATE TABLE [dbo].[tblUsers]
(
	UserId numeric(18,0) IDENTITY(1,1) PRIMARY KEY NOT NULL,
	FirstName Varchar(100),
	LastName Varchar(100)
)
