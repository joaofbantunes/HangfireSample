using System.Threading;
using System.Threading.Tasks;

namespace Contracts
{
    public interface IHandleJob
    {
        Task HandleAsync(string jobId, CancellationToken ct);
    }
}