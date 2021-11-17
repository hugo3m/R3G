using System.Collections.Generic;
using System.Linq;
using Menus.TestAndSetting;
using Recognizer;
using Recognizer.EvaluationTools;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Menus.ClassifierEvaluation.View
{
    /// <summary>
    /// lance les events de la classe notification
    /// </summary>
    public class ClassifierEvaluationView : ClassifierEvaluationElement
    {
        public TextMeshProUGUI LabelsAndCount;
        public InputField HoldOutDataProportion;
        public InputField KCrossValidationK;

        public GameObject ResultView;
        public TextMeshProUGUI TitleTest;
        public TextMeshProUGUI FmesureValue;
        public TextMeshProUGUI PrecisionValue;
        public TextMeshProUGUI RecallValue;
        public TextMeshProUGUI ErrorRateValue;
        public GridLayoutGroup Matrix;
        public RectTransform MatrixHolder;

        private string _currentTestClicked;

        /// <summary>
        /// Initialise la vue, affiche le nombre de donnée pour chaque classe et le nombre total
        /// </summary>
        public void InitView()
        {
            foreach (string classe in app.model.AppClassesLearned)
            {
                if(app.model.DataPerClass.ContainsKey(classe))
                    LabelsAndCount.text += classe + " : " + app.model.DataPerClass[classe].Count + " exemples \n";
            }

            LabelsAndCount.text += "\n Total examples : " +
                                   app.model.DataPerClass.Where((x) => app.model.AppClassesLearned.Contains(x.Key))
                                       .Aggregate(0, (acc, x) => acc + x.Value.Count);
        }

        /// <summary>
        /// Affiche le resultat de l'évaluation
        /// </summary>
        /// <param name="res"> le résultat</param>
        public void DisplayResult()
        {
            ResultView.SetActive(true);
            TitleTest.text = _currentTestClicked;
            EvaluationResult res = app.model.ResEval;
            FmesureValue.text = res.Fscore.ToString("0.00");
            PrecisionValue.text = res.Precision.ToString("0.00");
            RecallValue.text = res.Recall.ToString("0.00");
            ErrorRateValue.text = res.ErrorRate.ToString("0.00");


            //----the matrix----
            //clear the matrix
            foreach (Transform child in Matrix.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            int count = res.ConfusionMatrix.Count + 1; //+1 for the label
            Matrix.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            Matrix.constraintCount = count;
            float width = Matrix.gameObject.GetComponent<RectTransform>().rect.width;
            float height = Matrix.gameObject.GetComponent<RectTransform>().rect.height;
            Matrix.cellSize = new Vector2((width - (count - 1) * Matrix.spacing.x) / count, (height - (count - 1) * Matrix.spacing.y) / count);

            //float hei = 30 * count;
            MatrixHolder.rect.Set(MatrixHolder.rect.x, MatrixHolder.rect.y, MatrixHolder.rect.width,  MatrixHolder.rect.height);

            Font arial;
            arial = (Font) Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
            //labels : classes
            foreach (string classe in app.model.AppClassesLearned)
            {
                GameObject gm = new GameObject();
                Text text = gm.AddComponent<Text>();
                gm.transform.parent = Matrix.gameObject.transform;
                text.font = arial;
                text.text = classe;
                text.fontSize = 11;
                text.fontStyle = FontStyle.Bold;
                text.color = Color.black;
                text.alignment = TextAnchor.MiddleCenter;
                text.horizontalOverflow = HorizontalWrapMode.Overflow;
                text.verticalOverflow = VerticalWrapMode.Overflow;
                gm.transform.localPosition = Vector3.zero;
                gm.transform.localRotation = transform.rotation = Quaternion.identity;
                gm.transform.localScale = Vector3.one;
            }

            //an empty one
            GameObject g = new GameObject();
            g.AddComponent<Text>();
            g.transform.parent = Matrix.gameObject.transform;

            //fill teh matrix
            foreach (string classe in app.model.AppClassesLearned)
            {
                foreach (string c2 in app.model.AppClassesLearned)
                {
                    float val = res.ConfusionMatrix[classe][c2];

                    GameObject gm = new GameObject();
                    Text text = gm.AddComponent<Text>();
                    text.font = arial;
                    text.color = Color.black;
                    text.fontSize = 13;
                    text.alignment = TextAnchor.MiddleCenter;
                    text.horizontalOverflow = HorizontalWrapMode.Overflow;
                    text.verticalOverflow = VerticalWrapMode.Overflow;
                    
                    gm.transform.parent = Matrix.gameObject.transform;
                    text.text = DoFormat(val);
                    gm.transform.localPosition = Vector3.zero;
                    gm.transform.localRotation = transform.rotation = Quaternion.identity;
                    gm.transform.localScale = Vector3.one;
                }

                //the label
                GameObject g2 = new GameObject();
                Text text2 = g2.AddComponent<Text>();
                text2.font = arial;
                text2.color = Color.black;
                text2.fontStyle = FontStyle.Bold;
                text2.alignment = TextAnchor.MiddleCenter;
                g2.transform.parent = Matrix.gameObject.transform;
                g2.transform.localPosition = Vector3.zero;
                g2.transform.localRotation = transform.rotation = Quaternion.identity;
                g2.transform.localScale = Vector3.one;
                text2.text = classe;
            }
        }

        private static string DoFormat(float myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);

            if (s.EndsWith("00"))
            {
                return ((int) myNumber).ToString();
            }

            return s;
        }


        public void OnHoldOutClicked()
        {
            _currentTestClicked = " Hold out : " + float.Parse(HoldOutDataProportion.text).ToString("0.00") + "%";
            ClassifierEvaluationNotification.HoldOut(float.Parse(HoldOutDataProportion.text));
        }

        public void OnKCrossClicked()
        {
            _currentTestClicked = " Cross validation : K = " + float.Parse(KCrossValidationK.text);
            ClassifierEvaluationNotification.CrossValidation(int.Parse(KCrossValidationK.text));
        }

        private void Update()
        {
            if (app.model.NewValue)
            {
                app.model.NewValue = false;
                DisplayResult();
            }
        }
    }
}