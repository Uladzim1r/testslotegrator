using Microsoft.Extensions.Logging;
using System.Text;

namespace ApiIntegratorTests.Services;

public sealed class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> _logger;

    public LoggingHandler(ILogger<LoggingHandler> logger)
    {
        _logger = logger;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var requestId = Guid.NewGuid().ToString("N")[..8];
        var requestContent = await ReadContentAsync(request.Content, cancellationToken);

        _logger.LogInformation(
            "[{RequestId}] HTTP Request: {Method} {Uri}\nHeaders: {Headers}\nBody: {Body}",
            requestId,
            request.Method,
            request.RequestUri,
            FormatHeaders(request.Headers),
            requestContent);

        var response = await base.SendAsync(request, cancellationToken);
        var responseContent = await ReadContentAsync(response.Content, cancellationToken);

        _logger.LogInformation(
            "[{RequestId}] HTTP Response: {StatusCode}\nHeaders: {Headers}\nBody: {Body}",
            requestId,
            (int)response.StatusCode,
            FormatHeaders(response.Headers),
            responseContent);

        return response;
    }

    private static string FormatHeaders(System.Net.Http.Headers.HttpHeaders headers)
    {
        if (!headers.Any())
        {
            return "(none)";
        }

        var builder = new StringBuilder();
        foreach (var header in headers)
        {
            _ = builder.AppendLine($"  {header.Key}: {string.Join(", ", header.Value)}");
        }

        return builder.ToString();
    }

    private static async Task<string> ReadContentAsync(HttpContent? content, CancellationToken cancellationToken)
    {
        if (content is null)
        {
            return "(no content)";
        }

        var mediaType = content.Headers.ContentType?.MediaType;
        if (mediaType is not null &&
            (mediaType.Contains("json") || mediaType.Contains("text") || mediaType.Contains("xml")))
        {
            return await content.ReadAsStringAsync(cancellationToken);
        }

        return $"({content.Headers.ContentLength ?? 0} bytes, {mediaType ?? "unknown"} content)";
    }
}
