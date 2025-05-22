using UnityEditor;
using UnityEngine;
using ScriptableObjects.Chip;  // For ChipType enum
using LinkGame.LevelDesign;

namespace LinkGame.LevelDesign
{
    [CustomEditor(typeof(LevelEditor))]
    public class CustomInspector : Editor
    {
        private EditorTile selectedTile;
        private ChipType selectedChipType;

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
            selectedTile = null;

            if (Selection.activeGameObject != null)
            {
                selectedTile = Selection.activeGameObject.GetComponent<EditorTile>();
                if (selectedTile != null)
                {
                    selectedChipType = selectedTile.ChipType;
                }
            }
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            LevelEditor editor = (LevelEditor)target;

            GUILayout.Space(10);

            if (GUILayout.Button("Generate Board"))
            {
                editor.GenerateBoard();
            }

            if (GUILayout.Button("Clear Board"))
            {
                editor.ClearBoard();
            }

            GUILayout.Space(10);

            if (GUILayout.Button("Save Level"))
            {
                var levelData = editor.SaveLevel();
                Debug.Log($"Saved level with {levelData.tiles.Count} tiles.");
            }

            GUILayout.Space(15);

            if (selectedTile != null)
            {
                GUILayout.Label($"Selected Tile Coordinates: ({selectedTile.X}, {selectedTile.Y})");

                var newChipType = (ChipType)EditorGUILayout.EnumPopup("Chip Type", selectedChipType);
                if (newChipType != selectedChipType)
                {
                    Undo.RecordObject(selectedTile, "Change Chip Type");
                    selectedChipType = newChipType;

                    var chipConfigManager = editor.chipConfigManager;
                    if (chipConfigManager == null)
                    {
                        Debug.LogError("ChipConfigManager missing in LevelEditor.");
                        return;
                    }

                    var config = chipConfigManager.GetItemConfig(selectedChipType);
                    if (config == null)
                    {
                        Debug.LogError($"ChipConfig not found for {selectedChipType}");
                        return;
                    }

                    selectedTile.ConfigureSelf(config, selectedTile.X, selectedTile.Y);
                    EditorUtility.SetDirty(selectedTile);
                }
            }
            else
            {
                GUILayout.Label("Select an EditorTile in the scene to edit it here.");
            }
        }
    }
}
