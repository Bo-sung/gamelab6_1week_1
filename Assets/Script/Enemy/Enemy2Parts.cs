using UnityEngine;

public class Enemy2Parts : MonoBehaviour
{
    private Enemy2 parents;

    private void Awake()
    {
        parents = GetComponentInParent<Enemy2>();
    }

    private void OnTriggerEnter(Collider other)
    {
        parents.OnChildTriggerEnter(other, gameObject.tag);
    }

    private void OnTriggerExit(Collider other)
    {
        parents.OnChildTriggerOut(other, gameObject.tag);
    } 
}
