using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VPcmdIF;

public class ViperPlayerLook : MonoBehaviour
{
    public ViperHost vphost;
    public UInt32 sensornum = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (vphost.SensorData(sensornum))
        {
            RotateCamera();
        }
    }

    void RotateCamera()
    {
        vpPno pno = vphost.Pno();
        //float[] ori = pno.Ori(sensornum);
        //float[] pos = pno.PosRaw(sensornum);
        //Quaternion Qraw = pno.QuatRaw(sensornum);
        //Vector3 eraw = Qraw.eulerAngles;

        Quaternion QLH = pno.QuatLH(sensornum);

        transform.rotation = QLH; 
    }
}
