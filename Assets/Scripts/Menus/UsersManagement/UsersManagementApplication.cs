using System;
using Menus.UsersManagement.Controller;
using Menus.UsersManagement.Model;
using Menus.UsersManagement.View;
using Menus.UsersManagement.Controller;
using Menus.UsersManagement.Model;
using Menus.UsersManagement.View;
using UnityEngine;

namespace Menus.UsersManagement
{
    public class UsersManagementElement : MenuElement<UsersManagementApplication>
    {
    }
    public class UsersManagementApplication : AppMenuItem
    {
        [NonSerialized]
        public UsersManagementModel model;
        public UsersManagementView view;

        private void Awake()
        {
            model = new UsersManagementModel();
        }



        public override string GetTitle()
        {
            return "User Management";
        }
    }
}