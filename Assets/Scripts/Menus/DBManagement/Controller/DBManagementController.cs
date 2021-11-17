using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Menus.CreateGesture.Model;
using Recognizer;
using UnityEngine;
using System.Reflection;
using Menus.DBManagement;
using Recognizer.DataTools;

namespace Menus.DBManagement.Controller
{
    
    public class DBManagementController : MenuController<DBManagementApplication>
    {

        protected override void Start()
        {
            app.model.SetDBs(DBManager.GetInstance().GetDBs());
            app.view.InitView();
        }

        protected override void OnEnable()
        {
            DBManagementNotification.AddDBClicked += OnAddDBClicked;
            DBManagementNotification.OnSelectedDB += OnSelectedDB;
            DBManagementNotification.DeleteDBClicked += OnDeleteDBClicked;
            DBManagementNotification.SaveDBClicked += OnSaveDBClicked;
        }

        protected override void OnDisable()
        {
            DBManagementNotification.AddDBClicked -= OnAddDBClicked;
            DBManagementNotification.OnSelectedDB -= OnSelectedDB;
            DBManagementNotification.DeleteDBClicked -= OnDeleteDBClicked;
            DBManagementNotification.SaveDBClicked -= OnSaveDBClicked;
        }

        protected override void MoveRight()
        {
           
        }

        protected override void MoveLeft()
        {
           
        }

        
        protected override void Enter()
        {
            
        }

        protected override void Update()
        {

        }

        protected void OnAddDBClicked()
        {
            DataBase db = new DataBase("DefaultDB", "./Assets/Scripts/Recognizer/DataTools/DataBases", "LeapMotion");
            DBManager.GetInstance().AddDB(db);
            app.model.SetCurrentDB(db);
            app.view.RefreshCurrentDB();
            app.model.GetDBList().Clear();
            app.model.SetDBs(DBManager.GetInstance().GetDBs());
            app.view.RefreshDBList();
        }

        protected void OnDeleteDBClicked()
        {
            if (app.model.GetCurrentDB() != null)
            {
                DBManager.GetInstance().DeleteDB(app.model.GetCurrentDB());
                app.model.SetCurrentDB(null);
                app.model.GetDBList().Clear();
                app.model.SetDBs(DBManager.GetInstance().GetDBs());
                app.view.RefreshDBList();
                app.view.RefreshCurrentDB();
            }
        }

        protected void OnSelectedDB(DataBase selectedDB)
        {
            app.model.SetCurrentDB(selectedDB);
            app.view.RefreshCurrentDB();
        }

        protected void OnSaveDBClicked(DataBase db)
        {
            DBManager.GetInstance().AddDB(db);
            app.model.GetDBList().Clear();
            app.model.SetDBs(DBManager.GetInstance().GetDBs());
            app.view.RefreshDBList();
            app.model.SetCurrentDB(db);
            app.view.RefreshCurrentDB();
        }
        
    }
}