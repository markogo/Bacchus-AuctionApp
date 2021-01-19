using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.ViewModels
{
    public class BidViewModel
    {
        [Column(TypeName = "decimal(18,2)")]
        [Required(ErrorMessage = "Please enter your bid amount!")]
        [DisplayName("Bid amount(€)")]
        public decimal Amount { get; set; }
        
        [DisplayName("Full name")]
        [Required(ErrorMessage = "Please enter your full name!")]
        [MinLength(1), MaxLength(256)]
        public string FullName { get; set; }

        public string UserId { get; set; }

        public string ProductId { get; set; }
    }
}