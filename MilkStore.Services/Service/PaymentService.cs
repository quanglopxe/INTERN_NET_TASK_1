using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text;
using MilkStore.Contract.Repositories.Interface;
using MilkStore.Core.Base;
using MilkStore.Core.Constants;
using Newtonsoft.Json;

public class PaymentService
{
    private readonly IUnitOfWork unitOfWork;
    public PaymentService(IUnitOfWork unitOfWork)
    {
        this.unitOfWork = unitOfWork;
    }

    public async Task<string> PaymentMoMoURL(string orderId, decimal amount)
    {

        string partnerCode = "MOMOBKUN20180529";
        string accessKey = "klm05TvNBzhg7h7j";
        string endpoint = "https://test-payment.momo.vn/v2/gateway/api/create";
        string redirectUrl = Environment.GetEnvironmentVariable("CLIENT_DOMAIN") ?? throw new Exception("CLIENT_DOMAIN is not set");
        string ipnUrl = $"{Environment.GetEnvironmentVariable("SERVER_DOMAIN") ?? throw new Exception("SERVER_DOMAIN is not set")}/api/payment/MomoIPN";
        string? requestId = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString();
        string? orderInfo = "Thanh toán qua MoMo";
        string? requestType = "payWithATM";
        string? extraData = "";

        string secretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";

        // tạo chữ ký
        string? rawHash = $"accessKey={accessKey}&amount={amount}&extraData={extraData}&ipnUrl={ipnUrl}&orderId={orderId}&orderInfo={orderInfo}&partnerCode={partnerCode}&redirectUrl={redirectUrl}&requestId={requestId}&requestType={requestType}";
        string? signature = CreateSignature(rawHash, secretKey);

        PaymentMoMoModel? paymentRequest = new PaymentMoMoModel
        {
            PartnerCode = partnerCode,
            PartnerName = "Test",
            StoreId = "MomoTestStore",
            RequestId = requestId,
            Amount = amount,
            OrderId = orderId,
            OrderInfo = orderInfo,
            RedirectUrl = redirectUrl,
            IpnUrl = ipnUrl,
            Lang = "vi",
            ExtraData = extraData,
            RequestType = requestType,
            Signature = signature
        };

        using HttpClient? client = new HttpClient();
        HttpResponseMessage? response = await client.PostAsJsonAsync(endpoint, paymentRequest);
        string? result = await response.Content.ReadAsStringAsync();
        dynamic jsonResult = JsonConvert.DeserializeObject(result);

        // Trả về URL thanh toán từ MoMo
        if (jsonResult != null && jsonResult.payUrl != null)
        {
            return jsonResult.payUrl.toString();
        }

        throw new BaseException.ErrorException(StatusCodes.BadRequest, "BadRequest", "Error Payment");

    }
    private string CreateSignature(string rawData, string secretKey)
    {
        using HMACSHA256? hmacsha256 = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        byte[]? hash = hmacsha256.ComputeHash(Encoding.UTF8.GetBytes(rawData));
        return BitConverter.ToString(hash).Replace("-", "").ToLower();
    }

    public async Task ReceiveMoMoIPN(MoMoIPNRequest request)
    {
        string partnerCode = "MOMOBKUN20180529";
        string accessKey = "klm05TvNBzhg7h7j";
        string secretKey = "at67qH6mk8w5Y1nAyMoYKMWACiEi2bsa";
        string? rawHash = $"accessKey={accessKey}&amount={request.Amount}&extraData={request.ExtraData}&message={request.Message}&orderId={request.OrderId}&orderInfo={request.OrderInfo}&partnerCode={partnerCode}&payType={request.PayType}&requestId={request.RequestId}&responseTime={request.ResponseTime}&resultCode={request.ResultCode}&transId={request.TransId}";
        string? signature = CreateSignature(rawHash, secretKey);
        if (signature != request.Signature)
        {
            throw new BaseException.ErrorException(StatusCodes.BadRequest, "BadRequest", "Invalid signature");

        }
        // xử lý nghiệp vụ tại đây

        await unitOfWork.SaveAsync();

    }
}