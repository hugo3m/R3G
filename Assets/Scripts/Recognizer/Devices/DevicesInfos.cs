using System.Collections.Generic;
using Recognizer.DeviceInfoTaker;
using UnityEngine;

namespace Recognizer
{
    public class DevicesInfos
    {
        public enum Device
        {
            LeapMotion,KinectV1,KinectV2
        }
        
        public class DeviceInfo
        {
            public Device Device;
            public IDeviceInfoTaker Filler;
            public int FrameSize;
            public string PathModel;
            public string PathRaw;
            public string StartArguments;

            public DeviceInfo( Device device, IDeviceInfoTaker filler,int frameSize, string pathModel, string pathRaw, string startArguments)
            {
                Device = device;
                Filler = filler;
                FrameSize = frameSize;
                PathModel = pathModel;
                PathRaw = pathRaw;
                StartArguments = startArguments;
            }
        }

        public static Dictionary<Device,DeviceInfo> Infos = new Dictionary<Device, DeviceInfo>()
        {
            {Device.LeapMotion,new DeviceInfo(Device.LeapMotion,new LeapInfoTaker(), 23*2*3,"ModelFixed_LeapMotionFolder","RAW_LeapMotionDataFolder","0")},
            {Device.KinectV1,new DeviceInfo(Device.KinectV1,new KinectV1InfoTaker(), 20*3,"ModelFixedFolder","RAW_KinectDataFolder","1")},
            {Device.KinectV2,new DeviceInfo(Device.KinectV2,new KinectV2InfoTaker(), 25*3,"ModelFixedFolder","RAW_KinectDataFolder","1")},
        };
    }
}