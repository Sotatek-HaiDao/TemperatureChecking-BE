using SotatekTempCheck.Models;

namespace SotatekTempCheck.Services
{
    public interface IDigitalTwinsService
    {
        Task<IEnumerable<string>> GetListTwinsAsync();
        Task<TwinsModel> GetTempByTwinIdAsync(string twinId);
    }
}
