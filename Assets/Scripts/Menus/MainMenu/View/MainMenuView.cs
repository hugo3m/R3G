using System;
using System.Collections.Generic;
using Leap.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Menus.MainMenu.View
{
    public class MainMenuView : MainMenuElement
    {
        

        public Canvas Canvas;

        public GameObject MenuContainer;
        public float SpaceBetweenMenus=200;
        public RectTransform PrefabMenuView;
        
        
        private Vector2 _pos = Vector2.zero;


        private float _minX;
        private float _maxX;
        private RectTransform _rectOfContainer;
        private float _spaceToMove;


        // Start is called before the first frame update
        private void Awake()
        {
            _rectOfContainer  = MenuContainer.GetComponent<RectTransform>();
            _spaceToMove = (_rectOfContainer.rect.width + SpaceBetweenMenus);
        }
        
        /// <summary>
        /// Méthode initilisant la vue
        /// Utilise la méthode AppMenuItem.GetTitle pour identifier le menu
        /// </summary>
        public void InitView()
        {
            List<AppMenuItem> appMenuItems = app.model.appMenuItems;

            for (var index = 0; index < appMenuItems.Count; index++)
            {
                AppMenuItem item = appMenuItems[index];
                RectTransform menu = Instantiate(PrefabMenuView);
                menu.SetParent(MenuContainer.transform);
                menu.GetComponentInChildren<Text>().text = item.GetTitle();
                menu.localScale=Vector3.one;
                menu.offsetMin = new Vector2(index*(MenuContainer.GetComponent<RectTransform>().rect.width+(index==0?0:SpaceBetweenMenus)),0);
                menu.offsetMax = new Vector2(index*(MenuContainer.GetComponent<RectTransform>().rect.width+(index==0?0:SpaceBetweenMenus)),0);
                menu.SetLocalZ(0);
            }
            app.model.currentSelectedItem=0;
            _minX = 0;
            _maxX = (appMenuItems.Count-1) * ( MenuContainer.GetComponent<RectTransform>().rect.width + SpaceBetweenMenus);
        }

     
        
        // Update is called once per frame
        void Update()
        {
            Vector2 vec = _rectOfContainer.anchoredPosition;
            _rectOfContainer.anchoredPosition = Vector2.MoveTowards(vec, _pos, Time.deltaTime*6000);
        }


        public void MoveRight()
        {
            if(!(Math.Abs(_pos.x +_maxX) < 0.1f))
            {
                _pos = new Vector2(_pos.x -_spaceToMove , _pos.y);
                app.model.currentSelectedItem++;
            }
        }
        
        public void MoveLeft()
        {
            if (!(Math.Abs(_pos.x - _minX) < 0.1))
            {
                _pos = new Vector2(_pos.x + _spaceToMove, _pos.y);
                app.model.currentSelectedItem--;
            }

        }
    }
}
