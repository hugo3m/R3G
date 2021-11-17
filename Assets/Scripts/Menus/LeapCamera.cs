using System.Collections;
using System.Collections.Generic;
using Leap.Unity;
using Recognizer;
using UnityEngine;

public class LeapCamera : MonoBehaviour
{
    
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(PositionCamera());
    }

    IEnumerator PositionCamera()
    {
        IDeviceProvider transformLeapProvider = FindObjectOfType<IDeviceProvider>();
        while (transformLeapProvider==null)
        {
            transformLeapProvider = FindObjectOfType<IDeviceProvider>();
            yield return new WaitForSeconds(0.1f);
        }
        transform.position = transformLeapProvider.gameObject.transform.position +transformLeapProvider.CameraPosRelativ ;
        transform.LookAt(transformLeapProvider.gameObject.transform.position);

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
