using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class Enemy2 : MonoBehaviour
{
    [SerializeField]
    private Transform player;
    [SerializeField]
    private GameObject head;

    private ScoreUI scorUI;

    public int hp = 2;
    private bool isPassing = false;
    // 접근 속도
    public float speed = 0.15f;
    // 최대 접근 거리
    public float maxDistance = 2;
    private void Awake()
    {
        player = FindObjectOfType<Player>().transform;
        scorUI = FindObjectOfType<ScoreUI>();
    }

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if (Vector3.Distance(this.transform.position, player.position) >= maxDistance)
            this.transform.position = Vector3.MoveTowards(this.transform.position, player.position, speed);
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

    public void OnDamage()
    {
        hp--;
        if (hp <= 0)
        {
            scorUI?.ApplyScore(20);
            this.gameObject.SetActive(false);
        }
    }

    public void OnHeadshot()
    {
        scorUI?.ApplyScore(50);
        this.gameObject.SetActive(false);
    }
}