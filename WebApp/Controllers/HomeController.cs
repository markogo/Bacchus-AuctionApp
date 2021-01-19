using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using DAL.App.EF;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApp.ViewModels;

namespace WebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        
        private readonly AppDbContext _context;

        private const string Baseurl = "http://uptime-auction-api.azurewebsites.net/";

        public HomeController(ILogger<HomeController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SubmitBid(BidViewModel bid)
        {
            var products = new List<Product>();
            var productAvailable = false;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var httpResponseMessage = await client.GetAsync("api/Auction");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var result = httpResponseMessage.Content.ReadAsStringAsync().Result;

                    products = JsonSerializer.Deserialize<List<Product>>(result);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error!");
                }

                foreach (var product in products)
                {
                    if (product.productId == bid.ProductId)
                    {
                        var productFromDb = _context.Products.FirstOrDefault(p => p.productId == product.productId);

                        if (productFromDb == null)
                        {
                            _context.Products.Add(product);
                            await _context.SaveChangesAsync();
                        }
                        productAvailable = true;
                    }
                }
            }
            
            if (ModelState.IsValid && productAvailable)
            {
                var user = new User()
                {
                    Id = $"{bid.FullName}|{DateTime.Now.ToString("dd-MM-yyyy|HH:mm:ss")}",
                    FullName = bid.FullName
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var userBid = new UserBid()
                {
                    Amount = bid.Amount,
                    UserId = user.Id,
                    ProductId = bid.ProductId
                };

                _context.Add(userBid);
                await _context.SaveChangesAsync();
                
                TempData["successful"] = "Bid successful!";
            }
            else
            {
                TempData["unsuccessful"] = "Bid unsuccessful! Product is no longer available!";
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Index(string categoryName)
        {
            var products = new List<Product>();

            var showResetFilterBtn = false;
            
            var categories = new List<string>();
            var categoryViewModels = new List<CategoriesViewModel>();
            
            var productViews = new List<ProductViewModel>();

            if (HttpContext.Request.QueryString.HasValue)
            {
                showResetFilterBtn = true;
            }

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                
                client.DefaultRequestHeaders.Clear();
                
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var httpResponseMessage = await client.GetAsync("api/Auction");

                if (httpResponseMessage.IsSuccessStatusCode)
                {
                    var result = httpResponseMessage.Content.ReadAsStringAsync().Result;

                    products = JsonSerializer.Deserialize<List<Product>>(result);

                    foreach (var product in products)
                    {
                        if (!categories.Contains(product.productCategory))
                        {
                            categories.Add(product.productCategory);
                        }
                    }

                    foreach (var category in categories)
                    {
                        categoryViewModels.Add(new CategoriesViewModel
                        {
                            CategoryName = category,
                            Products = new List<string>()
                        });
                    }
                    
                    foreach (var product in products)
                    {
                        foreach (var categoryViewModel in categoryViewModels.ToList())
                        {
                            if (product.productCategory.Equals(categoryViewModel.CategoryName))
                            {
                                if (!categoryViewModel.Products.Contains(product.productCategory))
                                {
                                    categoryViewModel.Products.Add(product.productName);
                                }
                            }
                        }
                    }
                    
                    if (categoryName != null)
                    {
                        products = products.FindAll(p => p.productCategory.Equals(categoryName));
                    }

                    foreach (var product in products)
                    {
                        DateTime bidEndDate = DateTime.Parse(product.biddingEndDate);
                        
                        var productView = new ProductViewModel()
                        {
                            ProductId = product.productId,
                            ProductName = product.productName,
                            ProductDescription = product.productDescription,
                            ProductCategory = product.productCategory,
                            TimeLeft = bidEndDate.Subtract(DateTime.Now).ToString(@"mm\:ss")
                        };
                        productViews.Add(productView);
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server error!");
                }
            }
            
            return View(new HomeViewModel
            {
                Products = productViews, Categories = categoryViewModels, ShowResetFilterBtn = showResetFilterBtn
            });
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
        }
    }
}