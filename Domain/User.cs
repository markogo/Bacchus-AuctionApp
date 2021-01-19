using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Domain
{
    public class User
    {
        public string Id { get; set; }

        [DisplayName("Full name")]
        [Required]
        [MinLength(1), MaxLength(256)]
        public string FullName { get; set; }

        public ICollection<UserBid> UserBids { get; set; }
    }
}