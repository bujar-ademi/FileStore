using System.Linq;
using System.Reflection;

namespace FileStore.Persistence.Dapper
{
    /// <summary>
    ///     Dynamic query class.
    /// </summary>
    public sealed class DynamicQuery
    {
        /// <summary>
        ///     Gets the insert query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     The Sql query based on the item properties.
        /// </returns>
        public static string GetInsertQuery(string tableName, dynamic item)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            //var columns = props.Select(p => p.Name).Where(s => s.ToLower() != "id").ToArray();
            var columns = props.Select(p => p.Name).ToArray();

            // for debugging purposes 
            var tempString = string.Format("INSERT INTO [{0}] ({1}) OUTPUT inserted.ID VALUES (@{2})",
                tableName,
                string.Join(",", columns),
                string.Join(",@", columns));

            return tempString;
        }

        /// <summary>
        ///     Gets the update query.
        /// </summary>
        /// <param name="tableName">Name of the table.</param>
        /// <param name="item">The item.</param>
        /// <returns>
        ///     The Sql query based on the item properties.
        /// </returns>
        public static string GetUpdateQuery(string tableName, dynamic item)
        {
            PropertyInfo[] props = item.GetType().GetProperties();
            var columns = props.Select(p => p.Name).ToArray();

            var parameters = columns.Where(name => name != "Id").Select(name => name + "=@" + name).ToList();

            return string.Format("UPDATE [{0}] SET {1} WHERE ID=@ID", tableName, string.Join(",", parameters));
        }
    }
}
