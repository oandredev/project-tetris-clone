using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;
using Core.Contracts.Input;

namespace Features.Input
{
    [System.Serializable]
    internal struct MapDebugState
    {
        public string name;
        public bool enabled;
    }

    public class InputManager : MonoBehaviour, IInputService
    {
        public IEnumerable<ActionMapType> RegisteredMaps => _maps.Keys;

        public bool IsMapEnabled(ActionMapType map)
            => _maps.TryGetValue(map, out var actionMap) && actionMap.enabled;

        private InputActionAsset asset;
        public InputActionAsset Asset => asset;

        private readonly Dictionary<ActionMapType, InputActionMap> _maps = new();
        [SerializeField] private List<MapDebugState> _debugMaps = new();

        public void Initialize(InputActionAsset asset)
        {
            this.asset = asset;

            CreateEventSystem(asset);

            foreach (var map in asset.actionMaps)
            {
                if (System.Enum.TryParse<ActionMapType>(map.name, out var type))
                {
                    _maps[type] = map;
                    Debug.Log($"[InputManager] Map registered: {map.name}");
                }
                else
                {
                    Debug.LogWarning($"[InputManager] Map '{map.name}' has no corresponding entry in the ActionMapType enum — ignored.");
                }
            }

            foreach (var pair in _maps)
                pair.Value.Disable();

            _maps[ActionMapType.UI].Enable();

            RefreshDebug();
        }

        private void CreateEventSystem(InputActionAsset asset)
        {
            if (EventSystem.current != null)
            {
                Debug.LogWarning("[InputManager] EventSystem already exists — destroying.");
                Destroy(EventSystem.current.gameObject);
            }

            var go = new GameObject("[EventSystem]");
            DontDestroyOnLoad(go);
            go.AddComponent<EventSystem>();

            var uiModule = go.AddComponent<InputSystemUIInputModule>();
            uiModule.actionsAsset = asset;
        }

        public void EnableMap(ActionMapType map)
        {
            _maps[map].Enable();
            RefreshDebug();
        }

        public void DisableMap(ActionMapType map)
        {
            _maps[map].Disable();
            RefreshDebug();
        }

        public void SwitchToMap(ActionMapType target)
        {
            foreach (var pair in _maps)
            {
                pair.Value.Disable();
            }

            _maps[target].Enable();

            RefreshDebug();
        }

        private void RefreshDebug()
        {
            _debugMaps.Clear();

            foreach (var pair in _maps)
            {
                _debugMaps.Add(new MapDebugState
                {
                    name = pair.Key.ToString(),
                    enabled = pair.Value.enabled
                });
            }

#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }
    }
}