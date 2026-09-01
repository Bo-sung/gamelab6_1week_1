using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections;
using UnityEngine;

public class UpDownWall : MonoBehaviour
{
    private Vector3 offset;
    public float startDelayTime = 0.0f;
    public float delayTime = 1.0f;
    public float moveSpeed = 1.0f;
    public float range = 1.0f;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        offset = transform.position;
        StartCoroutine(UpDownWallCoroutine());
    }

    IEnumerator UpDownWallCoroutine()
    {
        yield return new WaitForSeconds(startDelayTime);
        while(true)
        {
            float offset = transform.position.y - this.offset.y;
            if (offset >= range)
            {
                moveSpeed = -Mathf.Abs(moveSpeed);
                transform.position = new Vector3 (transform.position.x, Mathf.Clamp(transform.position.y, this.offset.y - range, this.offset.y + range), transform.position.z);
                yield return new WaitForSeconds(delayTime);
            }
            else if (offset <= -range)
            {
                moveSpeed = Mathf.Abs(moveSpeed);
                transform.position = new Vector3 (transform.position.x, Mathf.Clamp(transform.position.y, this.offset.y - range, this.offset.y + range), transform.position.z);
                yield return new WaitForSeconds(delayTime);
            }
            transform.position += Vector3.up * moveSpeed * Time.deltaTime;
            yield return null;
        }
    }
}
