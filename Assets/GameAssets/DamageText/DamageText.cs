using UnityEngine;

public class DamageText : MonoBehaviour
{
    [SerializeField] private float _destroyAfter = 1.5f;
    
    void Start()
    {
        Destroy(gameObject, _destroyAfter);
    }
}
