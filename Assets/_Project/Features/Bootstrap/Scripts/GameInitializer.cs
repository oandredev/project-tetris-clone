using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Services.ServiceLocator;
using Core.Contracts.Scene;
using Core.Contracts.Task;
using Core.Runtime.Task;
using System;

namespace Features.Bootstrap
{
    public class GameInitializer : MonoBehaviour
    {
        [SerializeField] private List<MonoBehaviour> predefinedTaskObjects = new();
        //[SerializeField] private BootUI bootUI;

        private CancellationTokenSource _cts;

        private async void Start()
        {
            _cts = new CancellationTokenSource();

            var tasks = predefinedTaskObjects.OfType<ITask>().ToList();
            var runner = new TaskRunner(tasks);

            // bootUI?.Bind(runner);

            try
            {
                await runner.RunAsync(_cts.Token);

            }
            catch (Exception e)
            {
                Debug.LogError("[BOOT FAILED] " + e.ToString());
                return;
            }

            Debug.Log("[Boot] Completed");

            var op = ServiceLocator.Get<ISceneManager>().Load("Gameplay", LoadSceneMode.Single, _cts.Token);
            await op.WaitUntilActivatedAsync();
        }

        private void OnDestroy()
        {
            _cts?.Cancel();
            _cts?.Dispose();
        }
    }
}