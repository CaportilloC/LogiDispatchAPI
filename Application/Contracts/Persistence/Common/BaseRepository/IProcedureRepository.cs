using Application.Models.Procedures;
using Domain.Procedures.Common;

namespace Application.Contracts.Persistence.Common.BaseRepository
{
    public interface IProcedureRepository
    {
        Task<List<TProcedureResponse>> GetProcedureAsync<TProcedureResponse>(ProcedureRequest request) where TProcedureResponse : BaseProcedureResponse;

        Task ExecuteProcedureAsync(ProcedureRequest request);
    }
}
