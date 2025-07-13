using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] Vector3 _rotationAxis;
    [SerializeField] float _rotationSpeed;

    private void Update()
    {
        transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime);
    }
}