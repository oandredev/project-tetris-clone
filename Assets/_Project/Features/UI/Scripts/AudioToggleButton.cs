using UnityEngine;
using UnityEngine.UI;
using Features.Data;

namespace Features.UI
{
    public class AudioToggleButton : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image icon;
        [SerializeField] private Sprite audioOnSprite;
        [SerializeField] private Sprite audioOffSprite;

        private bool audioEnabled;

        private void Awake()
        {
            audioEnabled = SettingsService.GetAudioEnabled();
            ApplyState();

            button.onClick.AddListener(Toggle);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(Toggle);
        }

        private void Toggle()
        {
            audioEnabled = !audioEnabled;
            ApplyState();
            SettingsService.SetAudioEnabled(audioEnabled);
        }

        private void ApplyState()
        {
            AudioListener.volume = audioEnabled ? 1f : 0f;
            icon.sprite = audioEnabled ? audioOnSprite : audioOffSprite;
        }
    }
}