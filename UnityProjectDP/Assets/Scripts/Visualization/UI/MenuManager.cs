using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using OALProgramControl;
using AnimArch.Visualization.Animating;
using AnimArch.Visualization.Diagrams;
using AnimArch.Visualization.Animation;

namespace AnimArch.Visualization.UI
{
    public class MenuManager : Singleton<MenuManager>
    {
        FileLoader fileLoader;

        //UI Panels
        [SerializeField] private GameObject introScreen;
        [SerializeField] private GameObject animationScreen;
        [SerializeField] private GameObject mainScreen;
        [SerializeField] private Button saveBtn;
        [SerializeField] private TMP_Dropdown animationsDropdown;
        [SerializeField] private TMP_InputField scriptCode;
        [SerializeField] private GameObject PanelColors;
        [SerializeField] private GameObject PanelInteractiveIntro;
        [SerializeField] private GameObject PanelInteractive;
        [SerializeField] private GameObject PanelMethod;
        public bool isCreating = false;
        [SerializeField] private List<GameObject> methodButtons;
        [SerializeField] private TMP_Text ClassNameTxt;
        [SerializeField] private GameObject InteractiveText;
        [SerializeField] private GameObject PanelInteractiveShow;
        [SerializeField] private TMP_Text classFromTxt;
        [SerializeField] private TMP_Text classToTxt;
        [SerializeField] private TMP_Text methodFromTxt;
        [SerializeField] private TMP_Text methodToTxt;
        [SerializeField] private GameObject PanelInteractiveCompleted;
        [SerializeField] private Slider speedSlider;
        [SerializeField] private TMP_Text speedLabel;
        private string interactiveSource = "source";
        private string sourceClassName = "";
        [SerializeField] public GameObject panelError;
        [SerializeField] public GameObject panelAnimationPlay;
        [SerializeField] public GameObject panelStepMode;
        [SerializeField] public GameObject panelPlayMode;
        [SerializeField] private TMP_InputField sepInput;
        [SerializeField] private TMP_Text classTxt;
        [SerializeField] private TMP_Text methodTxt;
        [SerializeField] private Toggle hideRelToggle;
        public Anim createdAnim;
        public bool isPlaying = false;
        public Button[] playBtns;
        public GameObject playIntroTexts;
        public List<AnimMethod> animMethods;
        public bool isSelectingNode;
        
        struct InteractiveData
        {
            public string fromClass;
            public string fromMethod;
            public string relationshipName;
            public string toClass;
            public string toMethod;
        }


        InteractiveData interactiveData = new InteractiveData();

        private void Awake()
        {
            fileLoader = GameObject.Find("FileManager").GetComponent<FileLoader>();
        }

        private void Start()
        {
            UpdateSpeed();
        }

        //Update the list of created animations
        public void UpdateAnimations()
        {
            List<string> options = new List<string>();
            foreach (Anim anim in AnimationData.Instance.getAnimList())
            {
                options.Add(anim.AnimationName);
            }

            if (animationsDropdown != null)
            {
                animationsDropdown.ClearOptions();
                animationsDropdown.AddOptions(options);
            }
        }

        public void SetSelectedAnimation(string name)
        {
            animationsDropdown.value = animationsDropdown.options.FindIndex(option => option.text == name);
        }

        public void InitializeAnim()
        {
            createdAnim = new Anim("");
        }

        public void StartAnimate()
        {
            InteractiveText.GetComponent<DotsAnimation>().currentText =
                "Select source class\n for call function\ndirectly in diagram\n.";
            scriptCode.text = "";
            //OALScriptBuilder.GetInstance().Clear();
            InteractiveData interactiveData = new InteractiveData();
            isCreating = true;
            introScreen.SetActive(false);
            PanelInteractiveIntro.SetActive(true);
            PanelMethod.SetActive(false);
            PanelInteractive.SetActive(true);
            PanelInteractiveCompleted.SetActive(false);
            animationScreen.SetActive(true);
            mainScreen.SetActive(false);
        }

