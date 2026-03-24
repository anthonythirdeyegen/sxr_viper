using System;
using UnityEngine;
using VPcmdIF;

public class ViperHeadPose6Dof_NoSmoothing : MonoBehaviour
{
    public ViperHost vphost;
    public UInt32 sensornum = 0;

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
    public float positionScaleToMeters = 0.001f; // not used unless you choose to (kept for consistency)

    [Tooltip("Hard clamp for translation (meters). Set <= 0 to disable.")]
    public float maxRadiusMeters = 2.0f; // not used (kept)

    public bool invertRotX = false;
    public bool invertRotY = false;
    public bool invertRotZ = false;

    [Header("Recenter")]
    public KeyCode recenterKey = KeyCode.R;

    [Header("Rotation Calibration")]
    public bool calibrateOnFirstSample = true;   // leave true
    private bool haveCalib = false;
    private Quaternion qMount = Quaternion.identity; // sensor -> head alignment

    private Vector3 headStartLocal;
    private bool haveSensorRef = false;
    private Vector3 sensorStart;

    private bool haveOrigin = false; // kept (unused)
    private Vector3 originPos = Vector3.zero; // kept (unused)

    private Quaternion headStartRot;
    private bool haveSensorRotRef = false;
    private Quaternion sensorStartRot;

    void Start()
    {
        // Scene-authored starting position/rotation
        headStartLocal = transform.localPosition;
        headStartRot = transform.rotation;
    }

    private Quaternion ApplyRotationAxisMap(Quaternion q)
    {
        // This is a DEBUG remap: swap X <-> Z on the rotation axis, plus optional sign flips.
        // Done in axis-angle space (do NOT swap Euler).
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

        /* -------- ROTATION (calibrated, no smoothing) -------- */

        // Read sensor rotation
        Quaternion sensorNowRot = pno.QuatLH(sensornum);

        // Optional debug axis swap / inversion on the sensor quaternion itself
        if (applyRotAxisSwap || invertRotX || invertRotY || invertRotZ)
        {
            sensorNowRot = ApplyRotationAxisMap(sensorNowRot);
        }

        // Capture initial sensor rotation reference (after any remap above)
        if (!haveSensorRotRef)
        {
            sensorStartRot = sensorNowRot;
            haveSensorRotRef = true;

            if (calibrateOnFirstSample)
            {
                // Align sensor frame to the head's scene-authored frame at start
                qMount = headStartRot * Quaternion.Inverse(sensorStartRot);
                haveCalib = true;
            }
            else
            {
                haveCalib = false;
            }
        }

        // Apply rotation
        if (haveCalib)
        {
            // Best for head tracking: absolute aligned
            transform.rotation = qMount * sensorNowRot;
        }
        else
        {
            // Fallback: relative delta from start
            Quaternion deltaRot = sensorNowRot * Quaternion.Inverse(sensorStartRot);
            transform.rotation = deltaRot * headStartRot;
        }

        /* -------- POSITION (relative to scene start, no smoothing) -------- */
        if (applyPosition)
        {
            float[] pr = pno.PosRaw(sensornum);
            Vector3 sensorNow = new Vector3(pr[0], pr[1], pr[2]);

            if (applyPositionAxisSwap) // Change this to match source orientation.
            {
                sensorNow = new Vector3(
                    sensorNow.z,  // x
                    sensorNow.x,  // y
                    sensorNow.y   // z
                );
            }

            if (invertY)
            {
                sensorNow.y = -sensorNow.y;
            }

            if (invertX)
            {
                sensorNow.x = -sensorNow.x;
            }

            if (!haveSensorRef)
            {
                sensorStart = sensorNow;   // capture sensor reference once (post-map)
                haveSensorRef = true;
            }

            Vector3 delta = sensorNow - sensorStart;

            delta *= positionScaleToMeters;

            // Apply relative movement from scene-authored start
            transform.localPosition = headStartLocal + delta;
        }

        /* -------- RECENTER POSITION ONLY -------- */
        if (applyPosition && Input.GetKeyDown(recenterKey))
        {
            // Recenter = reset the sensorStart to the current mapped position sample
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
