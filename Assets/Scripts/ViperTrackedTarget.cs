using System;
using UnityEngine;

public class ViperTrackedTarget : MonoBehaviour
{
    public ViperTrackingOrigin trackingOrigin;
    public UInt32 sensornum = 0;

    [Header("Tracking")]
    public bool applyPosition = true;
    public bool applyRotation = true;

    [Header("Rotation Calibration")]
    public bool calibrateOnFirstSample = true;

    private bool haveRotRef = false;
    private Quaternion sensorStartRot;
    private Quaternion targetStartRot;
    private Quaternion qMount = Quaternion.identity;
    private bool haveCalib = false;

    void Start()
    {
        targetStartRot = transform.rotation;
    }

    void Update()
    {
        if (trackingOrigin == null)
            return;

        if (!trackingOrigin.HasFrame())
            return;

        if (applyPosition)
        {
            // Shared tracker-space position.
            // Every sensor is now in the SAME coordinate frame.
            transform.localPosition = trackingOrigin.GetOriginRelativePosition(sensornum);
        }

        if (applyRotation)
        {
            Quaternion sensorNowRot = trackingOrigin.GetMappedRotation(sensornum);

            if (!haveRotRef)
            {
                sensorStartRot = sensorNowRot;
                haveRotRef = true;

                if (calibrateOnFirstSample)
                {
                    qMount = targetStartRot * Quaternion.Inverse(sensorStartRot);
                    haveCalib = true;
                }
            }

            if (haveCalib)
            {
                transform.rotation = qMount * sensorNowRot;
            }
            else
            {
                Quaternion deltaRot = sensorNowRot * Quaternion.Inverse(sensorStartRot);
                transform.rotation = deltaRot * targetStartRot;
            }
        }
    }
}