using System;
using UnityEngine;

namespace Recognizer.DeviceInfoTaker
{
    public class KinectV1InfoTaker : IDeviceInfoTaker
    {
        private KinectManager _manager;

        public KinectV1InfoTaker()
        {
            this._manager = KinectManager.Instance;
        }

        public void Fill(ref double[] data)
        {
            if (_manager == null)
            {
                if (KinectManager.Instance == null)
                    return;
                _manager = KinectManager.Instance;
            }

            uint playerID = _manager.GetPlayer1ID();

            int nbJoints = (int) KinectWrapper.NuiSkeletonPositionIndex.Count;
            int idx = 0;
            for (int i = 0; i < nbJoints; i++)
            {
                SetFramePosition(ref data, _manager.GetJointPosition(playerID, i), ref idx);
            }
        }

        public void FillWithTime(ref double[] data, double timestamp)
        {
            if (_manager == null)
            {
                if (KinectManager.Instance == null)
                    return;
                _manager = KinectManager.Instance;
            }

            uint playerID = _manager.GetPlayer1ID();

            int nbJoints = (int) KinectWrapper.NuiSkeletonPositionIndex.Count;
            int idx = 0;
            for (int i = 0; i < nbJoints; i++)
            {
                SetFramePositionAndTime(ref data, _manager.GetJointPosition(playerID, i), ref idx, timestamp);
            }
        }

        static private void SetFramePosition(ref double[] leapData, Vector3 v, ref int startIdx)
        {
            leapData[startIdx] = v.x;
            leapData[startIdx + 1] = v.y;
            leapData[startIdx + 2] = v.z;
            startIdx += 3;
        }

        static private void SetFramePositionAndTime(ref double[] leapData, Vector3 v, ref int startIdx, double timestamp)
        {
            leapData[startIdx] = v.x;
            leapData[startIdx + 1] = v.y;
            leapData[startIdx + 2] = v.z;
            leapData[startIdx + 3] = timestamp;
            startIdx += 4;
        }
    }
}