using System;
using Recognizer;
using Recognizer.DataTools;

namespace Menus.Demonstration
{
    public class DemonstrationNotification
    {
        public static System.Action<UserData> OnSelectedUser;
        public static System.Action<DevicesInfos.Device> OnSelectedDevice;
        public static System.Action<DataBase> OnSelectedDb;
        public static System.Action<string> Validate;
        public static System.Action<bool> Record;
        public static System.Action StartStopTimer;
        public static System.Action ActiveConfigPanel;
        public static System.Action ActiveDemonstrationPanel;
        public static System.Action ActiveRecapPanel;
        public static System.Action OnDeleteClass;
        public static System.Action<GestureData> PlayFileReader;
        public static System.Action StartFileReader;
        public static System.Action StopFileReader;

    }
}