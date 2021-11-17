using System;
using System.Collections.Generic;
using Recognizer;
using Recognizer.DataTools;
using UnityEngine;

namespace Menus.DBManagement.Model
{
    public class DBManagementModel
    {
        private List<DataBase> _dataBases = new List<DataBase>();
        private DataBase _currentDataBase;

        public List<DataBase> GetDBList()
        {
            return this._dataBases;
        }

        public void SetDBs(List<DataBase> dbList)
        {
            foreach (DataBase db in dbList)
            {
                this._dataBases.Add(db);
            }
        }

        public DataBase GetCurrentDB()
        {
            return this._currentDataBase;
        }

        public void SetCurrentDB(DataBase selectedDB)
        {
            this._currentDataBase = selectedDB;
        }
    }
}