using System;
using UnityEngine;
using Windows.Kinect;
using System.Collections.Generic;

namespace Recognizer.DeviceInfoTaker
{
    public class KinectV2InfoTaker : IDeviceInfoTaker
    {
        private BodySourceManager _manager;
        private List<ulong> _Bodies = new List<ulong>();
        public KinectV2InfoTaker()
        {
            this._manager = BodySourceManager.GetInstance();
        }

        public void Fill(ref double[] data)
        {
            if (_manager == null)
            {
                if (BodySourceManager.GetInstance() == null)
                {
                    Debug.Log("Problème d'instance KinectV2InfoTaker");
                    return;
                }
                    
                _manager = BodySourceManager.GetInstance();
            }

            Windows.Kinect.Body[] preData = _manager.GetData();
            if (preData == null)
            {
                return;
            }

            List<ulong> trackedIds = new List<ulong>();
            foreach (var body in preData)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    trackedIds.Add(body.TrackingId);
                }
            }

            // First delete untracked bodies
            foreach (ulong trackingId in _Bodies.ToArray())
            {
                if (!trackedIds.Contains(trackingId))
                {
                    _Bodies.Remove(trackingId);
                }
            }

            foreach (var body in preData)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    if (!_Bodies.Contains(body.TrackingId))
                    {
                        _Bodies.Add(body.TrackingId);
                    }
                    int idx = 0;

                    for (Windows.Kinect.JointType jt = Windows.Kinect.JointType.SpineBase; jt <= Windows.Kinect.JointType.ThumbRight; jt++)
                    {
                        Windows.Kinect.Joint sourceJoint = body.Joints[jt];
                        SetFramePosition(ref data, GetVector3FromJoint(sourceJoint), ref idx);
                    }
                }
            }
        }

        public void FillWithTime(ref double[] data, double timestamp)
        {
            if (_manager == null)
            {
                if (BodySourceManager.GetInstance() == null)
                {
                    Debug.Log("Problème d'instance KinectV2InfoTaker");
                    return;
                }
                    
                _manager = BodySourceManager.GetInstance();
            }

            Windows.Kinect.Body[] preData = _manager.GetData();
            if (preData == null)
            {
                return;
            }

            List<ulong> trackedIds = new List<ulong>();
            foreach (var body in preData)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    trackedIds.Add(body.TrackingId);
                }
            }

            // First delete untracked bodies
            foreach (ulong trackingId in _Bodies.ToArray())
            {
                if(trackingId == null)
                {
                    continue;
                }

                if (!trackedIds.Contains(trackingId))
                {
                    _Bodies.Remove(trackingId);
                }
            }

            foreach (var body in preData)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    if (!_Bodies.Contains(body.TrackingId))
                    {
                        _Bodies.Add(body.TrackingId);
                    }
                    int idx = 0;

                    for (Windows.Kinect.JointType jt = Windows.Kinect.JointType.SpineBase; jt <= Windows.Kinect.JointType.ThumbRight; jt++)
                    {
                        Windows.Kinect.Joint sourceJoint = body.Joints[jt];
                        SetFramePositionWithTime(ref data, GetVector3FromJoint(sourceJoint), ref idx, timestamp);
                    }
                }
            }
        }
        private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
        {
            return new Vector3(joint.Position.X, joint.Position.Y, joint.Position.Z);
        }


        static private void SetFramePosition(ref double[] leapData, Vector3 v, ref int startIdx)
        {
            leapData[startIdx] = v.x;
            leapData[startIdx + 1] = v.y;
            leapData[startIdx + 2] = v.z;
            startIdx += 3;
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