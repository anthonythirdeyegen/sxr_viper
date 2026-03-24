using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class ClickableObject : MonoBehaviour, IPointerClickHandler
{
    [Header("Left button click event. Drag any handler here.")]
    public UnityEvent lbclick;
    [Header("Middle button click event. Drag any handler here.")]
    public UnityEvent mbclick;
    [Header("Right button click event. Drag any handler here.")]
    public UnityEvent rbclick;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Left click");
            lbclick.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Middle)
        {
            Debug.Log("Middle click");
            mbclick.Invoke();
        }
        else if (eventData.button == PointerEventData.InputButton.Right)
        {
            Debug.Log("Right click");
            rbclick.Invoke();
        }
    }
}


