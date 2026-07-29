using System.Net;

namespace Payments.Api.Models;

public sealed record PaymentResult(bool IsSuccessful, HttpStatusCode StatusCode);
