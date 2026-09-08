using System.Threading;
using UnityEngine.SceneManagement;

namespace Core.Contracts.Scene
{
    public interface ISceneManager
    {
        SceneLoadOperation Load(
            string sceneName,
            LoadSceneMode mode,
            CancellationToken ct = default);

        System.Threading.Tasks.Task UnloadAsync(
            string sceneName,
            CancellationToken ct = default);

        bool IsLoaded(string sceneName);
    }
}