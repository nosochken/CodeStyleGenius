using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Bullet : MonoBehaviour
{
    private Rigidbody _rigidbody;

    [SerializeField, Min(1)] private float _lifetime = 2f;

    public event Action<Bullet> LifetimeWasOver;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.useGravity = false;
    }

    private void OnEnable()
    {
        StartCoroutine(Expire());
    }

    public IEnumerator Move(Vector3 direction, float speed)
    {
        while (isActiveAndEnabled)
        {
            _rigidbody.transform.up = direction;
            _rigidbody.velocity = direction * speed;

            yield return null;
        }
    }

    private IEnumerator Expire()
    {
        var wait = new WaitForSecondsRealtime(_lifetime);
        yield return wait;

        LifetimeWasOver?.Invoke(this);
    }
}