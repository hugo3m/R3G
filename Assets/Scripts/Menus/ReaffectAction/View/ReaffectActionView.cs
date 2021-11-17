using System;
using System.Collections.Generic;
using Recognizer;
using UnityEngine;
using UnityEngine.UI;
using Button = UnityEngine.UI.Button;

namespace Menus.ReaffectAction.View
{
    /// <summary>
    /// lance les events :
    /// Replay
    /// Change
    /// AddChange
    /// </summary>
    public class ReaffectActionView : ReaffectActionElement
    {
        public RectTransform Panel;
        [NonSerialized]
        public List<Selectable> Buttons = new List<Selectable>();
        public Button relearnButton;
        public Button createGestureButton;
        public Button AddGestureToLearn;
        public Selectable gestActionPrefab;
        public RectTransform availableGestPanelContent;
        public RectTransform availableGestPanel;
        
        //public RectTransform gestToLearnPanel;
        public Selectable availabilityGestAction;
        public AnimationFileReader fileReader;
        public RawImage fileReaderRawImage;
        public Camera camera;

        private AppMenuItem _createGesture;
        private Selectable _selectedButton;
        private Selectable _destination;
        private Selectable _selectedGestToUpdate;
        private string _selectedAction;
        private bool _doExit = false;
        private bool _doMoveLeft = false;
        private bool _doMoveRight = false;
        private bool _doEnter = false;
        private List<Selectable> _availabilityGestList = new List<Selectable>();
        private List<Selectable> _toLearnGestList = new List<Selectable>();
        private float positionCounter;


        /// <summary>
        /// Construit la vue en fonction des actions et des gestes affectés, génère les boutons
        /// </summary>
        /// <param name="gestActions">les couples geste-> action</param>
        public void InitView(List<Tuple<string, string>> gestActions,List<string> otherGestures)
        {
            int counter = 0;
            positionCounter = 0;
            foreach (Selectable button in Buttons)
            {
                Destroy(button.gameObject);
            }
            Buttons.Clear();
            
            foreach (Tuple<string, string> gestAction in gestActions)
            {
                counter++;
                Selectable gestButton = Instantiate(gestActionPrefab, Panel.transform);
                gestButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(Panel.anchoredPosition.x, Panel.anchoredPosition.y + positionCounter);
                
                
                gestButton.name = gestAction.Item2;
                
                Text[] texts = gestButton.GetComponentsInChildren<Text>();
                texts[0].text = gestAction.Item2; //action text
                texts[1].text = gestAction.Item1; //gesture text
                // These methods are for clicking with mouse
                gestButton.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => ReaffectActionNotification.Replay(gestButton.GetComponentsInChildren<Text>()[1].text)); // Replay button
                gestButton.GetComponentsInChildren<Button>()[1].interactable = gestAction.Item1!="";
                gestButton.GetComponentsInChildren<Button>()[2].onClick.AddListener(() => ReaffectActionNotification.Change(false)); // Change button
                gestButton.GetComponentsInChildren<Button>()[2].onClick.AddListener(() => UpdateSelectedAction(gestButton, gestAction.Item2)); // Change button
                gestButton.GetComponentsInChildren<Button>()[3].onClick.AddListener(() =>
                {
                   ReaffectActionNotification.Deselect(gestAction.Item2, gestButton);
                    gestButton.GetComponentsInChildren<Button>()[1].interactable = false;

                }); // Deselect button
                gestButton.GetComponentsInChildren<Button>()[3].onClick.AddListener(() => texts[1].text = "");
                Buttons.Add(gestButton);
                positionCounter -=  35;
            }
            
            foreach (string classe in otherGestures)
            {
                counter++;
                AddGestToLeanButton(classe);
             
                //positionCounter -=  35;
            }
            
