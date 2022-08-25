using GeekShooping.Web.Models;
using GeekShooping.Web.Services.IServices;
using GeekShooping.Web.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GeekShooping.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize]
        public async Task<IActionResult> ProductIndex()
        {
            var token = await HttpContext.GetTokenAsync("access_token");
            var Products = await _productService.FindAllProducts(token);

            return View(Products);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProductCreate(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await HttpContext.GetTokenAsync("access_token");

                var response = await _productService.CreateProduct(model, token);
                if (response != null)
                    return RedirectToAction(nameof(ProductIndex));
            }

            return View(model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProductUpdate(ProductViewModel model)
        {
            if (ModelState.IsValid)
            {
                var token = await HttpContext.GetTokenAsync("access_token");

                var response = await _productService.UpdateProduct(model, token);
                if (response != null)
                    return RedirectToAction(nameof(ProductIndex));
            }

            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = Role.Admin)]
        public async Task<IActionResult> ProductDelete(ProductViewModel model)
        {
            var token = await HttpContext.GetTokenAsync("access_token");

            var response = await _productService.DeleteProductById(model.Id, token);
            if (response)
                return RedirectToAction(nameof(ProductIndex));

            return View(model);
        }

        public async Task<IActionResult> ProductCreate()
        {
            return View();
        }

        public async Task<IActionResult> ProductUpdate(int id)
        {

            if(id > 0)
            {
                var token = await HttpContext.GetTokenAsync("access_token");

                var product = await _productService.FindProductById(id, token);

                if (product != null)
                    return View(product);
            }

            return NotFound();

        }

        [Authorize]
        public async Task<IActionResult> ProductDelete(int id)
        {

            if (id > 0)
            {
                var token = await HttpContext.GetTokenAsync("access_token");

                var product = await _productService.FindProductById(id, token);

                if (product != null)
                    return View(product);
            }

            return NotFound();

        }

    }
}
