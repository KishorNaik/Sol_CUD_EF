ALTER PROC uspGetUsersMultiResultSet
AS
	BEGIN
		
		SELECT * FROM tblUsers

		SELECT * FROM tblUserLogin

	END
GO