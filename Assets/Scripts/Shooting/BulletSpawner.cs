using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

public class BulletSpawner : MonoBehaviour
{
    [SerializeField] private Bullet _bulletPrefab;
    [SerializeField] private Weapon _weapon;
    [SerializeField] private Target _target;

    [SerializeField, Min(1)] private int _poolCapacity = 5;
    [SerializeField, Min(1)] private int _poolMaxSize = 5;

    [SerializeField, Min(0)] private float _delay = 0.5f;
    [SerializeField, Min(1)] private float _bulletSpeed = 10f;

    private ObjectPool<Bullet> _pool;

    private void Awake()
    {
        _pool = new ObjectPool<Bullet>(
           createFunc: () => Create(),
           actionOnGet: (bullet) => ActOnGet(bullet),
           actionOnRelease: (bullet) => bullet.gameObject.SetActive(false),
           actionOnDestroy: (bullet) => ActnOnDestroy(bullet),
           collectionCheck: true,
           defaultCapacity: _poolCapacity,
           maxSize: _poolMaxSize);
    }

    private void Start()
    {
        StartCoroutine(Spawn());
    }

    private Bullet Create()
    {
        Bullet bullet = Instantiate(_bulletPrefab, _weapon.transform.position, Quaternion.identity);
        bullet.LifetimeWasOver += ReturnToPool;

        return bullet;
    }

    private void ActOnGet(Bullet bullet)
    {
        Vector3 bulletDirection = (_target.transform.position - _weapon.transform.position).normalized;

        bullet.transform.position = _weapon.transform.position + bulletDirection;

        bullet.gameObject.SetActive(true);

        StartCoroutine(bullet.Move(bulletDirection, _bulletSpeed));
    }

    private void ActnOnDestroy(Bullet bullet)
    {
        bullet.LifetimeWasOver -= ReturnToPool;

        Destroy(bullet.gameObject);
    }

    private IEnumerator Spawn()
    {
        var wait = new WaitForSecondsRealtime(_delay);

        while (_target.isActiveAndEnabled)
        {
            yield return wait;

            _pool.Get();
        }
    }

    private void ReturnToPool(Bullet bullet)
    {
        _pool.Release(bullet);
    }
}