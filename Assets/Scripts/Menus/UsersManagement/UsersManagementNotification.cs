using System;

namespace Menus.UsersManagement
{
    public class UsersManagementNotification
    {
        public static System.Action AddUserClicked;
        public static System.Action DeleteUserClicked;
        public static System.Action<UserData> SaveUserClicked;
        public static System.Action<UserData> OnSelectedUser;
        
    }
}
