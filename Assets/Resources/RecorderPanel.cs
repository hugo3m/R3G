using Leap.Unity;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Recognizer;
using Recognizer.DeviceInfoTaker;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RecorderPanel : MonoBehaviour 
{
	public TMP_InputField classNameField;
    public TMP_InputField tagField;
	//public Toggle autostartToggle;
	public TextMeshProUGUI startStopButton;
	public TMP_InputField delayField;
    //public Toggle autostopToggle;
    public TextMeshProUGUI countdown;

    double[] leapFrame = new double[138];

    private bool started = false;
    private bool record = false;
    private StreamWriter outData = null;
    public string outputDir = "./CapturedFiles";
    public IEnumerator delayedStart;

    // Use this for initialization
    void Start () 
	{
    }

    /*public void AutoStartChanged()
	{
        startButton.interactable = !autostartToggle.isOn;
        delayField.interactable = !autostartToggle.isOn;
    }*/

    public void StartStop()
    {
        if (started)
            StopRecording();
        else
            StartRecording();
    }

    private void StartRecording()
    {
        if (started)
            return;

        started = true;

        startStopButton.text = "Stop";
        delayField.interactable = false;
        
        delayedStart = DelayedRecording(int.Parse(delayField.text));
        StartCoroutine(delayedStart);
    }

    IEnumerator DelayedRecording(int delay)
    {
        int d = delay;
        if (d > 0)
        {
            while (d >= 0)
            {
                countdown.text = d.ToString();
                countdown.gameObject.SetActive(true);
                d--;
                yield return new WaitForSeconds(1);
            }
        }

        countdown.gameObject.SetActive(false);
        if (outData != null)
            StopRecording();
        if (tagField.text == "")
        {
            outData = new StreamWriter(GetUniqueFileName(outputDir + Path.DirectorySeparatorChar + classNameField.text + ".txt"));
        }
        else
        {
            outData = new StreamWriter(GetUniqueFileName(outputDir + Path.DirectorySeparatorChar + classNameField.text + "_" + tagField.text + ".txt"));
        }
        outData.WriteLine("<class=" + classNameField.text + ">");

        record = true;
    }

    /*public void AutoStopChanged()
    {
        stopButton.interactable = !autostopToggle.isOn;
    }*/

    private void StopRecording()
    {
        startStopButton.text = "Start";
        delayField.interactable = true;
        
        if (outData != null)
        {
            outData.WriteLine("</class=" + classNameField.text + ">");
            outData.Close();
            outData = null;
        }

        if (started && !record)
        {
            StopCoroutine(delayedStart);
            countdown.gameObject.SetActive(false);
        }

        started = false;
        record = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (record && (Hands.Left != null || Hands.Right != null))
        {
            // 2 (gauche / droit) * 23 (Ordre) * 3 (x,y,z) = 138 valeurs doubles en entrée
            LeapInfoTaker.UpdateBothHands(ref leapFrame);

            string frame = "";
            for (int i = 0; i < leapFrame.Length; i++)
                frame += leapFrame[i].ToString(System.Globalization.CultureInfo.InvariantCulture) + "   ";

            outData.WriteLine(frame);
        }
    }

    void OnApplicationQuit()
    {
        StopRecording();
    }

    static public string GetUniqueFileName(string filename)
    {
        string result;
        string pathR = Path.GetDirectoryName(filename);
        string fName = Path.GetFileNameWithoutExtension(filename);
        string extension = Path.GetExtension(filename);
        int idx = 0;

        if (!Directory.Exists(pathR))
            Directory.CreateDirectory(pathR);

        do
        {
            result = pathR + Path.DirectorySeparatorChar + fName + "_" + idx + extension;
            idx++;
        }
        while (File.Exists(result));

        return result;
    }
}
