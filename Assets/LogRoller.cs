using System;
using UnityEngine;

public class LogRoller : MonoBehaviour
{
    [SerializeField] Vector3 originalPos;
    [SerializeField] Vector3 originalRot;
    [SerializeField] Vector3 animPos;
    [SerializeField] Vector3 animRot;

    [SerializeField] float rotSpeed;
    [SerializeField] GameObject hitbox;

    void OnEnable()
    {
        transform.localPosition = animPos;
        transform.localRotation = Quaternion.Euler(animRot);
        hitbox.transform.localPosition = animPos;
        hitbox.transform.localRotation = Quaternion.Euler(Vector3.up * 90);
    }

    void OnDisable()
    {
        transform.localPosition = originalPos;
        transform.localRotation = Quaternion.Euler(originalRot);
        hitbox.transform.localPosition = originalPos;
        hitbox.transform.localRotation = Quaternion.Euler(Vector3.zero);
    }

    void Update()
    {
        this.transform.Rotate(Vector3.up * rotSpeed * Time.deltaTime);
    }

}
