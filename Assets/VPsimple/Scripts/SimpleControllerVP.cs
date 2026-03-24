//  VSS $Header: /PiDevTools11/Inc/PDIg4.h 18    1/09/14 1:05p Suzanne $  
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using VPcmdIF;
using System;
using System.Linq;

public class SimpleControllerVP : MonoBehaviour {
    public Slider divisor_slider;
    public Text divisor_value;
    public Slider sensors_slider;
    public Text sensors_value;
    public ViperHost vphost;


   // private PlStream plstream;
    private Vector3 prime_position;
    private GameObject[] knuckles;

    private int[] dropped;

	// Use this for initialization
    void Awake ()
    {
        // set divisor defaults
        divisor_slider.value = 1.0f;

        // set sensors defaults
        sensors_slider.value = 1;

        // get the stream component
        //plstream = GetComponent<PlStream>();

        // get knuckles
        //knuckles = GameObject.FindGameObjectsWithTag("Knuckle");
        knuckles = GameObject.FindGameObjectsWithTag("Knuckle").OrderBy(g => g.transform.GetSiblingIndex()).ToArray();
        dropped = new int[knuckles.Length];

        // set sensors_slider max value
        sensors_slider.maxValue = Mathf.Min(knuckles.Length, vphost.sensorcount);
    }

	void Start () {
        // initializes arrays, fixes positions
        if (vphost.Started())
        {
            Zero();
        }
        else
        {
            Debug.Log("SimpleControllerVP::Start - vphost not ready; no zero performed.");
        }
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();
	}

    // called before performing any physics calculations
    void FixedUpdate()
    {
        vpPno pno = vphost.Pno();

        // update divisor text
        divisor_value.text = divisor_slider.value.ToString("F1");

        // for each knuckle up to sensors slider value, update the position
        for (UInt32 i = 0; i < pno.SensorCount(); ++i)
        {
            if ((pno.sensmap & (UInt32)(1 << (Int32)i)) != 0)
            {

                Vector3 unity_position = pno.PosScreenVec(i) - prime_position; ;

                Quaternion unity_rotation = pno.QuatLH(i); 

                if (!knuckles[i].activeSelf)
                    knuckles[i].SetActive(true);
                knuckles[i].transform.position = unity_position / divisor_slider.value;
                knuckles[i].transform.rotation = unity_rotation;

                // set deactivate frame count to 10
                dropped[i] = 10;
            }
            else
            {
                if (knuckles[i].activeSelf)
                {
                    dropped[i] -= 1;
                    if (dropped[i] <= 0)
                        knuckles[i].SetActive(false);
                }
            }
        }
    }

    public void Zero()
    {
        Debug.Log(" Zero() ");
        vpPno pno = vphost.Pno();

        for (var i = 0; i < knuckles.Length; ++i)
            knuckles[i].transform.position = new Vector3(-1000, -1000, -1000);

        for (var i = 0; i < dropped.Length; ++i)
            dropped[i] = 0;

        //UInt32 map = pno.sensmap;

        for (UInt32 i = 0; i < pno.SensorCount(); i++)
        //for (var i = 0; i < plstream.active.Length; ++i)
        {

            if ((pno.sensmap & (UInt32)(1 << (Int32)i)) != 0)
            //if (pno.SensorData(i))
            //if (plstream.active[i])
            {
                prime_position = pno.PosScreenVec(i);
                //prime_position = plstream.positions[i];
                break;
            }
        }
    }

    public void OnViperHostReady()
    {
        Debug.Log(" OnViperHostReady : Zeroing!! ");
        Zero();
    }

    public void OnBtnBoresight()
    {
        Debug.Log(" OnBtnBoresight  ");
        vphost.BoresightAll();
    }

    public void OnBtnUnBoresight()
    {
        Debug.Log(" OnBtnUnBoresight  ");
        vphost.UnBoresightAll();
    }
}
