using System.Threading.Tasks;

namespace Eindwerk.Components
{
    public interface IQrScanningService
    {
        Task<string> ScanAsync();
    }
}