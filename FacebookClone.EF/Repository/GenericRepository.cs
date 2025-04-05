using FacebookClone.Core.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace FacebookClone.EF.Repository
{
	public class GenericRepository<T> : IGenericRepository<T> where T : class
	{
		private readonly FacebookContext _context;
		private readonly DbSet<T> _dbSet;

		public GenericRepository(FacebookContext context)
		{
			_context = context;
			_dbSet=_context.Set<T>();
		}
		public async Task AddAsync(T entity)
		{
			await _dbSet.AddAsync(entity);
		}

		public  Task Delete(T entity)
		{
			_dbSet.Remove(entity);
			return Task.CompletedTask;
		}

		public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
		{
			  return await _dbSet.Where(predicate).ToListAsync();
		}

		public async Task<IEnumerable<T>> GetAllAsync()
		{
			return await _dbSet.ToListAsync();
		}

		public async Task<T?> GetByIdAsync(Guid id)
		{
			
			return await _dbSet.FindAsync(id);
		}

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}

		public async Task Update(T entity)
		{
			_dbSet.Update(entity); 
			await _context.SaveChangesAsync();
		}
	}
}
