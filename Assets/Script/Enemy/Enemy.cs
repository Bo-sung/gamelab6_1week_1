using UnityEngine;

public class Enemy : EnemyBase
{
    protected override void Movement()
    {
        if (Vector3.Distance(this.transform.position, target.position) >= maxDistance)
            this.transform.position = Vector3.MoveTowards(this.transform.position, target.position, speed);
    }

    protected override void OnDamage()
    {
        ApplyScore(10);
        this.gameObject.SetActive(false);
    }

    protected override void Init()
    {

    }

    public void SetData(Transform target, IScoreHandler scoreHandler, float speed)
    {
        SetData(target, scoreHandler);
        this.speed = speed;
    }


    protected override void OnHit(Collider other)
    {
        OnDamage();
    }
}
