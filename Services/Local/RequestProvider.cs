namespace Petty.Services.Local
{
    public class RequestProvider
    {
        public Task<TResult> GetAsync<TResult>(string uri, string token = "")
        {
            throw new NotImplementedException();
        }

        public Task<TResult> PostAsync<TResult>(string uri, TResult data, string token = "", string header = "")
        {
            throw new NotImplementedException();
        }

        public Task<TResult> PostAsync<TResult>(string uri, string data, string clientId, string clientSecret)
        {
            throw new NotImplementedException();
        }

        public Task<TResult> PutAsync<TResult>(string uri, TResult data, string token = "", string header = "")
        {
            throw new NotImplementedException();
        }

        public Task DeleteAsync(string uri, string token = "")
        {
            throw new NotImplementedException();
        }
    }
}
