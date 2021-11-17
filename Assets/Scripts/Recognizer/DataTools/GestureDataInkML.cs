using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Leap.Unity.Query;
using Menus.UsersManagement.Controller;
using Menus.UsersManagement.Model;
using ProtoBuf;
using UnityEngine;

namespace Recognizer
{
    public class GestureDataInkML : GestureData
    {
        private UserData _currentUser;
        private List<GestureData> _directives;

        public GestureDataInkML(string path, string dataName, string data, string classe, UserData user,
            List<GestureData> directives) : base(path, data, classe,
            dataName)
        {
            _currentUser = user;
            _directives = directives;
        }

        public new string ExtractData()
        {
            string header = "<?xml version='1.0' encoding = 'utf-8'?>" +
                            "\n<ink xmlns='http://www.w3.org/2003/InkML'>";
            string format = "\n\t<traceFormat>" +
                            "\n\t\t<channel name='X' type='decimal'/>" +
                            "\n\t\t<channel name='Y' type='decimal'/>" +
                            "\n\t\t<channel name='Z' type='decimal'/>" +
                            "\n\t\t<channel name='T' type='decimal'/>" +
                            "\n\t</traceFormat>";
            string directives = _directives.Aggregate("\n\t<annotationXML type='directive'>", (current, gesture) => current + ("\n\t\t<annotation type='gesture'>" + gesture.Classe + "</annotation>"));

            directives += "\n\t</annotationXML>";
            string metadata = "\n\t<annotationXML type='sensor'>" +
                              "\n\t\t<annotation type='sensorModel'>" + RecoManager.GetInstance().DeviceInfo.Device +
                              "</annotation>" +
                              "\n\t</annotationXML>" +
                              "\n\t<annotationXML type='user'>" +
                              "\n\t\t<annotation type='lastName'>" + _currentUser.LastName + "</annotation>" +
                              "\n\t\t<annotation type='firstName'>" + _currentUser.FirstName + "</annotation>" +
                              "\n\t\t<annotation type='age'>" + _currentUser.Age + "</annotation>" +
                              "\n\t\t<annotation type='weight'>" + _currentUser.Weight + "</annotation>" +
                              "\n\t\t<annotation type='size'>" + _currentUser.Size + "</annotation>" +
                              "\n\t\t<annotation type='hand'>" + _currentUser.Hand + "</annotation>" +
                              "\n\t\t<annotation type='userId'>" + _currentUser.Id + "</annotation>" +
                              "\n\t</annotationXML>" + directives;

            string rawdata = "\n\t<traceGroup>";
            
            rawdata += DataFullOpti;
            
            rawdata += "\n\t</traceGroup>" +
                       "\n</ink>";
            
            return header + format + metadata + rawdata;
        }
    }
}