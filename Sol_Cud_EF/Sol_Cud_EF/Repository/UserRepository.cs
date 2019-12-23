﻿using Sol_Cud_EF.DbModel.DbContexts;
using Sol_Cud_EF.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using Sol_Cud_EF.Extensions;
using Sol_Cud_EF.DbModel.ResultSet;
using System.Data.Common;

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


            var data =
                (
                 await
                 eFCoreContext
                 .SqlQueryAsync<UserJoinResultSet>("EXEC uspGetUsersJoins")
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

      
    }
}
