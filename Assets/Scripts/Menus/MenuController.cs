using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Leap.Unity;
using Menus.MainMenu.Controller;
using Recognizer;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Menus
{
    [Serializable]
    public class DevicePrefab
    {
        public DevicesInfos.Device Device;
        public IDeviceProvider Provider;
    }
    
    public abstract class MenuController<T>: MenuElement<T> where T : AppMenuItem
    {

        public List<DevicePrefab> DevicePrefabs;
        
        private Dictionary<string, Action> _gestAction;
        private Stack<AppMenuItem> _menuItems;

        private Queue<Action> _toHandleNavigation = new Queue<Action>();
        private GameObject _containerMenu;

        private void Awake()
        {
            _gestAction = new Dictionary<string, Action>();
            _gestAction.Add("Right",MoveRight);
            _gestAction.Add("Left",MoveLeft);
            _gestAction.Add("Forward",Enter);
            _gestAction.Add("Backward",Exit);
        }

        // Start is called before the first frame update
        protected virtual void Start()
        {
            
        }

        protected virtual void OnEnable()
        {
            MenuNotificationGeneral.GestureRecognized += DoAction;
            if (MenuNotificationGeneral.OnNavigationRequired == null)//only the first menu will be subscribed
            {
                MenuNotificationGeneral.OnNavigationRequired += HandleNavigation;
                if (_menuItems != null)
                {
                    while (_menuItems.Count>1)
                    {
                        AppMenuItem appMenuItem = _menuItems.Pop();
                        Destroy(appMenuItem.gameObject);
                    }
                    _menuItems.Peek().Draw();
                }
                else
                {
                    _menuItems = new Stack<AppMenuItem>();
                    _menuItems.Push(app);
                }
                //on active le LeapReco seulement pour la racine
                Resources.FindObjectsOfTypeAll<RecoManager>()[0].gameObject.SetActive(true);
                
                _containerMenu = new GameObject("Menus_instanciated");
                _containerMenu.transform.SetParent( this.app.transform);

                StartCoroutine(InstanciateProviderWhenLaunched());
            }
            else
            {
                gameObject.transform.GetComponentInChildren<RecoManager>(true).gameObject.SetActive(false);
            }
        }

        protected void lateOnEnable()
        {
            MenuNotificationGeneral.GestureRecognized += DoAction;
            if (MenuNotificationGeneral.OnNavigationRequired == null)//only the first menu will be subscribed
            {
                MenuNotificationGeneral.OnNavigationRequired += HandleNavigation;
                if (_menuItems != null)
                {
                    while (_menuItems.Count > 1)
                    {
                        AppMenuItem appMenuItem = _menuItems.Pop();
                        Destroy(appMenuItem.gameObject);
                    }
                    _menuItems.Peek().Draw();
                }
                else
                {
                    _menuItems = new Stack<AppMenuItem>();
                    _menuItems.Push(app);
                }

            }
            else
            {
                gameObject.transform.GetComponentInChildren<RecoManager>(true).gameObject.SetActive(false);
            }
        }

        protected void lateInstanciateProvider()
        {
            //on active le LeapReco seulement pour la racine
            Resources.FindObjectsOfTypeAll<RecoManager>()[0].gameObject.SetActive(true);
            StartCoroutine(InstanciateProviderWhenLaunched());
        }

        private IEnumerator InstanciateProviderWhenLaunched()
        {
            while (RecoManager.GetInstance() == null)
            {
                Debug.LogWarning("RecoManagerNot yet Instanciated");
                yield return new WaitForSeconds(0.5f);
            }

            IDeviceProvider provider = FindObjectOfType<IDeviceProvider>();
            if (provider == null)
            {
                DevicePrefab devicePrefabs = DevicePrefabs.First(x => x.Device == RecoManager.GetInstance().Device);
                if (devicePrefabs.Provider != null)
                {
                    Instantiate(devicePrefabs.Provider);
                    GameObject c = GameObject.FindGameObjectWithTag("MainCamera");
                    c.transform.position = devicePrefabs.Provider.CameraPosRelativ;
                }
                else
                {
                    Debug.LogWarning("Provider device null, not instanciated");
                }    
            }
       
       }

        /// <summary>
        /// Gère la navigation entre les mennu
        /// </summary>
        /// <param name="menuItem"> le menu vers lequel on veut naviguer, est null si on veut retourner en arrière</param>
        private void HandleNavigation(AppMenuItem menuItem)
        {
            _toHandleNavigation.Enqueue(()=>DoHandleNavigationSync(menuItem));
        }


        private void DoHandleNavigationSync(AppMenuItem menuItem)
        {
            if (menuItem == null)
            {
                if (_menuItems.Count > 1)//il en faut minimum deux dans la pile pour pouvoir dépiler (toujours la racine dans la pile)
                {
                    AppMenuItem appMenuItem = _menuItems.Pop();
                    Destroy(appMenuItem.gameObject);
                }
                _menuItems.Peek().Draw();
            } 
            else
            {
                _menuItems.Peek().Undraw();
                AppMenuItem appMenuItem = Instantiate(menuItem);
                appMenuItem.transform.SetParent(_containerMenu.transform);
                _menuItems.Push(appMenuItem);
                appMenuItem.Draw();
            }
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            while (_toHandleNavigation.Count!=0)
            {
                _toHandleNavigation.Dequeue()();
            }

            if (_menuItems!=null && _menuItems.Count == 1 && !_menuItems.Peek().IsDrawn)
            {
                _menuItems.Peek().Draw();
            }
            
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Exit();
            }
        }
        /// <summary>
        /// Redigige vers la bonne méthode en fonction du geste
        /// </summary>
        /// <param name="geste">le geste reconnu</param>
        public void DoAction(string geste)
        {
            if(_gestAction.ContainsKey(geste))
                _gestAction[geste]();
        }
        /// <summary>
        /// Une action "aller a droite" a été effectué
        /// </summary>
        protected abstract void MoveRight();
        /// <summary>
        /// Une action "aller a gauche" a été effectué
        /// </summary>
        protected abstract void MoveLeft();
        
        /// <summary>
        /// Une action "entrer" a été effectué
        /// </summary>
        protected abstract void Enter();

        /// <summary>
        /// Une action "quitter" a été effectué
        /// </summary>
        protected virtual void Exit()
        {
            MenuNotificationGeneral.OnNavigationRequired(null);
        }

        protected virtual void OnDisable()
        {
            MenuNotificationGeneral.GestureRecognized -= DoAction;
            MenuNotificationGeneral.OnNavigationRequired -= HandleNavigation;
        }
    }
}
