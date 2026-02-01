using Lappy.DataAccess.Repository.IRepository;
using Lappy.Models;
using Lappy.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Razorpay.Api;


namespace LappyBag.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;
        public CartController(IUnitOfWork unitOfWork, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }
        // GET: CartController
        
        public ActionResult Index()
        {
            return View(getShoppingCartVM());
        }

        private ShoppingCartVM getShoppingCartVM()
        {
            var claimsIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            ShoppingCartVM shoppingCartVM = new ShoppingCartVM()
            {
                ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId, "Product"),
                OrderHeader = new OrderHeader()
            };
            foreach (var item in shoppingCartVM.ShoppingCartList)
            {
                double perProdPrice = getPriceAsPerQuantity(item) * item.Count ;
                shoppingCartVM.OrderHeader.OrderTotal += perProdPrice;
            }
            return shoppingCartVM;
        }

        private double getPriceAsPerQuantity(ShoppingCart shoppingCart)
        {
            int quantity = shoppingCart.Count;
            if (quantity<10)
            {
                return shoppingCart.Product.ListPrice;
            }
            else if(quantity<25)
            {
                return shoppingCart.Product.ListPrice10;
            }
            else if(quantity<100)
            {
                return shoppingCart.Product.ListPrice25;
            }
            else
            {
                return shoppingCart.Product.ListPrice100;
            }
        }

        [HttpPost]
        public ActionResult updateCart(int cartId, string actionType)
        {
            var claimsIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            
            var cart = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
            
            if (userId != cart.ApplicationUserId)
            {
                return Forbid();
            }
                if (actionType == "plus")
            {
                cart.Count++;
                _unitOfWork.ShoppingCart.Update(cart);
                
            }else if(actionType == "minus")
            {
                if(cart.Count>1)
                {
                    cart.Count--;
                    _unitOfWork.ShoppingCart.Update(cart);
                }
                else
                {
                    _unitOfWork.ShoppingCart.Remove(cart);
                    HttpContext.Session.SetInt32(SD.SessionCart, (int)HttpContext.Session.GetInt32(SD.SessionCart) - 1);
                }
            }
            else
            {
                _unitOfWork.ShoppingCart.Remove(cart);
                HttpContext.Session.SetInt32(SD.SessionCart, (int)HttpContext.Session.GetInt32(SD.SessionCart) - 1);
            }
            _unitOfWork.Save();
            return PartialView("_cartPartialView", getShoppingCartVM());
        }

        public ActionResult orderSummary()
        {
            ShoppingCartVM cart = getShoppingCartVM();
            var userDetails = _unitOfWork.ApplicationUser.Get(u => u.Id == cart.ShoppingCartList.FirstOrDefault().ApplicationUserId);
            cart.OrderHeader.Name = userDetails.Name;
            cart.OrderHeader.PhoneNumber = userDetails.PhoneNumber;
            cart.OrderHeader.StreetAddress = userDetails.StreetAddress;
            cart.OrderHeader.City = userDetails.City;
            cart.OrderHeader.State = userDetails.State;
            cart.OrderHeader.PinCode = userDetails.PinCode;
            return View(cart);
        }

        [HttpPost]
        public ActionResult orderSummary(ShoppingCartVM cart)
        {
            var claimsIdentity = (System.Security.Claims.ClaimsIdentity)User.Identity;
            var userId = claimsIdentity.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier).Value;
            var userDetails = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);
            cart.OrderHeader.ApplicationUserId = userId;
            cart.OrderHeader.OrderDate = System.DateTime.Now;
            cart.ShoppingCartList = _unitOfWork.ShoppingCart.GetAll(s => s.ApplicationUserId == userId, "Product");
            foreach (var item in cart.ShoppingCartList)
            {
                double perProdPrice = getPriceAsPerQuantity(item);
                cart.OrderHeader.OrderTotal += perProdPrice* item.Count;
            }
            if (userDetails.CompanyId.GetValueOrDefault() == 0)//Regular User
            {
                cart.OrderHeader.PaymentStatus = SD.PaymentStatusPending;
                cart.OrderHeader.OrderStatus = SD.StatusPending;
            }
            else
            {
                cart.OrderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
                cart.OrderHeader.OrderStatus = SD.StatusApproved;
            }
            _unitOfWork.OrderHeader.Add(cart.OrderHeader);
            _unitOfWork.Save();
            foreach (var item in cart.ShoppingCartList)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    OrderHeaderId = cart.OrderHeader.Id,
                    Count = item.Count,
                    Price = getPriceAsPerQuantity(item)
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
            }
            _unitOfWork.Save();

            if (userDetails.CompanyId.GetValueOrDefault() == 0)//Regular User
            {
                RazorpayClient client = new RazorpayClient(_configuration["Razorpay:KeyId"], _configuration["Razorpay:KeySecret"]);

                Razorpay.Api.Order order = client.Order.Create(new Dictionary<string, object>
                {
                    {"amount", cart.OrderHeader.OrderTotal}, // amount in the smallest currency unit
                    {"currency", "INR"},
                    {"receipt", cart.OrderHeader.Id.ToString()},
                    {"payment_capture", 1}
                });
                cart.OrderHeader.PaymentIntentId = order["id"].ToString();
                cart.OrderHeader.PaymentDate = DateTime.Now;
                _unitOfWork.OrderHeader.Update(cart.OrderHeader);
                _unitOfWork.Save();

                return RedirectToAction(nameof(Razorpay), new { id = cart.OrderHeader.Id });
            }
            _unitOfWork.ShoppingCart.RemoveRange(_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == userId));
            _unitOfWork.Save();
            return RedirectToAction(nameof(OrderConfirmation), new { id = cart.OrderHeader.Id });
        }
        
        public ActionResult OrderConfirmation(int id)
        {
            HttpContext.Session.SetInt32(SD.SessionCart, 0);
            return View(id);
        }

        public ActionResult Razorpay(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == id,includeProperties: "ApplicationUser");
            ViewBag.KeyId = _configuration["Razorpay:KeyId"];
            ViewBag.OrderId = orderHeader.PaymentIntentId;
            ViewBag.Amount = orderHeader.OrderTotal*100;
            ViewBag.Name = orderHeader.Name;
            ViewBag.Phone = orderHeader.PhoneNumber;
            ViewBag.Email = orderHeader.ApplicationUser.Email;
            return View(orderHeader);
        }

        public ActionResult PaymentSuccess(int orderHeaderId, string payment_id, string order_id, string signature)
        {

            RazorpayClient client = new RazorpayClient(_configuration["Razorpay:KeyId"], _configuration["Razorpay:KeySecret"]);

            Dictionary<string, string> options = new Dictionary<string, string>();
            options.Add("razorpay_order_id", order_id);
            options.Add("razorpay_payment_id", payment_id);
            options.Add("razorpay_signature", signature);

            Utils.verifyPaymentSignature(options);

            var orderHeader = _unitOfWork.OrderHeader.Get(u => u.Id == orderHeaderId, includeProperties: "ApplicationUser");
            orderHeader.PaymentDate = DateTime.Now;
            if (orderHeader.PaymentStatus != SD.PaymentStatusDelayedPayment)
            {
                orderHeader.OrderStatus = SD.StatusApproved;
            }
            orderHeader.PaymentStatus = SD.PaymentStatusApproved;
            orderHeader.TransactionId = payment_id;
            _unitOfWork.OrderHeader.Update(orderHeader);
            _unitOfWork.ShoppingCart.RemoveRange(_unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == orderHeader.ApplicationUserId));
            _unitOfWork.Save();

            return RedirectToAction(nameof(OrderConfirmation), new { id = orderHeaderId });
        }

        public IActionResult ReloadCartIcon()
        {
            return ViewComponent("ShoppingCart");
        }
    }
}
