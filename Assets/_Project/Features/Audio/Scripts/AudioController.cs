using UnityEngine;
using Features.Gameplay.GameLoop;
using Features.Data;

namespace Features.Audio
{
    public class AudioController : MonoBehaviour
    {
        // --- References ---
        [SerializeField] private GameLoopController gameLoopRef;
        [SerializeField] private GridManager gridManagerRef;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        // --- Audio Clips ---
        [SerializeField] private AudioClip lineClearClip;
        [SerializeField] private AudioClip hardDropClip;
        [SerializeField] private AudioClip pieceLockedClip;
        [SerializeField] private AudioClip gameOverClip;

        [SerializeField] private AudioClip moveClip;
        [SerializeField] private AudioClip moveFailedClip;


        // --- Music Pitch ---
        [SerializeField] private float minPitch = 1f;
        [SerializeField] private float maxPitch = 2f;

        private const float MinGameSpeed = 1f;
        private const float MaxGameSpeed = 10f;

        private void OnEnable()
        {
            AudioListener.volume = SettingsService.GetAudioEnabled() ? 1f : 0f;

            gameLoopRef.OnLinesCleared += HandleLinesCleared;
            gameLoopRef.OnHardDropStart += HandleHardDropStart;
            gameLoopRef.OnGameOverEffectStarted += HandleGameOver;
            gameLoopRef.OnPiecePut += HandlePiecePut;
            gridManagerRef.OnMove += HandleMove;
            gridManagerRef.OnMoveFailed += HandleMoveFailed;
        }

        private void OnDisable()
        {
            gameLoopRef.OnLinesCleared -= HandleLinesCleared;
            gameLoopRef.OnHardDropStart -= HandleHardDropStart;
            gameLoopRef.OnGameOverEffectStarted -= HandleGameOver;
            gameLoopRef.OnPiecePut -= HandlePiecePut;
            gridManagerRef.OnMove -= HandleMove;
            gridManagerRef.OnMoveFailed -= HandleMoveFailed;

        }

        private void Update()
        {
            UpdateMusicPitch();
        }

        private void UpdateMusicPitch()
        {
            float normalizedSpeed = Mathf.InverseLerp(
                MinGameSpeed,
                MaxGameSpeed,
                gameLoopRef.CurrentSpeed
            );

            musicSource.pitch = Mathf.Lerp(
                minPitch,
                maxPitch,
                normalizedSpeed
            );
        }

        private void HandleLinesCleared(int linesCleared)
        {
            PlaySfx(lineClearClip);
        }

        private void HandleHardDropStart()
        {
            PlaySfx(hardDropClip);
        }

        private void HandlePiecePut()
        {
            PlaySfx(pieceLockedClip, 2.5f, 0.1f);
        }

        private void HandleGameOver()
        {
            musicSource.volume = 0f;

            PlaySfx(gameOverClip);

            Invoke(nameof(RestoreMusicVolume), 1.5f);
        }

        private void HandleMove()
        {
            PlaySfx(moveClip, 1);
        }

        private void HandleMoveFailed()
        {
            PlaySfx(moveFailedClip, 1);
        }


        private void PlaySfx(AudioClip clip, float pitch = 1f, float volume = 0.25f)
        {
            if (clip == null)
                return;

            sfxSource.pitch = pitch;
            sfxSource.volume = volume;
            sfxSource.PlayOneShot(clip);
        }

        private void RestoreMusicVolume()
        {
            musicSource.volume = 0.5f;
        }
    }
}