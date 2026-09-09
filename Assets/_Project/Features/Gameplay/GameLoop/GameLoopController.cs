using System.Collections;
using UnityEngine;

namespace Features.Gameplay.Run
{
    public class GameLoopController : MonoBehaviour
    {
        [SerializeField] private float speed = 1f;
        [SerializeField] private int destroyedLines = 0;
        [SerializeField] private GridManager gridManagerRef;
        [SerializeField] private GridRenderer gridRendererRef;
        [SerializeField] private Piece.Piece[] availablePieces;

        [SerializeField] private GameObject ui;
        [SerializeField] private float fallTimer;
        [SerializeField] private bool isRunning;
        [SerializeField] private bool isBusy;  // true during the clearing animation (pauses input/dropping)


        private void ResetRun()
        {
            speed = 1f;
            fallTimer = 0f;
            destroyedLines = 0;
            gridManagerRef.Reset();
        }

        public void StartRun()
        {
            ResetRun();
            isRunning = true;
            isBusy = false;
            SpawnPiece();
        }

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
            if (Input.GetKey(KeyCode.Space)) gridManagerRef.MoveDown();

            if (Input.GetKeyDown(KeyCode.Q)) gridManagerRef.Rotate(-1);
            if (Input.GetKeyDown(KeyCode.E)) gridManagerRef.Rotate(1);
        }

        private void SpawnPiece()
        {
            Piece.Piece next = availablePieces[Random.Range(0, availablePieces.Length)];

            if (!gridManagerRef.SpawnPiece(next)) // GameOver
            {
                isRunning = false;
                gridManagerRef.LockPiece();
                ui.SetActive(true);
            }
        }
    }
}