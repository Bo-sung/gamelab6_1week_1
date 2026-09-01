using System.Collections;
using UnityEngine;

public class TitleCarMovement : MonoBehaviour
{
    public Rigidbody rb;
    public float force = 200000f;
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        StartCoroutine(MoveCar());
    }


    IEnumerator MoveCar()
    {
        yield return new WaitForSeconds(0.05f);
        rb.AddForce(transform.forward * force);
    }
}
