using System;
using UniRx;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace Core.Inputs
{
    public class PlayerInputService :  IPlayerInputService
    {
        private readonly CoreInputSystem _coreInputSystem;

        public IObservable<Vector2> CameraMove => _cameraMove;
        private readonly Subject<Vector2> _cameraMove = new();

        public IObservable<Vector2> CursorMove => _cursorMove;
        private readonly Subject<Vector2> _cursorMove = new();
        
        public IObservable<Vector2> CursorZoom => _cursorZoom;
        private readonly Subject<Vector2> _cursorZoom = new();

        public IObservable<Unit> MainClick => _onMainClick;
        private readonly Subject<Unit> _onMainClick = new();

        [Inject]
        public PlayerInputService(CoreInputSystem coreInputSystem)
        {
            _coreInputSystem = coreInputSystem;
        }

        public void Activate() =>
            _coreInputSystem.Enable();

        public void Deactivate() =>
            _coreInputSystem.Disable();

        public void Init()
        {
            Subscribe();
        }
        
        private void OnMainClick(InputAction.CallbackContext obj) =>
            _onMainClick.OnNext(Unit.Default);

        private void OnCameraZoomWithPressButton(InputAction.CallbackContext obj)
        {
            if(!_coreInputSystem.Player.CameraZoomEnable.enabled)
                return;
            
            OnCameraZoom(obj);
        }
        
        private void OnCameraZoom(InputAction.CallbackContext ctx)
        {
            Vector2 move = ctx.ReadValue<Vector2>();
            _cursorZoom.OnNext(move);
        }

        private void OnCameraMove(InputAction.CallbackContext ctx)
        {
            Vector2 move = ctx.ReadValue<Vector2>();
            _cameraMove.OnNext(move);
        }

        private void OnCursorMove(InputAction.CallbackContext ctx)
        {
            Vector2 move = ctx.ReadValue<Vector2>();
            _cursorMove.OnNext(move);
        }
        
        public void Dispose()
        {
            Unsubscribe();
        }
    
        private void Subscribe()
        {
            _coreInputSystem.Player.CameraMove.performed += OnCameraMove;
            _coreInputSystem.Player.CameraMove.canceled += OnCameraMove;

            _coreInputSystem.Player.CursorMove.performed += OnCursorMove;
            _coreInputSystem.Player.CursorMove.canceled += OnCursorMove;

            _coreInputSystem.Player.CameraZoom.performed += OnCameraZoom;
            _coreInputSystem.Player.CameraZoom.canceled += OnCameraZoom;
            
            _coreInputSystem.Player.CameraMove.performed += OnCameraZoomWithPressButton;
            _coreInputSystem.Player.CameraMove.canceled += OnCameraZoomWithPressButton;
            
            _coreInputSystem.Player.MainClick.started += OnMainClick;
        }
        
        private void Unsubscribe()
        {
            _coreInputSystem.Player.CameraMove.performed -= OnCameraMove;
            _coreInputSystem.Player.CameraMove.canceled -= OnCameraMove;

            _coreInputSystem.Player.CursorMove.performed -= OnCursorMove;
            _coreInputSystem.Player.CursorMove.canceled -= OnCursorMove;

            _coreInputSystem.Player.CameraZoom.performed -= OnCameraZoom;
            _coreInputSystem.Player.CameraZoom.canceled -= OnCameraZoom;
            
            _coreInputSystem.Player.CameraMove.performed -= OnCameraZoomWithPressButton;
            _coreInputSystem.Player.CameraMove.canceled -= OnCameraZoomWithPressButton;
            
            _coreInputSystem.Player.MainClick.started -= OnMainClick;
        }
    }
}