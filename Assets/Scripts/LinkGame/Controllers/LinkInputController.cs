using Interfaces;
using UnityEngine;
using UnityEngine.InputSystem;
using LinkGame;

namespace LinkGame.Controllers
{
    public class LinkInputController : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;

        private LinkInputMap _inputMap;
        private ITappable _currentTapped;
        private bool _canDrag;

        private void Start()
        {
            _inputMap = new LinkInputMap();
            RegisterInputActions();
        }

        public void ToggleInput(bool toggle)
        {
            if (toggle)
                _inputMap.Inputs.Enable();
            else
                _inputMap.Inputs.Disable();
        }

        private void RegisterInputActions()
        {
            _inputMap.Inputs.HoldToDrag.performed += HandleOnHold;
            _inputMap.Inputs.Drag.performed += HandleOnDrag;
            _inputMap.Inputs.HoldToDrag.canceled += HandleOnRelease;
        }

        private void HandleOnHold(InputAction.CallbackContext obj)
        {
            var tappable = TryGetTappable();
            if (tappable != null)
            {
                _canDrag = true;
                _currentTapped = tappable;
                GetContext()?.TryAppendToCurrentLink(_currentTapped);
                _currentTapped.OnTap();
            }
        }

        private void HandleOnDrag(InputAction.CallbackContext obj)
        {
            if (!_canDrag) return;

            var tappable = TryGetTappable();
            if (tappable != null)
            {
                _currentTapped.OnRelease();
                _currentTapped = tappable;
                _currentTapped.OnTap();
                GetContext()?.TryAppendToCurrentLink(tappable);
            }
        }

        private void HandleOnRelease(InputAction.CallbackContext obj)
        {
            _canDrag = false;
            GetContext()?.HandleOnRelease();
        }

        private ITappable TryGetTappable()
        {
            Ray ray = mainCamera.ScreenPointToRay(GetPointerPosition());
            if (Physics.Raycast(ray, out RaycastHit hit))
                return hit.collider.GetComponent<ITappable>();
            return null;
        }

        private Vector2 GetPointerPosition()
        {
            if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
                return Touchscreen.current.primaryTouch.position.ReadValue();
            return Mouse.current.position.ReadValue();
        }

        private LinkGameContext GetContext()
        {
            return GameController.Instance.CurrentContext as LinkGameContext;
        }
    }
}
