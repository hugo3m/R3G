using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ArrowsFileReader : MonoBehaviour
{

    public AnimationFileReader Reader;
    public GameObject theObjectToRotate;
    public Button RArrow;
    public Button LArrow;
    public Button TArrow;
    public Button BArrow;
    private bool _rotating=false ;

    // Start is called before the first frame update
    void Start()
    {
        if (theObjectToRotate == null)
        {
            
            RArrow.onClick.AddListener(()=>Reader.Rotate45Cam(new Vector3(0,1,0)));
            LArrow.onClick.AddListener(()=>Reader.Rotate45Cam(new Vector3(0,-1,0)));
            TArrow.onClick.AddListener(()=>Reader.Rotate45Cam(new Vector3(-1,0,0)));
            BArrow.onClick.AddListener(()=>Reader.Rotate45Cam(new Vector3(1,0,0)));
            return;
        }
        
        LArrow.onClick.AddListener(()=>Rotate45Cam(new Vector3(0,1,0)));
        RArrow.onClick.AddListener(()=>Rotate45Cam(new Vector3(0,-1,0)));
        TArrow.onClick.AddListener(()=>Rotate45Cam(new Vector3(-1,0,0)));
        BArrow.onClick.AddListener(()=>Rotate45Cam(new Vector3(1,0,0)));
        
        
        
    }
    
    public void Rotate45Cam(Vector3 axis)
    {
        if (!_rotating)
        {
            StartCoroutine(MoveFunction(axis));
        }
    }


    IEnumerator MoveFunction(Vector3 axis)
    {
        _rotating = true;
        float timeSinceStarted = 0f;

        Quaternion target = Quaternion.Euler(theObjectToRotate.transform.eulerAngles + axis * -45);
        Quaternion start = theObjectToRotate.transform.rotation;
        // If the object has arrived, stop the coroutine
        while (timeSinceStarted <= 1)
        {
            timeSinceStarted += Time.deltaTime * 1.5f;
            //_cameraPivot.transform.position = Vector3.Lerp(_cameraPivot.transform.position, newPosition, 0.8f*timeSinceStarted);
            //_cameraPivot.LookAt(_center);

            // EaseInOutQuint equation
            float lerp = 6 * Mathf.Pow(timeSinceStarted, 3) * Mathf.Pow(timeSinceStarted, 2) +
                         -15 * Mathf.Pow(timeSinceStarted, 2) * Mathf.Pow(timeSinceStarted, 2) +
                         10 * Mathf.Pow(timeSinceStarted, 3);
            theObjectToRotate.transform.rotation = Quaternion.Slerp(start, target, lerp);
            yield return null;
        }

        _rotating = false;
    }
 

    // Update is called once per frame
    void Update()
    {
    }
}
