using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EventTicketing.Core.Interfaces
{
    /// <summary>
    /// Interfaz genérica para repositorios siguiendo el patrón Repository
    /// </summary>
    /// <typeparam name="T">Tipo de entidad</typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Obtiene una entidad por su identificador
        /// </summary>
        Task<T> GetByIdAsync(Guid id);

        /// <summary>
        /// Obtiene todas las entidades
        /// </summary>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Busca entidades que cumplan con una condición
        /// </summary>
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);

        /// <summary>
        /// Agrega una nueva entidad
        /// </summary>
        Task AddAsync(T entity);

        /// <summary>
        /// Agrega un rango de entidades
        /// </summary>
        Task AddRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Actualiza una entidad existente
        /// </summary>
        Task UpdateAsync(T entity);

        /// <summary>
        /// Elimina una entidad
        /// </summary>
        Task DeleteAsync(T entity);

        /// <summary>
        /// Elimina un rango de entidades
        /// </summary>
        Task DeleteRangeAsync(IEnumerable<T> entities);

        /// <summary>
        /// Cuenta el número de entidades que cumplen con una condición
        /// </summary>
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null);
    }
}