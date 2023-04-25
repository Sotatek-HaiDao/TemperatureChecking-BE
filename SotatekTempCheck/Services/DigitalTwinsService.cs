using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using System.Linq;
using SotatekTempCheck.Models;

namespace SotatekTempCheck.Services
{
    public class DigitalTwinsService : IDigitalTwinsService
    {
        private static readonly HttpClient singletonHttpClientInstance = new HttpClient();
        private readonly DigitalTwinsClient client;
        private readonly IConfiguration _configuration;
        public DigitalTwinsService(IConfiguration configuration)
        {
            _configuration = configuration;
            var cred = new ManagedIdentityCredential();
            client = new DigitalTwinsClient(
                new Uri(_configuration["DigitalTwinsUrl"]),
                cred,
                new DigitalTwinsClientOptions
                {
                    Transport = new HttpClientTransport(singletonHttpClientInstance)
                });
        }
        public async Task<IEnumerable<string>> GetListTwinsAsync()
        {
            var list = client.QueryAsync<string>("SELECT * FROM digitaltwins");
            var ids = new List<string>();
            if (list is not null)
            {
                IAsyncEnumerator<string> enumerator = list.GetAsyncEnumerator();
                try
                {
                    while (await enumerator.MoveNextAsync())
                    {
                        ids.Add(enumerator.Current);
                    }
                }
                finally
                {
                    await enumerator.DisposeAsync();
                }
            }
            return ids;
        }

        public async Task<TwinsModel> GetTempByTwinIdAsync(string twinId)
        {
            var data = await client.GetDigitalTwinAsync<TwinsModel>(twinId);
            return data.Value;
        }
    }
}
