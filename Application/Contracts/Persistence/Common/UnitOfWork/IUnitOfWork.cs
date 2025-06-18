using Application.Contracts.Persistence.Common.BaseRepository;
using Domain.Entities.Common;

namespace Application.Contracts.Persistence.Common.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        //IInstanceRepository InstanceRepository { get; }
        IProcedureRepository ProcedureRepository { get; }
        IBaseRepository<TEntity> Repository<TEntity>() where TEntity : BaseEntity;
        Task<int> Complete();
    }
}
