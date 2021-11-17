using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowCharacter : MonoBehaviour
{

    public GameObject follow;

    private Vector3 cameraOffset;

    // Use this for initialization
    void Awake()
    {
        cameraOffset = gameObject.transform.position - follow.transform.position;
    }
	
	// Update is called once per frame
	void Update ()
    {
        gameObject.transform.position = follow.transform.position + cameraOffset;
    }
}
