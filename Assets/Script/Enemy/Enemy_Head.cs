using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SocialPlatforms.Impl;

public class Enemy_Head : EnemyBase
{
    [SerializeField]
    private GameObject head;

    [SerializeField]
    private NavMeshAgent navMeshAgent;

    protected int hp = 2;

    [SerializeField]
    private Material originHeadMat;
    [SerializeField]
    private Material blinkHeadMat;
    [SerializeField]
    private Material originBodyMat;
    [SerializeField]
    private Material blinkBodyMat;

    [SerializeField]
    private MeshRenderer headRender;
    [SerializeField]
    private MeshRenderer bodyRender;

    private float blinkTime = 2f;
    private float blinkTimer = 0f;
    private bool blinkEnabled = false;

    public void SetData(Transform target, IScoreHandler scoreHandler, GameScene.IMiniMap miniMapUI, int hp)
    {
        SetData(target, scoreHandler, miniMapUI);
        this.hp = hp;
    }

    protected override void Movement()
    {
        if (Vector3.Distance(this.transform.position, target.position) >= maxDistance)
            navMeshAgent.destination = target.position;
        else
            navMeshAgent.destination = transform.position;

        if (blinkTimer < 0f && blinkEnabled)
        {
            blinkEnabled = false;
            headRender.sharedMaterial = originHeadMat;
            bodyRender.sharedMaterial = originBodyMat;
        }
    }

    public void OnChildTriggerEnter(Collider other, string childTag)
    {
        if (childTag == "Head" && other.CompareTag("Arrow"))
        {
            OnHeadshot();
        }
        else if (childTag == "Body" && other.CompareTag("Arrow"))
        {
            OnDamage();
            blinkEnabled = true;
            blinkTimer = blinkTime;
            headRender.sharedMaterial = blinkHeadMat;
            bodyRender.sharedMaterial = blinkBodyMat;
        }
    }

    protected override void OnDisable()
    {
        StopAllCoroutines();
        headRender.sharedMaterial = originHeadMat;
        bodyRender.sharedMaterial = originBodyMat;
    }

    protected override void OnEnable()
    {
        hp = 2;
    }

    public void OnChildTriggerOut(Collider other, string childTag)
    {
        if (childTag == "Body")
        {

        }
    }

    protected override void OnDamage()
    {
        hp--;
        ApplyScore(10);
        if (hp <= 0)
        {
            Die();
        }
    }

    protected void OnHeadshot()
    {
        ApplyScore(50);
        Die();
    }

    protected override void Init()
    {
    }

    protected override void OnHit(Collider other)
    {
    }
}