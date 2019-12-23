CREATE PROC uspUsers
(
	@FirstName Varchar(100),
	@LastName Varchar(100)
)
AS
	BEGIN
		
		INSERT INTO tblUsers
		(
			FirstName,
			LastName
		)
		VALUES
		(
			@FirstName,
			@LastName
		)

	END
GO