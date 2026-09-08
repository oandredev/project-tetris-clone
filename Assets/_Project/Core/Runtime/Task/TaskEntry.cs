using System;
using Core.Contracts.Task;

namespace Core.Runtime.Task
{
    public sealed class TaskEntry
    {
        public ITask Task { get; }
        public TimeSpan ActualDuration { get; private set; }
        public bool IsCompleted { get; private set; }

        public TaskEntry(ITask task) => Task = task;

        public void MarkCompleted(TimeSpan duration)
        {
            ActualDuration = duration;
            IsCompleted = true;
        }
    }
}