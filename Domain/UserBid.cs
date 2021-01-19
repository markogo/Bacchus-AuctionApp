using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain
{
    public class UserBid
    {
        public int Id { get; set; }
        
        [DisplayName("Bid amount(€)")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }
        
        public string ProductId { get; set; }
        public Product Product { get; set; }
    }
}