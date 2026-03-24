using System;
using UnityEngine;
using VPcmdIF;

public class ViperHandPose: MonoBehaviour
{
    public ViperHost vphost;
    public UInt32 sensornum = 1;   // Hand tracker on sensor channel #2

    [Header("Position")]
    public bool applyPosition = true;

    [Header("Position Axis Swap")]
    public bool applyPositionAxisSwap = true;

    [Header("Rotation Axis Swap")]
    public bool applyRotAxisSwap = true;

    [Header("Invert Y Axis")]
    public bool invertY = true;

    [Header("Invert X Axis")]
    public bool invertX = true;

    [Tooltip("Set to 0.001 if PosRaw is millimeters, 1.0 if meters")]
    public float positionScaleToMeters = 0.001f;

    public bool invertRotX = false;
    public bool invertRotY = false;
    public bool invertRotZ = false;

    [Header("Recenter")]
    public KeyCode recenterKey = KeyCode.T;   // separate key from head recenter if desired

    [Header("Rotation Calibration")]
    public bool calibrateOnFirstSample = true;
    private bool haveCalib = false;
    private Quaternion qMount = Quaternion.identity;

    private Vector3 handStartLocal;
    private bool haveSensorRef = false;
    private Vector3 sensorStart;

    private Quaternion handStartRot;
    private bool haveSensorRotRef = false;
    private Quaternion sensorStartRot;

    void Start()
    {
        // Scene-authored starting position/rotation
        handStartLocal = transform.localPosition;
        handStartRot = transform.rotation;
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

    void Update()
    {
        if (vphost == null)
            return;

        if (!vphost.SensorData(sensornum))
            return;

        vpPno pno = vphost.Pno();

        /* -------- ROTATION -------- */
        Quaternion sensorNowRot = pno.QuatLH(sensornum);

        if (applyRotAxisSwap || invertRotX || invertRotY || invertRotZ)
        {
            sensorNowRot = ApplyRotationAxisMap(sensorNowRot);
        }

        if (!haveSensorRotRef)
        {
            sensorStartRot = sensorNowRot;
            haveSensorRotRef = true;

            if (calibrateOnFirstSample)
            {
                qMount = handStartRot * Quaternion.Inverse(sensorStartRot);
                haveCalib = true;
            }
            else
            {
                haveCalib = false;
            }
        }

        if (haveCalib)
        {
            transform.rotation = qMount * sensorNowRot;
        }
        else
        {
            Quaternion deltaRot = sensorNowRot * Quaternion.Inverse(sensorStartRot);
            transform.rotation = deltaRot * handStartRot;
        }

        /* -------- POSITION -------- */
        if (applyPosition)
        {
            float[] pr = pno.PosRaw(sensornum);
            Vector3 sensorNow = new Vector3(pr[0], pr[1], pr[2]);

            if (applyPositionAxisSwap)
            {
                sensorNow = new Vector3(
                    sensorNow.z,  // x
                    sensorNow.x,  // y
                    sensorNow.y   // z
                );
            }

            if (invertY)
                sensorNow.y = -sensorNow.y;

            if (invertX)
                sensorNow.x = -sensorNow.x;

            if (!haveSensorRef)
            {
                sensorStart = sensorNow;
                haveSensorRef = true;
            }

            Vector3 delta = sensorNow - sensorStart;
            delta *= positionScaleToMeters;

            transform.localPosition = handStartLocal + delta;
        }

        /* -------- RECENTER POSITION ONLY -------- */
        if (applyPosition && Input.GetKeyDown(recenterKey))
        {
            float[] pr = pno.PosRaw(sensornum);
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

            sensorStart = sensorNow;
            haveSensorRef = true;
        }
    }
}