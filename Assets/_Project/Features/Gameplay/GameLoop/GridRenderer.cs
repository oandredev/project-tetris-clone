using System.Collections;
using UnityEngine;

namespace Features.Gameplay.Run
{
    public class GridRenderer : MonoBehaviour
    {
        [SerializeField] private GridManager gridManager;
        [SerializeField] private GameObject cubePrefab;
        [SerializeField] private float cellSize = 1f;
        [SerializeField] private float clearStepDelay = 0.05f;
        private GameObject[,] cubes;
        private Renderer[,] renderers;
        private MaterialPropertyBlock propertyBlock;

        private int rows;
        private int cols;

        private static readonly int BaseColor = Shader.PropertyToID("_BaseColor");

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
        }

        private void OnEnable() => gridManager.OnGridChanged += Render;
        private void OnDisable() => gridManager.OnGridChanged -= Render;

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

                    Color baseColor = colors[y, x];
                    Color finalColor = value == 2 ? baseColor * 1.3f : baseColor;

                    propertyBlock.Clear();
                    propertyBlock.SetColor(BaseColor, finalColor);
                    renderers[y, x].SetPropertyBlock(propertyBlock);
                }
            }
        }

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

        private Vector3 GridToWorldPos(int x, int y)
        {
            return new Vector3(x * cellSize, (rows - 1 - y) * cellSize, 0);
        }
    }
}