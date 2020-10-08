using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEditor;
using System;
using System.Text.RegularExpressions;
using OALProgramControl;
using Assets.Scripts.Animation;

public class MenuManager : Singleton<MenuManager>
{

    FileLoader fileLoader;

    //UI Panels
    [SerializeField]
    private GameObject introScreen;
    [SerializeField]
    private GameObject animationScreen;
    [SerializeField]
    private GameObject mainScreen;
    [SerializeField]
    private Button saveBtn;
    [SerializeField]
    private TMP_Dropdown animationsDropdown;
    [SerializeField]
    private TMP_InputField scriptCode;
    [SerializeField]
    private GameObject PanelColors;
    [SerializeField]
    private GameObject PanelInteractiveIntro;
    [SerializeField]
    private GameObject PanelInteractive;
    [SerializeField]
    private GameObject PanelMethod;
    public bool isCreating=false;
    [SerializeField]
    private List<GameObject> methodButtons;
    [SerializeField]
    private TMP_Text ClassNameTxt;
    [SerializeField]
    private GameObject InteractiveText;
    [SerializeField]
    private GameObject PanelInteractiveShow;
    [SerializeField]
    private TMP_Text classFromTxt;
    [SerializeField]
    private TMP_Text classToTxt;
    [SerializeField]
    private TMP_Text methodFromTxt;
    [SerializeField]
    private TMP_Text methodToTxt;
    [SerializeField]
    private GameObject PanelInteractiveCompleted;
    [SerializeField]
    private Slider speedSlider;
    [SerializeField]
    private TMP_Text speedLabel;
    private string interactiveSource = "source";
    private string sourceClassName = "";
    [SerializeField]
    public GameObject panelError;
    [SerializeField]
    public GameObject panelAnimationPlay;
    [SerializeField]
    public GameObject panelStepMode;
    [SerializeField]
    public GameObject panelPlayMode;
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
        animationsDropdown.ClearOptions();
        animationsDropdown.AddOptions(options);
    }
    public void SetSelectedAnimation(string name)
    {
        animationsDropdown.value= animationsDropdown.options.FindIndex(option => option.text == name);
    }
    public void StartAnimate()
    {
        InteractiveText.GetComponent<DotsAnimation>().currentText= "Select source class\n for call function\ndirectly in diagram\n.";
        scriptCode.text = "";
        OALScriptBuilder.GetInstance().Clear();
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
        foreach(GameObject button in methodButtons)
        {
            button.SetActive(false);
        } 
        Class selectedClass = ClassDiagram.Instance.FindClassByName(name);
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
                        Animation.Instance.HighlightClass(interactiveData.fromClass, false);
                    }
                    interactiveData.fromClass = name;
                    Animation.Instance.HighlightClass(interactiveData.fromClass, true);
                    i++;

                }
                else
                {
                    if (i < methodButtons.Count && ClassDiagram.Instance.FindEdge(interactiveData.fromClass, name) != null)
                    {
                        methodButtons[i].SetActive(true);
                        methodButtons[i].GetComponentInChildren<TMP_Text>().text = m.Name + "()";
                    }
                    if (interactiveData.toClass != null)
                    {
                        Animation.Instance.HighlightClass(interactiveData.toClass, false);
                    }
                    interactiveData.toClass = name;
                    Animation.Instance.HighlightClass(interactiveData.toClass, true);
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
        if (interactiveData.fromMethod==null)
        {
            string methodName = methodButtons[buttonID].GetComponentInChildren<TMP_Text>().text;
            //scriptCode.text += "\n" + "call(\n" + ClassNameTxt.text + ", " + methodName+",";
            InteractiveText.GetComponent<DotsAnimation>().currentText = "Select target class\nfor call function\ndirectly in diagram\n.";
            interactiveData.fromMethod = methodName;
            Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod, true);
            UpdateInteractiveShow();
        }
        else
        {
            string methodName = methodButtons[buttonID].GetComponentInChildren<TMP_Text>().text;
            //scriptCode.text += "\n"+ClassNameTxt.text+", "+methodName+"\n);";
            InteractiveText.GetComponent<DotsAnimation>().currentText = "Select source class\nfor call function\ndirectly in diagram\n.";
            interactiveData.toMethod = methodName;
            UpdateInteractiveShow();
            Animation.Instance.HighlightClass(interactiveData.fromClass, false);
            Animation.Instance.HighlightClass(interactiveData.toClass, false);
            Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod, false);
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
            OALProgram.Instance.RelationshipSpace.GetRelationshipByClasses(interactiveData.fromClass, interactiveData.toClass).RelationshipName, interactiveData.toClass,
            interactiveData.toMethod
        );

        interactiveData = new InteractiveData();
    }

    //Save animation to file and memory
    public void SaveAnimation()
    {
        scriptCode.GetComponent<CodeHighlighter>().RemoveColors();
        Anim newAnim = new Anim("", scriptCode.text);
        fileLoader.SaveAnimation(newAnim);
        EndAnimate();
    }
    public void SelectAnimation()
    {
         String name= animationsDropdown.options[animationsDropdown.value].text;
        foreach(Anim anim in AnimationData.Instance.getAnimList())
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
        }
        if (interactiveData.fromMethod != null)
        {
            methodFromTxt.text = "Method: " + interactiveData.fromMethod;
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
        AnimationData.Instance.AnimSpeed = speedSlider.value;
        speedLabel.text = speedSlider.value.ToString()+"s";

    }
    public void PlayAnimation()
    {
        if (AnimationData.Instance.selectedAnim.AnimationName.Equals(""))
        {
            panelError.SetActive(true);
        }
        else
        {
            panelAnimationPlay.SetActive(true);
            mainScreen.SetActive(false);
            if (Animation.Instance.standardPlayMode)
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
    {   if(interactiveData.fromClass!=null)
            Animation.Instance.HighlightClass(interactiveData.fromClass, false);
        if (interactiveData.toClass != null)
            Animation.Instance.HighlightClass(interactiveData.toClass, false);
        if (interactiveData.fromMethod != null)
            Animation.Instance.HighlightMethod(interactiveData.fromClass, interactiveData.fromMethod, false);
        InteractiveText.GetComponent<DotsAnimation>().currentText = "Select source class\nfor call function\ndirectly in diagram\n.";
        interactiveData = new InteractiveData();
        UpdateInteractiveShow();
        PanelInteractiveIntro.SetActive(true);
        PanelMethod.SetActive(false);
    }
    public void ChangeMode()
    {
        Animation.Instance.UnhighlightAll();
        Animation.Instance.isPaused = false;
        if (Animation.Instance.standardPlayMode)
        {
            Animation.Instance.standardPlayMode = false;
            panelPlayMode.SetActive(false);
            panelStepMode.SetActive(true);
        }
        else
        {
            Animation.Instance.standardPlayMode = true;
            panelStepMode.SetActive(false);
            panelPlayMode.SetActive(true);
        }
    }
}
