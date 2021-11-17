using System.Collections.Generic;
using UnityEngine;

namespace Menus.MainMenu.Model
{
    public class MainMenuModel 
    {
        public List<AppMenuItem> appMenuItems;
        public int currentSelectedItem;
        public MainMenuModel(List<AppMenuItem> appMenuItems)
        {
            this.appMenuItems = appMenuItems;
        }

     
    }
}
