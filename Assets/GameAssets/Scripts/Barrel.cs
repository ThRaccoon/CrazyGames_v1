using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Barrel : MonoBehaviour, IDamageable
{
    private BarrelData _barrelData;
    private Vector3 _rotationAxis;
    private float _rotationSpeed = 90;
    private float _moveSpeed = 5;
    private float _multiplier;
    
    private void Update()
    {
        transform.position += Vector3.back * _moveSpeed * Time.deltaTime;
        transform.Rotate(_rotationAxis, _rotationSpeed * Time.deltaTime);
    }

    public void Init(BarrelData barrelData,float multiplier)
    {
        _multiplier = multiplier < 1 ? 1 : multiplier;
        _barrelData = barrelData;
        _rotationAxis = _barrelData.rotationAxis;
        _rotationSpeed = _barrelData.rotationSpeed;
        _moveSpeed = _barrelData.moveSpeed;

        Destroy(gameObject, _barrelData.lifeTime);
    }

    void BarrelDestroy()
    {
        InstantiateFXOnDestroy();
        
        switch (_barrelData.type)
        {
            case BarrelData.EBarrelType.EBuff:
                {
                    GiveBuff();
                }
                break;
            case BarrelData.EBarrelType.EExplosive:
                {
                    Explode();
                }
                break;
        }


        Destroy(gameObject);
    }

    public void InstantiateFXOnDestroy()
    {
        if(_barrelData.VFXPrefabOnDestroy != null)
        {
            var vfx = Instantiate(_barrelData.VFXPrefabOnDestroy, transform.position, transform.rotation);
            if (vfx != null)
            {
                Destroy(vfx, _barrelData.VFXOnDestroyLifeTime);
            }
        }

        AudioManager.SAudioManager.PlaySoundFXClip(_barrelData.breakingBarrelClip, _barrelData.breakingBarrelVolume, transform.position);
    }

    public void TakeDamage(float damage)
    {
        BarrelDestroy();
    }

    private void OnTriggerEnter(Collider other)
    {

        //If colide with Player{}

        BarrelDestroy();
        
    }

    private void Explode()
    {

    }

    private void GiveBuff()
    {

        if (BuffCardManager._SBuffCardManagerScript)
        {
            BuffCardManager._SBuffCardManagerScript.RollBuff();
            Player._SPlayerScript.InstantiateBuffSoundVisualEffect();
        }

    }

}