using System;
using System.Collections;
using System.Collections.Generic;
using Menus.UntrimmedVisualizer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputClickPositionDetector : MonoBehaviour,IPointerDownHandler
{
    private RectTransform _rect;

    // Start is called before the first frame update
    void Start()
    {
        _rect = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        float width = _rect.rect.width;

        Vector3 localMouse = _rect.InverseTransformPoint(Input.mousePosition);
        
        UntrimmedVisualizerNotification.UpdatePosLecture((localMouse.x+width/2)/width);
    }
}
