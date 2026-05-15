using BookStore.Mvc.Models.Api;

namespace BookStore.Mvc.Services.Interfaces;

public interface IApiClient
{
    Task<ApiResult<T>> GetAsync<T>(string endpoint);
    Task<ApiResult<TResponse>> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload);
    Task<ApiResult<TResponse>> PutAsync<TRequest, TResponse>(string endpoint, TRequest payload);
    Task<ApiResult<bool>> DeleteAsync(string endpoint);
}
