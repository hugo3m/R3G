using System;
using Menus.ClassifierEvaluation.Controller;
using Menus.ClassifierEvaluation.Model;
using Menus.ClassifierEvaluation.View;
using UnityEngine;

namespace Menus.ClassifierEvaluation
{
    public class ClassifierEvaluationElement : MenuElement<ClassifierEvaluationApplication>
    {
    }
    public class ClassifierEvaluationApplication: AppMenuItem
    {
        [NonSerialized]
        public ClassifierEvaluationModel model;
        public ClassifierEvaluationView view;

        protected override void Awake()
        {
            base.Awake();
            model=new ClassifierEvaluationModel();
        }

        private void Start()
        {
            Draw();
        }


        public override string GetTitle()
        {
            return "Évaluation du moteur";
        }
    }
}