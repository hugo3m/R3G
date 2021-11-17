using System;
using System.Collections.Generic;
using Recognizer;

namespace Menus.TestAndSetting
{
    public class TestAndSettingNotification
    {
        public static System.Action<string> ThetaModification;
        public static System.Action<float> CurvilineDistanceModification;
        public static System.Action<List<Tuple<LeapMotionJointType,bool>>> HandJointsModification;
        public static System.Action RelearnAsk;
    }
}