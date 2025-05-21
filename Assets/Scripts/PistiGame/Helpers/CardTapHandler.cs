using PistiGame;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Helpers
{
    public class CardTapHandler : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        private GameInputActions _inputActions;
        private Player _player;

        private void OnDisable()
        {
            if(_inputActions == null) return;
            _inputActions.Gameplay.Tap.performed -= OnTapPerformed;
            _inputActions.Disable();
        }
        
        public void Initialize()
        {
            _inputActions = new GameInputActions();
            _inputActions.Enable();
            _inputActions.Gameplay.Tap.performed += OnTapPerformed;
        }

        public void InjectPlayer(Player player)
        {
            _player = player;
        }

        private void OnTapPerformed(InputAction.CallbackContext context)
        {
            Debug.Log("tap performed");
            Vector2 screenPosition = Mouse.current.position.ReadValue();
            
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            {
                screenPosition = Touchscreen.current.primaryTouch.position.ReadValue();
            }
            
            Ray ray = mainCamera.ScreenPointToRay(screenPosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                var card = hit.collider.GetComponent<Card>();
                if (card != null)
                {
                    Debug.Log($"Tapped on card: {card.name}");
                    _player.OnCardPlayed(card);
                }
            }
        }
    }
}