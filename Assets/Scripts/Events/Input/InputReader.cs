
using Events.Channels;
using UnityEngine;
using UnityEngine.InputSystem;

//Credits: git-amend
namespace Events.Input
{
    [CreateAssetMenu(fileName = "InputReader", menuName = "Scriptable Objects/InputReader")]
    public class InputReader : ScriptableObject, PlayerInputActions.IPlayerActions, IInputReader
    {
        [SerializeField] private Vector2EventChannelSO Move;
        [SerializeField] private BoolEventChannelSO Jump;
        
        private PlayerInputActions _playerActions;
        
        public Vector2 Direction => _playerActions.Player.Move.ReadValue<Vector2>();
        public bool IsJumpKeyPressed => _playerActions.Player.Move.IsPressed();
        
        
        public void EnablePlayerActions()
        {
            if (_playerActions == null)
            {
                _playerActions = new PlayerInputActions();
                _playerActions.Player.SetCallbacks(this);
            }
            _playerActions.Player.Enable();
        }
    
        public void DisablePlayerActions()
        {
            _playerActions?.Player.Disable();
        }
        public void OnMove(InputAction.CallbackContext context)
        {
            Move.RaiseEvent(context.ReadValue<Vector2>());
        }
        public void OnJump(InputAction.CallbackContext context)
        {
            Jump.RaiseEvent(true);
        }
    
        public void OnLook(InputAction.CallbackContext context)
        {
            
        }
    
        public void OnAttack(InputAction.CallbackContext context)
        {
            
        }
    
        public void OnInteract(InputAction.CallbackContext context)
        {
            
        }
    
        public void OnCrouch(InputAction.CallbackContext context)
        {
            
        }
    
        public void OnPrevious(InputAction.CallbackContext context)
        {
            
        }
    
        public void OnNext(InputAction.CallbackContext context)
        {
            
        }
    
        public void OnSprint(InputAction.CallbackContext context)
        {
            
        }
    
        
    }
}
