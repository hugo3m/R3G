using UnityEngine;
using System.Linq;
using System;
using System.Globalization;
using Leap.Unity;
using System.IO;
using Recognizer;
using Recognizer.DeviceInfoTaker;

public class LeapRecorder : MonoBehaviour
{
    double[] leapFrame = new double[138];

    private bool leapoffline = false;

    ////////////////////////////////////////////////////
    /* For capturing leap files */

    private bool autoStartMode;
    private bool autoStopMode;
    private bool record = false;
    private string className;

    private StreamWriter outData = null;
    public string outputDir = "./CapturedFiles";

    ////////////////////////////////////////////////////
    /* For updating leap hands */
    void Start()
    {
    }

    void Update()
    {
        if (record && (Hands.Left != null || Hands.Right != null))
        {
            // 2 (gauche / droit) * 23 (Ordre) * 3 (x,y,z) = 138 valeurs doubles en entrée
            int idx = 0;
            LeapInfoTaker.UpdateHand(ref leapFrame, Hands.Left, ref idx);
            LeapInfoTaker.UpdateHand(ref leapFrame, Hands.Right, ref idx);

            string frame = "";
            for (int i = 0; i < leapFrame.Length; i++)
                frame += leapFrame[i] + "   ";

            outData.WriteLine(frame.ToString());
        }
    }

    /*private void updateHand(Leap.Hand hand, ref int startIdx)
    {
        Vector3 jointPos = new Vector3();

        // Ordre : 
        //      palmPosition, wristPosition, elbowPosition, 
        //      thumbPosition: METACARPAL, PROXIMAL, INTERMEDIATE, DISTAL
        //      indexPosition: METACARPAL, PROXIMAL, INTERMEDIATE, DISTAL
        //      middlePosition: METACARPAL, PROXIMAL, INTERMEDIATE, DISTAL
        //      ringPosition: METACARPAL, PROXIMAL, INTERMEDIATE, DISTAL
        //      pinkyPosition: METACARPAL, PROXIMAL, INTERMEDIATE, DISTAL
        // Si main ou articulation non detectée, envoyer 0 0 0

        if (hand != null)
            jointPos = hand.PalmPosition.ToVector3();
        else
            jointPos.Set(0, 0, 0);
        SetFramePosition(jointPos, ref startIdx);

        if (hand != null)
            jointPos = hand.WristPosition.ToVector3();
        else
            jointPos.Set(0, 0, 0);
        SetFramePosition(jointPos, ref startIdx);

        if (hand != null)
            jointPos = hand.Arm.ElbowPosition.ToVector3();
        else
            jointPos.Set(0, 0, 0);
        SetFramePosition(jointPos, ref startIdx);

        for (int f = 0; f <= (int)Leap.Finger.FingerType.TYPE_PINKY; ++f)
            for (int b = 0; b <= (int)Leap.Bone.BoneType.TYPE_DISTAL; ++b)
            {
                if (hand != null)
                    jointPos = hand.Fingers[f].bones[b].NextJoint.ToVector3();
                else
                    jointPos.Set(0, 0, 0);

                SetFramePosition(jointPos, ref startIdx);
            }
    }

    private void SetFramePosition(Vector3 v, ref int startIdx)
    {
        leapFrame[startIdx] = v.x;
        leapFrame[startIdx + 1] = v.y;
        leapFrame[startIdx + 2] = v.z;
        startIdx += 3;
    }*/

    void OnApplicationQuit()
    {
        stopRecording();
    }

    ////////////////////////////////////////////////////
    /* For classification / offline reading */


    ////////////////////////////////////////////////////
    /* For capturing leap files */

    public void setAutoStart(bool enable) { autoStartMode = enable; }
    public void setAutoStop(bool enable) { autoStopMode = enable; }
    public bool isRecording() { return record; }

    public void setClassName(string cName) { className = cName; }

    static public string GetUniqueFileName(string filename)
    {
        string result;
        string pathR = Path.GetDirectoryName(filename);
        string fName = Path.GetFileNameWithoutExtension(filename);
        string extension = Path.GetExtension(filename);
        int idx = 0;

        if(!Directory.Exists(pathR))
            Directory.CreateDirectory(pathR);
        
        do
        {
            result = pathR + Path.DirectorySeparatorChar + fName + "_" + idx + extension;
            idx++;
        }
        while (File.Exists(result));

        return result;
    }

    public void startRecording()
    {
        if (record)
            return;

        if (outData != null)
            stopRecording();

        outData = new StreamWriter(GetUniqueFileName(outputDir + Path.DirectorySeparatorChar + className + ".txt"));
        outData.WriteLine("<class=" + className + ">");

        record = true;
    }

    public void stopRecording()
    {
        record = false;
        if (outData != null)
        {
            outData.WriteLine("</class=" + className + ">");
            outData.Close();
            outData = null;
        }
    }

    
}