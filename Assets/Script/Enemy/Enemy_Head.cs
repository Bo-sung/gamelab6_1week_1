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
    private bool isPassing = false;

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
            if(this.enabled)
                StartCoroutine(Blink());
            isPassing = true;
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
    }

    private IEnumerator Blink()
    {

        headRender.sharedMaterial = blinkHeadMat;
        bodyRender.sharedMaterial = blinkBodyMat;

        yield return new WaitForSeconds(2f);

        headRender.sharedMaterial = originHeadMat;
        bodyRender.sharedMaterial = originBodyMat;
    }
}