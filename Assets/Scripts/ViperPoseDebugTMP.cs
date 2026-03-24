using UnityEngine;
using TMPro;
using VPcmdIF;
using System;

public class ViperPoseDebugTMP : MonoBehaviour
{
    public ViperHost vphost;
    public UInt32 sensornum = 0;

    public Transform headTransform;   // Head or Camera
    public TextMeshProUGUI debugText;

    void Update()
    {
        if (vphost == null || debugText == null || headTransform == null)
            return;

        if (!vphost.SensorData(sensornum))
            return;

        vpPno pno = vphost.Pno();

        float[] pr = pno.PosRaw(sensornum);
        Vector3 sensorPos = new Vector3(pr[0], pr[1], pr[2]);

        Quaternion sensorQuat = pno.QuatLH(sensornum);
        Vector3 sensorEuler = sensorQuat.eulerAngles;

        /* -------- HEAD TRANSFORM -------- */
        Vector3 headLocalPos = headTransform.localPosition;
        Vector3 headWorldPos = headTransform.position;

        Vector3 headLocalEuler = headTransform.localEulerAngles;
        Vector3 headWorldEuler = headTransform.eulerAngles;

        /* -------- DEBUG TEXT -------- */
        debugText.text =
            "= VIPER POSE DEBUG =\n\n" +

            "Sensor PosRaw:\n" +
            $"  X: {sensorPos.x:F3}\n" +
            $"  Y: {sensorPos.y:F3}\n" +
            $"  Z: {sensorPos.z:F3}\n\n" +

            "Sensor Rotation:\n" +
            $"  Quat:\n" +
            $"    X: {sensorQuat.x:F4}\n" +
            $"    Y: {sensorQuat.y:F4}\n" +
            $"    Z: {sensorQuat.z:F4}\n" +
            $"    W: {sensorQuat.w:F4}\n" +
            $"  Euler (deg):\n" +
            $"    X: {sensorEuler.x:F1}\n" +
            $"    Y: {sensorEuler.y:F1}\n" +
            $"    Z: {sensorEuler.z:F1}\n\n" +

            "Head Local Position:\n" +
            $"  X: {headLocalPos.x:F3}\n" +
            $"  Y: {headLocalPos.y:F3}\n" +
            $"  Z: {headLocalPos.z:F3}\n\n" +

            "Head World Position:\n" +
            $"  X: {headWorldPos.x:F3}\n" +
            $"  Y: {headWorldPos.y:F3}\n" +
            $"  Z: {headWorldPos.z:F3}\n\n" +

            "Head Rotation:\n" +
            $"  Local Euler:\n" +
            $"    X: {headLocalEuler.x:F1}\n" +
            $"    Y: {headLocalEuler.y:F1}\n" +
            $"    Z: {headLocalEuler.z:F1}\n" +
            $"  World Euler:\n" +
            $"    X: {headWorldEuler.x:F1}\n" +
            $"    Y: {headWorldEuler.y:F1}\n" +
            $"    Z: {headWorldEuler.z:F1}\n";
    }
}