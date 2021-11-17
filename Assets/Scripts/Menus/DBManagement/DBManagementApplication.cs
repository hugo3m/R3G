using System;
using Menus.DBManagement.Model;
using Menus.DBManagement.View;
using UnityEngine;

namespace Menus.DBManagement
{
    public class DBManagementElement : MenuElement<DBManagementApplication>
    {
    }
    public class DBManagementApplication : AppMenuItem
    {
        [NonSerialized]
        public DBManagementModel model;
        public DBManagementView view;

        private void Awake()
        {
            model = new DBManagementModel();
        }



        public override string GetTitle()
        {
            return "DB Management";
        }
    }
}