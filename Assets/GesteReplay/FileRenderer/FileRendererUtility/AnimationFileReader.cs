using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Recognizer;
using UnityEngine;


public class AnimationFileReader : MonoBehaviour
{
    private bool play = false;

    public string filename;

    //public float frameRate;
    public Transform joinPrefab;
    public GameObject OriginPrefab;
    private GameObject _origin;

    private Geste geste;

    [SerializeField] public Vector3 minPos;
    [SerializeField] public Vector3 maxPos;

    public bool debug;

    public int _currentFrameIndex; //l'indice de la frame courrante

    private List<Transform> _lesTransform; //liste des préfabs
    private List<float> _completness;

    private  Transform _cameraPivot;
    private Vector3 _cameraPosOffset;

    public string newGeste;
    private Vector3 _center;
    private Camera _cam;


    private bool _rotating = false;
    private Action<int> _keepInformedProgression;
    private List<int> _predictions;
    private int _toSkip=1;

    private void Start()
    {
        _cameraPivot = transform.Find("CameraPivot");
        _cam = _cameraPivot.GetComponentInChildren<Camera>();
        _predictions = null;
        if (RecoManager.GetInstance()?.Device == DevicesInfos.Device.LeapMotion)
        {
            joinPrefab.GetComponent<TrailRenderer>().endWidth = 0.08f;
        }
        else
        {
            joinPrefab.GetComponent<TrailRenderer>().endWidth = 0.25f;
        }

        if (filename != "")
            LoadGeste(filename);
        StartCoroutine(UpdatePos());
    }

    /// <summary>
    /// update the number of frames to progress at each time
    /// </summary>
    /// <param name="toSkip"> 0 = no progression, 1 = normal timing, 2 = skip ne frame on two..</param>
    public void SetToSkip(int toSkip)
    {
        this._toSkip = toSkip;
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

        Quaternion target = Quaternion.Euler(_cameraPivot.eulerAngles + axis * -45);
        Quaternion start = _cameraPivot.rotation;
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
            _cameraPivot.rotation = Quaternion.Slerp(start, target, lerp);
            yield return null;
        }

