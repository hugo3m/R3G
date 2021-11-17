using Leap.Unity;
using UnityEngine;

namespace Recognizer.DeviceInfoTaker
{
    public class LeapInfoTaker : IDeviceInfoTaker
    {
        public void Fill(ref double[] data)
        {
            UpdateBothHands(ref data);
        }

        public void FillWithTime(ref double[] data, double timestamp)
        {
            UpdateBothHandsWithTime(ref data, timestamp);
        }


        public static void UpdateBothHands(ref double[] leapData)
        {
            int idx = 0;
            UpdateHand(ref leapData, Hands.Left, ref idx);
            UpdateHand(ref leapData, Hands.Right, ref idx);
        }

        public static void UpdateHand(ref double[] leapData, Leap.Hand hand, ref int startIdx)
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
            SetFramePosition(ref leapData, jointPos, ref startIdx);

            if (hand != null)
                jointPos = hand.WristPosition.ToVector3();
            else
                jointPos.Set(0, 0, 0);
            SetFramePosition(ref leapData, jointPos, ref startIdx);

            if (hand != null)
                jointPos = hand.Arm.ElbowPosition.ToVector3();
            else
                jointPos.Set(0, 0, 0);
            SetFramePosition(ref leapData, jointPos, ref startIdx);

            for (int f = 0; f <= (int) Leap.Finger.FingerType.TYPE_PINKY; ++f)
            for (int b = 0; b <= (int) Leap.Bone.BoneType.TYPE_DISTAL; ++b)
            {
                if (hand != null)
                    jointPos = hand.Fingers[f].bones[b].NextJoint.ToVector3();
                else
                    jointPos.Set(0, 0, 0);

                SetFramePosition(ref leapData, jointPos, ref startIdx);
            }
        }

        static private void SetFramePosition(ref double[] leapData, Vector3 v, ref int startIdx)
        {
            leapData[startIdx] = v.x;
            leapData[startIdx + 1] = v.y;
            leapData[startIdx + 2] = v.z;
            startIdx += 3;
        }

        public static void UpdateBothHandsWithTime(ref double[] leapData, double timestamp)
        {
            int idx = 0;
            UpdateHandWithTime(ref leapData, Hands.Left, ref idx, timestamp);
            UpdateHandWithTime(ref leapData, Hands.Right, ref idx, timestamp);
        }

        public static void UpdateHandWithTime(ref double[] leapData, Leap.Hand hand, ref int startIdx, double timestamp)
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
            SetFramePositionWithTime(ref leapData, jointPos, ref startIdx, timestamp);

            if (hand != null)
                jointPos = hand.WristPosition.ToVector3();
            else
                jointPos.Set(0, 0, 0);
            SetFramePositionWithTime(ref leapData, jointPos, ref startIdx, timestamp);

            if (hand != null)
                jointPos = hand.Arm.ElbowPosition.ToVector3();
            else
                jointPos.Set(0, 0, 0);
            SetFramePositionWithTime(ref leapData, jointPos, ref startIdx, timestamp);

            for (int f = 0; f <= (int) Leap.Finger.FingerType.TYPE_PINKY; ++f)
            for (int b = 0; b <= (int) Leap.Bone.BoneType.TYPE_DISTAL; ++b)
            {
                if (hand != null)
                    jointPos = hand.Fingers[f].bones[b].NextJoint.ToVector3();
                else
                    jointPos.Set(0, 0, 0);

                SetFramePositionWithTime(ref leapData, jointPos, ref startIdx, timestamp);
            }
        }

        static private void SetFramePositionWithTime(ref double[] leapData, Vector3 v, ref int startIdx, double timestamp)
        {
            leapData[startIdx] = v.x;
            leapData[startIdx + 1] = v.y;
            leapData[startIdx + 2] = v.z;
            leapData[startIdx + 3] = timestamp;
            startIdx += 4;
        }
    }
}