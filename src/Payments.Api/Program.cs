using Payments.Api.Clients;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient<PaymentClient>(client =>
{
    client.BaseAddress = new Uri(
        builder.Configuration["PaymentProvider:BaseUrl"] ?? "https://payments.example.test/");
});

var app = builder.Build();

app.MapPost("/payments/{paymentId}", async (
    string paymentId,
    PaymentClient paymentClient,
    CancellationToken cancellationToken) =>
{
    var result = await paymentClient.ProcessAsync(paymentId, cancellationToken);
    return Results.Json(result, statusCode: (int)result.StatusCode);
});

app.MapGet("/health", () => Results.Ok(new { status = "healthy" }));

app.Run();
