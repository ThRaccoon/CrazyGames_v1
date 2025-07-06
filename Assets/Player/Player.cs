using UnityEngine;

public class Player : MonoBehaviour
{
    [field: Header("Components")]
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private GameObject _player;

    [field: Space(10)]
    [field: Header("Speeds")]
    [SerializeField] private float _verticalSpeed;
    [SerializeField] private float _horizontalSpeed;

    [field: Space(10)]
    [SerializeField] private float yPos;

    private float newX;
    private float newZ;
    private Vector3 oldPos;


    private void Update()
    {
        MovePlayerWithInBounds();
    }

    private void MovePlayerWithInBounds()
    {
        oldPos = _player.transform.position;
        
        newX = oldPos.x + _playerInputManager.movementInput.x * _horizontalSpeed * Time.deltaTime;
        newZ = oldPos.z + _verticalSpeed;

        if (newX <= 1.5 || newX >= 13.5)
        {
            newX = oldPos.x;
        }

        _player.transform.position = new Vector3(newX, yPos, newZ);
    }
}