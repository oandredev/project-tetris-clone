using System;
using UnityEngine.SceneManagement;
namespace Core.Contracts.Scene
{
    public sealed class SceneLoadOperation
    {
        private readonly System.Threading.Tasks.TaskCompletionSource<bool> _loadedTcs = new();
        private readonly System.Threading.Tasks.TaskCompletionSource<bool> _activatedTcs = new();

        public string SceneName { get; }
        public LoadSceneMode Mode { get; }
        public float Progress { get; private set; }
        public bool IsLoaded { get; private set; }
        public bool IsActivated { get; private set; }

        public event Action<float> OnProgressChanged;
        public event Action OnLoaded;
        public event Action OnActivated;

        public System.Threading.Tasks.Task WaitUntilLoadedAsync() => _loadedTcs.Task;
        public System.Threading.Tasks.Task WaitUntilActivatedAsync() => _activatedTcs.Task;

        public SceneLoadOperation(string sceneName, LoadSceneMode mode)
        {
            SceneName = sceneName;
            Mode = mode;
        }

        public void ReportProgress(float value)
        {
            Progress = value;
            OnProgressChanged?.Invoke(value);
        }
        
        //---------------------------------------------------------------------------
        
        public void MarkLoaded()
        {
            IsLoaded = true;
            OnLoaded?.Invoke();
            _loadedTcs.TrySetResult(true);
        }

        public void MarkActivated()
        {
            IsActivated = true;
            OnActivated?.Invoke();
            _activatedTcs.TrySetResult(true);
        }

        public void MarkCancelled()
        {
            _loadedTcs.TrySetCanceled();
            _activatedTcs.TrySetCanceled();
        }

        public void MarkFailed(Exception ex)
        {
            _loadedTcs.TrySetException(ex);
            _activatedTcs.TrySetException(ex);
        }
    }
}