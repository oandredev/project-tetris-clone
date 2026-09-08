using System;
using System.Threading;

namespace Core.Contracts.Task
{
    public interface ITask
    {
        string TaskName { get; }
        float EstimatedDuration { get; }
        float MinDuration { get; }
        System.Threading.Tasks.Task InitializeAsync(
            IProgress<float> progress,
            CancellationToken cancellationToken);
    }
}