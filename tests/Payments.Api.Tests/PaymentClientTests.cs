using System.Net;
using Payments.Api.Clients;
using Xunit;

namespace Payments.Api.Tests;

public sealed class PaymentClientTests
{
    [Fact]
    public async Task PaymentClient_SuccessfulResponse_ReturnsSuccessfulResult()
    {
        var handler = new SequenceHttpMessageHandler(HttpStatusCode.OK);
        var client = CreateClient(handler);

        var result = await client.ProcessAsync("pay-123");

        Assert.True(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task PaymentClient_DoesNotRetryBadRequest()
    {
        var handler = new SequenceHttpMessageHandler(HttpStatusCode.BadRequest);
        var client = CreateClient(handler);

        var result = await client.ProcessAsync("pay-123");

        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task PaymentClient_UnauthorisedResponse_ReturnsFailureResult()
    {
        var client = CreateClient(new SequenceHttpMessageHandler(HttpStatusCode.Unauthorized));

        var result = await client.ProcessAsync("pay-123");

        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.Unauthorized, result.StatusCode);
    }

    [Fact]
    public async Task PaymentClient_RequestTimeout_RetriesRequest()
    {
        var handler = new SequenceHttpMessageHandler(HttpStatusCode.RequestTimeout, HttpStatusCode.OK);
        var client = CreateClient(handler);

        var result = await client.ProcessAsync("pay-123");

        Assert.True(result.IsSuccessful);
        Assert.Equal(2, handler.RequestCount);
    }

    [Fact]
    public async Task PaymentClient_TooManyRequests_RetriesRequest()
    {
        var handler = new SequenceHttpMessageHandler(HttpStatusCode.TooManyRequests, HttpStatusCode.OK);
        var client = CreateClient(handler);

        var result = await client.ProcessAsync("pay-123");

        Assert.True(result.IsSuccessful);
        Assert.Equal(2, handler.RequestCount);
    }

    [Fact]
    public async Task PaymentClient_InternalServerError_RetriesRequest()
    {
        var handler = new SequenceHttpMessageHandler(HttpStatusCode.InternalServerError, HttpStatusCode.OK);
        var client = CreateClient(handler);

        var result = await client.ProcessAsync("pay-123");

        Assert.True(result.IsSuccessful);
        Assert.Equal(2, handler.RequestCount);
    }

    [Fact]
    public async Task PaymentClient_ServerErrorAtUpperBoundary_RetriesRequest()
    {
        var handler = new SequenceHttpMessageHandler((HttpStatusCode)599, HttpStatusCode.OK);
        var client = CreateClient(handler);

        var result = await client.ProcessAsync("pay-123");

        Assert.True(result.IsSuccessful);
        Assert.Equal(2, handler.RequestCount);
    }

    [Fact]
    public async Task PaymentClient_RedirectResponse_DoesNotRetryRequest()
    {
        var handler = new SequenceHttpMessageHandler(HttpStatusCode.NotModified);
        var client = CreateClient(handler);

        var result = await client.ProcessAsync("pay-123");

        Assert.False(result.IsSuccessful);
        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task PaymentClient_NetworkFailure_RetriesRequest()
    {
        var handler = new SequenceHttpMessageHandler(new HttpRequestException("Connection reset"), HttpStatusCode.OK);
        var client = CreateClient(handler);

        var result = await client.ProcessAsync("pay-123");

        Assert.True(result.IsSuccessful);
        Assert.Equal(2, handler.RequestCount);
    }

    [Fact]
    public async Task PaymentClient_RepeatedNetworkFailures_StopsAfterMaximumAttempts()
    {
        var handler = new SequenceHttpMessageHandler(new HttpRequestException("Connection reset"));
        var client = CreateClient(handler);

        await Assert.ThrowsAsync<HttpRequestException>(() => client.ProcessAsync("pay-123"));

        Assert.Equal(3, handler.RequestCount);
    }

    [Fact]
    public async Task PaymentClient_CancelledRequest_PropagatesCancellation()
    {
        using var cancellationSource = new CancellationTokenSource();
        cancellationSource.Cancel();
        var handler = new SequenceHttpMessageHandler(HttpStatusCode.OK);
        var client = CreateClient(handler);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => client.ProcessAsync("pay-123", cancellationSource.Token));
    }

    [Fact]
    public async Task PaymentClient_PaymentIdentifier_EscapesRequestPath()
    {
        var handler = new SequenceHttpMessageHandler(HttpStatusCode.OK);
        var client = CreateClient(handler);

        await client.ProcessAsync("pay/123");

        Assert.Equal("/payments/pay%2F123", handler.LastRequestUri?.AbsolutePath);
    }

    private static PaymentClient CreateClient(SequenceHttpMessageHandler handler)
    {
        var httpClient = new HttpClient(handler)
        {
            BaseAddress = new Uri("https://payments.example.test/")
        };

        return new PaymentClient(httpClient);
    }

    private sealed class SequenceHttpMessageHandler(params object[] outcomes) : HttpMessageHandler
    {
        private int requestCount;

        public int RequestCount => requestCount;

        public Uri? LastRequestUri { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            LastRequestUri = request.RequestUri;
            var index = Math.Min(Interlocked.Increment(ref requestCount) - 1, outcomes.Length - 1);
            var outcome = outcomes[index];

            return outcome switch
            {
                HttpStatusCode statusCode => Task.FromResult(new HttpResponseMessage(statusCode)),
                Exception exception => Task.FromException<HttpResponseMessage>(exception),
                _ => throw new InvalidOperationException("Unsupported fake response.")
            };
        }
    }
}
