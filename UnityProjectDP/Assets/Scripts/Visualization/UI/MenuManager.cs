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
    private GameObject PanelStartClassMethod;//
    [SerializeField]
    private TMP_InputField startClass;//
    [SerializeField]
    private TMP_InputField startMethod;//
    [SerializeField]
    private TMP_InputField ConsoleOutputField;//
    [SerializeField]
    public GameObject panelError;
    [SerializeField]
    public GameObject panelAnimationPlay;
    [SerializeField]
    public GameObject panelStepMode;
    [SerializeField]
    public GameObject panelPlayMode;
    [SerializeField]
    private TMP_InputField sepInput;
    [SerializeField]
    private TMP_Text classTxt;
    [SerializeField]
    private TMP_Text methodTxt;
    public Anim createdAnim;
    public bool isPlaying = false;
    public Button[] playBtns;
    public List<AnimMethod> animMethods;

    public GameObject PanelChooseAnimationStartMethod;
    public GameObject PanelSourceCodeAnimation;
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
    public void InitializeAnim()
    {
        createdAnim = new Anim("");
        createdAnim.Initialize();
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
        PanelStartClassMethod.SetActive(true);//
        startClass.text = "";//
        startMethod.text = "";//
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
        PanelStartClassMethod.SetActive(false);//
    }
    public void SelectClass(String name)
    {
        Debug.LogWarning("MenuManager::SelectClass " + (name ?? "NULL"));
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
                    if (sepInput.text.Length > 2 && !classTxt.text.Equals("class unselected") && !methodTxt.text.Equals("method unselected"))
                    {
                        createdAnim.SetMethodCode(classTxt.text, methodTxt.text,sepInput.text);
                    }
                    sepInput.interactable = false;
                    classTxt.text = name;
                    methodTxt.text = "method unselected";


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
            sepInput.interactable = true;
            sepInput.text = createdAnim.GetMethodBody(interactiveData.fromClass, interactiveData.fromMethod);
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
        if (!sepInput.text.EndsWith("\n") && sepInput.text.Length > 1)
            sepInput.text += "\n";
        sepInput.text+= OALScriptBuilder.GetInstance().AddCall(
            interactiveData.fromClass, interactiveData.fromMethod,
            OALProgram.Instance.RelationshipSpace.GetRelationshipByClasses(interactiveData.fromClass, interactiveData.toClass).RelationshipName, interactiveData.toClass,
            interactiveData.toMethod
        );
        createdAnim.SetMethodCode(interactiveData.fromClass, interactiveData.fromMethod, sepInput.text);
        interactiveData = new InteractiveData();
    }

    //Save animation to file and memory
    public void SaveAnimation()
    {
        if (!classTxt.text.Equals("class unselected") && !methodTxt.text.Equals("method unselected"))
        {
            createdAnim.SetMethodCode(classTxt.text, methodTxt.text, sepInput.text);
        }
        scriptCode.gameObject.SetActive(true);

        scriptCode.GetComponent<CodeHighlighter>().RemoveColors();
        createdAnim.Code = scriptCode.text;
        createdAnim.SetStartClassName(startClass.text);//
        createdAnim.SetStartMethodName(startMethod.text);//
        scriptCode.gameObject.SetActive(false);
        fileLoader.SaveAnimation(createdAnim);
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
            createdAnim = AnimationData.Instance.selectedAnim;
            scriptCode.text = createdAnim.Code;
            startClass.text = createdAnim.StartClass;//
            startMethod.text = createdAnim.StartMethod;//
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
            isPlaying = true;
            panelAnimationPlay.SetActive(true);
            mainScreen.SetActive(false);
            introScreen.SetActive(false);
            foreach (Button button in playBtns)
            {
                button.gameObject.SetActive(false);
            }
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
    public void SelectPlayClass(string name)
    {
        Debug.LogWarning("MenuManager::SelectPlayClass");
        PanelChooseAnimationStartMethod.SetActive(true);
        PanelSourceCodeAnimation.SetActive(false);
        Animation.Instance.UnhighlightAll();
        Animation.Instance.HighlightClass(name, true);
        Animation.Instance.startClassName = name;
        foreach (Button button in playBtns)
        {
            button.gameObject.SetActive(false);
        }
        Class selectedClass = ClassDiagram.Instance.FindClassByName(name);
        animMethods = AnimationData.Instance.selectedAnim.GetMethodsByClassName(name);
        int i = 0;
        if (animMethods != null)
        {
            Debug.LogWarning("MenuManager::SelectPlayClass anim methods is not null");
            foreach (AnimMethod m in animMethods )
            {
                Debug.LogWarning(m.Name);
                if (i < 4)
                {
                    playBtns[i].GetComponentInChildren<TMP_Text>().text = m.Name + "()";
                    playBtns[i].gameObject.SetActive(true);
                    i++;
                }
            }
        }
    }
    public void SelectPlayMethod(int id)
    {
        Animation.Instance.startMethodName = animMethods[id].Name;
        foreach (Button button in playBtns)
        {
            button.gameObject.SetActive(false);
        }
        Debug.Log("Selected class: " + Animation.Instance.startClassName + " Selected Method: " + Animation.Instance.startMethodName);
        Animation.Instance.HighlightClass(Animation.Instance.startClassName, false);
        AnimateSourceCodeAtMethodStart(Animation.Instance.startClassName, Animation.Instance.startMethodName);
    }
    public void UnshowAnimation()
    {
        Animation.Instance.UnhighlightAll();
    }
    public void EndPlay()
    {
        isPlaying = false;
        foreach (Button button in playBtns)
        {
            button.gameObject.SetActive(false);
        }
        Animation.Instance.startClassName = "";
        Animation.Instance.startMethodName = "";
        ConsoleOutputField.text = "";//
    }

    public void AnimateSourceCodeAtMethodStart(string className, string methodName)
    {
        PanelChooseAnimationStartMethod.SetActive(false);
        PanelSourceCodeAnimation.SetActive(true);

        PanelSourceCodeAnimation.GetComponent<PanelSourceCodeAnimation>().SetMethodLabelText(className, methodName);

        string sourceCode
            = OALProgram
                .Instance
                .ExecutionSpace
                .getClassByName(className)
                .getMethodByName(methodName)
                .ExecutableCode
                .ToCode();
        PanelSourceCodeAnimation.GetComponent<PanelSourceCodeAnimation>().SetSourceCodeText(sourceCode);
    }
}
