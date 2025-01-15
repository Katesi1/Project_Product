using MvcLaptop.Models;
using MvcLaptop.Services;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Controllers
{
    public class PayController : Controller
    {
        private readonly IVNPayService _vnPayService;
        public PayController(IVNPayService vnPayService)
        {
            _vnPayService = vnPayService;
        }

        public static Dictionary<string, string> vnp_TransactionStatus = new Dictionary<string, string>()
        {
            {"00","Transaction successful" },
            {"01","Transaction not completed" },
            {"02","Transaction failed" },
            {"04","Transaction is being rolled back" },
            {"05","VNPAY is processing the transaction" },
            {"06","VNPAY has sent a refund request to the bank" },
            {"07","Transaction is suspected of fraud" },
            {"09","Refund failed" }
        };

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Pay()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Pay(Order request)
        {
            if (ModelState.IsValid)
            {
                if (request.PaymentMethod == "VNPay")
                {
                    string cleanedAmount = request.TotalPrice.ToString()!.Replace(".", "");
                    var vnPayModel = new VNPaymentRequestModel
                    {
                        // Fill Amount 
                        TotalPrice = Double.Parse(cleanedAmount),
                        CreatedDate = DateTime.Now,
                        Description = $"{request.FullName} {request.PhoneNumber}",
                        FullName = request.FullName,
                    };
                    return Redirect(_vnPayService.CreatePaymentUrl(HttpContext, vnPayModel));
                }

                // Processed

                return View();
            }
            return View(request);
        }

        public IActionResult PaymentSuccess()
        {
            return View();
        }

        public IActionResult PaymentFail()
        {
            return View();
        }

        public IActionResult PaymentCallBack()
        {
            var response = _vnPayService.PaymentExecute(Request.Query);
            if (response.VNPayResponseCode == "00")
            {
                // Processed successfully
                return RedirectToAction(nameof(PaymentSuccess));
            }

            // Get the message corresponding to VNPayResponseCode from the dictionary
            if (vnp_TransactionStatus.TryGetValue(response.VNPayResponseCode!, out var message))
            {
                TempData["Message"] = $"Payment error: {message}";
            }
            else
            {
                TempData["Message"] = $"Unknown payment error: {response.VNPayResponseCode}";
            }

            return RedirectToAction(nameof(PaymentFail));
        }
    }
}