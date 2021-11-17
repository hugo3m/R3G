using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleRotator : MonoBehaviour
{
    public float Speed = 2f;

    private float _nextUpdate = 0;
    // Start is called before the first frame update
    void Start()
    {
        _nextUpdate = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > _nextUpdate + 1f/Speed)
        {
            _nextUpdate += 1f/Speed;
            transform.Rotate(Vector3.back, 5f);
        }
        //_lastUpdate += Time.deltaTime * Speed;
    }
}
