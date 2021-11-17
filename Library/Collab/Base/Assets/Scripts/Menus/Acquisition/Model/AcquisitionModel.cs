using System;
using System.Collections.Generic;
using Recognizer;
using Recognizer.DataTools;
using UnityEngine;
using Random = System.Random;

namespace Menus.Acquisition.Model
{
    public class AcquisitionModel
    {
        private string _acquisitionType ="";
        private List<string> _classSet  = new List<string>();
        private UserData _currentUser;
        private List<UserData> _users = new List<UserData>();
        private DevicesInfos.Device _device;
        private List<DevicesInfos.Device> _deviceList = new List<DevicesInfos.Device>();
        private DataBase _dataBase;
        private List<DataBase> _dataBases = new List<DataBase>();
        private float _startTime;
        private bool _finished = true;
        private List<GestureData> _gesturesToDo = new List<GestureData>();


        public List<GestureData> GetGesturesToDo()
        {
            return _gesturesToDo;
        }

        public void SetRandomlyGesturesToDo(int nbGesturesToDo, List<GestureData> gesturesList)
        {
            var random = new Random();
            for (int i = 0; i < nbGesturesToDo; i++)
            {
                int index = random.Next(gesturesList.Count);
                _gesturesToDo.Add(gesturesList[index]);
            }
        }
        public void SetStartTime(float start)
        {
            this._startTime = start;
        }
        public float GetStartTime()
        {
            return this._startTime;
        }

        public bool GetFinished()
        {
            return this._finished;
        }

        public void SetFinished(bool finishedOrNot)
        {
            _finished = finishedOrNot;
        }
        
        /// <summary>
        /// set the data base that will be used for this session
        /// </summary>
        /// <param name="dataBaseSelected"> data base to use selected by the user </param>
        public void SetDataBase(DataBase dataBaseSelected)
        {
            this._dataBase = dataBaseSelected;
        }

        public DataBase GetDataBase()
        {
            return this._dataBase;
        }

        public void SetDeviceList(Array devices)
        {
            foreach (DevicesInfos.Device device in devices)
            {
                this._deviceList.Add(device);
            }
        }

        public List<DevicesInfos.Device> GetDeviceList()
        {
            return this._deviceList;
        }

        /// <summary>
        /// add all the data bases available to the model
        /// </summary>
        /// <param name="dataBases"> array of all databases supported by the demonstrator for this session </param>
        public void SetDataBases(List<DataBase> dbs){
            foreach (DataBase database in dbs)
            {
                this._dataBases.Add(database);
            }
        }

        public List<DataBase> GetDataBases()
        {
            return this._dataBases;
        }
        
        /**
        Set the users with a list of UserData in parameter 
        */
        public void SetUsers(List<UserData> users){
            foreach (UserData user in users)
            {
                this._users.Add(user);
            }
        }

        /**
        Set the users with a list of UserData in parameter 
        */
        public List<UserData> GetUsers()
        {
            return this._users;
        }

        /**
        Parameter : UserData the new current user
        Goal : Set the new current User of the model
        */
        public void SetCurrentUser(UserData newCurrentUser){
            this._currentUser = newCurrentUser;
        }
        
        /**
        return the current user of the model
        */
        public UserData GetCurrentUser(){
            return this._currentUser;
        }
        
        /// <summary>
        /// add the different gesture classes available for detection in the model
        /// </summary>
        /// <param name="gestureDataClasses"> list of classes from the current data base </param>
        public void SetGestureDataClasses(List<string> gestureDataClasses){
            foreach (string gestureClass in gestureDataClasses)
            {
                _classSet.Add(gestureClass);
            }
        }

        public void SetDevice(DevicesInfos.Device device)
        {
            this._device = device;
        }

        public DevicesInfos.Device GetDevice()
        {
            return this._device;
        }

        public void SetAcquisitionType(string name)
        {
            this._acquisitionType = name;
        }

        public string GetAcquisitionType()
        {
            return this._acquisitionType;
        }
    }
}