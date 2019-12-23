CREATE PROC uspGetUsersJoins
AS
	BEGIN
		
		SELECT 
			U.FirstName,
			U.LastName,
			UL.UserName,
			UL.Password
		FROM 
			tblUsers as U
		INNER JOIN 
			tblUserLogin AS UL
		ON 
			U.UserId=UL.UserId

	END
GO