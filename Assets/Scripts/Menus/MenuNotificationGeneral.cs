using System;
using System.Collections.Generic;
using Recognizer;
using Recognizer.StrategyFusion;
using UnityEngine;

namespace Menus
{
    public class MenuNotificationGeneral : AppNotificationForLeapReco
    {
        public static System.Action<string> GestureRecognized;
        public static System.Action<ResultStrategy> ClassScore;
        public static System.Action<RecoManager.StatusReco> OnStatusRecoChanged;
        
        //null to exit
        public static System.Action<AppMenuItem> OnNavigationRequired;
        
        
        public override Action<string> GetGestureRecognizedAction()
        {
            return GestureRecognized;
        }

        public override Action<ResultStrategy> GetClassDetailedScoresAction()
        {
            return ClassScore;
        }

        public override Action<RecoManager.StatusReco> GetStatusRecoChangeAction()
        {
            return OnStatusRecoChanged;
        }
    }
}