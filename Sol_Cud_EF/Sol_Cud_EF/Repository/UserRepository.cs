using Sol_Cud_EF.DbModel.DbContexts;
using Sol_Cud_EF.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
//using Sol_Cud_EF.Extensions;
using Sol_Cud_EF.DbModel.ResultSet;
using System.Data.Common;
using EntityFrameworkCore.Query;

namespace Sol_Cud_EF.Repository
{
    public class UserRepository
    {
        private readonly EFCoreContext eFCoreContext = null;

        public UserRepository(EFCoreContext eFCoreContext)
        {
            this.eFCoreContext = eFCoreContext;
        }

        private async Task<TblUsers> MappingUserTable(UserModel userModel)
        {
            return await Task.Run(() =>
            {

                var tblUserModel = new TblUsers()
                {
                    UserId = userModel.UserId,
                    FirstName = userModel?.FirstName,
                    LastName = userModel?.LastName
                };

                return tblUserModel;

            });

        }

        private async Task<TblUserLogin> MappingUserLoginTable(UserModel userModel)
        {
            return await Task.Run(() =>
            {

                var tblUserLoginModel = new TblUserLogin()
                {
                    UserName = userModel?.UserLogin?.UserName,
                    Password = userModel?.UserLogin?.Password,
                    UserId = userModel?.UserLogin?.UserId
                };

                return tblUserLoginModel;

            });
        }


