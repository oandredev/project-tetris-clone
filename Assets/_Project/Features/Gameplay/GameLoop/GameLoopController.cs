using System;
using System.Collections;
using UnityEngine;
using Features.Data;

namespace Features.Gameplay.GameLoop
{
    public class GameLoopController : MonoBehaviour
    {
        private const int PointsPerLine = 1000;
        [SerializeField] private float speed = 1f;
        [SerializeField] private int destroyedLines = 0;
        [SerializeField] private int currentScore = 0;

        [SerializeField] private GridManager gridManagerRef;
        [SerializeField] private GridRenderer gridRendererRef;
        [SerializeField] private Piece.Piece[] availablePieces;

        [SerializeField] private Animator anim;
        private float fallTimer;
        private bool isRunning;
        private bool isBusy;
        private PieceBag pieceBag;

        public event Action<int, int> OnGameOver;
        public event Action<Piece.Piece> OnNextPieceChanged;

        public int CurrentScore => currentScore;
        public int CurrentLine => destroyedLines;

        //---------------------------------------------------------------------------

        private void ResetRun()
        {
            speed = 1f;
            fallTimer = 0f;
            destroyedLines = 0;
            currentScore = 0;
            gridManagerRef.Reset();
            pieceBag = new PieceBag(availablePieces);
        }

        public void StartRun()
        {
            ResetRun();
            anim.SetInteger("Screen", 1);
            isRunning = true;
            isBusy = false;
            SpawnPiece();
        }

        //---------------------------------------------------------------------------

        private void Update()
        {
            if (!isRunning || isBusy) return;

            fallTimer += Time.deltaTime;
            float interval = 1f / speed;

            if (fallTimer >= interval)
            {
                fallTimer -= interval;
                Tick();
            }

            HandleInput();
        }

        private void Tick()
        {
            if (gridManagerRef.CanMoveDown())
            {
                gridManagerRef.MoveDown();
                return;
            }

            LockAndAdvance();
        }

        //---------------------------------------------------------------------------

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

        private void HandleInput()
        {
            if (Input.GetKeyDown(KeyCode.A)) gridManagerRef.MoveLeft();
            if (Input.GetKeyDown(KeyCode.D)) gridManagerRef.MoveRight();
            if (Input.GetKeyDown(KeyCode.S)) gridManagerRef.MoveDown();

            if (Input.GetKeyDown(KeyCode.Q)) gridManagerRef.Rotate(-1);
            if (Input.GetKeyDown(KeyCode.E)) gridManagerRef.Rotate(1);

            if (Input.GetKeyDown(KeyCode.Space))
            {
                fallTimer = 0f;
                gridManagerRef.HardDrop();
                LockAndAdvance();
            }
        }

        private void SpawnPiece()
        {
            Piece.Piece current = pieceBag.Next();

            if (!gridManagerRef.SpawnPiece(current)) // GameOver
            {
                anim.SetInteger("Screen", 0);

                isRunning = false;
                ScoreService.SaveRun(currentScore, destroyedLines);
                OnGameOver?.Invoke(currentScore, destroyedLines);
                ResetRun();
                return;
            }

            OnNextPieceChanged?.Invoke(pieceBag.Peek());
        }

        //---------------------------------------------------------------------------

        private void AddScore(int linesCleared)
        {
            int points = Mathf.RoundToInt(linesCleared * PointsPerLine * speed);
            currentScore += points;
        }
    }
}