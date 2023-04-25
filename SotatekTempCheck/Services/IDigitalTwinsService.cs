using SotatekTempCheck.Models;

namespace SotatekTempCheck.Services
{
    public interface IDigitalTwinsService
    {
        Task<IEnumerable<dynamic>> GetListTwinsAsync();
        Task<TwinsModel> GetTempByTwinIdAsync(string twinId);
    }
}
