using UnityEngine.InputSystem;
namespace Core.Contracts.Input
{
    public interface IInputService
    {
        void EnableMap(ActionMapType map);
        void DisableMap(ActionMapType map);
        void SwitchToMap(ActionMapType map);

        InputActionAsset Asset { get; }
    }
}