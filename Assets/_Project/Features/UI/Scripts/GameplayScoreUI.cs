using UnityEngine;
using TMPro;
using Features.Gameplay.GameLoop;

namespace Features.UI
{
    public class GameplayScoreUI : MonoBehaviour
    {
        // --- References (Inspector) ---
        [SerializeField] private GameLoopController gameLoopRef;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text linesText;
        [SerializeField] private float countSpeedScore = 1000;
        [SerializeField] private float countSpeedLines = 1;

        // --- Runtime state (should not appear in the Inspector) ---
        private float displayedScore;
        private float displayedLine;

        #region Unity Lifecycle

        private void Update()
        {
            UpdatePoints();
            UpdateLines();
        }

        #endregion

        #region Score

        private void UpdatePoints()
        {
            int target = gameLoopRef.CurrentScore;

            if (target == 0)
            {
                displayedScore = 0;
                scoreText.text = "POINTS:\n0";
                return;
            }

            displayedScore = Mathf.MoveTowards(displayedScore, target, countSpeedScore * Time.deltaTime);
            scoreText.text = $"POINTS:\n{Mathf.RoundToInt(displayedScore).ToString()}";
        }

        #endregion

        #region Lines

        private void UpdateLines()
        {
            int target = gameLoopRef.CurrentLine;

            if (target == 0)
            {
                displayedLine = 0;
                linesText.text = "LINES:\n0";
                return;
            }

            displayedLine = Mathf.MoveTowards(displayedLine, target, countSpeedLines * Time.deltaTime);
            linesText.text = $"LINES:\n{Mathf.RoundToInt(displayedLine).ToString()}";
        }

        #endregion
    }
}