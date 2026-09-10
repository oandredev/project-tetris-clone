using UnityEngine;

namespace Features.Gameplay.GameLoop
{
    public class PiecePreviewRenderer : MonoBehaviour
    {
        // --- References (Inspector) ---
        [SerializeField] private GameLoopController gameLoopRef;
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private int previewGridSize = 4;

        // --- Runtime state (should not appear in the Inspector) ---
        private GameObject[] cubes;
        private MaterialPropertyBlock propertyBlock;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        #region Unity Lifecycle

        private void Awake()
        {
            cubes = new GameObject[4];
            propertyBlock = new MaterialPropertyBlock();

            for (int i = 0; i < 4; i++)
            {
                GameObject cube = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity, transform);
                cube.SetActive(false);
                cubes[i] = cube;
            }
        }

        private void OnEnable() => gameLoopRef.OnNextPieceChanged += Render;
        private void OnDisable() => gameLoopRef.OnNextPieceChanged -= Render;

        #endregion

        #region Rendering

        private void Render(Piece.Piece piece)
        {
            int[,] shape = piece.GetRotation(piece.GetRotationByIndex(0));
            int size = piece.GridSize;
            Color color = piece.GetPieceColor();

            float offset = (previewGridSize - size) / 2f;

            int index = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (shape[y, x] != 1) continue;

                    GameObject cube = cubes[index++];
                    cube.transform.localPosition = GridToLocalPos(x + offset, y + offset);
                    cube.SetActive(true);

                    propertyBlock.Clear();
                    propertyBlock.SetColor(BaseColor, color);
                    cube.GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
                }
            }
        }

        #endregion

        #region Internal Helpers

        private Vector3 GridToLocalPos(float x, float y)
        {
            return new Vector3(x * cellSize, -y * cellSize, 0);
        }

        #endregion
    }
}