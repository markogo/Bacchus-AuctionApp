using System.Collections.Generic;

namespace WebApp.ViewModels
{
    public class HomeViewModel
    {
        public BidViewModel Bid { get; set; }
        public IEnumerable<ProductViewModel> Products { get; set; }
        public IEnumerable<CategoriesViewModel> Categories { get; set;  }
        public bool ShowResetFilterBtn { get; set; }
    }
}