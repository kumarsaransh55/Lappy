using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;
using Lappy.Utility;
using Microsoft.AspNetCore.Mvc;

namespace LappyBag.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public OrderController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public IActionResult Index(string status= "all")
        {
            ViewBag.Status = status;
            return View();
        }

        public IActionResult Details(int orderId)
        {
            OrderVM orderDetailsViewModel = new()
            {
                orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderId, includeProperties: "ApplicationUser"),
                orderDetails = _unitOfWork.OrderDetail.GetAll(u => u.OrderHeaderId == orderId, includeProperties: "Product")
            };
            return View(orderDetailsViewModel);
        }

        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            List<OrderHeader> obj;
            switch(status)
            {
                case "pending":
                    obj = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus == SD.PaymentStatusDelayedPayment, includeProperties: "ApplicationUser").ToList();
                    break;
                case "inprocess":
                    obj = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus == SD.StatusInProcess, includeProperties: "ApplicationUser").ToList();
                    break;
                case "completed":
                    obj = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus == SD.StatusShipped, includeProperties: "ApplicationUser").ToList();
                    break;
                case "approved":
                    obj = _unitOfWork.OrderHeader.GetAll(u => u.OrderStatus == SD.StatusApproved, includeProperties: "ApplicationUser").ToList();
                    break;
                default:
                    obj = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser").ToList();
                    break;
            }
            return Json(new { data = obj });
        }
        #endregion
    }
}