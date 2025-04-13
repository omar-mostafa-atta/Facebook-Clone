 
using System.Linq.Expressions;
 

namespace FacebookClone.Core.IRepository
{
	public interface IGenericRepository<T> where T : class
	{
		Task<T?> GetByIdAsync(Guid id);
		IQueryable<T> AsQueryable();
		Task<IEnumerable<T>> GetAllAsync();
		Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
		Task AddAsync(T entity);
		Task Update(T entity);
		Task Delete(T entity);
		Task<int> SaveChangesAsync();
	}
}
