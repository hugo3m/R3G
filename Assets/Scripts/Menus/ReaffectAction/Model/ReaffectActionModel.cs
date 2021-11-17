using System;
using System.Collections.Generic;
using System.Linq;
using Menus;
using Recognizer;
using UnityEngine;
using UnityEngine.UI;

namespace UnityEditor
{
    public class ReaffectActionModel
    {
        
        public List<Tuple<string, GestureData, bool>> AvailabilityClasses { get; set; }
        public List<string> ToLearnClasses { get; set; } = new List<string>();
        public List<string> InitToLearnClassesTotal { get; set; } = new List<string>();
        public AppMenuItem createGesture;
        //gest->action
        public List<Tuple<string, string>> Changes = new List<Tuple<string, string>>();
        private IActionProvider _actionProvider;
        private List<Tuple<string, string>> _gestAction = new List<Tuple<string, string>>();
        private List<string> _learned = new List<string>();
        private List<string> _unUsedActions = new List<string>();
        private bool _hasChange;

        public ReaffectActionModel(IActionProvider ap, AppMenuItem createGesture)
        {
            _actionProvider = ap;
            this.createGesture = createGesture;
            _hasChange = false;
        }
        
        
        /// <summary>
        /// Dissocie un geste de son action
        /// </summary>
        /// <param name="action"></param>
        /// <param name="selectedButton"></param>
        public void DeselectGesture(string action, Selectable selectedButton)
        {
            _unUsedActions.Add(action);
            Changes.Remove(Changes.First(x => x.Item2 == action));
        }
        
        /// <summary>
        /// Ajoute un changement, un geste associé à une autre action
        /// </summary>
        /// <param name="geste"></param>
        /// <param name="action"></param>
        /// <param name="selectedButton"></param>
        public void AddChange(string geste, string action, Selectable selectedButton)
        {
            _hasChange = true;
            if (!Changes.Contains(Changes.FirstOrDefault(x => x.Item1 == geste)))
            {
                if(Changes.FirstOrDefault(x => x.Item2 == action) != null)
                    Changes.Remove(Changes.First(x => x.Item2 == action));
                
                Changes.Add(new Tuple<string, string>(geste, action));
                if (_unUsedActions.Contains(action))
                    _unUsedActions.Remove(action);
            }
        }


        /// <summary>
        /// Cf diagramme de séquence "Reaffect_Model_DoChange"
        /// Effectue le changement, appel au leapRecoManager
        /// </summary>
        public void DoChange()
        {
            RecoManager reco = RecoManager.GetInstance();
            _gestAction = _actionProvider.GetMapAction();
            List<Tuple<string, string>> newMap = GenerateNewMapGestAction(new List<Tuple<string, string>>(_gestAction), new List<Tuple<string, string>>(Changes));
            Changes.Clear();
            _actionProvider.UpdateMap(newMap);
            List<string> gestsNeeded = new List<string>();
            foreach (Tuple<string,string> tuple in newMap)
            {
                //ajouter les gestes sans actions
                if(tuple.Item1!="")
                    gestsNeeded.Add(tuple.Item1);
            }
            gestsNeeded.AddRange(ToLearnClasses);
            Debug.Log(gestsNeeded);
            _learned = reco.ParamRecoManager.GetAppClassesLearnt();
            if (DoNeedRelearn(_learned, gestsNeeded))
            {
                Debug.Log("relearn needed");
                reco.ParamRecoManager.SetAppClasses(gestsNeeded);
                reco.RelearnAndStart();
            }
        }


        /// <returns>true si il y a un changement de prévu, false sinon</returns>
        public bool HasChanges()
        {
            List<string> gestsNeededNow = new List<string>();
            foreach (Tuple<string,string> tuple in Changes)
            {
                    gestsNeededNow.Add(tuple.Item1);
            }
            gestsNeededNow.AddRange(ToLearnClasses);
            
            foreach (string toLearnClass in InitToLearnClassesTotal)                //ajouter les gestes sans actions
            {
                if (!gestsNeededNow.Contains(toLearnClass))
                    return true;
            }
            return _hasChange || InitToLearnClassesTotal.Count != gestsNeededNow.Count;
        }

        public List<Tuple<string, string>> GetChanges()
        {
            return Changes;
        }

  
        /// <summary>
        /// Vérifie si un apprentissage est nécessaire (différence non nulle)
        /// </summary>
        /// <param name="learned">les gestes actuellement appris par le moteur</param>
        /// <param name="gestsNeeded">les gestes dont le classifieur a besoin</param>
        /// <returns>true si un réapprentissage est nécessaire, faux sinon</returns>
        private bool DoNeedRelearn(List<string> learned, List<string> gestsNeeded)
        {
            foreach (string gest in gestsNeeded)
            {
                if (!learned.Contains(gest))
                {
                    return true;
                }
            }

            return learned.Count!=gestsNeeded.Count;
        }
        /// <summary>
        /// Génére une nouvelle liste de couple gest-> action, en considérant l'ancienne liste et les nouveaux changements
        /// </summary>
        /// <param name="oldMap">L'ancienne liste de couple actuellement apprise par le moteur</param>
        /// <param name="modification">les nouveaux changements</param>
        /// <returns></returns>
        private List<Tuple<string, string>> GenerateNewMapGestAction(List<Tuple<string, string>> oldMap, List<Tuple<string, string>> modification)
        {
            List<Tuple<string, string>> newMap = new List<Tuple<string, string>>();
            foreach (Tuple<string, string> gestAction in oldMap)
            {
                if (_unUsedActions.Contains(gestAction.Item2)) continue;
                
                if (modification.Contains(modification.Find(x => x.Item2 == gestAction.Item2)))
                {
                    newMap.Add(new Tuple<string, string>(modification.First(x => x.Item2 == gestAction.Item2).Item1, gestAction.Item2));
                    modification.Remove(modification.First(x => x.Item2== gestAction.Item2));

                    //newMap.Add(modification.FirstOrDefault(x => x.Value == gestAction.Value).Key, gestAction.Value);
                    
                } else
                    newMap.Add(new Tuple<string, string>(gestAction.Item1, gestAction.Item2));


                /*string modificationAction = modification.FirstOrDefault(x => x.Value == gestAction.Value).Key;
                newMap.Add(string.IsNullOrEmpty(modificationAction) ? gestAction.Key : modificationAction, gestAction.Value);*/
            }

            return newMap;
        }


    }
}