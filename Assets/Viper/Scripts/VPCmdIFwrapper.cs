
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

namespace VPcmdIF
{
    enum _VPERR : UInt32
    {
        E_VP_SUCCESS = 0           /**< 0, No error occured. */
        , E_VPERR_NAK_RESP            /**< 1, Tracker rejected the command. */
        , E_VPERR_MEMORY_ALLOCATION   /**< 2, Memory could not be allocated. */
        , E_VPERR_INVALID_CONTEXT     /**< 3, The specified context is invalid. */
        , E_VPERR_INVALID_ACTION      /**< 4, The specified action is invalid. */
        , E_VPERR_INVALID_SENSOR      /**< 5, The specified sensor is invalid. */
        , E_VPERR_INVALID_SENSOR_ACT  /**< 6, The specified sensor and action combination is invalid.*/
        , E_VPERR_INVALID_PARAMS      /**< 7, An additional specified parameter is invalid. */
        , E_VPERR_INVALID_SIZE        /**< 8, The size field of the received frame is invalid. */
        , E_VPERR_INVALID_CRC         /**< 9, The CRC of the received frame is invalid */
        , E_VPERR_TIMEOUT             /**< 10, A timeout was reached before all the data could be recieved. */
        , E_VPERR_PLATFORM            /**< 11, Indicates a system error in hi bits. */
        , E_VPERR_LIBUSB              /**< 12, Indicates an error received from libusb library in high bits */
        , E_VPERR_UNKNOWN             /**< 13, The error is unknown. */
        , E_VPERR_NOTFOUND            /**< 14, Device not found */
        , E_VPERR_SERIO               /**< 15, Error received from VPserIO library in high bits */
        , E_VPERR_CMDRSP_MISMATCH     /**< 16, Command-Response mismatch */
        , E_VPERR_BUF_TOO_SMALL       /**< 17, Supplied buffer is too small */

        //, E_VPERR_MAX/**< 15, Not an actual error, marks the end of the error list. */

        , E_CTXERR_INVALID_DEVHND //= E_VPERR_MAX
        , E_CTXERR_THREAD_ALREADY_RUNNING
        , E_CTXERR_THREAD_ALREADY_STOPPED
        , E_CTXERR_INVALID_CONDITION
        , E_CTXERR_FRAME_HEADER_ERROR
        , E_CTXERR_FRAME_CRC_ERROR
        , E_CTXERR_MEMALLOC_ERROR
        , E_CTXERR_HUB_DUPLICATE
        , E_CTXERR_CMDRSP_NOMATCH
        , E_CTXERR_HUBIO_NOT_STARTED
        , E_CTXERR_HUBDISC_NOT_STARTED
        , E_CTXERR_INVALID_MASK
        , E_CTXERR_NO_HUB_DISCOVERED
        , E_CTXERR_CMDIO_THREAD_ERROR
        , E_CTXERR_HUB_RECV_QUEUE_EMPTY
        , E_CTXERR_HUB_FILTERED
        , E_CTXERR_HUB_BUFFIO_NOT_READY
        , E_CTXERR_HUB_SOCKET_ERROR
        , E_CTXERR_HUB_CMD_SOCKET_NOT_READY
        , E_CTXERR_NO_MORE_DATA
        //, E_CTXERR_MAX

        , E_DEVIOERR_NO_CNX //= E_CTXERR_MAX
        , E_DEVIOERR_FRAME_HEADER

        //, E_DEVIOERR_USB_MIN
        , E_DEVIOERR_USB_WRITERR //= E_DEVIOERR_USB_MIN
        , E_DEVIOERR_USB_READERR
        //, E_DEVIOERR_USB_MAX = E_DEVIOERR_USB_READERR

        //, E_DEVIOERR_SER_MIN
        , E_DEVIOERR_SER_WRITERR //= E_DEVIOERR_SER_MIN
        , E_DEVIOERR_SER_READERR
        //, E_DEVIOERR_SER_MAX = E_DEVIOERR_USB_READERR

        , E_DEVIOERR_MAX
    }

    enum _VPACT : UInt32
    {
        CMD_ACTION_SET = 0           /**< Used with a command and payload to change state. */
        , CMD_ACTION_GET             /**< Used with a command to request current state. */
        , CMD_ACTION_RESET           /**< Used with a command to restore default state. */
        , CMD_ACTION_ACK             /**< Command received and processed, possible payload included. */
        , CMD_ACTION_NAK             /**< Command received, error found: always includes NAK_INFO struct in payload. */

