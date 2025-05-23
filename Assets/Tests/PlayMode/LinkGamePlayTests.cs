using System.Collections;
using GridSystem;
using Helpers;
using LinkGame;
using LinkGame.Helpers;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Grid = GridSystem.Grid;

namespace Tests.PlayMode
{
    public class LinkGamePlayTests
    {
        private Grid _grid;
        private GameObject _gridGO;

        [SetUp]
        public void SetUp()
        {
            _grid = new Grid(5, 5);
            ServiceLocator.Register(_grid);
            _gridGO = new GameObject("GridRoot");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_gridGO);
        }

        [UnityTest]
        public IEnumerator LinkSearcher_HasPossibleLink_ReturnsTrue()
        {
            CreateTile(0, 0, ChipType.Circle);
            CreateTile(0, 1, ChipType.Circle);
            CreateTile(0, 2, ChipType.Circle);

            yield return null;

            var searcher = new LinkSearcher();
            Assert.IsTrue(searcher.HasPossibleLink());
        }

        [UnityTest]
        public IEnumerator LinkSearcher_HasPossibleLink_ReturnsFalse()
        {
            CreateTile(0, 0, ChipType.Circle);
            CreateTile(0, 1, ChipType.Octagon);
            CreateTile(0, 2, ChipType.Diamond);

            yield return null;

            var searcher = new LinkSearcher();
            Assert.IsFalse(searcher.HasPossibleLink());
        }

        private void CreateTile(int x, int y, ChipType chipType)
        {
            var cellGO = new GameObject($"Cell_{x}_{y}");
            cellGO.transform.parent = _gridGO.transform;

            var cell = cellGO.AddComponent<BaseCell>();
            cell.ConfigureSelf(x, y);

            var tileGO = new GameObject($"Tile_{x}_{y}");
            tileGO.transform.parent = cellGO.transform;

            var tile = tileGO.AddComponent<BaseTile>();
            tile.X = x;
            tile.Y = y;
            tile.Layer = LinkUtilities.DefaultChipLayer;
            tile.ChipType = chipType;

            cell.SetTile(tile);
        }
    }
}