using Microsoft.Data.SqlClient;
using System.Data;

namespace Application.Models.Procedures
{
    public class ProcedureRequest
    {
        public readonly string ProcedureName;
        public SqlParameter[] Parameters { get; private set; }
        public string Routine { get; private set; } = null!;

        public ProcedureRequest(string procedureName, SqlParameter[]? parameters = null)
        {
            ProcedureName = procedureName;
            Parameters = parameters ?? [];
            SetProcedureRoutine();
        }

        private void SetProcedureRoutine()
        {
            var paramNamesList = Parameters?
                .OrderBy(x => x.Direction == ParameterDirection.Input ? 0 : 1)
                .Select(p => p.Direction == ParameterDirection.Output ? p.ParameterName + " OUTPUT" : p.ParameterName);

            var paramNames = paramNamesList is not null && paramNamesList.Any()
                ? string.Join(", ", paramNamesList)
                : string.Empty;

            Routine = $"EXEC {ProcedureName} {paramNames}";
        }
    }
}
