using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sol_Cud_EF.Extensions
{
    public static class SqlQueryExtensions
    {
        public static IList<T> SqlQuery<T>(this DbContext db, string sql, params object[] parameters) where T : class
        {
            using (var db2 = new ContextForQueryType<T>(db.Database.GetDbConnection()))
            {
                return db2.Query<T>().FromSql(sql, parameters).ToList();
            }
        }

        public static IList<T> SqlQuery<T>(this DbContext db, string sql) where T : class
        {
            using (var db2 = new ContextForQueryType<T>(db.Database.GetDbConnection()))
            {
                return db2.Query<T>().FromSql(sql).ToList();
            }
        }

        public static async Task<IList<T>> SqlQueryAsync<T>(this DbContext db, string sql, params object[] parameters) where T : class
        {
            using (var db2 = new ContextForQueryType<T>(db.Database.GetDbConnection()))
            {
                return await db2.Query<T>().FromSql(sql, parameters).ToListAsync();
            }
        }

        public static async Task<IList<T>> SqlQueryAsync<T>(this DbContext db, string sql) where T : class
        {
            using (var db2 = new ContextForQueryType<T>(db.Database.GetDbConnection()))
            {
                return await db2.Query<T>().FromSql(sql).ToListAsync();
            }
        }

        public static async Task<TMultipleResultSet> SqlQueryMultipleAsync<TMultipleResultSet>(
            this DbContext db,
            string sql,
            CommandType commandType,
            Func<DbDataReader,Task<TMultipleResultSet>> funcReaders
            )
            where TMultipleResultSet: class
        {

            var connection = db.Database.GetDbConnection();

            if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
            {
                await connection.OpenAsync();
            }

            var command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = sql;
            command.Connection = connection;

           

            var reader = await command.ExecuteReaderAsync();

            var data=await funcReaders(reader);

            await connection.CloseAsync();
               
            return data;

        }

        public static async Task<TMultipleResultSet> SqlQueryMultipleAsync<TMultipleResultSet>(
           this DbContext db,
           string sql,
           List<SqlParameter> listSqlParameter,
           CommandType commandType,
           Func<DbDataReader, Task<TMultipleResultSet>> funcReaders
           )
           where TMultipleResultSet : class
        {

            var connection = db.Database.GetDbConnection();

            if (connection.State == ConnectionState.Closed || connection.State == ConnectionState.Broken)
            {
                await connection.OpenAsync();
            }

            var command = connection.CreateCommand();
            command.CommandType = commandType;
            command.CommandText = sql;
            command.Connection = connection;

            if(listSqlParameter!=null)
            {
                command.Parameters.AddRange(listSqlParameter.Cast<Object>().ToArray());
            }
            
            var reader = await command.ExecuteReaderAsync();

            var data = await funcReaders(reader);

            await connection.CloseAsync();

            return data;

        }



        private class ContextForQueryType<T> : DbContext where T : class
        {
            private readonly DbConnection connection;

            public ContextForQueryType(DbConnection connection)
            {
                this.connection = connection;
            }

            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                // switch on the connection type name to enable support multiple providers
                // var name = con.GetType().Name;
                optionsBuilder.UseSqlServer(connection, options => options.EnableRetryOnFailure());

                base.OnConfiguring(optionsBuilder);
            }

            protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                modelBuilder.Query<T>();
                base.OnModelCreating(modelBuilder);
            }
        }
    }
}
