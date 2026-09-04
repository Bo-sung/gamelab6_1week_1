using UnityEngine;

public class ArrowShaker : MonoBehaviour
{
    float shakeAngleMax = 1f;
    float shakeAngleMin = -1f;

    [SerializeField]
    float curTimerVar = 0;
    float MaxTimeVar = 10000;
    float accumVal = 0;

    void Update()
    {
        curTimerVar += Time.deltaTime * accumVal;
        // 사인파로 0 1 순환
        float lerpSinVal = Mathf.Sin(curTimerVar * 3);
        // 사인파로 0 1 순환
        float lerpCosnVal = Mathf.Cos(curTimerVar * 3);
        // 사인파로 0 1 순환
        float lerpTanVal = Mathf.Tan(curTimerVar * 3);
        // 범위 회전 적용
        float curSinAngle = Mathf.Lerp(shakeAngleMin * accumVal, shakeAngleMax * accumVal, lerpSinVal * lerpSinVal);
        float curSin2Angle = Mathf.Lerp(shakeAngleMax * accumVal, shakeAngleMin * accumVal, lerpSinVal * lerpSinVal);
        float curCosAngle = Mathf.Lerp(shakeAngleMin * accumVal, shakeAngleMax * accumVal, lerpSinVal * lerpSinVal);
        float curTanAngle = Mathf.Lerp(shakeAngleMin * accumVal, shakeAngleMax * accumVal, lerpSinVal * lerpSinVal);

        transform.localRotation = Quaternion.Euler(curSinAngle, transform.localRotation.y, curSin2Angle);
        accumVal += Time.deltaTime;
    }
}
