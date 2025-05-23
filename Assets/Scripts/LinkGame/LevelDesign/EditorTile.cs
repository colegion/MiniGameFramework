using GridSystem;
using Helpers;
using ScriptableObjects.Chip;
using UnityEditor;
using UnityEngine;
using Grid = GridSystem.Grid;

namespace LinkGame.LevelDesign
{
    public class EditorTile : BaseTile
    {
        private LevelEditor _editor;
        public override void ConfigureSelf(ChipConfig config, int x, int y)
        {
            _x = x;
            _y = y;
            _chipType = config.chipType;
            _position = new Vector2Int(x, y);
            
            tileView.SetSprite(config.chipSprite);
            
            var grid = ServiceLocator.Get<Grid>();
            grid.PlaceTileToParentCell(this);
            
            var cell = grid.GetCell(x, y);
            if (cell != null)
            {
                transform.position = cell.GetWorldPosition();
                cell.SetTile(this);
            }

            ConfigureTileData();
        }

        public void InjectEditor(LevelEditor editor)
        {
            _editor = editor;
        }
        
        private void OnMouseDown()
        {
            _editor.SetSelectedTile(this);
        }
        
        protected override void ResetSelf()
        {
            tileView.ResetSelf();
            tileView.ToggleVisuals(false);
            _position = Vector2Int.zero;
            gameObject.SetActive(false);
        }
    }
}