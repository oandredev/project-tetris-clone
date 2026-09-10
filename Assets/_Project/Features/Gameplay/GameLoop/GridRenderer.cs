using System.Collections;
using UnityEngine;

namespace Features.Gameplay.GameLoop
{
    public class GridRenderer : MonoBehaviour
    {
        // --- References (Inspector) ---
        [SerializeField] private GridManager gridManager;
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float clearStepDelay = 0.05f;
        [SerializeField] private Color ghostColor = Color.white;
        [SerializeField] private float gameOverRowDelay = 0.05f;

        // --- Runtime state (should not appear in the Inspector) ---
        private GameObject[,] cubes;
        private Renderer[,] renderers;
        private GameObject[] ghostCubes;
        private MaterialPropertyBlock propertyBlock;
        private int rows;
        private int cols;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

        #region Unity Lifecycle

        private void Awake()
        {
            int[,] initial = gridManager.GetGridSnapshot();

            rows = initial.GetLength(0);
            cols = initial.GetLength(1);

            cubes = new GameObject[rows, cols];
            renderers = new Renderer[rows, cols];
            propertyBlock = new MaterialPropertyBlock();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    GameObject cube = Instantiate(cubePrefab, GridToWorldPos(x, y), Quaternion.identity, transform);
                    cube.SetActive(false);

                    cubes[y, x] = cube;
                    renderers[y, x] = cube.GetComponent<Renderer>();
                }
            }

            ghostCubes = new GameObject[4];
            for (int i = 0; i < 4; i++)
            {
                GameObject ghost = Instantiate(cubePrefab, Vector3.zero, Quaternion.identity, transform);
                ghost.SetActive(false);
                ghostCubes[i] = ghost;
            }
        }

        private void OnEnable() => gridManager.OnGridChanged += Render;
        private void OnDisable() => gridManager.OnGridChanged -= Render;

        #endregion

        #region Rendering

        private void Render()
        {
            int[,] grid = gridManager.GetGridSnapshot();
            Color[,] colors = gridManager.GetColorSnapshot();

            for (int y = 0; y < rows; y++)
            {
                for (int x = 0; x < cols; x++)
                {
                    int value = grid[y, x];
                    GameObject cube = cubes[y, x];

                    if (value == 0)
                    {
                        cube.SetActive(false);
                        continue;
                    }

                    cube.SetActive(true);

                    Color color = colors[y, x];

                    propertyBlock.Clear();
                    propertyBlock.SetColor(BaseColor, color);
                    renderers[y, x].SetPropertyBlock(propertyBlock);
                }
            }

            RenderGhost();
        }

        private void RenderGhost()
        {
            foreach (var cube in ghostCubes)
                cube.SetActive(false);

            if (!gridManager.HasActivePiece) return;

            Vector2Int ghostPos = gridManager.GetGhostPosition();
            int[,] shape = gridManager.GetCurrentShape();
            int size = gridManager.GetCurrentSize();

            int index = 0;
            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    if (shape[y, x] != 1) continue;

                    GameObject cube = ghostCubes[index++];
                    cube.transform.position = GridToWorldPos(ghostPos.x + x, ghostPos.y + y);
                    cube.SetActive(true);

                    propertyBlock.Clear();
                    propertyBlock.SetColor(BaseColor, ghostColor);
                    cube.GetComponent<Renderer>().SetPropertyBlock(propertyBlock);
                }
            }
        }

        #endregion

        #region Line Clear Animation

        public IEnumerator PlayLineClearAnimation(int[] rowsToClear)
        {
            for (int x = 0; x < cols; x++)
            {
                foreach (int row in rowsToClear)
                {
                    cubes[row, x].SetActive(false);
                }
                yield return new WaitForSeconds(clearStepDelay);
            }
        }


        public IEnumerator PlayGameOverAnimation()
        {
            for (int y = rows - 1; y >= 0; y--) // y = rows-1 é o fundo visual, y = 0 é o topo
            {
                for (int x = 0; x < cols; x++)
                    cubes[y, x].SetActive(false);

                yield return new WaitForSeconds(gameOverRowDelay);
            }
        }

        #endregion

        #region Internal Helpers

        private Vector3 GridToWorldPos(int x, int y)
        {
            return new Vector3(x * cellSize, (rows - 1 - y) * cellSize, 0);
        }

        #endregion
    }
}