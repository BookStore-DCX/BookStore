using System.Net;
using System.Text;
using System.Text.Json;
using BookStore.Mvc.Models.Api;
using BookStore.Mvc.Services.Interfaces;

namespace BookStore.Mvc.Services.Implementations;

public class ApiClient : IApiClient
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<ApiResult<T>> GetAsync<T>(string endpoint)
    {
        var client = _httpClientFactory.CreateClient("BookStoreApi");
        var response = await client.GetAsync(endpoint);
        return await ReadResponse<T>(response);
    }

    public async Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload)
    {
        var client = _httpClientFactory.CreateClient("BookStoreApi");
        var response = await client.PostAsync(endpoint, ToJson(payload));
        return await ReadResponse<TResponse>(response);
    }

    public async Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest payload)
    {
        var client = _httpClientFactory.CreateClient("BookStoreApi");
        var response = await client.PutAsync(endpoint, ToJson(payload));
        return await ReadResponse<TResponse>(response);
    }

    public async Task<ApiResult<bool>> DeleteAsync(string endpoint)
    {
        var client = _httpClientFactory.CreateClient("BookStoreApi");
        var response = await client.DeleteAsync(endpoint);

        if (response.StatusCode == HttpStatusCode.NoContent)
        {
            return ApiResult<bool>.Success(true, "Deleted", (int)response.StatusCode);
        }

        var result = await ReadResponse<object>(response);
        return result.IsSuccess
            ? ApiResult<bool>.Success(true, result.Message, result.StatusCode)
            : ApiResult<bool>.Failure(result.Message, result.StatusCode);
    }

    private StringContent ToJson<T>(T payload)
    {
        var json = JsonSerializer.Serialize(payload, _jsonOptions);
        return new StringContent(json, Encoding.UTF8, "application/json");
    }

    private async Task<ApiResult<T>> ReadResponse<T>(HttpResponseMessage response)
    {
        var content = await response.Content.ReadAsStringAsync();
        var statusCode = (int)response.StatusCode;

        if (string.IsNullOrWhiteSpace(content))
        {
            return response.IsSuccessStatusCode
                ? ApiResult<T>.Success(default, string.Empty, statusCode)
                : ApiResult<T>.Failure(response.ReasonPhrase ?? "Request failed.", statusCode);
        }

        try
        {
            var wrapped = JsonSerializer.Deserialize<ApiResponse<T>>(content, _jsonOptions);
            if (wrapped is not null)
            {
                if (response.IsSuccessStatusCode && wrapped.Success)
                {
                    return ApiResult<T>.Success(wrapped.Data, wrapped.Message ?? string.Empty, statusCode);
                }

                var message = wrapped.Message
                    ?? (wrapped.Errors is { Count: > 0 } ? string.Join(" ", wrapped.Errors) : "Request failed.");

                return ApiResult<T>.Failure(message, statusCode);
            }
        }
        catch (JsonException)
        {
        }

        var problemMessage = TryReadProblemDetails(content);
        if (!string.IsNullOrWhiteSpace(problemMessage))
        {
            return ApiResult<T>.Failure(problemMessage, statusCode);
        }

        return response.IsSuccessStatusCode
            ? ApiResult<T>.Success(JsonSerializer.Deserialize<T>(content, _jsonOptions), string.Empty, statusCode)
            : ApiResult<T>.Failure(content, statusCode);
    }

    private static string TryReadProblemDetails(string content)
    {
        try
        {
            using var document = JsonDocument.Parse(content);
            var root = document.RootElement;
            var messages = new List<string>();

            if (root.TryGetProperty("title", out var title) && title.ValueKind == JsonValueKind.String)
            {
                messages.Add(title.GetString()!);
            }

            if (root.TryGetProperty("errors", out var errors) && errors.ValueKind == JsonValueKind.Object)
            {
                foreach (var field in errors.EnumerateObject())
                {
                    if (field.Value.ValueKind != JsonValueKind.Array)
                    {
                        continue;
                    }

                    foreach (var error in field.Value.EnumerateArray())
                    {
                        if (error.ValueKind == JsonValueKind.String)
                        {
                            messages.Add(error.GetString()!);
                        }
                    }
                }
            }

            return string.Join(" ", messages.Distinct());
        }
        catch (JsonException)
        {
            return string.Empty;
        }
    }
}
