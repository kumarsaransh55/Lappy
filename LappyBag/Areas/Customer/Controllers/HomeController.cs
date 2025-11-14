using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Lappy.DataAccess.Data;
using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;

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
            IEnumerable<Product> allProducts = _unitOfWork.Product.GetAll("Category");
            return View(allProducts);
        }

        public IActionResult Details(int id)
        {
            Product product = _unitOfWork.Product.Get(a => a.Id == id, "Category");
            return View(product);
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
