using FileStore.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileStore.Application.Interfaces
{
    public interface IRepository<T> where T : EntityBase
    {
        void Add(T item);
        Task AddAsync(T item);
        void Update(T item);
        Task UpdateAsync(T item);
        void Remove(T item);
        Task RemoveAsync(T item);
        T GetById(Guid id);
        Task<T> GetByIdAsync(Guid id);
        IEnumerable<T> FindAll();
        Task<IEnumerable<T>> FindAllAsync();
    }
}
