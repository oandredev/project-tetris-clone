using System;
using System.Collections;
using UnityEngine;
using Features.Data;

namespace Features.Gameplay.GameLoop
{
    public class GameLoopController : MonoBehaviour
    {
        private const int PointsPerLine = 1000;

        // --- References (Inspector) ---
        [SerializeField] private GridManager gridManagerRef;
        [SerializeField] private GridRenderer gridRendererRef;
        [SerializeField] private Piece.Piece[] availablePieces;
        [SerializeField] private Animator anim;
        [SerializeField] private float hardDropStepDelay = 0.02f;

        // --- Runtime state (should not appear in the Inspector) ---
        private float speed = 1f;
        private int destroyedLines;
        private int currentScore;
        private float fallTimer;
        private bool isRunning;
        private bool isBusy;          // true durante animação de limpeza de linha — bloqueia TUDO
        private bool isHardDropping;  // true durante a queda rápida — só pausa o Tick automático
        private PieceBag pieceBag;
        private static readonly int[] LineBonusMultiplier = { 0, 1, 3, 5, 8 };

        // --- Events ---
        public event Action<int, int> OnGameOver;
        public event Action<Piece.Piece> OnNextPieceChanged;

        // --- Public read-only properties ---
        public int CurrentScore => currentScore;
        public int CurrentLine => destroyedLines;

        private void Start()
        {
            OnGameOver?.Invoke(0, 0);
        }

        #region Unity Lifecycle

        private void Update()
        {
            if (!isRunning || isBusy) return;

            if (!isHardDropping)
            {
                fallTimer += Time.deltaTime;
                float interval = 1f / speed;

                if (fallTimer >= interval)
                {
                    fallTimer -= interval;
                    Tick();
                }
            }

            HandleInput();
        }

        #endregion

        #region Run Control

        public void StartRun()
        {
            ResetRun();
            anim.SetInteger("Screen", 1);
            isRunning = true;
            isBusy = false;
            SpawnPiece();
        }

        private void ResetRun()
        {
            speed = 1f;
            fallTimer = 0f;
            destroyedLines = 0;
            currentScore = 0;
            isHardDropping = false;
            gridManagerRef.Reset();
            pieceBag = new PieceBag(availablePieces);
        }

        #endregion

        #region Fall Loop

        private void Tick()
        {
            if (gridManagerRef.CanMoveDown())
            {
                gridManagerRef.MoveDown();
                return;
            }

            LockAndAdvance();
        }

        private void LockAndAdvance()
        {
            gridManagerRef.LockPiece();

            int[] completedLines = gridManagerRef.FindCompletedLines();

            if (completedLines.Length > 0)
                StartCoroutine(HandleLineClear(completedLines));
            else
                SpawnPiece();
        }

        private IEnumerator HandleLineClear(int[] rows)
        {
            isBusy = true;

            yield return StartCoroutine(gridRendererRef.PlayLineClearAnimation(rows));

            gridManagerRef.ClearLines(rows);

            AddScore(rows.Length);
            destroyedLines += rows.Length;
            UpdateDelay();

            isBusy = false;
            SpawnPiece();
        }

        private void UpdateDelay()
        {
            speed = 1 + (destroyedLines / 5);
        }

        #endregion

        #region Input

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.A)) gridManagerRef.MoveLeft();
            if (Input.GetKeyDown(KeyCode.D)) gridManagerRef.MoveRight();
            if (Input.GetKeyDown(KeyCode.S)) gridManagerRef.MoveDown();

            if (Input.GetKeyDown(KeyCode.Q)) gridManagerRef.Rotate(-1);
            if (Input.GetKeyDown(KeyCode.E)) gridManagerRef.Rotate(1);

            if (Input.GetKeyDown(KeyCode.Space) && !isHardDropping)
                StartCoroutine(HardDropRoutine());
        }

        private IEnumerator HardDropRoutine()
        {
            isHardDropping = true;
            fallTimer = 0f;

            while (gridManagerRef.CanMoveDown())
            {
                gridManagerRef.MoveDown();
                yield return new WaitForSeconds(hardDropStepDelay);
            }

            isHardDropping = false;
            LockAndAdvance();
        }

        #endregion

        #region Pieces

        private void SpawnPiece()
        {
            Piece.Piece current = pieceBag.Next();

            if (!gridManagerRef.SpawnPiece(current)) // GameOver
            {
                isRunning = false;
                StartCoroutine(HandleGameOver());
                return;
            }

            OnNextPieceChanged?.Invoke(pieceBag.Peek());
        }

        private IEnumerator HandleGameOver()
        {
            yield return StartCoroutine(gridRendererRef.PlayGameOverAnimation());

            anim.SetInteger("Screen", 0);
            ScoreService.SaveRun(currentScore, destroyedLines);
            OnGameOver?.Invoke(currentScore, destroyedLines);
            ResetRun();
        }

        #endregion

        #region Score

        private void AddScore(int linesCleared)
        {
            int multiplier = GetLineBonusMultiplier(linesCleared);
            int points = Mathf.RoundToInt(multiplier * PointsPerLine * speed);
            currentScore += points;
        }

        private int GetLineBonusMultiplier(int linesCleared)
        {
            if (linesCleared >= 1 && linesCleared < LineBonusMultiplier.Length)
                return LineBonusMultiplier[linesCleared];

            return linesCleared;
        }

        #endregion
    }
}