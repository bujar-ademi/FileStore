using Dapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStore.Persistence.Dapper
{
    public static class DapperExtensions
    {
        public static T Insert<T>(this IDbConnection cnn, string tableName, dynamic param)
        {
            IEnumerable<T> result = SqlMapper.Query<T>(cnn, DynamicQuery.GetInsertQuery(tableName, param), param);
            return result.First();
        }

        public static async Task<T> InsertAsync<T>(this IDbConnection cnn, string tableName, dynamic param)
        {
            IEnumerable<T> result = await SqlMapper.QueryAsync<T>(cnn, DynamicQuery.GetInsertQuery(tableName, param), param);
            return result.First();
        }

        public static void Update(this IDbConnection cnn, string tableName, dynamic param)
        {
            SqlMapper.Execute(cnn, DynamicQuery.GetUpdateQuery(tableName, param), param);
        }

        public static async Task UpdateAsync(this IDbConnection cnn, string tableName, dynamic param)
        {
            await SqlMapper.ExecuteAsync(cnn, DynamicQuery.GetUpdateQuery(tableName, param), param);
        }
    }
}
