using System.Net.Http.Json;
using Payments.Api.Models;
using Payments.Api.Resilience;

namespace Payments.Api.Clients;

public sealed class PaymentClient(HttpClient httpClient)
{
    public async Task<PaymentResult> ProcessAsync(
        string paymentId,
        CancellationToken cancellationToken = default)
    {
        for (var attempt = 1; attempt <= PaymentRetryPolicy.MaximumAttempts; attempt++)
        {
            try
            {
                using var response = await httpClient.PostAsJsonAsync(
                    $"payments/{Uri.EscapeDataString(paymentId)}",
                    new { paymentId },
                    cancellationToken);

                if (!PaymentRetryPolicy.ShouldRetry(response.StatusCode)
                    || attempt == PaymentRetryPolicy.MaximumAttempts)
                {
                    return new PaymentResult(response.IsSuccessStatusCode, response.StatusCode);
                }
            }
            catch (HttpRequestException) when (attempt < PaymentRetryPolicy.MaximumAttempts)
            {
            }
        }

        throw new InvalidOperationException("The payment request completed without a result.");
    }
}
