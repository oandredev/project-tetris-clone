using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using Core.Contracts.Task;
using UnityEngine;

namespace Core.Runtime.Task
{
    public sealed class TaskRunner
    {
        private readonly List<TaskEntry> _entries;
        private readonly float _totalWeight;

        private float _globalProgress;
        private readonly Stopwatch _totalTimer = new();

        public IReadOnlyList<TaskEntry> Entries => _entries;
        public TaskEntry CurrentEntry { get; private set; }
        public float GlobalProgress => _globalProgress;
        public TimeSpan TotalDuration => _totalTimer.Elapsed;

        public event Action<float> OnGlobalProgressChanged;
        public event Action<TaskEntry> OnTaskStarted;
        public event Action<TaskEntry, float> OnTaskProgressChanged;
        public event Action<TaskEntry> OnTaskCompleted;
        public event Action OnAllCompleted;

        public TaskRunner(IEnumerable<ITask> tasks)
        {
            _entries = tasks.Select(t => new TaskEntry(t)).ToList();
            _totalWeight = _entries.Sum(e => e.Task.EstimatedDuration);

            // Fallback
            if (_totalWeight <= 0f)
                _totalWeight = _entries.Count;
        }

        public async System.Threading.Tasks.Task RunAsync(CancellationToken ct)
        {
            _totalTimer.Restart();

            for (int i = 0; i < _entries.Count; i++)
            {
                ct.ThrowIfCancellationRequested();

                var entry = _entries[i];
                CurrentEntry = entry;
                OnTaskStarted?.Invoke(entry);

                float completedWeight = _entries.Take(i).Sum(e => e.Task.EstimatedDuration);
                float taskWeight = entry.Task.EstimatedDuration > 0
                    ? entry.Task.EstimatedDuration
                    : 1f;

                var taskTimer = Stopwatch.StartNew();

                var progress = new Progress<float>(p =>
                {
                    float global = (completedWeight + p * taskWeight) / _totalWeight;
                    SetGlobalProgress(global);
                    OnTaskProgressChanged?.Invoke(entry, p);
                });

                await entry.Task.InitializeAsync(progress, ct);

                var minDuration = TimeSpan.FromSeconds(entry.Task.MinDuration);
                if (taskTimer.Elapsed < minDuration)
                {
                    var remaining = minDuration - taskTimer.Elapsed;
                    await System.Threading.Tasks.Task.Delay(remaining, ct);
                }

                taskTimer.Stop();
                entry.MarkCompleted(taskTimer.Elapsed);

                OnTaskCompleted?.Invoke(entry);
            }

            _totalTimer.Stop();
            CurrentEntry = null;
            SetGlobalProgress(1f);
            OnAllCompleted?.Invoke();
        }

        private void SetGlobalProgress(float value)
        {
            _globalProgress = Mathf.Clamp01(value);
            OnGlobalProgressChanged?.Invoke(_globalProgress);
        }
    }
}