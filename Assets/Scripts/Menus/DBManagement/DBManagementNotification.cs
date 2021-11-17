using System;
using Recognizer.DataTools;

namespace Menus.DBManagement
{
    public class DBManagementNotification
    {
        public static System.Action AddDBClicked;
        public static System.Action DeleteDBClicked;
        public static System.Action<DataBase> SaveDBClicked;
        public static System.Action<DataBase> OnSelectedDB;
    }
}
