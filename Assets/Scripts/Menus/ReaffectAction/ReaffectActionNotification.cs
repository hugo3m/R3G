using UnityEngine.UI;

namespace Menus.ReaffectAction
{
    public class ReaffectActionNotification
    {
        public static System.Action<string> Replay;
        public static System.Action<bool> Change;
        public static System.Action<string, string, Selectable> AddChange;
        public static System.Action<string,string> AddToLearn;
        public static System.Action<string, Selectable> Deselect;
        public static System.Action<string,Selectable> RemoveGestureToLearn;
        public static System.Action Init;

     
    }
}