        public void EndAnimate()
        {
            isCreating = false;
            PanelInteractiveIntro.SetActive(false);
            PanelInteractive.SetActive(false);
            animationScreen.SetActive(false);
            saveBtn.interactable = false;
            mainScreen.SetActive(true);
            introScreen.SetActive(true);
            PanelInteractiveCompleted.SetActive(false);
            PanelInteractiveShow.SetActive(false);
        }

        public void SelectClass(String name)
        {
            foreach (GameObject button in methodButtons)
            {
                button.SetActive(false);
            }

            Class selectedClass = DiagramPool.Instance.ClassDiagram.FindClassByName(name).ParsedClass;
            PanelInteractiveIntro.SetActive(false);
            ClassNameTxt.text = name;
            PanelMethod.SetActive(true);
            int i = 0;
            if (selectedClass.Methods != null)
            {
                foreach (Method m in selectedClass.Methods)
                {
                    if (interactiveData.fromMethod == null)
                    {
                        if (i < methodButtons.Count)
                        {
                            methodButtons[i].SetActive(true);
                            methodButtons[i].GetComponentInChildren<TMP_Text>().text = m.Name + "()";
                        }

                        if (interactiveData.fromClass != null)
                        {
                            Animating.Animation.Instance.HighlightClass(interactiveData.fromClass, false, -1);
                        }

                        interactiveData.fromClass = name;
                        Animating.Animation.Instance.HighlightClass(interactiveData.fromClass, true);
                        i++;
                        if (sepInput.text.Length > 2 && !classTxt.text.Equals("class unselected") &&
                            !methodTxt.text.Equals("method unselected"))
                        {
                            createdAnim.SetMethodCode(classTxt.text, methodTxt.text, sepInput.text);
                        }

                        sepInput.interactable = false;
                        classTxt.text = name;
                        methodTxt.text = "method unselected";
                    }
                    else
                    {
                        if (i < methodButtons.Count &&
                            DiagramPool.Instance.ClassDiagram.FindEdge(interactiveData.fromClass, name) != null)
                        {
                            methodButtons[i].SetActive(true);
                            methodButtons[i].GetComponentInChildren<TMP_Text>().text = m.Name + "()";
                        }

                        if (interactiveData.toClass != null)
                        {
                            Animating.Animation.Instance.HighlightClass(interactiveData.toClass, false);
                        }

                        interactiveData.toClass = name;
                        Animating.Animation.Instance.HighlightClass(interactiveData.toClass, true);
                        i++;
                    }
                }
            }

            UpdateInteractiveShow();
            PanelInteractiveIntro.SetActive(false);
            PanelMethod.SetActive(true);
        }

        public void SelectMethod(int buttonID)
        {
            if (interactiveData.fromMethod == null)
            {
                string methodName = methodButtons[buttonID].GetComponentInChildren<TMP_Text>().text;
                //scriptCode.text += "\n" + "call(\n" + ClassNameTxt.text + ", " + methodName+",";
                InteractiveText.GetComponent<DotsAnimation>().currentText =
                    "Select target class\nfor call function\ndirectly in diagram\n.";
                interactiveData.fromMethod = methodName;
                Animating.Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod,
                    true);
                sepInput.interactable = true;
                sepInput.text = createdAnim.GetMethodBody(interactiveData.fromClass, interactiveData.fromMethod);
                UpdateInteractiveShow();
            }
            else
            {
                string methodName = methodButtons[buttonID].GetComponentInChildren<TMP_Text>().text;
                //scriptCode.text += "\n"+ClassNameTxt.text+", "+methodName+"\n);";
                InteractiveText.GetComponent<DotsAnimation>().currentText =
                    "Select source class\nfor call function\ndirectly in diagram\n.";
                interactiveData.toMethod = methodName;
                UpdateInteractiveShow();
                Animating.Animation.Instance.HighlightClass(interactiveData.fromClass, false);
                Animating.Animation.Instance.HighlightClass(interactiveData.toClass, false);
                Animating.Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod,
                    false);
                WriteCode();
            }

