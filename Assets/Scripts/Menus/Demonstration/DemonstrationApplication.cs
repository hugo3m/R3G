using System;
using Menus.Demonstration.Model;
using Menus.Demonstration.View;
namespace Menus.Demonstration
{
    public class DemonstrationElement : MenuElement<DemonstrationApplication>
    {
    }
    public class DemonstrationApplication: AppMenuItem
    {
        [NonSerialized]
        public DemonstrationModel model;
        public DemonstrationView view;

        private void Awake()
        {
            model = new DemonstrationModel();
        }

        

        public override string GetTitle()
        {
            return "Demonstration";
        }
    }
}