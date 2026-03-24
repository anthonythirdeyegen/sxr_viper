using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using VPcmdIF;

public class ViperHost : MonoBehaviour
{
    [Header("Here's a cool event! Drag anything here!")]
    public UnityEvent ready_event;

    public bool useViper=true;
    public _VPTRACELEV tracelev = _VPTRACELEV.VPCMD_NOTRACE;

    private VPdev vpdev;
    private bool goodstart = false;
    public Int32 sensornum = -1;
    public bool boresight = false;
    public uint sensormap = 0;
    public uint sourcemap = 0;
    public uint sensorcount = 0;
    public uint sourcecount = 0;
    private uint stationmap;


    void Start()
    {
        Debug.Log(" Start : ");
        //if (ready_event == null)
        //    ready_event = new UnityEvent();

        if (useViper)
        {
            vpdev = new VPdev();
            vpdev.tracelev = tracelev;

            print("Viper Wrapper Status : " + vpdev.ErrReport());

            if (vpdev.Connect())
                print("Viper " + (vpdev.Connected() ? " Connected" : "Not Connected"));
            else
            {
                print("Viper Connect Error : " + vpdev.ErrReport());
                goodstart = false;
            }

            if (vpdev.Connected())
            {
                if (!vpdev.GetStationInfo(ref sensormap, ref sensorcount, ref sourcemap, ref sourcecount))
                {
                    print("Viper GetStationMap() Error : " + vpdev.ErrReport());
                    goodstart = false;
                }
                else
                if (!vpdev.SetQuaternion())
                {
                    print("Viper SetQuaternion() Error : " + vpdev.ErrReport());
                    goodstart = false;

                }
                else if (boresight && (!vpdev.BoresightZero((UInt32)sensornum)))
                {
                    print("Viper BoresightZero(" + sensornum + ") Error : " + vpdev.ErrReport());
                    goodstart = false;
                }

                else if (!vpdev.StartCont())
                {
                    print("Viper StartCont Error : " + vpdev.ErrReport());
                    goodstart = false;
                }
                else
                {
                    goodstart = true;
                    if (ready_event != null)
                    {
                        Debug.Log(" ViperHost::Start. Startup complete, invoking ready_event.");
                        ready_event.Invoke();
                    }
                    else
                    {
                        Debug.Log(" ViperHost::ready_event is null");
                    }
                }
            }
        }
    }

    public vpPno Pno()
    {
        return vpdev.pno;
    }

    public bool SensorData( UInt32 sensnum )
    {
        bool bRet = false;
        if (useViper)
        {
            bRet = vpdev.pno.SensorData(sensnum);
        }

        return bRet;
    }

    public bool BoresightAll()
    {
        Int32 sns_all = -1;

        bool bRet = false;
        if (useViper)
        {
            bRet = vpdev.BoresightZero((UInt32)sns_all);
        }
        return bRet;
    }
    public bool UnBoresightAll()
    {
        Int32 sns_all = -1;

        bool bRet = false;
        if (useViper)
        {
            bRet = vpdev.UnBoresight((UInt32)sns_all);
        }
        return bRet;
    }

    void Update()
    {
        if (useViper)
        {
            if (vpdev.Sample())
            {
                sensormap = vpdev.pno.sensmap;
                sensorcount = vpdev.pno.sensorCount;
            }
        }
    }
    void OnDestroy()
    {
        if (useViper && vpdev.Connected())
        {
            vpdev.StopCont();
            vpdev.Disconnect();
            print("OnDestroy: Viper " + (vpdev.Connected() ? " Still Connected!" : "Disconnected"));
            print("OnDestroy: Viper cmdIF Status : " + vpdev.ErrReport());
        }
    }

    public bool Started() { return goodstart; }
}
