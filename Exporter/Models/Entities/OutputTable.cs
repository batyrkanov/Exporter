using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Exporter.Models.Entities
{
    [Table("OutputTables")]
    public class OutputTable
    {
        public int OutputTableId { get; set; }        
        public string Name { get; set; }
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int SqlQueryId { get; set; }

        public virtual SqlQuery SqlQuery { get; set; }
    }
}