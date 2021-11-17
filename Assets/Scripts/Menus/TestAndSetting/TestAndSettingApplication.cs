using System;
using Menus.TestAndSetting.Controller;
using Menus.TestAndSetting.Model;
using Menus.TestAndSetting.View;
using UnityEngine;

namespace Menus.TestAndSetting
{
    public class TestAndSettingElement : MenuElement<TestAndSettingApplication>
    {
    }
    public class TestAndSettingApplication: AppMenuItem
    {
        
        public TestAndSettingView view;
        [NonSerialized]
        public TestAndSettingModel model;

        protected override void Awake()
        {
            base.Awake();
            model = new TestAndSettingModel();    
        }


        protected void Start()
        {
        }

        public override string GetTitle()
        {
            return "Test et configuration du moteur";
        }
    }
}