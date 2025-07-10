using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private float _destroyAfter = 1.5f;
    void Start()
    {
        Destroy(gameObject, _destroyAfter);
    }
}