            PanelInteractiveIntro.SetActive(true);
            PanelMethod.SetActive(false);
        }

        private void WriteCode()
        {
            if (!scriptCode.text.EndsWith("\n") && scriptCode.text.Length > 1)
                scriptCode.text += "\n";
            scriptCode.text += OALScriptBuilder.GetInstance().AddCall(
                interactiveData.fromClass, interactiveData.fromMethod,
                OALProgram.Instance.RelationshipSpace
                    .GetRelationshipByClasses(interactiveData.fromClass, interactiveData.toClass).RelationshipName,
                interactiveData.toClass,
                interactiveData.toMethod
            );
            if (!sepInput.text.EndsWith("\n") && sepInput.text.Length > 1)
                sepInput.text += "\n";
            sepInput.text += OALScriptBuilder.GetInstance().AddCall(
                interactiveData.fromClass, interactiveData.fromMethod,
                OALProgram.Instance.RelationshipSpace
                    .GetRelationshipByClasses(interactiveData.fromClass, interactiveData.toClass).RelationshipName,
                interactiveData.toClass,
                interactiveData.toMethod
            );
            createdAnim.SetMethodCode(interactiveData.fromClass, interactiveData.fromMethod, sepInput.text);
            interactiveData = new InteractiveData();
        }

        //Save animation to file and memory
        public void SaveAnimation()
        {
            if (sepInput.text.Length > 2 && !classTxt.text.Equals("class unselected") &&
                !methodTxt.text.Equals("method unselected"))
            {
                createdAnim.SetMethodCode(classTxt.text, methodTxt.text, sepInput.text);
            }

            scriptCode.gameObject.SetActive(true);

            scriptCode.GetComponent<CodeHighlighter>().RemoveColors();
            createdAnim.Code = scriptCode.text;
            scriptCode.gameObject.SetActive(false);
            fileLoader.SaveAnimation(createdAnim);
            EndAnimate();
        }

        public void SelectAnimation()
        {
            String name = animationsDropdown.options[animationsDropdown.value].text;
            foreach (Anim anim in AnimationData.Instance.getAnimList())
            {
                if (name.Equals(anim.AnimationName))
                    AnimationData.Instance.selectedAnim = anim;
            }
        }

        public void OpenAnimation()
        {
            if (AnimationData.Instance.getAnimList().Count > 0)
            {
                SelectAnimation();
                StartAnimate();
                scriptCode.text = AnimationData.Instance.selectedAnim.Code;
                AnimationData.Instance.RemoveAnim(AnimationData.Instance.selectedAnim);
                UpdateAnimations();
            }
        }

        public void ActivatePanelColors(bool show)
        {
            PanelColors.SetActive(show);
        }

        public void UpdateInteractiveShow()
        {
            PanelInteractiveCompleted.SetActive(false);
            PanelInteractiveShow.SetActive(true);
            classFromTxt.text = "Class: ";
            methodFromTxt.text = "Method: ";
            classToTxt.text = "Class: ";
            if (interactiveData.fromClass != null)
            {
                classFromTxt.text = "Class: " + interactiveData.fromClass;
                classTxt.text = interactiveData.fromClass;
            }

            if (interactiveData.fromMethod != null)
            {
                methodFromTxt.text = "Method: " + interactiveData.fromMethod;
                methodTxt.text = interactiveData.fromMethod;
                sepInput.interactable = true;
            }

            if (interactiveData.toClass != null)
            {
                classToTxt.text = "Class: " + interactiveData.toClass;
            }

            if (interactiveData.toMethod != null)
            {
                PanelInteractiveCompleted.SetActive(true);
            }
        }

        public void UpdateSpeed()
        {
            if (speedSlider && speedLabel)
            {
                AnimationData.Instance.AnimSpeed = speedSlider.value;
                speedLabel.text = speedSlider.value.ToString() + "s";
            }
        }

        public void PlayAnimation()
        {
            if (AnimationData.Instance.selectedAnim.AnimationName.Equals(""))
            {
                panelError.SetActive(true);
            }
            else
            {
                isPlaying = true;
                panelAnimationPlay.SetActive(true);
                mainScreen.SetActive(false);
                foreach (Button button in playBtns)
                {
                    button.gameObject.SetActive(false);
                }

                playIntroTexts.SetActive(true);
                if (Animating.Animation.Instance.standardPlayMode)
                {
                    panelStepMode.SetActive(false);
                    panelPlayMode.SetActive(true);
                }
                else
                {
                    panelPlayMode.SetActive(false);
                    panelStepMode.SetActive(true);
                }
            }
        }

        public void ResetInteractiveSelection()
        {
            if (interactiveData.fromClass != null)
                Animating.Animation.Instance.HighlightClass(interactiveData.fromClass, false);
            if (interactiveData.toClass != null)
                Animating.Animation.Instance.HighlightClass(interactiveData.toClass, false);
            if (interactiveData.fromMethod != null)
                Animating.Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod,
                    false);
            InteractiveText.GetComponent<DotsAnimation>().currentText =
                "Select source class\nfor call function\ndirectly in diagram\n.";
            interactiveData = new InteractiveData();
            UpdateInteractiveShow();
            PanelInteractiveIntro.SetActive(true);
            PanelMethod.SetActive(false);
        }

        public void ChangeMode()
        {
            Animating.Animation.Instance.UnhighlightAll();
            Animating.Animation.Instance.isPaused = false;
            if (Animating.Animation.Instance.standardPlayMode)
            {
                Animating.Animation.Instance.standardPlayMode = false;
                panelPlayMode.SetActive(false);
                panelStepMode.SetActive(true);
            }
            else
            {
                Animating.Animation.Instance.standardPlayMode = true;
                panelStepMode.SetActive(false);
                panelPlayMode.SetActive(true);
            }
        }

        public void SelectPlayClass(string name)
        {
            DiagramPool.Instance.ObjectDiagram.ResetDiagram();
            DiagramPool.Instance.ObjectDiagram.LoadDiagram();

            Animating.Animation.Instance.UnhighlightAll();
            Animating.Animation.Instance.HighlightClass(name, true);

            playIntroTexts.SetActive(false);
            Animating.Animation.Instance.startClassName = name;
            foreach (Button button in playBtns)
            {
                button.gameObject.SetActive(false);
            }

            Class selectedClass = DiagramPool.Instance.ClassDiagram.FindClassByName(name).ParsedClass;
            animMethods = AnimationData.Instance.selectedAnim.GetMethodsByClassName(name);
            int i = 0;
            if (animMethods != null)
            {
                foreach (AnimMethod m in animMethods)
                {
                    if (i < 4)
                    {
                        playBtns[i].GetComponentInChildren<TMP_Text>().text = m.Name + "()";
                        playBtns[i].gameObject.SetActive(true);
                        i++;
                    }
                }
            }

            CDClassInstance instance = OALProgram.Instance.ExecutionSpace.getClassByName(name).CreateClassInstance();
            ObjectInDiagram od = DiagramPool.Instance.ObjectDiagram.AddObject(name, "client", instance);
            DiagramPool.Instance.ObjectDiagram.AddObject(od);
            DiagramPool.Instance.ObjectDiagram.ShowObject(od);
            DiagramPool.Instance.RelationsClassToObject[0].Show();
        }

        public void SelectPlayMethod(int id)
        {
            Animating.Animation.Instance.startMethodName = animMethods[id].Name;
            foreach (Button button in playBtns)
            {
                button.gameObject.SetActive(false);
            }

            playIntroTexts.SetActive(true);
            Debug.Log("Selected class: " + Animating.Animation.Instance.startClassName + "Selected Method: " +
                      Animating.Animation.Instance.startMethodName);
            Animating.Animation.Instance.HighlightClass(Animating.Animation.Instance.startClassName, false);
        }

        public void UnshowAnimation()
        {
            Animating.Animation.Instance.UnhighlightAll();
        }

        public void EndPlay()
        {
            isPlaying = false;
            foreach (Button button in playBtns)
            {
                button.gameObject.SetActive(false);
            }

            Animating.Animation.Instance.startClassName = "";
            Animating.Animation.Instance.startMethodName = "";
        }

        public void HideGraphRelations()
        {
            if (hideRelToggle.isOn)
            {
                foreach (var interGraphRelation in DiagramPool.Instance.RelationsClassToObject)
                {
                    interGraphRelation.Show();
                }
            }
            else
            {
                foreach (var interGraphRelation in DiagramPool.Instance.RelationsClassToObject)
                {
                    interGraphRelation.Hide();
                }
            }
        }
    }
}