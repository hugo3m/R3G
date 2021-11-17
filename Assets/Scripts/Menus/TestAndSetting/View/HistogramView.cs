using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Menus.TestAndSetting.View
{
    public class HistogramView : MonoBehaviour
    {
        private List<float> _vals;
        public GameObject OneUnitBar;

        private List<GameObject> bars;

        public static float ScaleOneUnitY = 10f;

        private RectTransform _rectTrans;

        private float SPACE_BETWEEN_BARS=2f;

        // Start is called before the first frame update

        private void Awake()
        {
            _rectTrans = gameObject.GetComponent<RectTransform>();
        }

        void Start()
        {
        }


        public void SetVals(List<float> valeurs, List<string> labels)
        {
            _rectTrans.sizeDelta = new Vector2(_rectTrans.sizeDelta.x, 0);
            this._vals = valeurs;
            bars = new List<GameObject>();

            float sizeOneBar = (_rectTrans.sizeDelta.x - (valeurs.Count - 1) * SPACE_BETWEEN_BARS ) / (valeurs.Count);

            int i = 0;
            foreach (float v in _vals)
            {
                GameObject newBar = Instantiate(OneUnitBar,gameObject.transform);
                OneUnitBar.gameObject.transform.localScale=Vector3.one;

                newBar.GetComponent<RectTransform>().sizeDelta = new Vector2(sizeOneBar, ScaleOneUnitY * v);
                newBar.GetComponent<RectTransform>().localPosition = new Vector3(
                    bars.Count * (sizeOneBar + SPACE_BETWEEN_BARS) 
                    - _rectTrans.sizeDelta.x / 2 
                   ,
                    ScaleOneUnitY * v / 2f, 0f);

                newBar.GetComponentsInChildren<TextMeshProUGUI>()[0].text = labels[i];

                bars.Add(newBar);
                i++;
            }
            GameObject limite = Instantiate(OneUnitBar,gameObject.transform);
            limite.name = "Limite";
            limite.GetComponent<Image>().color = Color.black;
            limite.GetComponent<Image>().type = Image.Type.Simple;
            limite.GetComponent<RectTransform>().sizeDelta = new Vector2(2, 100f);
            limite.GetComponent<RectTransform>().localPosition = new Vector3(
                -_rectTrans.sizeDelta.x/2,
               0, 0f);
        }


        /// <summary>
        /// Set the value of the specified bar of the current histogramme
        /// Have to be called after initView
        /// </summary>
        /// <param name="barIndex">the index of the bar to change value</param>
        /// <param name="value"></param>
        public void SetValue(int barIndex, float value)
        {
            GameObject bar = this.bars[barIndex];
            Vector2 size = _rectTrans.sizeDelta;
            RectTransform rectTr = bar.GetComponent<RectTransform>();
            rectTr.sizeDelta = new Vector2(rectTr.sizeDelta.x, ScaleOneUnitY * value);
           
        }


        public void SetScore(int barIndex, float value)
        {
            bars[barIndex].GetComponentsInChildren<TextMeshProUGUI>()[1].text = value.ToString("0.00");
        }

        public void ColorOne(int classifier, Color color)
        {
            GameObject bar = this.bars[classifier];
            bar.GetComponent<Image>().color = color;
            bar.GetComponentInChildren<TextMeshProUGUI>().color = Color.red;
        }

        // Update is called once per frame
        public void Update()
        {
        }

        public void CleanScores()
        {
            foreach (var bar in bars)
            {
                bar.GetComponentsInChildren<TextMeshProUGUI>()[1].text ="";
            }
        }
    }
}