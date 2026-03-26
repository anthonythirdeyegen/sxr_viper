using System;
using UnityEngine;
using VPcmdIF;

public class ViperTrackingOrigin : MonoBehaviour
{
    public ViperHost vphost;

    [Header("Shared Position Mapping")]
    public bool applyPositionAxisSwap = true;
    public bool invertX = true;
    public bool invertY = true;
    [Tooltip("Set to 0.001 if PosRaw is millimeters, 1.0 if meters")]
    public float positionScaleToMeters = 1.0f;

    [Header("Shared Rotation Mapping")]
    public bool applyRotAxisSwap = true;
    public bool invertRotX = false;
    public bool invertRotY = false;
    public bool invertRotZ = false;

    [Header("Shared Recenter")]
    public KeyCode recenterKey = KeyCode.R;
    public UInt32 originSensor = 0;   //head sensor

    private bool haveOriginPosition = false;
    private Vector3 originSensorPos;

    private bool haveFrame = false;
    private vpPno currentFrame;

    void Update()
    {
        if (vphost == null)
            return;

        if (!vphost.SensorData(originSensor))
            return;

        currentFrame = vphost.Pno();
        haveFrame = true;

        if (!haveOriginPosition)
        {
            originSensorPos = GetMappedPosition(originSensor);
            haveOriginPosition = true;
        }

        if (Input.GetKeyDown(recenterKey))
        {
            originSensorPos = GetMappedPosition(originSensor);
            haveOriginPosition = true;
        }
    }

    public bool HasFrame()
    {
        return haveFrame;
    }

    public bool HasOrigin()
    {
        return haveOriginPosition;
    }

    public vpPno Frame()
    {
        return currentFrame;
    }

    public Vector3 GetMappedPosition(UInt32 sensornum)
    {
        if (!haveFrame)
            return Vector3.zero;

        float[] pr = currentFrame.PosRaw(sensornum);
        Vector3 sensorNow = new Vector3(pr[0], pr[1], pr[2]);

        if (applyPositionAxisSwap)
        {
            sensorNow = new Vector3(
                sensorNow.z,
                sensorNow.x,
                sensorNow.y
            );
        }

        if (invertY) sensorNow.y = -sensorNow.y;
        if (invertX) sensorNow.x = -sensorNow.x;

        return sensorNow * positionScaleToMeters;
    }

    public Vector3 GetOriginRelativePosition(UInt32 sensornum)
    {
        Vector3 p = GetMappedPosition(sensornum);

        if (!haveOriginPosition)
            return p;

        return p - (originSensorPos);
    }

    public Quaternion GetMappedRotation(UInt32 sensornum)
    {
        if (!haveFrame)
            return Quaternion.identity;

        Quaternion q = currentFrame.QuatLH(sensornum);

        if (applyRotAxisSwap || invertRotX || invertRotY || invertRotZ)
            q = ApplyRotationAxisMap(q);

        return q;
    }

    private Quaternion ApplyRotationAxisMap(Quaternion q)
    {
        float ang;
        Vector3 ax;
        q.ToAngleAxis(out ang, out ax);

        if (ax.sqrMagnitude <= 1e-8f)
            return Quaternion.identity;

        ax.Normalize();

        if (applyRotAxisSwap)
        {
            ax = new Vector3(ax.y, ax.x, ax.z);
        }

        if (invertRotX) ax.x = -ax.x;
        if (invertRotY) ax.y = -ax.y;
        if (invertRotZ) ax.z = -ax.z;

        return Quaternion.AngleAxis(ang, ax);
    }
}