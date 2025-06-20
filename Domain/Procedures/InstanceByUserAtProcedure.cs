using Domain.Procedures.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Procedures
{
    public class InstanceByUserAtProcedure : BaseProcedureResponse
    {
        [Column("ID")]
        public int Id { get; set; }
        [Column("NAME")]
        public string Name { get; set; } = null!;
        [Column("DESCRIPTION")]
        public string Description { get; set; } = null!;
        [Column("REGISTRATION_DATE")]
        public DateTime RegistrationDate { get; set; }
        [Column("CREATION_AT")]
        public DateTime CreationAt { get; set; }
        [Column("USER_AT")]
        public string UserAt { get; set; } = null!;
        [Column("UPDATED_AT")]
        public DateTime UpdatedAt { get; set; }
        [Column("UPDATE_USER")]
        public string UpdateUser { get; set; } = null!;
    }
}
