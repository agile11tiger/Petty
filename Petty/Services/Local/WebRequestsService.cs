using Petty.Extensions;

namespace Petty.Services.Local
{
    /// <summary>
    /// https://learn.microsoft.com/en-us/dotnet/api/system.net.http.httpclient?view=net-5.0
    /// </summary>
    public class WebRequestsService : Service
    {
        private readonly HttpClient _client = new();

        public async Task DownloadAsync(
            Uri uri,
            string pathToSave,
            Action<double> updateProgressPercentages = default,
            CancellationToken cancellationToken = default)
        {
            try
            {
                using var response = await _client.GetAsync(uri, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    using var contentStream = await response.Content.ReadAsStreamAsync(cancellationToken);
                    using var archiveStream = new FileStream(pathToSave, FileMode.CreateNew);
                    await contentStream.CopyToAsync(archiveStream, response.Content.Headers.ContentLength.Value, updateProgressPercentages, cancellationToken: cancellationToken);
                }
                else
                    throw new FileLoadException();
            }
            catch (Exception ex)
            {
                _loggerService.Log(ex);
                throw;
            }
        }
    }
}
