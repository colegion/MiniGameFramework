using LinkGame.LevelDesign;
using UnityEngine;

namespace GridSystem
{
    public static class BoardUtility
    {
        public static void CreateCells(int width, int height, Transform parent, BaseCell lightPrefab, BaseCell darkPrefab, LevelEditor editor = null)
        {
            float xOffset = width / 2f;
            float yOffset = height / 2f;

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    var prefab = (x + y) % 2 == 0 ? lightPrefab : darkPrefab;
                    Vector3 cellPosition = new Vector3(x - xOffset, 0, y - yOffset);
                    var cell = Object.Instantiate(prefab, cellPosition, prefab.transform.rotation, parent);
                    cell.ConfigureSelf(x, y);
                    
                    if(cell.TryGetComponent(out CellEditorHandler editorHandler))
                        editorHandler.InjectEditor(editor);
                }
            }
        }
    }
}