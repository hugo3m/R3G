using System.Collections.Generic;
using UnityEngine;

namespace Menus
{
    public class MenuElement<T> : MonoBehaviour where T : Object
    {
        // Gives access to the application and all instances.
        //public T app { get { return GameObject.FindObjectOfType<T>(); }}
        public T app => Assert();

        private T _application;

        /// <summary>
        /// Finds an instance of 'T' if 'obj' is null. Returns 'obj' otherwise.
        /// If 'global' is 'true' searches in all scope, otherwise, searches in children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="global"></param>
        /// <returns></returns>
        public T Assert()
        {
            if (_application == null)
            {
                T type = GameObject.FindObjectOfType<T>();
                _application = type;
            }

            return _application;
        }
    }

    public abstract class AppMenuItem : MonoBehaviour
    {
        public bool IsDrawn { get; set; }
        
        // Start is called before the first frame update
        void Start()
        {
        }

        protected bool _hasToBeDraw;

        protected bool _hasToBeUndraw;

        // Update is called once per frame
        protected virtual void Awake()
        {
            _hasToBeDraw = false;
            _hasToBeUndraw = false;
        }

        public void Draw()
        {
            _hasToBeDraw = true;
        }

        public void Undraw()
        {
            _hasToBeUndraw = true;
        }

        protected virtual void Update()
        {
            if (_hasToBeDraw)
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    gameObject.transform.GetChild(i).gameObject.SetActive(true);
                }

                IsDrawn = true;

                _hasToBeDraw = false;
            }

            if (_hasToBeUndraw)
            {
                for (int i = 0; i < gameObject.transform.childCount; i++)
                {
                    gameObject.transform.GetChild(i).gameObject.SetActive(false);
                }
                IsDrawn = false;

                _hasToBeUndraw = false;
            }
        }



        public abstract string GetTitle();
    }
}