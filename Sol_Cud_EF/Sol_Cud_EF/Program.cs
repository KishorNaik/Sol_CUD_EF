using Sol_Cud_EF.Model;
using Sol_Cud_EF.Repository;
using System;
using System.Threading.Tasks;

namespace Sol_Cud_EF
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            try
            {
                Task.Run(async() => {

                    var userRepositoryObj = new UserRepository(new DbModel.DbContexts.EFCoreContext());

                    // Add
                    //bool flag = await userRepositoryObj?.AddTaskAsync(new UserModel()
                    //{

                    //    FirstName = "Kishor",
                    //    LastName = "Naik",
                    //    UserLogin = new UserLoginModel()
                    //    {
                    //        UserName = "Kishor11",
                    //        Password = "123"
                    //    }

                    //});  

                    //Update
                    //bool flag = await userRepositoryObj?.UpdateTaskAsync(new UserModel()
                    //{
                    //    UserId=1,
                    //    FirstName = "Eshaan",
                    //    LastName = "Naik",
                    //    UserLogin = new UserLoginModel()
                    //    {
                    //        UserName = "Eshaan11",
                    //        Password = "1234567890"
                    //    }

                    //});

                    // Delete
                    //bool flag = await userRepositoryObj.DeleteTaskAsync(new UserModel() { UserId = 1 });

                    // Add Stored Procedure
                    //bool flag = await userRepositoryObj?.AddTaskStoredProcedure(new UserModel()
                    //{

                    //    FirstName = "Kishor",
                    //    LastName = "Naik",
                    //    UserLogin = new UserLoginModel()
                    //    {
                    //        UserName = "Kishor11",
                    //        Password = "123"
                    //    }

                    //});

                    //// Get User Data
                    try
                    {
                        var data = await userRepositoryObj.GetUserDataStoredProcedure();
                    }
                    catch (Exception ex)
                    {

                    }

                    // Get User Selected Data
                    try
                    {
                        var data = await userRepositoryObj.GetUserDataSelectedColumnStoredProcedure();
                    }
                    catch (Exception ex)
                    {

                    }

                    //Get Join Data
                    try
                    {
                        var data = await userRepositoryObj.GetUserJoinDataSelectedColumnStoredProcedure();
                    }
                    catch (Exception ex)
                    {

                    }

                    //await userRepositoryObj?.GetUserMultipleResultSetStoredProcedures();

                    try
                    {
                        var result = await userRepositoryObj?.GetUserMultipleResultSetSP();

                        var result1 = await userRepositoryObj?.GetUserMultipleResultSetSP();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                    

                }).Wait();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
