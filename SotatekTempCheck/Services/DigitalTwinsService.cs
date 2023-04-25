using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using System.Linq;
using SotatekTempCheck.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

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
        public async Task<IEnumerable<TwinsModel>> GetListTwinsAsync()
        {
            var list = client.QueryAsync<dynamic>("SELECT * FROM digitaltwins");
            var ids = new List<TwinsModel>();
            if (list is not null)
            {
                IAsyncEnumerator<dynamic> enumerator = list.GetAsyncEnumerator();
                try
                {
                    while (await enumerator.MoveNextAsync())
                    {
                        string json = JsonConvert.SerializeObject(enumerator.Current);
                        JsonDocument document = JsonDocument.Parse(json);
                        TwinsModel model = new TwinsModel();

                        model.Id = document.RootElement.GetProperty("$dtId").GetString();
                        model.ETag = document.RootElement.GetProperty("$etag").GetString();
                        JsonElement metadataElement = document.RootElement.GetProperty("$metadata");
                        model.Metadata.Model = metadataElement.GetProperty("$model").GetString();
                        model.Metadata.lastUpdateTime = metadataElement.GetProperty("$lastUpdateTime").GetDateTime();

                        ids.Add(model);
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
            var data = await client.GetDigitalTwinAsync<dynamic>(twinId);
            string json = JsonConvert.SerializeObject(data.Value);
            JsonDocument document = JsonDocument.Parse(json);
            TwinsModel model = new TwinsModel();

            model.Id = document.RootElement.GetProperty("$dtId").GetString();
            model.ETag = document.RootElement.GetProperty("$etag").GetString();
            JsonElement metadataElement = document.RootElement.GetProperty("$metadata");
            model.Metadata.Model = metadataElement.GetProperty("$model").GetString();
            model.Metadata.lastUpdateTime = metadataElement.GetProperty("$lastUpdateTime").GetDateTime();
            model.Temperature = document.RootElement.GetProperty("Temperature").GetDouble();
            return model;
        }
    }
}
