using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OALProgramControl;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Assets.Scripts.AnimationControl.OAL;


//Controls the entire animation process
public class Animation : Singleton<Animation>
{
    private ClassDiagram classDiagram;
    public Color classColor;
    public Color methodColor;
    public Color relationColor;
    public GameObject LineFill;
    private int BarrierSize;
    private int CurrentBarrierFill;
    [HideInInspector]
    public bool AnimationIsRunning = false;
    private readonly Object AnimationBoolLock = new Object();
    [HideInInspector]
    public bool isPaused = false;
    [HideInInspector]
    public bool standardPlayMode=true;
    public bool nextStep=false;
    private bool prevStep = false;
    private List<GameObject> Fillers;
    private void Awake()
    {
        classDiagram = GameObject.Find("ClassDiagram").GetComponent<ClassDiagram>();
        standardPlayMode = true;
    }

    // Main Couroutine for compiling the OAL of Animation script and then starting the visualisation of Animation
    public IEnumerator Animate()
    {
        Fillers = new List<GameObject>();
        lock (this.AnimationBoolLock)
        {
            if (this.AnimationIsRunning)
            {
                yield break;
            }
            else
            {
                this.AnimationIsRunning = true;
            }
        }

        bool Success;
        AnimationCommandStorage ACS = null;
        Debug.Log("In try block");
        List<Anim> animations = AnimationData.Instance.getAnimList();
        Anim selectedAnimation = AnimationData.Instance.selectedAnim;
        if (animations != null)
        {
            if (animations.Count > 0 && selectedAnimation.AnimationName.Equals(""))
                selectedAnimation = animations[0];
        }
        OALProgram Program = OALProgram.Instance;
        string Code = selectedAnimation.Code;
        Debug.Log("Code: ");
        Debug.Log(Code);
        OALProgram.Instance.SuperScope = OALParserBridge.Parse(Code);
        ACS = new AnimationCommandStorage();
        bool temp = Program.PreExecute(ACS);
        Debug.Log("Done executing: " + temp.ToString());
        ACS.ClearSteps();
        Success = true;
    
        if(Success)
        {
            Debug.Log("We have " + ACS.AnimationSteps.Count() + " anim sequences");
            foreach (List<AnimationCommand> AnimationSequence in ACS.AnimationSteps)
            {
                BarrierSize = AnimationSequence.Count;
                Debug.Log("Filling barrier of size " + BarrierSize);
                CurrentBarrierFill = 0;
                if (!AnimationSequence.Any())
                {
                    continue;
                }
                if (AnimationSequence[0].IsCall)
                {
                    foreach (AnimationCommand Command in AnimationSequence)
                    {
                        StartCoroutine(Command.Execute());
                    }
                    yield return StartCoroutine(BarrierFillCheck());
                }
                else
                {
                    foreach (AnimationCommand Command in AnimationSequence)
                    {
                        Command.Execute();
                    }
                }
            }
        }
        lock (this.AnimationBoolLock)
        {
            this.AnimationIsRunning = false;
        }
    }
    public void IncrementBarrier()
    {
        this.CurrentBarrierFill++;
    }
    public IEnumerator BarrierFillCheck()
    {
        yield return new WaitUntil(() => CurrentBarrierFill >= BarrierSize);
    }
    public void StartAnimation()
    {
        isPaused = false;
        StartCoroutine("Animate");
    }

    //Couroutine that can be used to Highlight class for a given duration of time
    public IEnumerator AnimateClass(string className, float animationLength)
    {
        HighlightClass(className, true);
        yield return new WaitForSeconds(animationLength);
        HighlightClass(className, false);

    }

