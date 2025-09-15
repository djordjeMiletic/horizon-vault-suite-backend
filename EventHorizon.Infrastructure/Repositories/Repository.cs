using EventHorizon.Application.Interfaces;
using EventHorizon.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EventHorizon.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly EventHorizonDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(EventHorizonDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public IQueryable<T> Query()
    {
        return _dbSet.AsQueryable();
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
}