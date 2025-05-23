using UnityEditor;
using UnityEngine;

namespace LinkGame.LevelDesign
{
    [CustomEditor(typeof(LevelEditor))]
    public class CustomInspector : Editor
    {
        private EditorTile _selectedTile;
        private ChipType _selectedChipType;
        private LevelEditor _editor;

        private void OnEnable()
        {
            Selection.selectionChanged += OnSelectionChanged;
            UpdateSelectedTile();
        }

        private void OnDisable()
        {
            Selection.selectionChanged -= OnSelectionChanged;
        }

        private void OnSelectionChanged()
        {
            UpdateSelectedTile();
            Repaint();
        }

        private void UpdateSelectedTile()
        {
            _selectedTile = null;

            if (Selection.activeGameObject != null)
            {
                _selectedTile = Selection.activeGameObject.GetComponent<EditorTile>();
                if (_selectedTile != null)
                {
                    _selectedChipType = _selectedTile.ChipType;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LevelEditor editor = (LevelEditor)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Board")) editor.GenerateBoard();
            if (GUILayout.Button("Clear Board")) editor.ClearBoard();

            GUILayout.Space(10);

            if (GUILayout.Button("Save Level"))
            {
                var levelData = editor.SaveLevel();
                Debug.Log($"Saved level with {levelData.tiles.Count} tiles.");
            }

            GUILayout.Space(15);

            if (editor.selectedTile != null)
            {
                var tile = editor.selectedTile;
                GUILayout.Label($"Selected Tile: ({tile.X}, {tile.Y})");

                var newType = (ChipType)EditorGUILayout.EnumPopup("Chip Type", tile.ChipType);
                if (newType != tile.ChipType)
                {
                    var config = editor.chipConfigManager.GetItemConfig(newType);
                    if (config != null)
                    {
                        Undo.RecordObject(tile, "Change Chip Type");
                        tile.ConfigureSelf(config, tile.X, tile.Y);
                        EditorUtility.SetDirty(tile);
                    }
                }
            }
            else
            {
                GUILayout.Label("Click a tile in the scene to edit.");
            }
        }
    }
}
