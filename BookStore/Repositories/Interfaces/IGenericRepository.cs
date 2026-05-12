namespace BookStore.Repositories.Interfaces
{
	public interface IGenericRepository<T> where T : class
	{
		Task<IEnumerable<T>> GetAllAsync();
		Task<T?> GetByIdAsync(params object[] id);
		Task AddAsync(T entity);
		Task UpdateAsync(T entity);
		Task DeleteAsync(params object[] id);
		Task<bool> ExistsAsync(params object[] id);
	}

}