        _rotating = false;
    }


    public void clear()
    {
        if (_lesTransform == null)
            return;
        foreach (Transform t in _lesTransform)
        {
            Destroy(t.gameObject);
        }
    }

    public void Clear()
    {
        if (_lesTransform == null || _lesTransform.Count == 0)
            return;
        foreach (Transform t in _lesTransform)
        {
            Destroy(t.gameObject);
        }
        _lesTransform.Clear();
        geste = null;
    }

    /// <summary>
    /// event to be notified by the current index
    /// </summary>
    /// <param name="keepInformedProgression"></param>
    public void SetEventInformProgression(Action<int> keepInformedProgression)
    {
        this._keepInformedProgression = keepInformedProgression;
    }

    public void LoadGeste(string filename)
    {
        geste = new Geste(filename);
        minPos = geste.min;
        maxPos = geste.max;

        if (_origin == null)
            _origin = Instantiate(OriginPrefab);

        _origin.transform.position = maxPos + transform.position - minPos - (minPos.y + maxPos.y) * 0.05f * Vector3.up;
        _center = new Vector3((minPos.x + maxPos.x) / 2, (minPos.y + maxPos.y) / 2, (minPos.z + maxPos.z) / 2) +
                  transform.position - minPos;
        _completness = Enumerable.Repeat(0f, geste.nbJointures).ToList();
        _currentFrameIndex = 1;

        _cameraPosOffset = (maxPos + minPos) / 2 + transform.position - minPos;

        _cam.farClipPlane = 70;
        _cam.nearClipPlane = -70;
        _cam.orthographicSize =
            Mathf.Max(maxPos.x - minPos.x, Mathf.Max(maxPos.y - minPos.y), Mathf.Max(maxPos.z - minPos.z)) / 2 + 4;


        //crée la liste des préfabs
        _lesTransform = new List<Transform>();
        for (int j = 0; j < geste.nbJointures; ++j)
        {
            Vector3 initialRelativeCoord = geste.frames[0].coordonnes[j];
            Transform jointure = Instantiate(joinPrefab, initialRelativeCoord - minPos + transform.position,
                Quaternion.identity, transform);
            if (geste.nbJointures < 30) //kinect
            {
                jointure.localScale = Vector3.one * 1.5f;
            }

            _lesTransform.Add(jointure);
        }

        UpdateCameraPos();
    }

    public void LoadNewGeste(string filename)
    {
        clear();
        LoadGeste(filename);
    }


    //function de debug, affiche le rectangle à render
    private void DrawZone()
    {
        Vector3 p2 = new Vector3(maxPos.x, minPos.y, minPos.z) + transform.position - minPos;
        Vector3 p1 = new Vector3(minPos.x, minPos.y, minPos.z) + transform.position - minPos;
        Vector3 p3 = new Vector3(maxPos.x, minPos.y, maxPos.z) + transform.position - minPos;
        Vector3 p4 = new Vector3(minPos.x, minPos.y, maxPos.z) + transform.position - minPos;

        //ou vector3.up * maxPos.y  
        Vector3 p5 = p1 + new Vector3(0, maxPos.y - minPos.y, 0);
        Vector3 p6 = p2 + new Vector3(0, maxPos.y - minPos.y, 0);
        Vector3 p7 = p3 + new Vector3(0, maxPos.y - minPos.y, 0);
        Vector3 p8 = p4 + new Vector3(0, maxPos.y - minPos.y, 0);

        Debug.DrawLine(p1, p2);
        Debug.DrawLine(p2, p3);
        Debug.DrawLine(p3, p4);
        Debug.DrawLine(p4, p1);

        Debug.DrawLine(p5, p6);
        Debug.DrawLine(p6, p7);
        Debug.DrawLine(p7, p8);
        Debug.DrawLine(p8, p5);

        Debug.DrawLine(p1, p5);
        Debug.DrawLine(p2, p6);
        Debug.DrawLine(p3, p7);
        Debug.DrawLine(p4, p8);
    }

    private void Update()
    {
        if (debug) DrawZone();

        //if (play) UpdateJoinPos();
    }

    //la camera regard la zone du dessus
    //et est pos au centre
    private void UpdateCameraPos()
    {
        _cameraPivot.rotation = Quaternion.identity;
        _cameraPivot.position = _cameraPosOffset;
        //_cam.transform.LookAt(_center);
    }

    private IEnumerator UpdatePos()
    {
        while (true)
        {
            if (play)
            {
                UpdateJoinPos();
            }
            yield return new WaitForSeconds(RecoManager.FREQUENCY_RECORD_SEND_DATA);
        }
    }

    private void UpdateJoinPos()
    {
        FrameRenderer currentFrameRenderer;
//        try
//        {
            currentFrameRenderer = geste.frames[_currentFrameIndex];
            if (_keepInformedProgression != null)
                _keepInformedProgression(_currentFrameIndex);
//        }
//        catch (Exception e)
//        {
//            print("WARNING "+e.Message);
//            return;
//        }


        int jointureCount = currentFrameRenderer.coordonnes.Count;
        for (int j = 0; j < jointureCount; ++j)
        {
            Transform currentJoin = _lesTransform[j];

            Vector3 targetedPosition = currentFrameRenderer.coordonnes[j];

            currentJoin.GetComponent<MeshRenderer>().enabled =
                currentFrameRenderer.joinRendered
                    [j]; //on affiche la main seulement si la coordonnée est différente de zéro, sinon c'est qu'elle n'était pas détectée
            currentJoin.position = targetedPosition - minPos + transform.position;
        }


        _currentFrameIndex += _toSkip;
        if (_currentFrameIndex >= geste.nbFrames - 1)
        {
            OnIterationEnd();
        }
    }

    private void ResetTrailPrefab()
    {
        foreach (Transform t in _lesTransform)
        {
            t.GetComponent<TrailRenderer>().Clear();
        }
    }

    private void OnIterationEnd()
    {
        _currentFrameIndex = 1;
        ResetJoinPos();
        ResetTrailPrefab();
    }

    private void ResetJoinPos()
    {
        for (int i = 0; i < _lesTransform.Count; i++)
        {
            Transform currentJoint = _lesTransform[i];
            currentJoint.position = geste.frames[0].coordonnes[i] - minPos + transform.position;
        }
    }

    public int Play()
    {
        play = true;
        if (geste == null)
            return 0;
        return geste.nbFrames;
    }

    public void Pause()
    {
        play = false;
    }

    public void ResetAnim()
    {
        _currentFrameIndex = 1;
        play = true;
    }

    public void ResetAnimWithPause()
    {
        _currentFrameIndex = 1;
        play = false;
    }

    public void SetCurrentFrame(int indexToGo)
    {
        _currentFrameIndex = indexToGo;
    }

    public bool isPause()
    {
        return !play;
    }

    public bool isLoaded()
    {
        return geste != null;
    }
}