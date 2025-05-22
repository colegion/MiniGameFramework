using GridSystem;
using UnityEngine;

namespace LinkGame.LevelDesign
{
    public class CellEditorHandler : MonoBehaviour
    {
        [SerializeField] private BaseCell cell;
        private LevelEditor _editor;

        public void SetEditor(LevelEditor editor)
        {
            _editor = editor;
        }

        private void OnMouseDown()
        {
            if (_editor == null) return;

            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("spawn");
                _editor.SpawnTileAt(cell.X, cell.Y);
            }
            else if (Input.GetMouseButtonDown(1))
            {
                _editor.RemoveTileAt(cell.X, cell.Y);
            }
        }
    }
}