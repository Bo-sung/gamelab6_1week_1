using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;


public class Player : MonoBehaviour
{

    [SerializeField]
    PlayerController controller;
    [SerializeField]
    ArrowController arrow;

    public int hitEffectDuration = 1;


    public System.Action<EnemyBase> OnPlayerHit;


    [SerializeField]
    private Volume hitVolume;

    private Coroutine hitEffectCoroutine;

    private void Awake()
    {
        controller = GetComponent<PlayerController>();
        OnPlayerHit += OnHit;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            if (other.TryGetComponent<EnemyBase>(out var enemy))
                OnPlayerHit?.Invoke(enemy);
        }
    }

    private void OnHit(EnemyBase enemy)
    {
        if (hitEffectCoroutine != null)
        {
            StopCoroutine(hitEffectCoroutine);
        }

        StartCoroutine(HitEffectSmooth());
    }

    IEnumerator HitEffectSmooth()
    {
        var time = 0f;
        while (time < hitEffectDuration)
        {
            time += Time.deltaTime;
            hitVolume.weight = Mathf.Lerp(1, 0, time / hitEffectDuration);
            yield return null;
        }
    }
}
