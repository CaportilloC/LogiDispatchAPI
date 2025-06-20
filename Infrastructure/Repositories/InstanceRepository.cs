using Application.Attributes.Services;
using Application.Contracts.Persistence.Common;
using Infrastructure.DbContexts;
using Infrastructure.Repositories.Common.BaseRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories
{
    //[RegisterService(ServiceLifetime.Transient)]
    //public class InstanceRepository(ApplicationDbContext context) : BaseRepository<InstanceEntity>(context), IInstanceRepository
    //{
    //    public async Task<InstanceEntity?> GetInstanceByNameAsync(string name)
    //    {
    //        var response = await _context.InstanceEntity
    //            .FirstOrDefaultAsync(x => x.Name.ToUpper() == name.ToUpper());
    //        return response;
    //    }
    //}
}
