using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private PlayerInput _playerInput;

    public Vector2 movementInput { get; private set; }


    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();


        _playerInput.Player.AD.performed += GetMovementInput;
        _playerInput.Player.AD.canceled += GetMovementInput;
    }

    private void OnDisable()
    {
        _playerInput.Player.AD.performed -= GetMovementInput;
        _playerInput.Player.AD.canceled -= GetMovementInput;


        _playerInput.Disable();
    }


    private void GetMovementInput(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
    }
}