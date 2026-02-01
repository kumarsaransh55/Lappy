using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;
using Lappy.Utility;
using LappyBag.Areas.Customer.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;
using System.Security.Claims;

namespace LappyBag.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        [BindProperty]
        public OrderVM OrderVM { get; set; }

        public OrderController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        public IActionResult Index(string status = "all")
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

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult UpdateOrderDetail()
        {
            int orderheaderId = OrderVM.orderHeader.Id;
            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderheaderId);
            orderHeaderFromDb.Name = OrderVM.orderHeader.Name;
            orderHeaderFromDb.PhoneNumber = OrderVM.orderHeader.PhoneNumber;
            orderHeaderFromDb.StreetAddress = OrderVM.orderHeader.StreetAddress;
            orderHeaderFromDb.City = OrderVM.orderHeader.City;
            orderHeaderFromDb.State = OrderVM.orderHeader.State;
            orderHeaderFromDb.PinCode = OrderVM.orderHeader.PinCode;
            if (OrderVM.orderHeader.Carrier != null)
            {
                orderHeaderFromDb.Carrier = OrderVM.orderHeader.Carrier;
            }
            if (OrderVM.orderHeader.TrackingNumber != null)
            {
                orderHeaderFromDb.TrackingNumber = OrderVM.orderHeader.TrackingNumber;
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["successmsg"] = "Order Details Updated Successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderheaderId });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult StartProcessing()
        {
            int orderheaderId = OrderVM.orderHeader.Id;
            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderheaderId);
            orderHeaderFromDb.OrderStatus = SD.StatusInProcess;
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["successmsg"] = "Order Status Updated to Processing";
            return RedirectToAction("Details", "Order", new { orderId = orderheaderId });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult ShipOrder()
        {
            int orderheaderId = OrderVM.orderHeader.Id;
            OrderHeader orderHeaderFromDb = _unitOfWork.OrderHeader.Get(u => u.Id == orderheaderId);
            orderHeaderFromDb.Carrier = OrderVM.orderHeader.Carrier;
            orderHeaderFromDb.TrackingNumber = OrderVM.orderHeader.TrackingNumber;
            orderHeaderFromDb.OrderStatus = SD.StatusShipped;
            orderHeaderFromDb.ShippingDate = DateTime.Now;
            if (orderHeaderFromDb.PaymentStatus == SD.PaymentStatusDelayedPayment)
            {
                orderHeaderFromDb.PaymentDueDate = DateOnly.FromDateTime(DateTime.Now.AddDays(30));
            }
            _unitOfWork.OrderHeader.Update(orderHeaderFromDb);
            _unitOfWork.Save();
            TempData["successmsg"] = "Order Shipped Successfully";
            return RedirectToAction("Details", "Order", new { orderId = orderheaderId });
        }


        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        public IActionResult CancelOrder()
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == OrderVM.orderHeader.Id);

            if (orderHeader.PaymentStatus == SD.PaymentStatusApproved)
            {
                string keyId = _configuration["Razorpay:KeyId"];
                string keySecret = _configuration["Razorpay:KeySecret"];

                RazorpayClient client = new RazorpayClient(keyId, keySecret);

                string paymentId = orderHeader.TransactionId;

                Dictionary<string, object> refundAttributes = new Dictionary<string, object>();
                refundAttributes.Add("amount", Convert.ToInt32(orderHeader.OrderTotal)); // paise
                refundAttributes.Add("speed", "normal");


                Refund refund = client.Payment.Fetch(paymentId).Refund(refundAttributes);

                orderHeader.PaymentStatus = SD.StatusRefunded;
                orderHeader.OrderStatus = SD.StatusCancelled;
            }
            else
            {
                orderHeader.OrderStatus = SD.StatusCancelled;
                orderHeader.PaymentStatus = SD.StatusCancelled;
            }
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.Save();
            TempData["successmsg"] = "Order Cancelled Successfully";
            return RedirectToAction("Details", "Order", new { orderId = OrderVM.orderHeader.Id });
        }

        [HttpPost]
        [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
        [HttpPost]
        public ActionResult DetailsPayNow()
        {
            RazorpayClient client = new RazorpayClient(_configuration["Razorpay:KeyId"], _configuration["Razorpay:KeySecret"]);
            var orderHeaderfromDB = _unitOfWork.OrderHeader.Get(u => u.ApplicationUserId == OrderVM.orderHeader.ApplicationUserId);
            Razorpay.Api.Order order = client.Order.Create(new Dictionary<string, object>
                {
                    {"amount", OrderVM.orderHeader.OrderTotal}, // amount in the smallest currency unit
                    {"currency", "INR"},
                    {"receipt", OrderVM.orderHeader.Id.ToString()},
                    {"payment_capture", 1}
                });
            orderHeaderfromDB.PaymentIntentId = order["id"].ToString();
            orderHeaderfromDB.PaymentDate = DateTime.Now;
            _unitOfWork.OrderHeader.Update(orderHeaderfromDB);
            _unitOfWork.Save();

            return RedirectToAction("Razorpay", "Cart",new {area ="Customer", id = orderHeaderfromDB.Id });
        }


        #region API CALLS
        [HttpGet]
        public IActionResult GetAll(string status)
        {
            IEnumerable<OrderHeader> orderHeaders;

            // 1. Get the Current User's Role and ID
            if (User.IsInRole(SD.Role_Admin) || User.IsInRole(SD.Role_Employee))
            {
                // ADMIN: Get EVERYTHING from the database
                orderHeaders = _unitOfWork.OrderHeader.GetAll(includeProperties: "ApplicationUser");
            }
            else
            {
                // CUSTOMER: Get ONLY their own orders
                var claimsIdentity = (ClaimsIdentity)User.Identity;
                var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

                // Filter by ApplicationUserId
                orderHeaders = _unitOfWork.OrderHeader.GetAll(u => u.ApplicationUserId == userId, includeProperties: "ApplicationUser");
            }

            // 2. Apply the Status Filter (Pending, Approved, etc.)
            switch (status)
            {
                case "pending":
                    orderHeaders = orderHeaders.Where(u => u.PaymentStatus == SD.PaymentStatusDelayedPayment);
                    break;
                case "inprocess":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusInProcess);
                    break;
                case "completed":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusShipped);
                    break;
                case "approved":
                    orderHeaders = orderHeaders.Where(u => u.OrderStatus == SD.StatusApproved);
                    break;
                default:
                    // "all" - do nothing, keep list as is
                    break;
            }

            // 3. Return JSON for the DataTable
            return Json(new { data = orderHeaders });
        }
        #endregion
    }
}