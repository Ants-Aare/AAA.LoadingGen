using System.Threading;
using Cysharp.Threading.Tasks;

namespace AAA.LoadingGen.Runtime
{
    public interface IAsyncLoadingStep
    {
        public UniTask LoadAsync(CancellationToken cancellationToken);
    }
}