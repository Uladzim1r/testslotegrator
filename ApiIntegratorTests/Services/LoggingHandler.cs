using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;
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
        try
        {
            var (requestContent, requestReplacedContent) = await ReadAndResetContentAsync(request.Content, cancellationToken);
            if (requestReplacedContent is not null)
            {
                request.Content = requestReplacedContent;
            }

            _logger.LogInformation(
                "[{RequestId}] HTTP Request: {Method} {Uri}\nHeaders: {Headers}\nBody: {Body}",
                requestId,
                request.Method,
                request.RequestUri,
                FormatHeaders(request.Headers),
                requestContent);

            var response = await base.SendAsync(request, cancellationToken);
            var (responseContent, responseReplacedContent) = await ReadAndResetContentAsync(response.Content, cancellationToken);
            if (responseReplacedContent is not null)
            {
                response.Content = responseReplacedContent;
            }

            _logger.LogInformation(
                "[{RequestId}] HTTP Response: {StatusCode}\nHeaders: {Headers}\nBody: {Body}",
                requestId,
                (int)response.StatusCode,
                FormatHeaders(response.Headers),
                responseContent);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "[{RequestId}] LoggingHandler failed for {Method} {Uri}", requestId, request.Method, request.RequestUri);
            throw;
        }
    }

    private static string FormatHeaders(HttpHeaders headers)
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

    private static async Task<(string Content, HttpContent? ResetContent)> ReadAndResetContentAsync(HttpContent? content, CancellationToken cancellationToken)
    {
        if (content is null)
        {
            return ("(no content)", null);
        }

        var mediaType = content.Headers.ContentType?.MediaType;
        var bodyBytes = await content.ReadAsByteArrayAsync(cancellationToken);

        HttpContent resetContent;
        if (mediaType is not null &&
            (mediaType.Contains("json") || mediaType.Contains("text") || mediaType.Contains("xml")))
        {
            var bodyText = Encoding.UTF8.GetString(bodyBytes);
            resetContent = new StringContent(bodyText, Encoding.UTF8, mediaType);
            return (bodyText, resetContent);
        }

        var memoryStream = new MemoryStream(bodyBytes);
        resetContent = new StreamContent(memoryStream);
        CopyContentHeaders(content.Headers, resetContent.Headers);
        return ($"({bodyBytes.Length} bytes, {mediaType ?? "unknown"} content)", resetContent);
    }

    private static void CopyContentHeaders(HttpContentHeaders source, HttpContentHeaders destination)
    {
        foreach (var header in source)
        {
            if (header.Key.Equals("Content-Type", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            destination.TryAddWithoutValidation(header.Key, header.Value);
        }
    }
}
