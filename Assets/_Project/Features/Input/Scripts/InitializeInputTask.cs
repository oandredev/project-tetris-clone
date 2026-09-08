using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using Core.Contracts.Input;
using Core.Contracts.Task;
using Core.Services.ServiceLocator;

namespace Features.Input
{
    public sealed class InitializeInputTaskConfig : MonoBehaviour, ITask
    {
        [SerializeField] private InputActionAsset inputActions;

        public string TaskName => "Input System";
        public float EstimatedDuration => 0f;
        public float MinDuration => 0f;

        public Task InitializeAsync(IProgress<float> progress, CancellationToken cancellationToken)
        {
            var go = new GameObject("[InputManager]");
            DontDestroyOnLoad(go);

            var manager = go.AddComponent<InputManager>();
            manager.Initialize(inputActions);

            ServiceLocator.Register<IInputService>(manager);

            progress?.Report(1f);
            return Task.CompletedTask;
        }
    }
}