        , CMD_ACTION_MAX

    }

    public enum _VPPOSU : Byte
    {
        POS_INCH = 0 /**< Inches. */
        , POS_FOOT   /**< Feet. */
        , POS_CM     /**< Centimeters. */
        , POS_METER  /**< Meters. */

        , POS_MAX
    }
    public enum _VPORIU : Byte
    {
        ORI_EULER_DEGREE = 0 /**< Euler angles in degrees. */
       , ORI_EULER_RADIAN   /**< Euler angles in radians. */
       , ORI_QUATERNION     /**< Quaternions. */

       , ORI_MAX
    }

    public enum _VPTRACELEV : UInt32
    {
        VPCMD_NOTRACE
        , VPCMD_TRACE_1_MIN
        , VPCMD_TRACE_2_COMPACT
        , VPCMD_TRACE_3_NORMALIO
        , VPCMD_TRACE_4_ALLIO
        , VPCMD_TRACE_5_CODETRACE = 5
        , VPCMD_TRACE_7_VERBOSE = 7
    }
    
    public class VPcmdErr
    {
        public static string ToString( Int32 r )
        {
            _VPERR err = (_VPERR)r;
            return ((_VPERR)r).ToString();
        }
        public static string Report(Int32 r)
        {
            return r.ToString() + " : " + ToString(r);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct vpframeinfo
    {
        public IntPtr pF;
        public UInt32 size;
        public UInt32 uiFCountRx;
        public UInt32 iFrameErr;
        public vpframeinfo( UInt32 p=0, UInt32 sz=0, UInt32 fc=0, UInt32 e=0)
        {
            pF = (IntPtr)p;
            size = sz;
            uiFCountRx = fc;
            iFrameErr = e;
        }
    }
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct vphubbf
    {
        [FieldOffset(0)]
        System.Byte digio;
        [FieldOffset(1)]
        System.Byte srclock;
        [FieldOffset(2)]
        System.UInt16 res;

        public vphubbf(UInt32 v = 0)
        {
            digio = (byte)(v & 0xff);
            srclock = (byte)((v & 0xff00)>>8);
            res = (UInt16)((v & 0xffff0000)>>16);
        }


        public static implicit operator vphubbf(UInt32 v)
        {
            return new vphubbf(v);
        }
    }



    [StructLayout(LayoutKind.Explicit, Pack=1)]
    public struct vpsensbf
    {
        [FieldOffset(0)] System.Byte sensnum;
        [FieldOffset(1)] System.Byte bfUnits;
        [FieldOffset(2)] System.UInt16 res;

        public vpsensbf( UInt32 v=0)
        {
            sensnum = (byte)(v & 0xff);
            bfUnits = (byte)((v & 0xff00)>>8);
            res = 0;
        }

        public byte Sensor()
        {
            return sensnum;
        }
        public _VPPOSU posunits()
        {
            return (_VPPOSU)(bfUnits & 0x0f);
        }
        public _VPORIU oriunits()
        {
            return (_VPORIU)((bfUnits & 0xf0)>>4);
        }
        public static _VPORIU oriunits(UInt32 bf)
        {
            return (_VPORIU)((bf & 0xf0) >> 4);
        }
        public static bool Euler(UInt32 bf)
        {
            return oriunits(bf) != _VPORIU.ORI_QUATERNION;
        }
        public byte units( _VPPOSU pos, _VPORIU ori)
        {
            UInt32 u = (UInt32)ori;
            u = u << 4;
            u += (UInt32)pos;
            bfUnits = (byte)u;
            return bfUnits;
        }

        public static UInt32 sens(UInt32 bf)
        {
            return (bf & 0xff);
        }
        public static implicit operator vpsensbf(UInt32 v)
        {
            return new vpsensbf(v);
        }
    }

    public struct sdat
    {
        public UInt32 bf;
        public float X, Y, Z, A, E, R, W;
    }
    public struct vpsensframe
    {
        public UInt32 bf;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] pos;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public float[] ori;

        public UInt32 Sensor()
        {
            return vpsensbf.sens(bf);
        }
        public vpsensframe( UInt32 v=0 )
        {
            bf = v;
            pos = new float[3];
            ori = new float[4];
        }

        public float [] Euler()
        {
            if (vpsensbf.Euler(bf))
                return ori;

            else
                return ori;
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct pnohdr
    {
        public UInt32 seuid;
        public UInt32 frame;
        public UInt32 bf;
        public UInt32 sensorCount;
        public void Init()
        {
            seuid = 0;
            frame = 0;
            bf = 0;
            sensorCount = 0;
        }
    }
    public class vpPno
    {
        UInt32 seuid;
        UInt32 frame;
        vphubbf bf;
        public UInt32 sensorCount;
        vpsensframe[] sarr;

        pnohdr hdr;
        public UInt32 sensmap;

        public vpPno()
        {
            Init();
        }
        private void Init()
        {
            seuid = 0;
            frame = 0;
            bf = 0;
            sensorCount = 0;
            sensmap = 0;
            hdr.Init();
        }
 
        public UInt32 SensorCount()
        {
            return sensorCount;
        }
        public bool SensorData( UInt32 s)
        {
             return (sensmap & (UInt32)(1 << (Int32)s)) != 0;
        }
        public UInt32 Unpack( ref vpframeinfo fi)
        {
            Init();

            if ((fi.size == 0) || (fi.pF == IntPtr.Zero))
                return sensorCount;

            IntPtr pF = fi.pF;
            hdr = Marshal.PtrToStructure<pnohdr>(pF);
            pF += Marshal.SizeOf<pnohdr>();

            sensorCount = hdr.sensorCount;
            bf = hdr.bf;
            if (sensorCount != 0)
            {
                sarr = new vpsensframe[16];

                for( int i=0; i<sensorCount; i++)
                {
                    vpsensframe sf = Marshal.PtrToStructure<vpsensframe>(pF);
                    pF += Marshal.SizeOf<vpsensframe>();

                    vpsensbf sbf = sf.bf;
                    Int32 sensnum = sbf.Sensor();
                    sarr[sensnum] = sf;
                    sensmap |= (UInt32)(1 << sensnum);
                }
            }

            return sensorCount;
        }

        public float[] PosRaw(UInt32 s)
        {
            return sarr[s].pos;
        }

        public Vector3 PosScreenVec(UInt32 s)
        {
            Vector3 v = new Vector3(sarr[s].pos[1], -(sarr[s].pos[2]), sarr[s].pos[0]);
            return v;
        }

        public float[] Ori(UInt32 s)
        {
            return sarr[s].ori;
        }

        //Raw Qtrn from tracker.  Note native Tracker Qtrn params are output in w,x,y,z order
        public Quaternion QuatRaw(UInt32 s)
        {
            Quaternion r = new Quaternion(sarr[s].ori[1], sarr[s].ori[2], sarr[s].ori[3], sarr[s].ori[0]);
            return r;
        }

        //LH = Left Handed
        public Quaternion QuatLH(UInt32 s)
        {
            Quaternion LH;
            LH.w =  QuatRaw(s).w;   // w
            LH.x = -(QuatRaw(s).y); //-y
            LH.y = QuatRaw(s).z;    // z
            LH.z = -(QuatRaw(s).x); //-x


            return LH;
        }
        public UInt32 fc()
        {
            return hdr.frame;
        }
    }

    public class VPCmdIF //wrapper
    {
        [DllImport("Kernel32.dll")]
        public static extern int AllocConsole();

        [DllImport(dll)]
        public static extern void vpcmdif_trace(UInt32 ilev);

        [DllImport(dll)]
        public static extern int vpcmdif_init(ref UInt64 ctx);

        [DllImport(dll)]
        public static extern int vpctx_connectusb(UInt64 ctx, ref UInt64 hnd, UInt16 pid = VIPER_PID);

        [DllImport(dll)]
        public static extern int vpctx_dev_release(UInt64 ctx, UInt64 hnd);

        [DllImport(dll)]
        public static extern int vpcmdif_release(UInt64 ctx);

        //int VPCMD_API vpcmd_dev_units			(vpcmd_context, vpdev_hnd, eCmdActions, UNITS_CONFIG *);
        [DllImport(dll)]
        public static extern int vpcmd_dev_units(UInt64 ctx, UInt64 hnd, UInt32 act, UInt32[] units);

        ///int VPCMD_API vpcmd_dev_stationmap(vpcmd_context, vpdev_hnd, STATION_MAP*);
        [DllImport(dll)]
        public static extern int vpcmd_dev_stationmap(UInt64 ctx, UInt64 hnd, UInt32[] stamap);

        ///**/int VPCMD_API vpcmd_sns_boresight		(vpcmd_context, vpdev_hnd, int sns, eCmdActions, BORESIGHT_CONFIG *);
        [DllImport(dll)]
        public static extern int vpcmd_sns_boresight(UInt64 ctx, UInt64 hnd, UInt32 sns, UInt32 act, float[] bangles);


        [DllImport(dll)]
        public static extern int vpcmd_dev_contpno(UInt64 ctx, UInt64 hnd, UInt32 act, IntPtr fc);

        [DllImport(dll)]
        public static extern int vpctx_dev_lastpnof(UInt64 ctx, UInt64 hnd, ref vpframeinfo fi);

        private const UInt16 VIPER_PID = 0xBF01;

#if UNITY_STANDALONE_WIN
        const string dll = "VPcmdIF";
        //#elif UNITY_STANDALONE_OSX
        //    const string dll = "VPcmdIFBUNDLE";
        //#elif UNITY_IOS
        //    const string dll = "__Internal";
        //#elif UNITY_ANDROID
        //    const string dll = "Somelibname";
#else
    const string dll = "";
#endif
    }

    public class VPdev
    {

        private static UInt64 ctx = 0;

        private static UInt64 devhnd = 0;
        public Int32 err = 0;
        public static string errstr = "no error";
        private static vpframeinfo fi;
        public vpPno pno;
        //private static UInt16 pid = VIPER_PID;

        public _VPTRACELEV tracelev = _VPTRACELEV.VPCMD_NOTRACE;


        public VPdev()
        {
            err = 0;    
            ctx = 0;    
            devhnd = 0;
            fi = new vpframeinfo();
            pno = new vpPno();
        }
        public bool Init()
        {
           
            err = 0; 
            errstr = "no error";
            if (ctx == 0)
            {
                err = VPCmdIF.vpcmdif_init(ref ctx);
            }
            else
                Debug.Log(" Init() ctx non-zero : ctx=" + ctx);

            return (err == 0);
        }

        public bool Connect()
        {
            bool bret = false;

            if (tracelev != _VPTRACELEV.VPCMD_NOTRACE)
            {
                VPCmdIF.AllocConsole();
            }
            VPCmdIF.vpcmdif_trace((UInt32)tracelev);

            if (!Init())
            {
                Debug.Log(" Connect() : Init() failed.");
            }
            else if (devhnd != 0)
            {
                Debug.Log(" Connect() : devhnd != 0: devhnd=" + devhnd );
                bret = true; //already connected
            }
            else
            {
                Debug.Log(" Connect() : ctx=" + ctx );
                err = VPCmdIF.vpctx_connectusb(ctx, ref devhnd);
                Debug.Log(" Connect() : vpctx_connectusb result :" + VPcmdErr.Report(err)  + "devhnd=" + devhnd + " tracelev=" + tracelev);
                bret = (err == 0);
            }

            return bret;
        }

        public  bool Disconnect()
        {
            // To Do: Add ctx cleanup function to destroy context if all devs released.
            err = VPCmdIF.vpctx_dev_release(ctx, devhnd);
            Debug.Log(" Disconnect vpctx_dev_release : " + VPcmdErr.Report(err));
            if (err == 0)
                devhnd = 0;
            err = VPCmdIF.vpcmdif_release(ctx);
            Debug.Log(" Disconnect vpcmdif_release : " + VPcmdErr.Report(err));
            if (err == 0)
                ctx = 0;
            return err==0;
        }

        public bool SetQuaternion()
        {
            //UInt32[] units = { (UInt32)_VPPOSU.POS_INCH, (UInt32)_VPORIU.ORI_QUATERNION };
            UInt32[] units = { (UInt32)_VPPOSU.POS_METER, (UInt32)_VPORIU.ORI_QUATERNION };
            err = VPCmdIF.vpcmd_dev_units(ctx, devhnd, (UInt32)_VPACT.CMD_ACTION_SET, units);
            Debug.Log(" Set Quaternion : " + VPcmdErr.Report(err));
            return err == 0;
        }
        public bool BoresightZero( UInt32 sens, bool bQuat=true )
        {
            float[] bangles = { ((bQuat) ? 1.0f : 0.0f), 0.0f, 0.0f, 0.0f };
            err = VPCmdIF.vpcmd_sns_boresight(ctx, devhnd, sens, (UInt32)(_VPACT.CMD_ACTION_SET), bangles);
            Debug.Log(" BoresightZero : " + VPcmdErr.Report(err));

            return err == 0;
        }
        public bool UnBoresight(UInt32 sens)
        {
            err = VPCmdIF.vpcmd_sns_boresight(ctx, devhnd, sens, (UInt32)(_VPACT.CMD_ACTION_RESET), null);
            Debug.Log(" UnBoresight : " + VPcmdErr.Report(err));

            return err == 0;
        }

        public bool GetStationInfo( ref UInt32 sensmap, ref UInt32 senscount, ref UInt32 srcmap, ref UInt32 srccount )
        {
            // this compiles .. but there has got to be a better way.
            UInt32[] stamap = { 0 };

            err = VPCmdIF.vpcmd_dev_stationmap(ctx, devhnd, stamap);
            Debug.Log(" GetStationMap : " + VPcmdErr.Report(err));


            sensmap = vpstamapbf.sensormap(stamap[0]);
            srcmap = (UInt32) vpstamapbf.sourcemap(stamap[0]);

            return err == 0;
        }
        public bool StartCont( )
        {
            err = VPCmdIF.vpcmd_dev_contpno(ctx, devhnd, (UInt32)(_VPACT.CMD_ACTION_SET), IntPtr.Zero);
            Debug.Log(" StartCont : " + VPcmdErr.Report(err));
            return err ==0;
        }
        public bool StopCont()
        {
            err = VPCmdIF.vpcmd_dev_contpno(ctx, devhnd, (UInt32)(_VPACT.CMD_ACTION_RESET), IntPtr.Zero);
            Debug.Log(" StopCont : " + VPcmdErr.Report(err));
            return err == 0;
        }
        public bool Sample( )
        {
            fi.size = 0;
            err = VPCmdIF.vpctx_dev_lastpnof(ctx, devhnd, ref fi);
            bool bRet = err == 0;
            if (bRet)
            {
                //Debug.Log(" Sample : fi size " + fi.size + VPcmdErr.Report(err));
                if (fi.size != 0)
                    pno.Unpack(ref fi);
            }
            else
            {
               // Debug.LogError(" Sample : " + VPcmdErr.Report(err));
            }

            return bRet;
        }
        public bool Connected()
        {
            return (devhnd != 0);
        }

        public  string ErrReport()
        {
            return VPcmdErr.Report(err);
        }


        void Start()
        {
            err = 10;
            errstr = "ten";
        }

        //    // Update is called once per frame
        //    void Update()
        //    {

        //    }
    }
    [StructLayout(LayoutKind.Explicit, Pack = 1)]
    public struct vpstamapbf
    {
        [FieldOffset(0)] System.UInt16 sensmap;
        [FieldOffset(2)] System.Byte res1;
        [FieldOffset(3)] System.SByte srcmap;
        // note this bitfield can't access res2, a 4 bit field
        // [FieldOffset(3)] System.SByte srcmap;

        public static UInt32 max_map_sensors = 16;
        public static UInt32 max_map_sources = 4;

        //public vpstamapbf( UInt32 v=0)
        //{
        //    sensmap = (UInt16)(v & 0xffff);
        //    res1 = 0;
        //    srcmap = (SByte)(v & 0x0f000000);
        //}

        //public UInt16 SensMap()
        //{
        //    return sensmap;
        //}
        //public SByte SrcMap()
        //{
        //    return srcmap;
        //}

        public static UInt16 sensormap(UInt32 bf)
        {
            return (UInt16)(bf & 0xff);
        }

        public static SByte sourcemap(UInt32 bf)
        {
            return (SByte)(bf & 0x0f000000);
        }

        public static UInt32 sensorcount(UInt32 bf)
        {
            UInt16 map = sensormap(bf);
            UInt32 count = 0;

            for (int i = 0; i < max_map_sensors; i++)
            {

                if (((1 << i) & map) != 0)
                {
                    count++;
                }
            }
            return count;
        }
        public static UInt32 sourcecount(UInt32 bf)
        {
            SByte map = sourcemap(bf);
            UInt32 count = 0;

            for (int i = 0; i < (int)max_map_sources; i++)
            {
                if (((1 << i) & map) != 0)
                {
                    count++;
                }
            }
            return count;
        }
    }

}
