using NUnit.Framework;
using UnityEngine;
using GridSystem;
using Helpers;
using LinkGame.Helpers;
using Grid = GridSystem.Grid;

namespace Tests.EditMode
{
    public class LinkGameEditTests
    {
        private Grid _grid;
        private GameObject _cellGO;

        [SetUp]
        public void Setup()
        {
            // Setup Grid and inject into service locator
            _grid = new Grid(5, 5);
            ServiceLocator.Register(_grid); // Ensure this mocks the actual system's registration

            _cellGO = new GameObject("TestCell");
        }

        [TearDown]
        public void TearDown()
        {
            Object.DestroyImmediate(_cellGO);
        }

        [Test]
        public void Grid_InitializedWithCorrectDimensions()
        {
            Assert.AreEqual(5, _grid.Width);
            Assert.AreEqual(5, _grid.Height);
        }

        [Test]
        public void IsCoordinateValid_ReturnsExpectedResults()
        {
            Assert.IsTrue(_grid.IsCoordinateValid(0, 0));
            Assert.IsTrue(_grid.IsCoordinateValid(4, 4));
            Assert.IsFalse(_grid.IsCoordinateValid(-1, 0));
            Assert.IsFalse(_grid.IsCoordinateValid(5, 5));
        }

        [Test]
        public void PlaceAndRetrieveCell_WorksCorrectly()
        {
            var cell = _cellGO.AddComponent<BaseCell>();
            cell.ConfigureSelf(2, 3);

            var fetched = _grid.GetCell(2, 3);
            Assert.AreEqual(cell, fetched);
        }

        [Test]
        public void SetCell_PreventsOverwrite()
        {
            var cell1 = _cellGO.AddComponent<BaseCell>();
            cell1.ConfigureSelf(1, 1);

            var duplicateCellGO = new GameObject("DuplicateCell");
            var cell2 = duplicateCellGO.AddComponent<BaseCell>();
            cell2.ConfigureSelf(1, 1); // Should log an error but not overwrite

            var stored = _grid.GetCell(1, 1);
            Assert.AreEqual(cell1, stored);

            Object.DestroyImmediate(duplicateCellGO);
        }

        [Test]
        public void BaseCell_TileSetAndClear_Works()
        {
            var cell = _cellGO.AddComponent<BaseCell>();
            cell.ConfigureSelf(0, 0);

            var tileGO = new GameObject("Tile");
            var tile = tileGO.AddComponent<BaseTile>();
            tile.Layer = LinkUtilities.DefaultChipLayer;
            tile.X = 0;
            tile.Y = 0;

            cell.SetTile(tile);
            Assert.AreEqual(tile, cell.GetTile(LinkUtilities.DefaultChipLayer));
            
            cell.SetTileNull(LinkUtilities.DefaultChipLayer);
            Assert.IsNull(cell.GetTile(LinkUtilities.DefaultChipLayer));

            Object.DestroyImmediate(tileGO);
        }

        [Test]
        public void BaseCell_IsTileAvailableForLayer_ReturnsCorrectValue()
        {
            var cell = _cellGO.AddComponent<BaseCell>();
            cell.ConfigureSelf(0, 0);

            Assert.IsTrue(cell.IsTileAvailableForLayer(0));

            var tileGO = new GameObject("Tile");
            var tile = tileGO.AddComponent<BaseTile>();
            tile.Layer = 0;
            tile.X = 0;
            tile.Y = 0;

            cell.SetTile(tile);
            Assert.IsFalse(cell.IsTileAvailableForLayer(0));

            Object.DestroyImmediate(tileGO);
        }
    }
}
