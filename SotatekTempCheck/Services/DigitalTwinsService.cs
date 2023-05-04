using Azure.Core.Pipeline;
using Azure.DigitalTwins.Core;
using Azure.Identity;
using System.Linq;
using SotatekTempCheck.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

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
            //var options = new DefaultAzureCredentialOptions()
            //{
            //    ExcludeInteractiveBrowserCredential = true
            //};

            //var cred = new DefaultAzureCredential(options);
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
            var list = client.QueryAsync<object>("SELECT * FROM digitaltwins");
            var ids = new List<TwinsModel>();
            if (list is not null)
            {
                IAsyncEnumerator<object> enumerator = list.GetAsyncEnumerator();
                try
                {
                    while (await enumerator.MoveNextAsync())
                    {
                        var data = (JObject)JsonConvert.DeserializeObject(enumerator.Current.ToString());
                        TwinsModel model = new TwinsModel();
                        model.Id = (string)data["$dtId"];
                        model.ETag = (string)data["$etag"];
                        var lastUpdateTime = (string)data["$metadata"]["$lastUpdateTime"];
                        Metadata metadata = new Metadata();
                        metadata.lastUpdateTime = DateTime.Parse(lastUpdateTime);
                        metadata.Model = (string)data["$metadata"]["$model"];
                        model.Metadata = metadata;
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
            var twin = await client.GetDigitalTwinAsync<object>(twinId);
            var data = (JObject)JsonConvert.DeserializeObject(twin.Value.ToString());
            TwinsModel model = new TwinsModel();
            model.Id = (string)data["$dtId"];
            model.ETag = (string)data["$etag"];
            double temperature = 0;
            double humidity = 0;
            Double.TryParse((string)data["Temperature"],out temperature);
            Double.TryParse((string)data["Humidity"], out humidity);
            model.Temperature = temperature;
            model.Humidity = humidity;
            var lastUpdateTime = (string)data["$metadata"]["$lastUpdateTime"];
            Metadata metadata = new Metadata();
            metadata.lastUpdateTime = DateTime.Parse(lastUpdateTime);
            metadata.Model = (string)data["$metadata"]["$model"];
            model.Metadata = metadata;
            return model;
        }
    }
}