        private async Task<(TblUsers tblUsers, TblUserLogin tblUsersLogin)> GetUserDataAsync(decimal id)
        {
            try
            {
                var tblUsersObj =
                    await
                    eFCoreContext
                    ?.TblUsers
                    ?.FirstOrDefaultAsync((leTblUsers) => leTblUsers.UserId == id);

                var tblUserLoginObj =
                        await
                        eFCoreContext
                        ?.TblUserLogin
                        ?.FirstOrDefaultAsync((leTblUsers) => leTblUsers.UserId == id);

                (TblUsers tblUsers, TblUserLogin tblUsersLogin) tuplesObj = (
                        tblUsers: tblUsersObj,
                        tblUsersLogin: tblUserLoginObj
                    );

                return tuplesObj;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Boolean> AddTaskAsync(UserModel userModel)
        {
            try
            {
                TblUsers tblUsers = await this.MappingUserTable(userModel);

                await
                    eFCoreContext
                    ?.TblUsers
                    ?.AddAsync(tblUsers);

                await
                    eFCoreContext
                    ?.SaveChangesAsync();

                userModel.UserLogin.UserId = tblUsers.UserId; // Identity Value

                await eFCoreContext
                    ?.TblUserLogin
                    ?.AddAsync(await this.MappingUserLoginTable(userModel));

                await
                   eFCoreContext
                   ?.SaveChangesAsync();


                return true;

            }
            catch
            {
                throw;
            }
        }

        public async Task<Boolean> UpdateTaskAsync(UserModel userModel)
        {
            try
            {
                // get User Data based on Id
                (TblUsers tblUsers, TblUserLogin tblUsersLogin) tuplesObj = await GetUserDataAsync(userModel.UserId);

                if (tuplesObj.tblUsers != null)
                {

                    tuplesObj.tblUsers.FirstName = userModel.FirstName;
                    tuplesObj.tblUsers.LastName = userModel.LastName;


                    //Update

                    eFCoreContext
                    ?.TblUsers
                    ?.Update(tuplesObj.tblUsers);

                    await
                    eFCoreContext
                    ?.SaveChangesAsync();

                    tuplesObj.tblUsersLogin.UserName = userModel?.UserLogin?.UserName;
                    tuplesObj.tblUsersLogin.Password = userModel?.UserLogin?.Password;

                    eFCoreContext
                        ?.TblUserLogin
                        ?.Update(tuplesObj.tblUsersLogin);

                    await
                        eFCoreContext
                        ?.SaveChangesAsync();


                }

                return true;

            }
            catch
            {
                throw;
            }
        }

        public async Task<Boolean> DeleteTaskAsync(UserModel userModel)
        {
            try
            {


                (TblUsers tblUsers, TblUserLogin tblUsersLogin) tuplesObj = await this.GetUserDataAsync(userModel.UserId);


                eFCoreContext
                ?.TblUsers
                ?.Remove(tuplesObj.tblUsers);

                await
                    eFCoreContext
                    .SaveChangesAsync();

                eFCoreContext
                ?.TblUserLogin
                ?.Remove(tuplesObj.tblUsersLogin);

                await
                    eFCoreContext
                    .SaveChangesAsync();


                return true;



            }
            catch
            {
                throw;
            }
        }

        public async Task<Boolean> AddTaskStoredProcedure(UserModel userModel)
        {
            List<SqlParameter> listSqlParameters = new List<SqlParameter>();
            listSqlParameters.Add(new SqlParameter("FirstName", userModel.FirstName));
            listSqlParameters.Add(new SqlParameter("LastName", userModel.LastName));

            await
                eFCoreContext
                .Database
                .ExecuteSqlCommandAsync("EXEC uspUsers @FirstName,@LastName", listSqlParameters.Cast<Object>().ToArray());

            return true;
        }

        public async Task<List<UserModel>> GetUserDataStoredProcedure()
        {


            var data =
                (await
                eFCoreContext
                .TblUsers
                .FromSql("EXEC uspGetUsers")
                .ToListAsync()
                )
                .Select((leTblUsers) => new UserModel()
                {
                    FirstName = leTblUsers.FirstName,
                    LastName = leTblUsers.LastName
                })
                ?.ToList();

            return data;

        }

        public async Task<List<UserModel>> GetUserDataSelectedColumnStoredProcedure()
        {


            var data =
                (
                 await
                 eFCoreContext
                 .SqlQueryAsync<UserSelectedResultSet>("EXEC uspGetUsersSelected")
                )
                .Select((leTblUsers) => new UserModel()
                {
                    FirstName = leTblUsers.FirstName
                })
                ?.ToList();

            return data;

        }

        public async Task<List<UserModel>> GetUserJoinDataSelectedColumnStoredProcedure()
        {
            List<SqlParameter> listSqlParameter = new List<SqlParameter>();
            listSqlParameter.Add(new SqlParameter("@UserId", 2));

            var data =
                (
                 await
                 eFCoreContext
                 .SqlQueryAsync<UserJoinResultSet>("EXEC uspGetUsersJoins @UserId", listSqlParameter)
                
                )
                .Select((leTblUsers) => new UserModel()
                {
                    FirstName = leTblUsers.FirstName,
                    LastName = leTblUsers.LastName,
                    UserLogin = new UserLoginModel()
                    {
                        UserName = leTblUsers.UserName,
                        Password = leTblUsers.Password
                    }
                })
                ?.ToList();

            return data;

        }

		//https://www.yogihosting.com/stored-procedures-entity-framework-core/
        public async Task<(List<UserModel>,List<UserLoginModel>)> GetUserMultipleResultSetStoredProcedures()
        {
            return await Task.Run(() => {

                List<UserModel> userModel = new List<UserModel>();
                List<UserLoginModel> userLoginModel = new List<UserLoginModel>();

                using (var cnn = eFCoreContext.Database.GetDbConnection())
                {
                    var cmm = cnn.CreateCommand();
                    cmm.CommandType = System.Data.CommandType.StoredProcedure;
                    cmm.CommandText = "[dbo].[uspGetUsersMultiResultSet]";
                    //cmm.Parameters.AddRange()
                    cmm.Connection = cnn;
                    cnn.Open();
                    var reader = cmm.ExecuteReader();

                    while (reader.Read())
                    {
                        
                        userModel.Add(new UserModel()
                        {
                            FirstName = Convert.ToString(reader["FirstName"]),
                            LastName=Convert.ToString(reader["LastName"])

                        });
                    }
                    reader.NextResult(); //move the next record set
                    while (reader.Read())
                    {
                        userLoginModel.Add(new UserLoginModel()
                        {
                            UserName = Convert.ToString(reader["UserName"]),
                            Password = Convert.ToString(reader["Password"])
                        });  
                    }

                    reader.Close();
                   

                    (List<UserModel> userList, List<UserLoginModel> userLoginList) tuplesObj =
                        (
                            userList: userModel,
                            userLoginList: userLoginModel
                        );

                  
                    return tuplesObj;
                }

               
            });
            
        }

        public async Task<UserMultipleSelectResultSet> GetUserMultipleResultSetSP()
        {
            List<UserModel> userModel = new List<UserModel>();
            List<UserLoginModel> userLoginModel = new List<UserLoginModel>();

            return
                await
                eFCoreContext
                .SqlQueryMultipleAsync<UserMultipleSelectResultSet>(
                    "[dbo].[uspGetUsersMultiResultSet]",
                    System.Data.CommandType.StoredProcedure,
                    async (leDataReader) =>
                    {
                        while (leDataReader.Read())
                        {

                            userModel.Add(new UserModel()
                            {
                                FirstName = Convert.ToString(leDataReader["FirstName"]),
                                LastName = Convert.ToString(leDataReader["LastName"])

                            });
                        }
                        
                        await leDataReader.NextResultAsync(); //move the next record set

                        while (leDataReader.Read())
                        {
                            userLoginModel.Add(new UserLoginModel()
                            {
                                UserName = Convert.ToString(leDataReader["UserName"]),
                                Password = Convert.ToString(leDataReader["Password"])
                            });
                        }

                        UserMultipleSelectResultSet userMultipleSelectResultSet = new UserMultipleSelectResultSet()
                        {
                            UserModelList = userModel,
                            UserLoginList = userLoginModel
                        };

                        return userMultipleSelectResultSet;
                    }
                    );
        }

      
    }
}
