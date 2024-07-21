namespace InnerApiLib
{
    public interface IInnerApiClient
    {
        Task<BaseResponse<T>> GetAsync<T>(string url, CancellationToken cancellationToken);

        Task<BaseResponse<T>> PostAsync<T>(string url, CancellationToken cancellationToken);

        Task<BaseResponse<T>> PostAsync<T>(string url, HttpContent? content, CancellationToken cancellationToken);

        Task<BaseResponse<T>> PutAsync<T>(string url, CancellationToken cancellationToken);

        Task<BaseResponse<T>> PutAsync<T>(string url, HttpContent? content, CancellationToken cancellationToken);

        Task<BaseResponse<T>> DeleteAsync<T>(string url, CancellationToken cancellationToken);
    }
}
