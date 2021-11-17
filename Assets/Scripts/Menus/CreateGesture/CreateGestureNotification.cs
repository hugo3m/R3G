using System;
using Recognizer;

namespace Menus.CreateGesture
{
    public class CreateGestureNotification
    {
        public static System.Action<string> ClassSelected;
        public static System.Action<GestureData> DataSelected;
        public static System.Action DeleteData;
        public static System.Action ReplayClicked;
        public static  System.Action<bool> Record;
    }
}