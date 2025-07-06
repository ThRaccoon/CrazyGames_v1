using UnityEngine;

using UnityEngine;

public class BarrelRotation : MonoBehaviour
{
    [SerializeField] float rotationSpeed = 90f;

    [SerializeField] Vector3 rotationAxis = Vector3.forward;

    void Update()
    {
        transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
    }
}



