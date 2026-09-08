using System;
using System.Collections.Generic;
using System.Threading;
using Core.Contracts.Scene;
using UnityEngine;
using UnityEngine.SceneManagement;
using Core.Services.ServiceLocator;

namespace Features.Scene
{
    public class GameSceneManager : MonoBehaviour, ISceneManager
    {
        private readonly Dictionary<string, SceneLoadOperation> _pending = new();

        private void Awake()
        {
            if (ServiceLocator.TryGet<ISceneManager>(out _))
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject);
            ServiceLocator.Register<ISceneManager>(this);
        }

        public SceneLoadOperation Load(
            string sceneName,
            LoadSceneMode mode,
            CancellationToken ct = default)
        {
            if (_pending.TryGetValue(sceneName, out var existing))
            {
                Debug.LogWarning($"[SceneManager] '{sceneName}' já está sendo carregada.");
                return existing;
            }

            var operation = new SceneLoadOperation(sceneName, mode);
            _pending[sceneName] = operation;

            _ = RunLoadAsync(operation, ct);

            return operation;
        }

        public async System.Threading.Tasks.Task UnloadAsync(
            string sceneName,
            CancellationToken ct = default)
        {
            if (!IsLoaded(sceneName))
            {
                Debug.LogWarning($"[SceneManager] '{sceneName}' não está carregada.");
                return;
            }

            var asyncOp = SceneManager.UnloadSceneAsync(sceneName);

            while (!asyncOp.isDone)
            {
                ct.ThrowIfCancellationRequested();
                await System.Threading.Tasks.Task.Yield();
            }
        }

        public bool IsLoaded(string sceneName)
        {
            var scene = SceneManager.GetSceneByName(sceneName);
            return scene.IsValid() && scene.isLoaded;
        }

        private async System.Threading.Tasks.Task RunLoadAsync(SceneLoadOperation op, CancellationToken ct)
        {
            try
            {
                var asyncOp = SceneManager.LoadSceneAsync(op.SceneName, op.Mode);
                asyncOp.allowSceneActivation = false;

                while (asyncOp.progress < 0.9f)
                {
                    ct.ThrowIfCancellationRequested();
                    op.ReportProgress(asyncOp.progress / 0.9f);
                    await System.Threading.Tasks.Task.Yield();
                }

                op.ReportProgress(1f);
                op.MarkLoaded();

                asyncOp.allowSceneActivation = true;

                while (!asyncOp.isDone)
                {
                    ct.ThrowIfCancellationRequested();
                    await System.Threading.Tasks.Task.Yield();
                }

                op.MarkActivated();
            }
            catch (OperationCanceledException)
            {
                op.MarkCancelled();
                Debug.Log($"[SceneManager] Load de '{op.SceneName}' cancelado.");
            }
            catch (Exception ex)
            {
                op.MarkFailed(ex);
                Debug.LogError($"[SceneManager] Erro ao carregar '{op.SceneName}': {ex.Message}");
            }
            finally
            {
                _pending.Remove(op.SceneName);
            }
        }
    }
}