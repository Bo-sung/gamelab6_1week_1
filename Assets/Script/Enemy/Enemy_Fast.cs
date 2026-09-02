using UnityEngine;

public class Enemy_Fast : EnemyBase
{
    public void SetData(Transform target, IScoreHandler scoreHandler, float speed)
    {
        SetData(target, scoreHandler);
        this.speed = speed;
    }

    protected override void Movement()
    {
        if (Vector3.Distance(this.transform.position, target.position) >= maxDistance)
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed);
    }

    protected override void OnDamage()
    {
        ApplyScore(20);
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
