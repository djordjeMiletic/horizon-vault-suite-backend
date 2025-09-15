using EventHorizon.Application.Interfaces;
using EventHorizon.Infrastructure.Data;

namespace EventHorizon.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventHorizonDbContext _context;
    private readonly Dictionary<Type, object> _repositories = new();

    public UnitOfWork(EventHorizonDbContext context)
    {
        _context = context;
    }

    public IRepository<T> Repository<T>() where T : class
    {
        if (_repositories.ContainsKey(typeof(T)))
            return (IRepository<T>)_repositories[typeof(T)];

        var repository = new Repository<T>(_context);
        _repositories.Add(typeof(T), repository);
        return repository;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}