using System.Net;

namespace Payments.Api.Resilience;

public static class PaymentRetryPolicy
{
    public const int MaximumAttempts = 3;

    public static bool ShouldRetry(HttpStatusCode statusCode)
    {
        var numericStatusCode = (int)statusCode;

        return statusCode is HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests
            || numericStatusCode is >= 500 and <= 599
            || numericStatusCode >= 400;
    }
}
