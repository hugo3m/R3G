using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Recognizer
{
    public class GestureData
    {
        public string Path { get; set; }
        protected string[] Data  { get; set; }
        protected string DataFullOpti  { get; set; }
        public string Classe  { get; set; }
        public string DataName  { get; set; }


        protected bool _isDataOneString;
        protected  Action<StringBuilder>  _promiseData;
        protected  string _promiseDataReaded;

        public GestureData(string path, string[] data, string classe,string dataName)
        {
            _isDataOneString = false;
            Path = path;
            Data = data;
            Classe = classe;
            DataName = dataName;
        }
        public GestureData(string path, Action<StringBuilder> data, string classe,string dataName)
        {
            _isDataOneString = false;
            Path = path;
            _promiseData = data;
            Classe = classe;
            DataName = dataName;
        }
        
        public GestureData(string path, string data, string classe,string dataName)
        {
            _isDataOneString = true;
            Path = path;
            DataFullOpti = data;
            Classe = classe;
            DataName = dataName;
        }


        public GestureData(string classe,string dataName)
        {
            Classe = classe;
            DataName = dataName;
        }

        public string ExtractData()
        {
            if (!_isDataOneString)
            {
                if (_promiseData!=null)
                {
                    if (_promiseDataReaded != null)//si déjà lu
                        return _promiseDataReaded;
                    StringBuilder s = new StringBuilder();
                    _promiseData(s);
                    _promiseDataReaded = s.ToString();
                    return _promiseDataReaded;
                }
                else
                {
                    string str = "<class="+Classe+">\n";
                    str+=String.Join("\n",Data);
                    str += "</class="+Classe+">";
                    return str;
                }
             
            }
            //else
            return "<class="+Classe+">\n"+DataFullOpti+"</class="+Classe+">";
        }
    }
}