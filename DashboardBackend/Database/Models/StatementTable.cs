using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

#nullable disable

namespace DashboardBackend.Database.Models
{
    [Keyless]
    [Table("STATEMENT_TABLES")]
    public partial class StatementTable
    {
        [Column("MGR")]
        [StringLength(400)]
        public string Mgr { get; set; }
        [Column("IDENTIFIER")]
        [StringLength(400)]
        public string Identifier { get; set; }
        [Column("IDENTIFIER_SHORT")]
        [StringLength(60)]
        public string IdentifierShort { get; set; }
        [Column("SCHEMA_NAME")]
        [StringLength(60)]
        public string SchemaName { get; set; }
        [Column("SCHEMA_NAME_FULL")]
        [StringLength(60)]
        public string SchemaNameFull { get; set; }
        [Column("TABLE_NAME")]
        [StringLength(400)]
        public string TableName { get; set; }
        [Column("CREATED", TypeName = "datetime")]
        public DateTime? Created { get; set; }
    }
}
