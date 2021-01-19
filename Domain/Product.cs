using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Domain
{
    public class Product
    {
        public string productId { get; set; }
        
        [DisplayName("Product")]
        public string productName { get; set; }

        public string productDescription { get; set; }

        public string productCategory { get; set; }

        public string biddingEndDate { get; set; }
        
        public ICollection<UserBid> UserBids { get; set; }
    }
}