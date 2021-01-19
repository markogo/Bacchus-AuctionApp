using System.Collections.Generic;

namespace WebApp.ViewModels
{
    public class CategoriesViewModel
    {
        public string CategoryName { get; set; }

        public IList<string> Products { get; set; }
    }
}