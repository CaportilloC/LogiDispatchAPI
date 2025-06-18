using Application.Attributes.Services;
using Application.Contracts.Persistence.Common.BaseRepository;
using Application.Models.Procedures;
using Domain.Procedures.Common;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Repositories.Common.BaseRepository
{
    [RegisterService(ServiceLifetime.Transient)]
    public class ProcedureRepository(ApplicationDbContext context) : IProcedureRepository
    {
        private readonly ApplicationDbContext _context = context;

        public async Task<List<TProcedureResponse>> GetProcedureAsync<TProcedureResponse>(ProcedureRequest request)
            where TProcedureResponse : BaseProcedureResponse
        {
            var response = await _context.Database.SqlQueryRaw<TProcedureResponse>(request.Routine, request.Parameters).ToListAsync();
            return response;
        }

        public async Task ExecuteProcedureAsync(ProcedureRequest request)
        {
            await _context.Database.ExecuteSqlRawAsync(request.Routine, request.Parameters);
        }
    }
}
