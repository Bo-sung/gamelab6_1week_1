using UnityEngine;

public static class CommonUtils
{
    public static (float yaw, float pitch) ReCalcInput(Vector3 direction, float minPitch = -20f, float maxPitch = 70f)
    {
        float yaw = -999f;
        float pitch = -999f;
        if (direction.sqrMagnitude < 0.000001f)
            return (yaw, pitch);


        float horizontalMagnitude =
            Mathf.Sqrt
            (
                direction.x * direction.x +
                direction.z * direction.z
            );

        float targetPitch = Mathf.Atan2(-direction.y, horizontalMagnitude) * Mathf.Rad2Deg;

        // 수직에 너무 가까우면 yaw는 기존 값 유지
        if (horizontalMagnitude > 0.0001f)
        {
            float targetYaw = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

            yaw += Mathf.DeltaAngle(yaw, targetYaw);
        }

        pitch = Mathf.Clamp
            (
            targetPitch,
            minPitch,
            maxPitch
        );


        return (yaw, pitch);
    }
}
