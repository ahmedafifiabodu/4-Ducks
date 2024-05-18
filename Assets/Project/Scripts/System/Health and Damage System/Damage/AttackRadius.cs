using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class AttackRadius : MonoBehaviour
{
    private List<IDamagable> _damagables = new List<IDamagable>();

    [SerializeField] private float _damageAmount = 10;
    [SerializeField] private float _attackDelay = 0.5f;
    UnityEvent<IDamagable> OnAttack;
    private Coroutine _attackCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if(damagable != null)
        {
            _damagables.Add(damagable);
        }
        if(_attackCoroutine == null)
        {
            _attackCoroutine = StartCoroutine(Attack());
        }
    }
    private void OnTriggerExit(Collider other)
    {
        IDamagable damagable = other.GetComponent<IDamagable>();
        if( damagable != null ) 
        {
            _damagables.Remove(damagable );
        }
        if (_damagables.Count == 0)
        {
            StopCoroutine(_attackCoroutine);
            _attackCoroutine = null;
        }
    }
    private IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds( _attackDelay );
        yield return wait;

        IDamagable closestDamagable = null;
        float closestDistance = float.MaxValue;

        while(_damagables.Count > 0)
        {
            for(int i = 0;  i < _damagables.Count; i++) 
            {
                Transform damagableTransform = _damagables[i].GetTransform();
                float distance = Vector3.Distance(transform.position, damagableTransform.position);
                if(distance <= closestDistance ) 
                {
                    closestDistance = distance;
                    closestDamagable = _damagables[i];
                }
            }
            if(closestDamagable != null)
            {
                OnAttack?.Invoke(closestDamagable);
                closestDamagable.TakeDamage(_damageAmount);
            }
            closestDamagable = null;
            closestDistance = float.MaxValue;

            yield return wait;
            
            _damagables.RemoveAll(DisableDamageables);
        }
        _attackCoroutine = null;
    }
    private bool DisableDamageables(IDamagable damagable)
    {
        return damagable != null && damagable.GetTransform().gameObject.activeSelf;
    }
}
