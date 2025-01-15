using MvcLaptop.Models;
using Microsoft.Extensions.Options;
using MvcLaptop.Utils;
using MvcLaptop.Utils.ConfigOptions.VNPay;

namespace MvcLaptop.Services
{
    public interface IVNPayService
    {
        string CreatePaymentUrl(HttpContext context, VNPaymentRequestModel model);
        VNPaymentResponseModel PaymentExecute(IQueryCollection collections);
    }
    public class VNPayService : IVNPayService
    {
        private readonly VNPayConfigOptions _options;

        public VNPayService(IOptions<VNPayConfigOptions> options)
        {
            _options = options.Value;
        }

        public string CreatePaymentUrl(HttpContext context, VNPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnPay = new VNPayLibrary();

            vnPay.AddRequestData("vnp_Version", _options.Version!);
            vnPay.AddRequestData("vnp_Command", _options.Command!);
            vnPay.AddRequestData("vnp_TmnCode", _options.TmnCode!);
            vnPay.AddRequestData("vnp_Amount", (model.TotalPrice * 100).ToString());

            vnPay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnPay.AddRequestData("vnp_CurrCode", _options.CurrCode!);
            vnPay.AddRequestData("vnp_IpAddr", UtilityHelper.GetIpAddress(context));
            vnPay.AddRequestData("vnp_Locale", _options.Locale!);

            vnPay.AddRequestData("vnp_OrderInfo", "Thanh toán cho đơn hàng: " + model.Id);
            vnPay.AddRequestData("vnp_OrderType", "other");
            vnPay.AddRequestData("vnp_ReturnUrl", _options.PaymentBackReturnUrl!);

            vnPay.AddRequestData("vnp_TxnRef", tick);

            var paymentUrl = vnPay.CreateRequestUrl(_options.BaseUrl!, _options.HashSecret!);

            return paymentUrl;
        }

        public VNPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnPay = new VNPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnPay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnPay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnPay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnPay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnPay.GetResponseData("vnp_OrderInfo");

            bool checkSignature = vnPay.ValidateSignature(vnp_SecureHash!, _options.HashSecret!);
            if (!checkSignature)
            {
                return new VNPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VNPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "VNPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VNPayResponseCode = vnp_ResponseCode
            };
        }
    }
}