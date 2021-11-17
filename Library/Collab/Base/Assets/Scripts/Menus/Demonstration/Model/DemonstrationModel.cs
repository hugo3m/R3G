using System;
using System.Collections.Generic;
using Recognizer;
using Recognizer.DataTools;
using Recognizer.DemonstrationClient;
using Random = System.Random;

namespace Menus.Demonstration.Model
{
    public class DemonstrationModel
    {

        private UserData _currentUser;
        private List<UserData> _users = new List<UserData>();
        private DevicesInfos.Device _device;
        private List<DevicesInfos.Device> _devices = new List<DevicesInfos.Device>();
        private DataBase _dataBase;
        private List<DataBase> _dataBases = new List<DataBase>();
        private float _startTime;
        private bool _finished = true;
        private List<GestureData> _gesturesToDo = new List<GestureData>();
        private int _nbGesturesToDo = 0;
        private List<GestureData> _availableGestures = new List<GestureData>();
        private Dictionary<string, Func<List<GestureData>>> _dataClasses;
        private GestureData _currentGesture;
        private AIResult _result;

        public void SetCurrentGesture(GestureData gesture)
        {
            _currentGesture = gesture;
        }

        public void SetAIResult(AIResult res)
        {
            _result = res;
        }

        public AIResult GetAIResult()
        {
            return _result;
        }

        public GestureData GetCurrentGesture()
        {
            return _currentGesture;
        }

        public int GetNbGesturesToDo()
        {
            return _nbGesturesToDo;
        }

        public void SetNbGesturesToDo(int n)
        {
            if (n != 0)
            {
                _nbGesturesToDo = n;
            }
        }

        public List<GestureData> GetAvailableGestures()
        {
            return _availableGestures;
        }

        public void SetDataClassesAndAvailableGestures(Dictionary<string, Func<List<GestureData>>> datas)
        {
            this._dataClasses = datas;
            _availableGestures.Clear();
            foreach (Func<List<GestureData>> s in this._dataClasses.Values)
            {
                if (s().Count > 0)
                {
                    _availableGestures.Add(s()[0]);
                }
            }
        }

        public List<GestureData> GetGesturesToDo()
        {
            return _gesturesToDo;
        }

        public Dictionary<string, Func<List<GestureData>>> GetDataClasses()
        {
            return this._dataClasses;
        }

        public void SetRandomlyGesturesToDo(int nbGesturesToDo)
        {
            var random = new Random();
            if (_availableGestures.Count > 0)
            {
                for (int i = 0; i < nbGesturesToDo; i++)
                {
                    int index = random.Next(_availableGestures.Count);
                    _gesturesToDo.Add(_availableGestures[index]);
                }
            }
        }

        public void SetGesturesToDo()
        {
            _gesturesToDo = new List<GestureData>();
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
                this._devices.Add(device);
            }
        }

        public List<DevicesInfos.Device> GetDeviceList()
        {
            return this._devices;
        }

        /// <summary>
        /// add all the data bases available to the model
        /// </summary>
        /// <param name="dataBases"> array of all databases supported by the demonstrator for this session </param>
        public void SetDataBases(List<DataBase> dbs)
        {
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
        public void SetUsers(List<UserData> users)
        {
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
        public void SetCurrentUser(UserData newCurrentUser)
        {
            this._currentUser = newCurrentUser;
        }

        /**
        return the current user of the model
        */
        public UserData GetCurrentUser()
        {
            return this._currentUser;
        }

        public void SetDevice(DevicesInfos.Device device)
        {
            this._device = device;
        }

        public DevicesInfos.Device GetDevice()
        {
            return this._device;
        }
    }
}
