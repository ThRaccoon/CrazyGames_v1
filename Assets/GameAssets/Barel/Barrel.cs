using UnityEngine;

public class Barrel : MonoBehaviour
{
    [SerializeField] Vector3 _rotationAxis;
    [SerializeField] float _rotationSpeed;

    [Space(15)]
    [SerializeField] private float _lifeTime;

    private void Awake()
    {
        Destroy(gameObject, _lifeTime);
    }

    private void Update()
    {
        transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime);
    }

    
    public void GiveBuff() 
    {
       if(BuffCardManager._SBuffCardManagerScript)
        {
            BuffCardManager._SBuffCardManagerScript.RollBuff();
        }

       Destroy(gameObject);
    }
}