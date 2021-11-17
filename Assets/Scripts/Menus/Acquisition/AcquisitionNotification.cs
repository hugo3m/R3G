using System;
using Recognizer.DataTools;
using Recognizer;

namespace Menus.Acquisition
{
    public class AcquisitionNotification
    {
        public static System.Action<UserData> OnSelectedUser;
        public static System.Action<DevicesInfos.Device> OnSelectedDevice;
        public static System.Action<DataBase> OnSelectedDb;
        public static System.Action<string> Validate;
        public static System.Action<bool,bool> Record;
        public static System.Action OnDataClasses;
        public static System.Action StartStopTimer;
        public static System.Action ActiveReplayPanel;
        public static System.Action ActiveAcquisitionPanel;
        public static System.Action<GestureData> PlayFileReader;
        public static System.Action StartFileReader;
        public static System.Action StopFileReader;
        public static System.Action CleanFileReader;
        public static System.Action ActiveMenuPanel;
        public static System.Action ActiveGestureClassesPanel;
        public static System.Action ActiveConfigPanel;
        public static System.Action ActiveCreateClassPanel;
        public static System.Action<string> SetClassName;
        public static System.Action<GestureData> OnSelectedGesture;
        public static System.Action OnAddClass;
        public static System.Action OnDeleteClass;
        public static System.Action ActiveDirectivesPanel;
        public static System.Action OnRandomClasses;
        public static System.Action ValidateGesturesToDo;

    }
}
