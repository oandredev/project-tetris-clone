using UnityEngine;
using System;
using System.Collections.Generic;

namespace Features.Gameplay.GameLoop
{
    public class GridManager : MonoBehaviour
    {
        [System.Serializable]
        public class Row
        {
            public int[] columns = new int[10];
        }

        // --- Grid data ---
        private Row[] grid = new Row[20];
        private Color[,] cellColors;

        // --- Active piece state ---
        private Piece.Piece pieceNow;
        private Vector2Int currentPos;
        private int currentRotationIndex; // 0=up, 1=right, 2=down, 3=left
        private bool hasActivePiece;

        // --- Events ---
        public event Action OnGridChanged;

        // --- Public read-only properties ---
        public bool HasActivePiece => hasActivePiece;

        #region Unity Lifecycle

        private void Awake()
        {
            for (int i = 0; i < grid.Length; i++)
                if (grid[i] == null)
                    grid[i] = new Row();

            cellColors = new Color[grid.Length, grid[0].columns.Length];
        }

        #endregion

        #region Spawn

        public bool SpawnPiece(Piece.Piece piece)
        {
            pieceNow = piece;
            currentRotationIndex = 0;

            int size = piece.GridSize;
            int spawnX = (grid[0].columns.Length - size) / 2;
            currentPos = new Vector2Int(spawnX, 0);

            if (!IsValidPosition(currentPos, currentRotationIndex))
            {
                hasActivePiece = false;
                return false;
            }

            hasActivePiece = true;
            WritePiece(2);
            OnGridChanged?.Invoke();
            return true;
        }

        #endregion

        #region Movement

        public void MoveDown() => TryMove(new Vector2Int(0, 1));
        public void MoveLeft() => TryMove(new Vector2Int(-1, 0));
        public void MoveRight() => TryMove(new Vector2Int(1, 0));

        public bool CanMoveDown()
        {
            return IsValidPosition(currentPos + new Vector2Int(0, 1), currentRotationIndex);
        }

        public void Rotate(int direction)
        {
            int newRotationIndex = (currentRotationIndex + direction + 4) % 4;

            WritePiece(0);

            if (IsValidPosition(currentPos, newRotationIndex))
                currentRotationIndex = newRotationIndex;

            WritePiece(2);
            OnGridChanged?.Invoke();
        }

        public void HardDrop()
        {
            WritePiece(0);

            while (IsValidPosition(currentPos + new Vector2Int(0, 1), currentRotationIndex))
                currentPos += new Vector2Int(0, 1);

            WritePiece(2);
            OnGridChanged?.Invoke();
        }

        private void TryMove(Vector2Int delta)
        {
            WritePiece(0);

            Vector2Int newPos = currentPos + delta;
            if (IsValidPosition(newPos, currentRotationIndex))
                currentPos = newPos;

            WritePiece(2);
            OnGridChanged?.Invoke();
        }

        #endregion

        #region Locking & Line Clearing

        public void LockPiece()
        {
            int[,] shape = pieceNow.GetRotation(pieceNow.GetRotationByIndex(currentRotationIndex));
            int size = pieceNow.GridSize;

            for (int y = 0; y < size; y++)
                for (int x = 0; x < size; x++)
                    if (shape[y, x] == 1)
                        grid[currentPos.y + y].columns[currentPos.x + x] = 1;

            OnGridChanged?.Invoke();
        }

        public int[] FindCompletedLines()
        {
            List<int> completed = new List<int>();

            for (int y = 0; y < grid.Length; y++)
            {
                bool full = true;
                for (int x = 0; x < grid[y].columns.Length; x++)
                {
                    if (grid[y].columns[x] == 0)
                    {
                        full = false;
                        break;
                    }
                }
                if (full) completed.Add(y);
            }

            return completed.ToArray();
        }

        public void ClearLines(int[] rowsToClear)
        {
            if (rowsToClear.Length == 0) return;

            var rowsSet = new HashSet<int>(rowsToClear);
            var remainingRows = new List<Row>();
            var remainingColors = new List<Color[]>();

            for (int y = 0; y < grid.Length; y++)
            {
                if (rowsSet.Contains(y)) continue;

                remainingRows.Add(grid[y]);

                Color[] rowColors = new Color[grid[y].columns.Length];
                for (int x = 0; x < rowColors.Length; x++)
                    rowColors[x] = cellColors[y, x];
                remainingColors.Add(rowColors);
            }

            int emptyCount = grid.Length - remainingRows.Count;
            var newGrid = new Row[grid.Length];
            var newColors = new Color[grid.Length, grid[0].columns.Length];

            for (int i = 0; i < emptyCount; i++)
                newGrid[i] = new Row();

            for (int i = 0; i < remainingRows.Count; i++)
            {
                int destY = emptyCount + i;
                newGrid[destY] = remainingRows[i];
                for (int x = 0; x < remainingColors[i].Length; x++)
                    newColors[destY, x] = remainingColors[i][x];
            }

            grid = newGrid;
            cellColors = newColors;

            OnGridChanged?.Invoke();
        }

        #endregion

        #region Ghost / Preview

        public Vector2Int GetGhostPosition()
        {
            Vector2Int ghostPos = currentPos;
            while (IsValidPosition(ghostPos + new Vector2Int(0, 1), currentRotationIndex))
                ghostPos += new Vector2Int(0, 1);
            return ghostPos;
        }

        public int[,] GetCurrentShape() => pieceNow.GetRotation(pieceNow.GetRotationByIndex(currentRotationIndex));
        public int GetCurrentSize() => pieceNow.GridSize;

        #endregion

        #region Snapshots

        public int[,] GetGridSnapshot()
        {
            int rows = grid.Length;
            int cols = grid[0].columns.Length;
            int[,] snapshot = new int[rows, cols];

            for (int y = 0; y < rows; y++)
                for (int x = 0; x < cols; x++)
                    snapshot[y, x] = grid[y].columns[x];

            return snapshot;
        }

        public Color[,] GetColorSnapshot() => cellColors;

        #endregion

        #region Internal Helpers

        private void WritePiece(int value)
        {
            int[,] shape = pieceNow.GetRotation(pieceNow.GetRotationByIndex(currentRotationIndex));
            int size = pieceNow.GridSize;
            Color color = pieceNow.GetPieceColor();

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (shape[y, x] != 1) continue;

                    int gridX = currentPos.x + x;
                    int gridY = currentPos.y + y;

                    grid[gridY].columns[gridX] = value;
                    if (value != 0)
                        cellColors[gridY, gridX] = color;
                }
            }
        }

        private bool IsValidPosition(Vector2Int pos, int rotationIndex)
        {
            int[,] shape = pieceNow.GetRotation(pieceNow.GetRotationByIndex(rotationIndex));
            int size = pieceNow.GridSize;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (shape[y, x] != 1) continue;

                    int gridX = pos.x + x;
                    int gridY = pos.y + y;

                    if (gridX < 0 || gridX >= grid[0].columns.Length) return false;
                    if (gridY < 0 || gridY >= grid.Length) return false;
                    if (grid[gridY].columns[gridX] == 1) return false;
                }
            }
            return true;
        }

        #endregion

        #region Reset

        public void Reset()
        {
            for (int y = 0; y < grid.Length; y++)
            {
                grid[y] = new Row();
                for (int x = 0; x < grid[y].columns.Length; x++)
                {
                    cellColors[y, x] = default;
                }
            }

            pieceNow = null;
            currentPos = Vector2Int.zero;
            currentRotationIndex = 0;

            OnGridChanged?.Invoke();
        }

        #endregion
    }
}