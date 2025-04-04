using Dapper;
using FileStore.Application.Interfaces;
using FileStore.Domain.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStore.Persistence.Dapper
{
    public abstract class Repository<T> : IRepository<T> where T : EntityBase
    {
        private readonly IContext _context;
        private readonly string _tableName;

        protected Repository(IContext context, string tableName)
        {
            _context = context;
            _tableName = tableName;
        }

        internal IDbConnection Connection => new SqlConnection(_context.GetConnectionString());

        public void Add(T item)
        {
            using (var cn = Connection)
            {
                var parameters = (object)Mapping(item);
                cn.Open();
                item.Id = cn.Insert<Guid>(_tableName, parameters);
            }
        }

        public async Task AddAsync(T item)
        {
            using (var cn = Connection)
            {
                var parameters = (object)Mapping(item);
                cn.Open();
                item.Id = await cn.InsertAsync<Guid>(_tableName, parameters);
            }
        }

        public void Update(T item)
        {
            using (var cn = Connection)
            {
                var parameters = (object)Mapping(item);
                cn.Open();

                cn.Update(_tableName, parameters);
            }
        }

        public async Task UpdateAsync(T item)
        {
            using (var cn = Connection)
            {
                var parameters = (object)Mapping(item);
                cn.Open();
                await cn.UpdateAsync(_tableName, parameters);
            }
        }

        public void Remove(T item)
        {
            using (var cn = Connection)
            {
                cn.Open();
                cn.Execute("DELETE FROM " + _tableName + " WHERE Id=@Id", new { Id = item.Id });
            }
        }

        public async Task RemoveAsync(T item)
        {
            using (var cn = Connection)
            {
                cn.Open();
                await cn.ExecuteAsync("DELETE FROM " + _tableName + " WHERE Id=@Id", new { Id = item.Id });
            }
        }

        public IEnumerable<T> FindAll()
        {
            IEnumerable<T> items = null;

            using (var cn = Connection)
            {
                cn.Open();
                items = cn.Query<T>("SELECT * FROM " + _tableName);
            }

            return items;
        }

        public async Task<IEnumerable<T>> FindAllAsync()
        {
            IEnumerable<T> items = null;
            using (var cn = Connection)
            {
                cn.Open();
                items = await cn.QueryAsync<T>("SELECT * FROM " + _tableName);
            }
            return items;
        }

        public T GetById(Guid id)
        {
            var item = default(T);

            using (var cn = Connection)
            {
                cn.Open();
                item = cn.Query<T>("SELECT * FROM " + _tableName + " WHERE Id=@Id", new { Id = id }).SingleOrDefault();
            }

            return item;
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            var item = default(T);

            using (var cn = Connection)
            {
                cn.Open();
                item = await cn.QueryFirstAsync<T>("SELECT * FROM " + _tableName + " WHERE Id=@Id", new { Id = id });
            }
            return item;
        }

        /// <summary>
        ///     Mapping the object to the insert/update columns.
        /// </summary>
        /// <param name="item">The item.</param>
        /// <returns>The parameters with values.</returns>
        /// <remarks>In the default case, we take the object as is with no custom mapping.</remarks>
        internal virtual dynamic Mapping(T item)
        {
            return item;
        }
    }
}
