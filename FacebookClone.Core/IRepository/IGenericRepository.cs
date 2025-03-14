 
using System.Linq.Expressions;
 

namespace FacebookClone.Core.IRepository
{
	public interface IGenericRepository<T> where T : class
	{
		Task<T?> GetByIdAsync(string id);
		Task<IEnumerable<T>> GetAllAsync();
		Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
		Task AddAsync(T entity);
		void Update(T entity);
		void Delete(T entity);
		Task<int> SaveChangesAsync();
	}
}
