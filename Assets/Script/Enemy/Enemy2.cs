using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Enemy2 : EnemyBase
{
    [SerializeField]
    private GameObject head;

    protected int hp = 2;
    private bool isPassing = false;

    public void SetData(Transform target, IScoreHandler scoreHandler, float speed, int hp)
    {
        SetData(target, scoreHandler);
        this.speed = speed;
        this.hp = hp;
    }

    protected override void Movement()
    {
        if (Vector3.Distance(this.transform.position, target.position) >= maxDistance)
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed);
    }

    public void OnChildTriggerEnter(Collider other, string childTag)
    {
        if (childTag == "Head" && other.CompareTag("Arrow") && !isPassing)
        {
            OnHeadshot();
        }
        else if (childTag == "Body" && other.CompareTag("Arrow"))
        {
            OnDamage();
            isPassing = true;
        }
    }

    public void OnChildTriggerOut(Collider other, string childTag)
    {
        if (childTag == "Body")
        {
            isPassing = false;
        }
    }

    protected override void OnDamage()
    {
        hp--;
        if (hp <= 0)
        {
            ApplyScore(20);
            this.gameObject.SetActive(false);
        }
    }

    protected void OnHeadshot()
    {
        ApplyScore(50);
        this.gameObject.SetActive(false);
    }

    protected override void Init()
    {
    }

    protected override void OnHit(Collider other)
    {
        OnDamage();
    }
}