using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField]
    private Transform player;

    public float speed = 0.1f;

    private void Update()
    {
        Movement();
    }

    private void Movement()
    {
        this.transform.position = Vector3.MoveTowards(this.transform.position, player.position, speed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            OnDamage();
        }
    }

    public void OnDamage()
    {
        Destroy(this);
    }
}
