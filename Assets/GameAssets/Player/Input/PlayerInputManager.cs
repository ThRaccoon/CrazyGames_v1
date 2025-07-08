using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputManager : MonoBehaviour
{
    [SerializeField] private Player _player;

    private PlayerInput _playerInput;


    private void Awake()
    {
        _playerInput = new PlayerInput();
    }

    private void OnEnable()
    {
        _playerInput.Enable();


        _playerInput.Player.LMB.performed += GetLMBInput;
    }

    private void OnDisable()
    {
        _playerInput.Player.LMB.performed -= GetLMBInput;


        _playerInput.Disable();
    }


    private void GetLMBInput(InputAction.CallbackContext ctx)
    {
        _player.SelectTarget();
    }
}