using CarRental_BE.Models.Common;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace CarRental_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VnPayController : ControllerBase
    {
        private readonly VnPayConfig _config;

        public VnPayController(IOptions<VnPayConfig> config)
        {
            _config = config.Value;
        }

        [HttpGet("createpayment")]
        public IActionResult CreatePayment()
        {
            try
            {
                string amount = "100000"; // VND
                string txnRef = DateTime.Now.Ticks.ToString();
                string orderInfo = "Thanh toan don hang test";
                string vnp_Version = "2.1.0";
                string vnp_Command = "pay";
                string vnp_OrderType = "other";
                string locale = "vn";
                string currCode = "VND";

                var vnp_Params = new SortedDictionary<string, string>
        {
            { "vnp_Version", vnp_Version },
            { "vnp_Command", vnp_Command },
            { "vnp_TmnCode", _config.TmnCode },
            { "vnp_Amount", (int.Parse(amount) * 100).ToString() }, // Chuyển sang đơn vị nhỏ nhất (cent)
            { "vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss") },
            { "vnp_CurrCode", currCode },
            { "vnp_IpAddr", HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1" },
            { "vnp_Locale", locale },
            { "vnp_OrderInfo", orderInfo },
            { "vnp_OrderType", vnp_OrderType },
            { "vnp_ReturnUrl", _config.ReturnUrl },
            { "vnp_TxnRef", txnRef }
            // Loại bỏ vnp_SecureHashType khỏi signData
        };

                // Tạo chuỗi signData (không mã hóa URL)
                string signData = string.Join("&", vnp_Params.Select(kv => $"{kv.Key}={kv.Value}"));
                Console.WriteLine("signData: " + signData); // Debug

                // Tạo chữ ký bảo mật
                string secureHash = CreateHmacSha256(_config.HashSecret, signData);
                Console.WriteLine("secureHash: " + secureHash); // Debug

                // Tạo chuỗi queryString (có mã hóa URL)
                string queryString = string.Join("&", vnp_Params.Select(kv => $"{kv.Key}={WebUtility.UrlEncode(kv.Value)}"));
                queryString += $"&vnp_SecureHash={secureHash}";

                string paymentUrl = $"{_config.Url}?{queryString}";

                return Ok(new
                {
                    code = "00",
                    url = paymentUrl,
                    message = "Tạo URL thanh toán thành công"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    code = "99",
                    url = "",
                    message = $"Lỗi: {ex.Message}"
                });
            }
        }
        [HttpGet("return")]
        public IActionResult Return()
        {
            try
            {
                // Lấy các tham số trả về từ VNPay
                var queryString = Request.QueryString.Value;
                if (string.IsNullOrEmpty(queryString))
                {
                    return BadRequest("Không nhận được dữ liệu từ VNPay.");
                }

                var response = HttpUtility.ParseQueryString(queryString);
                var vnp_Params = new SortedDictionary<string, string>();
                foreach (string key in response.AllKeys)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        vnp_Params.Add(key, response[key]);
                    }
                }

                // Lấy các tham số cần thiết
                string vnp_SecureHash = vnp_Params["vnp_SecureHash"];
                vnp_Params.Remove("vnp_SecureHash"); // Loại bỏ vnp_SecureHash khỏi danh sách để tính lại chữ ký
                string signData = string.Join("&", vnp_Params.Select(kv => $"{kv.Key}={kv.Value}"));

                // Tính lại chữ ký để xác thực
                string secureHash = CreateHmacSha256(_config.HashSecret, signData);

                if (secureHash.Equals(vnp_SecureHash, StringComparison.OrdinalIgnoreCase))
                {
                    // Chữ ký hợp lệ, xử lý kết quả giao dịch
                    string vnp_ResponseCode = vnp_Params["vnp_ResponseCode"];
                    string vnp_TxnRef = vnp_Params["vnp_TxnRef"];
                    string vnp_Amount = vnp_Params["vnp_Amount"];
                    string vnp_TransactionStatus = vnp_Params["vnp_TransactionStatus"];

                    if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                    {
                        // Giao dịch thành công
                        return Ok(new
                        {
                            code = "00",
                            message = "Thanh toán thành công",
                            transactionId = vnp_TxnRef,
                            amount = vnp_Amount
                        });
                    }
                    else
                    {
                        // Giao dịch thất bại
                        return Ok(new
                        {
                            code = "99",
                            message = "Thanh toán thất bại",
                            responseCode = vnp_ResponseCode,
                            transactionId = vnp_TxnRef
                        });
                    }
                }
                else
                {
                    // Chữ ký không hợp lệ
                    return BadRequest("Chữ ký không hợp lệ.");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { code = "99", message = $"Lỗi hệ thống: {ex.Message}" });
            }
        }
        private string CreateHmacSha256(string key, string inputData)
        {
            var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            byte[] hashValue = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hashValue).Replace("-", "").ToLower();
        }
    }
}