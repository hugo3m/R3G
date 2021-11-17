using System;
using System.ComponentModel;

namespace Recognizer
{
    [Serializable]
    public enum LeapMotionJointType
    {
        //-----LEFT----
        [Description("LEFT_PALM")]
        LEFT_PALM = 0 ,
        [Description("LEFT_WRIST")]
        LEFT_WRIST , 
        [Description("LEFT_THUMB")]
        LEFT_THUMB ,
        [Description("LEFT_INDEX")]
        LEFT_INDEX , 
        [Description("LEFT_MIDDLE")]
        LEFT_MIDDLE ,
        [Description("LEFT_RING")]
        LEFT_RING,
        [Description("LEFT_PINKY")]
        LEFT_PINKY ,
        //-----RIGHT----
        [Description("RIGHT_PALM")]
        RIGHT_PALM ,
        [Description("RIGHT_WRIST")]
        RIGHT_WRIST,
        [Description("RIGHT_THUMB")]
        RIGHT_THUMB ,
        [Description("RIGHT_INDEX")]
        RIGHT_INDEX ,
        [Description("RIGHT_MIDDLE")]
        RIGHT_MIDDLE ,
        [Description("RIGHT_RING")]
        RIGHT_RING ,
        [Description("RIGHT_PINKY")]
        RIGHT_PINKY 
    }
    public static class JointTypeExtension
    {
        public static string ToDescriptionString(this LeapMotionJointType val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
                .GetType()
                .GetField(val.ToString())
                .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    } 
}