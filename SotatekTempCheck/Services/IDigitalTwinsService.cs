using SotatekTempCheck.Models;

namespace SotatekTempCheck.Services
{
    public interface IDigitalTwinsService
    {
        Task<IEnumerable<TwinsModel>> GetListTwinsAsync();
        Task<TwinsModel> GetTempByTwinIdAsync(string twinId);
    }
}