            _selectedButton = Buttons[0];
            _createGesture = app.model.createGesture;
            //relearnButton.onClick.AddListener(HideScrollPanel);
            relearnButton.onClick.AddListener(()=>
            {
                app.model.DoChange();
                relearnButton.interactable = false;
                ReaffectActionNotification.Init();
            });
            relearnButton.interactable = false;
            createGestureButton.onClick.AddListener(() => MenuNotificationGeneral.OnNavigationRequired(_createGesture));
            AddGestureToLearn.onClick.AddListener(() => AddGestToLeanButton(""));
            //gesturesToLearnButton.onClick.AddListener(() => ReaffectActionNotification.);
            
            //Panel.GetComponent<RectTransform>().sizeDelta = new Vector2(Panel.GetComponent<RectTransform>().sizeDelta.x, counter*Panel.GetComponent<GridLayoutGroup>().cellSize.y);
        }

        private void AddGestToLeanButton(string classe)
        {
            Selectable gestButton = Instantiate(gestActionPrefab, Panel.transform);
            gestButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(Panel.anchoredPosition.x, Panel.anchoredPosition.y + positionCounter);
            positionCounter -=  35;
            
            /*Panel.GetComponent<RectTransform>().sizeDelta = new Vector2(Panel.GetComponent<RectTransform>().sizeDelta.x, 
                Panel.GetComponent<RectTransform>().sizeDelta.y + Panel.GetComponent<GridLayoutGroup>().cellSize.y);*/
                    
            gestButton.name = "";
                
            Text[] texts = gestButton.GetComponentsInChildren<Text>();
            texts[0].text = ""; //action text
            texts[1].text = classe; //gesture text
            // These methods are for clicking with mouse
            gestButton.GetComponentsInChildren<Button>()[1].onClick.AddListener(() => ReaffectActionNotification.Replay(gestButton.GetComponentsInChildren<Text>()[1].text)); // Replay button
            gestButton.GetComponentsInChildren<Button>()[1].interactable = classe!="";
            gestButton.GetComponentsInChildren<Button>()[2].onClick.AddListener(() => ReaffectActionNotification.Change(true)); // Change button
            gestButton.GetComponentsInChildren<Button>()[2].onClick.AddListener(() => UpdateSelectedAction(gestButton, classe)); // Change button
            gestButton.GetComponentsInChildren<Button>()[3].onClick.AddListener(() =>
            {
                ReaffectActionNotification.RemoveGestureToLearn(gestButton.GetComponentsInChildren<Text>()[1].text,gestButton);
                gestButton.GetComponentsInChildren<Text>()[1].text = "";
                gestButton.GetComponentsInChildren<Button>()[1].interactable = false;
            }); // Deselect button
            Buttons.Add(gestButton);
        }

    

        private void HideIfClickedOutside() {
            if (Input.GetMouseButton(0) && availableGestPanel.gameObject.activeSelf &&
                !RectTransformUtility.RectangleContainsScreenPoint(
                    availableGestPanel,
                    Input.mousePosition,
                    camera)) {
                HideScrollPanel(availableGestPanel);
            }
        }


        private void Update()
        {
            if (_doExit)
            {
                MenuNotificationGeneral.OnNavigationRequired(null);
                _doExit = false;
            }
            
            
            HideIfClickedOutside();
        }

        public void HideScrollPanel(RectTransform panel)
        {
            panel.gameObject.SetActive(false);
            //panel.GetComponent<CanvasGroup>().interactable = true;
            //panel.GetComponent<CanvasGroup>().alpha = 1.0f;
            foreach (Selectable gest in _availabilityGestList)
                Destroy(gest.gameObject);
            _availabilityGestList.Clear();
        }

        private void UpdateSelectedAction(Selectable gestureButton, string action)
        {
            _selectedAction = action;
            _selectedGestToUpdate = gestureButton;
            _selectedButton = _selectedGestToUpdate;
        }

     
       
        /// <summary>
        /// Joue le fichier passé en paramètre, dans une fenetre plus grande (comme dans la création de geste)
        /// </summary>
        /// <param name="data">la donnée à play</param>
        public void Play(GestureData data)
        {
            //fileReader.filename = ".\\RecognitionServer\\AppRecognizer\\RAW_LeapMotionDataFolder\\" + data.DataName;
            fileReaderRawImage.gameObject.SetActive(true);
            if (data != null)
            {
                fileReader.LoadNewGeste(data.Path);
                fileReader.Play();   
            }
        }

