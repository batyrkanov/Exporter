namespace Exporter.Models.Entities
{
    public class SqlQueryParameter
    {
        public int SqlQueryParameterId { get; set; }
        public int SqlQueryId { get; set; }
        public int ParameterId { get; set; }

        public virtual SqlQuery SqlQuery { get; set; }
        public virtual Parameter Parameter { get; set; }
    }
}