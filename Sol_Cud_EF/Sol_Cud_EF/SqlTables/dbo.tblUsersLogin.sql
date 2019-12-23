CREATE TABLE [dbo].[tblUserLogin]
(
	[UserLoginId] NUmeric(18,0) IDENTITY(1,1)  NOT NULL PRIMARY KEY,
	UserName Varchar(100),
	Password Varchar(100)
)
