using Application.Attributes.Services;
using Application.Contracts.Persistence.Common;
using Application.Contracts.Persistence.Common.BaseRepository;
using Application.Contracts.Persistence.Common.UnitOfWork;
using Application.Exceptions;
using Domain.Entities.Common;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Common.BaseRepository;
using Microsoft.Extensions.DependencyInjection;
using System.Collections;

namespace Infrastructure.Repositories.Common.UnitOfWork
{
    [RegisterService(ServiceLifetime.Scoped)]
    public class UnitOfWork(ApplicationDbContext context) : IUnitOfWork
    {
        private bool _disposed = false;
        private Hashtable _repositories;
        private readonly ApplicationDbContext _context = context;
        //private IInstanceRepository _instanceRepository;
        private IProcedureRepository _procedureRepository;

        //public IInstanceRepository InstanceRepository => _instanceRepository ??= new InstanceRepository(_context);
        public IProcedureRepository ProcedureRepository => _procedureRepository ??= new ProcedureRepository(_context);

        public async Task<int> Complete()
        {
            try
            {
                return await _context.SaveChangesAsync();
            }
            catch (Exception)
            {
                throw new ApiException("Error writing to data container");
            }
        }

        public IBaseRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity
        {
            _repositories ??= [];

            var type = typeof(TEntity).Name;

            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(BaseRepository<>);
                var repositoryInstance = Activator.CreateInstance(repositoryType.MakeGenericType(typeof(TEntity)), _context);
                _repositories.Add(type, repositoryInstance);
            }
            return (IBaseRepository<TEntity>)_repositories[type]!;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
