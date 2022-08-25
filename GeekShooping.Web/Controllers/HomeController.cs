using GeekShooping.Web.Models;
using GeekShooping.Web.Services.IServices;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GeekShooping.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IProductService _productService;
        private readonly ICartService _cartService;

        public HomeController(ILogger<HomeController> logger, 
            IProductService productService,
            ICartService cartService)
        {
            _logger = logger;
            _productService = productService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var Products = await _productService.FindAllProducts("");
            return View(Products);
        }

        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var Products = await _productService.FindProductById(id, token);
            return View(Products);
        }

        [HttpPost]
        [ActionName("Details")]
        [Authorize]
        public async Task<IActionResult> DetailsPost(ProductViewModel model)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var cartViewModel = new CartViewModel()
            {
                CartHeader = new CartHeaderViewModel
                {
                    UserID = User.Claims.Where(U => U.Type == "sub")?.FirstOrDefault()?.Value
                }
            };

            var cartDetail = new CartDetailViewModel
            {
                //CartHeader = cartViewModel.CartHeader,
                Count = model.Count,
                ProductId = model.Id,
                Product = await _productService.FindProductById(model.Id, token)
            };

            var cartDetails = new List<CartDetailViewModel>();
            cartDetails.Add(cartDetail);

            cartViewModel.CartDetails = cartDetails;

            var response = await _cartService.AddItemToCart(cartViewModel, token);

            if(response != null)
            {
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [Authorize]
        public async Task<IActionResult> Login()
        {
            var token = HttpContext.GetTokenAsync("access_token");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult Logout()
        {
            return SignOut("Cookies", "oidc");
        }
    }
}