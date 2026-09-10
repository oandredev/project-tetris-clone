using UnityEngine;
using TMPro;
using Features.Data;
using Features.Gameplay.GameLoop;

namespace Features.UI
{
    public class GameOverUI : MonoBehaviour
    {
        // --- References (Inspector) ---
        [SerializeField] private GameLoopController gameLoopRef;

        [SerializeField] private TMP_Text recordScoreText;
        [SerializeField] private TMP_Text recordLinesText;
        [SerializeField] private TMP_Text totalScoreText;
        [SerializeField] private TMP_Text totalLinesText;

        #region Unity Lifecycle

        private void OnEnable() => gameLoopRef.OnGameOver += HandleGameOver;
        private void OnDisable() => gameLoopRef.OnGameOver -= HandleGameOver;

        #endregion

        #region Game Over Handling

        private void HandleGameOver(int score, int lines)
        {
            recordScoreText.text = $"POINTS:\n {ScoreService.GetHighScore()}";
            recordLinesText.text = $"LINES: \n {ScoreService.GetHighLines()}";

            totalScoreText.text = $"POINTS: \n {ScoreService.GetTotalScore()}";
            totalLinesText.text = $"LINES: \n {ScoreService.GetTotalLines()}";
        }
        #endregion
    }
}