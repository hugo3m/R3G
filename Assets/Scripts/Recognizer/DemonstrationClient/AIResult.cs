using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Recognizer.DemonstrationClient
{
    public class AIResult
    {
        public double timestamp = 0;
        public double rejection = 0;
        public List<double> scores = new List<double>(0);

        public AIResult(double timestamp = 0.0, double rejection = 0.0, List<double> scores = null)
        {
            this.timestamp = timestamp;
            this.rejection = rejection;
            this.scores = scores;
            if (scores == null)
            {
                this.scores = new[] {0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0}.ToList();
            }
        }

        public AIResult(string AIMessage)
        {
            string[] message = AIMessage.Split(';');

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            timestamp = Convert.ToDouble(message[0], provider);
            rejection = Convert.ToDouble(message[1], provider);

            string[] scorestab = message[2].Split('_');
            foreach (var t in scorestab)
            {
                scores.Add(Convert.ToDouble(t, provider));
            }
        }

        public override string ToString()
        {
            return "time: " + timestamp.ToString() + "; rejection: " + rejection.ToString() + "; predicted: " +
                   PredictedGesture().ToString();
        }

        public int PredictedGesture()
        {
            var index = -1;
            if (rejection > 0.5)
            {
                var max = double.NegativeInfinity;

                for (int i = 0; i < scores.Count; i++)
                {
                    if (!(scores[i] > max)) continue;
                    max = scores[i];
                    index = i;
                }
            }
            return index;
        }
    }
}