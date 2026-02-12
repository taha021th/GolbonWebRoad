using System.ComponentModel.DataAnnotations.Schema;

namespace GolbonWebRoad.Domain.Entities
{
    public class Review
    {
        public int Id { get; set; }
        public string ReviewText { get; set; }
        public int Rating { get; set; }
        public DateTime ReviewDate { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }

        public bool Status { get; set; } = false;
        public bool IsShowHomePage { get; set; }
    }
}