    //Couroutine that can be used to Highlight method for a given duration of time
    public IEnumerator AnimateMethod(string className, string methodName, float animationLength)
    {
        HighlightMethod(className, methodName, true);
        yield return new WaitForSeconds(animationLength);
        HighlightMethod(className, methodName, false);

    }
    //Couroutine that can be used to Highlight edge for a given duration of time
    public IEnumerator AnimateEdge(string relationshipName, float animationLength)
    {
        HighlightEdge(relationshipName, true);
        yield return new WaitForSeconds(animationLength);
        HighlightEdge(relationshipName, false);
    }
    public IEnumerator AnimateFill(OALCall Call)
    {
        GameObject newFiller = Instantiate(LineFill);
        Fillers.Add(newFiller);
        newFiller.transform.position = classDiagram.graph.transform.GetChild(0).transform.position;
        newFiller.transform.SetParent(classDiagram.graph.transform);
        newFiller.transform.localScale = new Vector3(1, 1, 1);
        LineFiller lf = newFiller.GetComponent<LineFiller>();
        GameObject edge = classDiagram.FindEdge(Call.RelationshipName);
        if (edge != null)
        {
            bool flip = false;
            if (classDiagram.FindOwnerOfRelation(/*Call.CallerClassName, Call.CalledClassName*/Call.RelationshipName).Equals(Call.CalledClassName))
            {
                flip = true;
            }
            yield return lf.StartCoroutine(lf.AnimateFlow(edge.GetComponent<UILineRenderer>().Points, flip));
        }
    }
    //Method used to Highlight/Unhighlight single class by name, depending on bool value of argument 
    public void HighlightClass(string className, bool isToBeHighlighted)
    {
        GameObject node = classDiagram.FindNode(className);
        BackgroundHighlighter bh = null;
        if (node != null)
        {
            bh = node.GetComponent<BackgroundHighlighter>();
        }
        else
        {
            Debug.Log("Node " + className + " not found");
        }
        if (bh != null)
        {
            if (isToBeHighlighted)
            {
                bh.HighlightBackground();
            }
            else
            {
                bh.UnhighlightBackground();
            }
        }
        else
        {
            Debug.Log("Highligher component not found");
        }
    }
    //Method used to Highlight/Unhighlight single method by name, depending on bool value of argument 
    public void HighlightMethod(string className, string methodName, bool isToBeHighlighted)
    {
        GameObject node = classDiagram.FindNode(className);
        TextHighlighter th = null;
        if (node != null)
        {
            th = node.GetComponent<TextHighlighter>();
        }
        else
        {
            Debug.Log("Node " + className + " not found");
        }
        if (th != null)
        {
            if (isToBeHighlighted)
            {
                th.HighlightLine(methodName);
            }
            else
            {
                th.UnHighlightLine(methodName);
            }

        }
        else
        {
            Debug.Log("TextHighligher component not found");
        }
    }
    //Method used to Highlight/Unhighlight single edge by name, depending on bool value of argument 
    public void HighlightEdge(string relationshipName, bool isToBeHighlighted)
    {
        GameObject edge = classDiagram.FindEdge(relationshipName);
        if (edge != null)
        {
            if (isToBeHighlighted)
            {
                edge.GetComponent<UEdge>().ChangeColor(relationColor);
            }
            else
            {
                edge.GetComponent<UEdge>().ChangeColor(Color.white);
            }
        }
        else
        {
            Debug.Log(relationshipName + " NULL Edge ");
        }
    }
    //Couroutine used to Resolve one OALCall consisting of Caller class, caller method, edge, called class, called method
    // Same coroutine is called for play or step mode
    public IEnumerator ResolveCallFunct(OALCall Call)
    {
        int step = 0;
        float speedPerAnim = AnimationData.Instance.AnimSpeed;
        float timeModifier=1f;
        while (step < 7)
        {
            if (isPaused)
            {
                yield return new WaitForFixedUpdate();
            }
            else
            {
                switch (step)
                {
                    case 0: HighlightClass(Call.CallerClassName, true); break;
                    case 1: HighlightMethod(Call.CallerClassName, Call.CallerMethodName, true); break;
                    case 2: yield return StartCoroutine(AnimateFill(Call)); timeModifier = 0f; break;
                    case 3: HighlightEdge(Call.RelationshipName, true); timeModifier = 0.5f; break;
                    case 4: HighlightClass(Call.CalledClassName, true); timeModifier = 1f; break;
                    case 5: HighlightMethod(Call.CalledClassName, Call.CalledMethodName, true); timeModifier = 1.25f; break;
                    case 6:
                        HighlightClass(Call.CallerClassName, false);
                        HighlightMethod(Call.CallerClassName, Call.CallerMethodName, false);
                        HighlightClass(Call.CalledClassName, false);
                        HighlightMethod(Call.CalledClassName, Call.CalledMethodName, false);
                        HighlightEdge(Call.RelationshipName, false);
                        timeModifier = 1f;
                        break;

                }
                step++;
                if (standardPlayMode)
                {
                    yield return new WaitForSeconds(AnimationData.Instance.AnimSpeed * timeModifier);
                }
                //Else means we are working with step animation
                else
                {
                    if (step == 2) step = 3;
                    nextStep = false;
                    prevStep = false; 
                    yield return new WaitUntil(() => nextStep);
                    if (prevStep == true)
                    {
                        if (step > 0) step --;
                        if (step == 2) step = 1;
                        switch (step)
                        {
                            case 0: HighlightClass(Call.CallerClassName, false); break;
                            case 1: HighlightMethod(Call.CallerClassName, Call.CallerMethodName, false); break;
                            case 3: HighlightEdge(Call.RelationshipName, false); break;
                            case 4: HighlightClass(Call.CalledClassName, false); break;
                            case 5: HighlightMethod(Call.CalledClassName, Call.CalledMethodName, false); break;

                        }
                        if(step>-1) step--;
                        if (step == 2) step = 1;
                        switch (step)
                        {
                            case 0: HighlightClass(Call.CallerClassName, false); break;
                            case 1: HighlightMethod(Call.CallerClassName, Call.CallerMethodName, false); break;
                            case 3: HighlightEdge(Call.RelationshipName, false); break;
                            case 4: HighlightClass(Call.CalledClassName, false); break;
                            case 5: HighlightMethod(Call.CalledClassName, Call.CalledMethodName, false); break;

                        }
                    }
                    yield return new WaitForFixedUpdate();
                    nextStep = false;
                    prevStep = false;
                }
            }
        }
    }
    
    public string GetColorCode(string type)
    {
        if (type == "class")
        {
            return ColorUtility.ToHtmlStringRGB(classColor);
        }
        if(type == "method")
        {
            return ColorUtility.ToHtmlStringRGB(methodColor);
        }
        if (type == "relation")
        {
            return ColorUtility.ToHtmlStringRGB(relationColor);
        }
        return "";
    }
    //Method used to stop all animations and unhighlight all objects
    public void UnhighlightAll()
    {
        isPaused = false;
        StopAllCoroutines();
        foreach (Class c in ClassDiagram.Instance.GetClassList())
        {
            HighlightClass(c.Name, false);
            foreach (Method m in c.Methods)
            {
                HighlightMethod(c.Name, m.Name, false);
            }
        }
        foreach (Relation r in ClassDiagram.Instance.GetRelationList())
        {
            HighlightEdge(r.OALName, false);
        }
        AnimationIsRunning = false;
    }

    public void Pause()
    {
        if (isPaused)
        {
            isPaused = false;
        }
        else
        {
            isPaused = true;
        }
    }
    public void NextStep()
    {
        if (AnimationIsRunning == false)
            StartAnimation();
        else
            nextStep = true;
    }
    public void PrevStep()
    {
        nextStep = true;
        prevStep = true;
    }
}
