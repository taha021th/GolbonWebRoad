using System.ComponentModel.DataAnnotations.Schema;

namespace GolbonWebRoad.Domain.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Level { get; set; }

        public DateTimeOffset TimeStamp { get; set; }
        public string? Exception { get; set; }
        [Column(TypeName = "jsonb")]
        public string Properties { get; set; }
    }
}
