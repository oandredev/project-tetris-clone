using UnityEngine;
using TMPro;
using Features.Gameplay.GameLoop;

namespace Features.UI
{
    public class GameplayScoreUI : MonoBehaviour
    {
        [SerializeField] private GameLoopController gameLoopRef;
        [SerializeField] private TMP_Text scoreText;
        [SerializeField] private TMP_Text linesText;
        [SerializeField] private float countSpeedScore = 1000;
        [SerializeField] private float countSpeedLines = 1;

        private float displayedScore;
        private float displayedLine;

        private bool _correctionScore = true;
        private bool _correctionLine = true;
        //---------------------------------------------------------------------------

        private void Update()
        {
            UpdatePoints();
            UpdateLines();
        }

        private void UpdatePoints()
        {
            int target = gameLoopRef.CurrentScore;

            if (target == 0 && _correctionScore == true)
            {
                displayedScore = 0;
                scoreText.text = "POINTS:\n0";
            }
            else
            {
                _correctionScore = false;
            }

            displayedScore = Mathf.MoveTowards(displayedScore, target, countSpeedScore * Time.deltaTime);
            scoreText.text = $"POINTS:\n{Mathf.RoundToInt(displayedScore).ToString()}";
        }

        private void UpdateLines()
        {
            int target = gameLoopRef.CurrentLine;

            if (target == 0 && _correctionLine == true)
            {
                displayedLine = 0;
                linesText.text = "LINES:\n0";
            }
            else
            {
                _correctionLine = false;
            }

            displayedLine = Mathf.MoveTowards(displayedLine, target, countSpeedLines * Time.deltaTime);
            linesText.text = $"LINES:\n{Mathf.RoundToInt(displayedLine).ToString()}";
        }
    }
}