        /// <summary>
        /// affiche un écran avec la liste des classes, celles qui sont indisponibles sont grisé, le geste est éventuellement joué en boucle
        /// </summary>
        /// <param name="avaibleClasses">Liste de triplet <nomClasse,Donnée Représentative, avaible></param>
        public void ShowAvailableClasses(List<Tuple<string,GestureData,bool>> avaibleClasses, bool isOnlyGestToLearn)
        {
            float positionCounter = 0.0f;
            float counter = 0;
            availableGestPanel.gameObject.SetActive(true);
            if (_availabilityGestList.Count != 0) return;
            
            foreach (Tuple<string, GestureData, bool> availability in avaibleClasses)
            {
                counter++;
                Selectable availabilityButton = Instantiate(availabilityGestAction, availableGestPanelContent.transform);
                //availabilityButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(availableGestPanelContent.anchoredPosition.x, availableGestPanelContent.anchoredPosition.y+positionCounter);
                availabilityButton.name = availability.Item1;
                Text availabilityText = availabilityButton.GetComponentInChildren<Text>();
                availabilityText.text = availability.Item1;
                availabilityButton.interactable = availability.Item3;
                availabilityButton.GetComponent<Button>().onClick.AddListener(() =>
                {
                    if(!isOnlyGestToLearn)
                         ReaffectActionNotification.AddChange(availability.Item1, _selectedAction, _selectedButton);
                    else                         
                        ReaffectActionNotification.AddToLearn(_selectedButton.GetComponentsInChildren<Text>()[1].text,availability.Item1);
                    _selectedButton.GetComponentsInChildren<Button>()[1].interactable = true;
                });
                availabilityButton.GetComponent<Button>().onClick.AddListener(() => _selectedButton = availabilityButton);
                availabilityButton.GetComponent<Button>().onClick.AddListener(() => _selectedGestToUpdate.GetComponentsInChildren<Text>()[1].text = _selectedButton.name);
                _availabilityGestList.Add(availabilityButton);
                positionCounter -= 30.0f;
            }
            //availableGestPanelContent.GetComponent<RectTransform>().sizeDelta = new Vector2(availableGestPanelContent.GetComponent<RectTransform>().sizeDelta.x, counter*30);
        }

      /*  public void ShowToLearnClasses(List<string> toLearnClasses)
        {
            float positionCounter = 0.0f;
            float counter = 0;
            gestToLearnPanel.gameObject.SetActive(true);
            if (_toLearnGestList.Count != 0) return;

            foreach (string toLearn in toLearnClasses)
            {
                counter++;
                Selectable toLearnButton = Instantiate(availabilityGestAction, gestToLearnPanelContent.transform);
                toLearnButton.GetComponent<RectTransform>().anchoredPosition = new Vector2(gestToLearnPanelContent.anchoredPosition.x, gestToLearnPanelContent.anchoredPosition.y+positionCounter);
                toLearnButton.name = toLearn;
                Text toLearnText = toLearnButton.GetComponentInChildren<Text>();
                toLearnText.text = toLearn;
                toLearnButton.GetComponent<Button>().onClick.AddListener(() => ReaffectActionNotification.AddToLearn(toLearn));
                _toLearnGestList.Add(toLearnButton);
                positionCounter -= 30.0f;
            }
        }*/

        public void DoMoveLeft()
        {
            _doMoveLeft = true;
        }
        
        public void DoMoveRight()
        {
            _doMoveRight = true;
        }
        
        public void DoEnter()
        {
            _doEnter = true;
        }
        
        public void DoExit()
        {
            _doExit = true;
        }

        public void DisplayRelarn(bool hasChanges)
        {
            relearnButton.interactable = hasChanges;
        }
    }
}