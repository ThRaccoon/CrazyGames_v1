using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private PlayerInputManager _playerInputManager;
    [SerializeField] private GameObject _player;

    [field: Space(10)]
    [Header("Speeds")]
    [SerializeField] private float _verticalSpeed;
    [SerializeField] private float _horizontalSpeed;

    [field: Space(10)]
    [SerializeField] private float yPos;

    [field: Space(10)]
    [Header("Boundaries")]
    [SerializeField] private float leftBound;
    [SerializeField] private float rightBound;


    private void Update()
    {
        MovePlayerWithInBounds();
    }


    private void MovePlayerWithInBounds()
    {
        Vector3 oldPos = _player.transform.position;

        float newX = oldPos.x + _playerInputManager.movementInput.x * _horizontalSpeed * Time.deltaTime;
        float newZ = oldPos.z + _verticalSpeed;

        if (newX <= leftBound || newX >= rightBound)
        {
            newX = oldPos.x;
        }

        _player.transform.position = new Vector3(newX, yPos, newZ);
    }
}