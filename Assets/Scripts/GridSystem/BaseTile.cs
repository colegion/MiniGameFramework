using Controllers;
using Helpers;
using Interfaces;
using LinkGame;
using LinkGame.Controllers;
using LinkGame.Helpers;
using LinkGame.LevelDesign;
using ScriptableObjects.Chip;
using UnityEngine;
using UnityEngine.Tilemaps;
using TileData = LinkGame.Helpers.TileData;

namespace GridSystem
{
    public class BaseTile : MonoBehaviour, ITappable, IPoolable
    {
        [SerializeField] private Collider tileCollider;
        [SerializeField] protected TileView tileView;
    
        public int X { get; set; }
        public int Y { get; set; }
        public int Layer { get; set; }

        public ChipType ChipType { get; set; }

        protected Grid Grid;
        
        protected Vector2Int _position;
        private TileData _tileData;
        private ITileTracker _tileTracker;
        
        public TileData TileData => _tileData;
    
        public virtual void ConfigureSelf(ChipConfig config, int x, int y)
        {
            X = x;
            Y = y;
            Layer = LinkUtilities.DefaultChipLayer;
            _position = new Vector2Int(x, y);
            ChipType = config.chipType;
            tileView.SetSprite(config.chipSprite);
            ConfigureTileData();

            EnsureDependencies();
            Grid.PlaceTileToParentCell(this);
            _tileTracker.AppendTileData(_tileData);
        }
    
        public void OnTap()
        {
            tileView.AnimateOnHighlight(true);
        }

        public void OnRelease()
        {
            tileView.AnimateOnHighlight(false);
        }

        public void OnLinked()
        {
            tileView.Disappear(() =>
            {
                _tileTracker.ReturnTileToPool(this);
            });
        }

        public void UpdatePosition(Vector2Int position)
        {
            SetPosition(position);
            tileView.MoveTowardsTarget(Grid.GetCell(X, Y).GetTarget(), SetTransform);
        }

        public void SetTransform()
        {
            if (Grid == null) Grid = ServiceLocator.Get<Grid>();

            BaseCell cell = Grid.GetCell(X, Y);
            if (cell != null)
            {
                cell.SetTile(this);
                transform.position = cell.GetWorldPosition();
                Grid.PlaceTileToParentCell(this);
            }
            else
            {
                Debug.LogWarning($"Cell at {X}, {Y} not found! Using fallback position.");
                transform.position = new Vector3(X, .25f, Y);
            }
        }

        public void SetPosition(Vector2Int position)
        {
            EnsureDependencies();
            _tileTracker.RemoveTileData(_tileData);
            Grid.ClearTileOfParentCell(this);
            _position = position;
            X = _position.x;
            Y = _position.y;
            ConfigureTileData();
            _tileTracker.AppendTileData(_tileData);
        }

        public void HighlightView(HighlightType type)
        {
            tileView.HighlightSelf(type);
        }

        public void ConfigureTileData()
        {
            _tileData = new TileData()
            {
                xCoord = X,
                yCoord = Y,
                chipType = ChipType
            };
        }

        protected virtual void ResetSelf()
        {
            if(_tileTracker != null) _tileTracker.RemoveTileData(_tileData);
            Grid.ClearTileOfParentCell(this);
            tileView.ResetSelf();
            tileView.ToggleVisuals(false);
            _position = Vector2Int.zero;
            _tileData = null;
            gameObject.SetActive(false);
        }
        
        private void EnsureDependencies()
        {
            Grid ??= ServiceLocator.Get<Grid>();
            
            if (_tileTracker == null && GameController.Instance.CurrentContext is ITileTracker tracker)
                _tileTracker = tracker;
        }

        public Vector2Int GetPosition()
        {
            return _position;
        }

        public void OnPooled()
        {
            tileView.ToggleVisuals(false);
        }

        public void OnFetchFromPool()
        {
            tileView.ToggleVisuals(true);
        }

        public void OnReturnPool()
        {
            ResetSelf();
        }

        public PoolableTypes GetPoolableType()
        {
            return PoolableTypes.BaseTile;
        }

        public GameObject GetGameObject()
        {
            return gameObject;
        }
    }
}
