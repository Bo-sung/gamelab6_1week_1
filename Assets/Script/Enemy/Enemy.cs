using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    // 접근 속도
    public float speed = 0.1f;
    // 최대 접근 거리
    public float maxDistance = 2;

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        if(Vector3.Distance(this.transform.position, player.position) >= maxDistance)
        this.transform.position = Vector3.MoveTowards(this.transform.position, player.position, speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Arrow"))
        {
            OnDamage();
        }
    }

    public void OnDamage()
    {
        Destroy(this);
    }
}
