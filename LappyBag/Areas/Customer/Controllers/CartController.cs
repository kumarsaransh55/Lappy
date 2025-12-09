using Lappy.DataAccess.Repository;
using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LappyBag.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        // GET: CartController
        public ActionResult Index()
        {
            var claimsIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId, "Product"),
                OrderTotal = 0
            };
            foreach(var item in shoppingCartVM.ShoppingCartList)
            {
                double perProdPrice = getPriceAsPerQuantity(item);
                shoppingCartVM.OrderTotal += perProdPrice;
            }
            return View(shoppingCartVM);
        }

        public double getPriceAsPerQuantity(ShoppingCart shoppingCart)
        {
            double price = 0;
            int quantity = shoppingCart.Count;
            if (quantity<10)
            {
                price = shoppingCart.Product.ListPrice * quantity;
            }
            else if(quantity<26)
            {
                price = shoppingCart.Product.ListPrice10 * quantity;
            }
            else if(quantity<51)
            {
                price = shoppingCart.Product.ListPrice25 * quantity;
            }
            else
            {
                price = shoppingCart.Product.ListPrice100 * quantity;
            }
            return price;
        }

        // GET: CartController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CartController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CartController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CartController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CartController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CartController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CartController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
