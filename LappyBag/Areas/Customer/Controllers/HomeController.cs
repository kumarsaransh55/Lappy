using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Lappy.DataAccess.Data;
using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;
using Microsoft.AspNetCore.Authorization;

namespace LappyBag.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> allProducts = _unitOfWork.Product.GetAll(includeProperties:"Category");
            return View(allProducts);
        }

        public IActionResult Details(int id)
        {
            ShoppingCart cart = new ShoppingCart()
            {
                Count = 1,
                ProductId = id,
                Product = _unitOfWork.Product.Get(a => a.Id == id, "Category")
            };

            return View(cart);
        }

        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart cart)
        {
            cart.Id = 0;
            var claimsIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            cart.ApplicationUserId = userId;
            //If Product already exists in cart, then increase the count
            ShoppingCart cartFromDb = _unitOfWork.ShoppingCart.Get(
                a => a.ApplicationUserId == userId && a.ProductId == cart.ProductId);
            if (cartFromDb != null)
            {
                cartFromDb.Count += cart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromDb);
            }
            else
            {
                _unitOfWork.ShoppingCart.Add(cart);
            }
            _unitOfWork.Save();
            TempData["Successmsg"] = "Product added to cart successfully";
            return RedirectToAction(nameof(Index));
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
    }
}
