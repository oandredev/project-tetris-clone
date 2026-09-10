using UnityEngine;

namespace Features.Gameplay.Piece
{
    [System.Serializable]
    public class PieceRow
    {
        public int[] cells = new int[4];

        public PieceRow() { }

        public PieceRow(int a, int b, int c, int d = 0)
        {
            cells = new int[] { a, b, c, d };
        }
    }

    [System.Serializable]
    public class RotationState
    {
        public PieceRow[] rows = new PieceRow[4]
        {
        new PieceRow(), new PieceRow(), new PieceRow(), new PieceRow()
        };
    }

    [CreateAssetMenu(fileName = "Piece", menuName = "Scriptable Objects/Piece")]
    public class Piece : ScriptableObject
    {
        [SerializeField] private int gridSize = 3;
        [SerializeField] private Color _pieceColor = Color.blue;

        [SerializeField]
        private RotationState up = new RotationState
        {
            rows = new PieceRow[]
            {
                new PieceRow(0, 0, 1, 0),
                new PieceRow(1, 1, 1, 0),
                new PieceRow(0, 0, 0, 0),
                new PieceRow(0, 0, 0, 0)
            }
        };

        [SerializeField]
        private RotationState right = new RotationState
        {
            rows = new PieceRow[]
            {
                new PieceRow(0, 1, 0, 0),
                new PieceRow(0, 1, 0, 0),
                new PieceRow(0, 1, 1, 0),
                new PieceRow(0, 0, 0, 0)
            }
        };

        [SerializeField]
        private RotationState down = new RotationState
        {
            rows = new PieceRow[]
            {
                new PieceRow(0, 0, 0, 0),
                new PieceRow(1, 1, 1, 0),
                new PieceRow(1, 0, 0, 0),
                new PieceRow(0, 0, 0, 0)
            }
        };

        [SerializeField]
        private RotationState left = new RotationState
        {
            rows = new PieceRow[]
            {
                new PieceRow(1, 1, 0, 0),
                new PieceRow(0, 1, 0, 0),
                new PieceRow(0, 1, 0, 0),
                new PieceRow(0, 0, 0, 0)
            }
        };

        //---------------------------------------------------------------------------

        public int GridSize => gridSize;

        public RotationState GetRotationByIndex(int index)
        {
            switch (index)
            {
                case 0: return up;
                case 1: return right;
                case 2: return down;
                case 3: return left;
                default: return up;
            }
        }

        public int[,] GetRotation(RotationState state)
        {
            int[,] result = new int[gridSize, gridSize];
            for (int y = 0; y < gridSize; y++)
                for (int x = 0; x < gridSize; x++)
                    result[y, x] = state.rows[y].cells[x];
            return result;
        }

        public Color GetPieceColor()
        {
            return _pieceColor;
        }
    }
}