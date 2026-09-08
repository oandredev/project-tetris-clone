using System;
using System.Threading;
using Core.Contracts.Task;
using UnityEngine;

namespace Features.Shader
{
    public class ShaderWarmupTask : MonoBehaviour, ITask
    {
        public string TaskName => "Warmup | Shaders";
        public float EstimatedDuration => 2f;
        public float MinDuration => 1f;

        [SerializeField] private ShaderVariantCollection[] collections;

        public async System.Threading.Tasks.Task InitializeAsync(
            IProgress<float> progress,
            CancellationToken ct)
        {
            for (int i = 0; i < collections.Length; i++)
            {
                ct.ThrowIfCancellationRequested();

                collections[i].WarmUp();

                progress?.Report((float)(i + 1) / collections.Length);

                await System.Threading.Tasks.Task.Yield();
            }
        }
    }
}