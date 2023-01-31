using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using OALProgramControl;
using TMPro;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;
using Assets.Scripts.AnimationControl.OAL;
using AnimArch.Visualization.Diagrams;
using UMSAGL.Scripts;
using Object = System.Object;

namespace AnimArch.Visualization.Animating
{
    //Controls the entire animation process
    public class Animation : Singleton<Animation>
    {
        private ClassDiagram classDiagram;
        private ObjectDiagram objectDiagram;
        public Color classColor;
        public Color methodColor;
        public Color relationColor;
        public GameObject LineFill;
        private int BarrierSize;
        private int CurrentBarrierFill;
        [HideInInspector] public bool AnimationIsRunning = false;
        [HideInInspector] public bool isPaused = false;
        [HideInInspector] public bool standardPlayMode = true;
        public bool nextStep = false;
        private bool prevStep = false;
        private List<GameObject> Fillers;

        public string startClassName;
        public string startMethodName;

        private void Awake()
        {
            classDiagram = GameObject.Find("ClassDiagram").GetComponent<ClassDiagram>();
            objectDiagram = GameObject.Find("ObjectDiagram").GetComponent<ObjectDiagram>();
            standardPlayMode = true;
        }

        // Main Couroutine for compiling the OAL of Animation script and then starting the visualisation of Animation
        public IEnumerator Animate()
        {
            Fillers = new List<GameObject>();

            if (AnimationIsRunning)
            {
                yield break;
            }
            AnimationIsRunning = true;

            List<Anim> animations = AnimationData.Instance.getAnimList();
            Anim selectedAnimation = AnimationData.Instance.selectedAnim;
            if (animations != null)
            {
                if (animations.Count > 0 && selectedAnimation.AnimationName.Equals(""))
                    selectedAnimation = animations[0];
            }

            OALProgram Program = OALProgram.Instance;
            List<AnimClass> MethodsCodes = selectedAnimation.GetMethodsCodesList(); //Filip
            string Code = selectedAnimation.Code; //toto potom mozno pojde prec
            Debug.Log("Code: ");
            Debug.Log(Code);

            foreach (AnimClass classItem in MethodsCodes) //Filip
            {
                CDClass Class = Program.ExecutionSpace.getClassByName(classItem.Name);

                foreach (AnimMethod methodItem in classItem.Methods)
                {
                    CDMethod Method = Class.getMethodByName(methodItem.Name);

                    //ak je methodItem.Code nie je prazdny retazec tak parsuj
                    //if (!string.IsNullOrWhiteSpace(methodItem.Code))        //toto asi uz nebude potrebne
                    //{
                    EXEScopeMethod MethodBody = OALParserBridge.Parse(methodItem.Code);
                    Method.ExecutableCode = MethodBody;
                    //}
                    /*else {////
                        Method.ExecutableCode = null;
                    }///*/
                }
            }

            CDClass startClass = Program.ExecutionSpace.getClassByName(startClassName);
            if (startClass == null)
            {
                Debug.LogError(string.Format("Error, Class \"{0}\" not found", startClassName ?? "NULL"));
            }

            CDMethod startMethod = startClass.getMethodByName(startMethodName);
            if (startMethod == null)
            {
                Debug.LogError(string.Format("Error, Method \"{0}\" not found", startMethodName ?? "NULL"));
            }

            //najdeme startMethod z daneho class stringu a method stringu, ak startMethod.ExecutableCode je null tak return null alebo yield break
            EXEScopeMethod MethodExecutableCode = Program.ExecutionSpace.getClassByName(startClassName)
                .getMethodByName(startMethodName).ExecutableCode;
            if (MethodExecutableCode == null)
            {
                Debug.Log("Warning, EXEScopeMethod of selected Method is null");
                yield break;
            }

            OALProgram.Instance.SuperScope = MethodExecutableCode; //StartMethod.ExecutableCode
            //OALProgram.Instance.SuperScope = OALParserBridge.Parse(Code); //Method.ExecutableCode dame namiesto OALParserBridge.Parse(Code) pre metodu ktora bude zacinat

            Debug.Log("Abt to execute program");
            int i = 0;

            bool Success = true;
            while (Success && Program.CommandStack.HasNext())
            {
                EXECommand CurrentCommand = Program.CommandStack.Next();
                bool ExecutionSuccess = CurrentCommand.PerformExecution(Program);

                Debug.Log("Command " + i++ + ". Success: " + ExecutionSuccess +
                          ". Command type: " + CurrentCommand.GetType().Name);

                if (CurrentCommand.GetType().Equals(typeof(EXECommandCall)))
                {
                    BarrierSize = 1;
                    CurrentBarrierFill = 0;

                    var oalCall = ((EXECommandCall) CurrentCommand).CreateOALCall();
                    StartCoroutine(ResolveCallFunct(oalCall));

                    var referencingVariableName = ((EXECommandCall) CurrentCommand).InstanceName;
                    long instanceId = CurrentCommand.GetSuperScope()
                        .FindReferencingVariableByName(referencingVariableName).ReferencedInstanceId;

                    objectDiagram.AddRelation(((EXECommandCall) CurrentCommand).CallerMethodInfo.ClassName,
                        instanceId, ((EXECommandCall) CurrentCommand).CalledClass, "ASSOCIATION");

                    yield return StartCoroutine(BarrierFillCheck());
                }
                else if (CurrentCommand.GetType() == typeof(EXECommandQueryCreate))
                {
                    BarrierSize = 1;
                    CurrentBarrierFill = 0;
                    StartCoroutine(ResolveCreateObject(CurrentCommand));
                    yield return StartCoroutine(BarrierFillCheck());
                }
                else if (CurrentCommand.GetType() == typeof(EXECommandAssignment))
                {
                    ResolveAssignment(CurrentCommand);
                }

                Success = Success && ExecutionSuccess;
            }

            /*
            if (Success)
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
            */
            Debug.Log("Over");
            AnimationIsRunning = false;
        }

        private void ResolveAssignment(EXECommand CurrentCommand)
        {
            EXECommandAssignment assignment = (EXECommandAssignment) CurrentCommand;

            if (assignment.AttributeName != null)
            {
                EXEReferencingVariable variable =
                    assignment.GetSuperScope().FindReferencingVariableByName(assignment.VariableName);
                objectDiagram.AddAttributeValue(variable.ReferencedInstanceId,
                    assignment.AttributeName, assignment.AssignedExpression.ToCode());
            }
        }

        private IEnumerator ResolveCreateObject(EXECommand CurrentCommand)
        {
            string referencingVariableName = ((EXECommandQueryCreate) CurrentCommand).ReferencingVariableName;
            string className = ((EXECommandQueryCreate) CurrentCommand).ClassName;
            long instanceId = CurrentCommand.GetSuperScope()
                .FindReferencingVariableByName(referencingVariableName).ReferencedInstanceId;
            
            CDClass variableClass = OALProgram.Instance.ExecutionSpace.getClassByName(className);
            
            CDClassInstance classInstance = variableClass.GetInstanceByID(instanceId); 
            ObjectInDiagram objectInDiagram = objectDiagram.AddObject(className, referencingVariableName, classInstance);

            DiagramPool.Instance.ObjectDiagram.AddObject(objectInDiagram);
            
            InterGraphRelation relation = null;
            foreach (var interGraphRelation in DiagramPool.Instance.RelationsClassToObject)
            {
                if (interGraphRelation.Class.ClassInfo.Name.Equals(className) && interGraphRelation.Object.Instance.UniqueID == instanceId)
                {
                    relation = interGraphRelation;
                }
            }
            
            int step = 0;
            float speedPerAnim = AnimationData.Instance.AnimSpeed;
            float timeModifier = 1f;
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
                        case 0:
                            HighlightClass(className, true);
                            break;
                        case 1:
                            // yield return StartCoroutine(AnimateFill(Call));
                            timeModifier = 0f;
                            break;
                        case 3:
                            relation.Show();
                            relation.Highlight();
                            timeModifier = 1f;
                            break;
                        case 2:
                            objectDiagram.ShowObject(objectInDiagram);
                            timeModifier = 0.5f;
                            break;
                        case 6:
                            HighlightClass(className, false);
                            relation.UnHighlight();
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
                        if (step == 1) step = 2;
                        nextStep = false;
                        prevStep = false;
                        yield return new WaitUntil(() => nextStep);
                        if (prevStep)
                        {
                            if (step > 0) step--;
                            step = UnhighlightObjectCreationStepAnimation(step, className, objectInDiagram, relation);
            
                            if (step > -1) step--;
                            step = UnhighlightObjectCreationStepAnimation(step, className, objectInDiagram, relation);
                        }
            
                        yield return new WaitForFixedUpdate();
                        nextStep = false;
                        prevStep = false;
                    }
                }
            }
            
            IncrementBarrier();
        }
        
        private int UnhighlightObjectCreationStepAnimation(int step, string className, ObjectInDiagram od, InterGraphRelation relation)
        {
            if (step == 1) step = 2;
            switch (step)
            {
                case 0:
                    HighlightClass(className, false);
                    break;
                case 2:
                    relation.UnHighlight();
                    break;
                case 3:
                    break;
            }

            return step;
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

        public IEnumerator AnimateObject(long objectId, float animationLength)
        {
            HighlightObject(objectId, true);
            yield return new WaitForSeconds(animationLength);
            HighlightObject(objectId, false);
        }

        //Couroutine that can be used to Highlight method for a given duration of time
        public IEnumerator AnimateMethod(string className, string methodName, float animationLength)
        {
            HighlightMethod(className, methodName, true);
            yield return new WaitForSeconds(animationLength);
            HighlightMethod(className, methodName, false);
        }

        //Couroutine that can be used to Highlight edge for a given duration of time
        public IEnumerator AnimateEdge(string relationshipName, float animationLength, OALCall call)
        {
            HighlightEdge(relationshipName, true, call);
            yield return new WaitForSeconds(animationLength);
            HighlightEdge(relationshipName, false, call);
        }

        public IEnumerator AnimateFill(OALCall Call)
        {
            //Debug.Log("Filip, hrana: " + Call.RelationshipName); //Filip
            GameObject edge = classDiagram.FindEdge(Call.RelationshipName);
            if (edge != null)
            {
                if (edge.CompareTag("Generalization") || edge.CompareTag("Implements") ||
                    edge.CompareTag("Realisation"))
                {
                    HighlightEdge(Call.RelationshipName, true, Call);
                    yield return new WaitForSeconds(AnimationData.Instance.AnimSpeed / 2);
                }
                else
                {
                    yield return FillNewFiller(classDiagram.FindOwnerOfRelation(Call.RelationshipName), Call.CalledClassName, edge);
                }
            }
        }

        private object FillNewFiller(string ownerOfRelation, string calledClassName, GameObject edge)
        {
            GameObject newFiller = Instantiate(LineFill);
            Fillers.Add(newFiller);
            newFiller.transform.position = classDiagram.graph.transform.GetChild(0).transform.position;
            newFiller.transform.SetParent(classDiagram.graph.transform);
            newFiller.transform.localScale = new Vector3(1, 1, 1);
            LineFiller lf = newFiller.GetComponent<LineFiller>();
            bool flip = false;
            if (ownerOfRelation.Equals(calledClassName))
            {
                flip = true;
            }

            return lf.StartCoroutine(lf.AnimateFlow(edge.GetComponent<UILineRenderer>().Points, flip));
        }

        //Method used to Highlight/Unhighlight single class by name, depending on bool value of argument 
        public void HighlightClass(string className, bool isToBeHighlighted, long instanceID = -1)
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
                    //Debug.Log("Filip, classa: " + className); //Filip
                }
                else
                {
                    bh.UnhighlightBackground();
                }
            }
            else
            {
                Debug.Log("Highlighter component not found");
            }

            // if (instanceID == -1)
            // {
            //     HighlightObjects(className, isToBeHighlighted);
            // }
            // else
            // {
            //     HighlightObject(instanceID, isToBeHighlighted);
            // }
        }

        public void HighlightObjects(string className, bool isToBeHighlighted)
        {
            ClassInDiagram classByName = classDiagram.FindClassByName(className);

            if (classByName == null)
            {
                Debug.Log("Node " + className + " not found");
            }

            if (classByName != null)
            {
                foreach (var classInfoInstance in classByName.ClassInfo.Instances)
                {
                    long id = classInfoInstance.UniqueID;
                    HighlightObject(id, isToBeHighlighted);
                }
            }
            else
            {
                Debug.Log("Highlighter component not found");
            }
        }

        //Method used to Highlight/Unhighlight single class by name, depending on bool value of argument 
        public void HighlightObject(long objectUniqueId, bool isToBeHighlighted)
        {
            GameObject node = objectDiagram.FindByID(objectUniqueId).VisualObject;
            BackgroundHighlighter bh = null;
            if (node != null)
            {
                bh = node.GetComponent<BackgroundHighlighter>();
            }
            else
            {
                Debug.Log("Node " + objectUniqueId + " not found");
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
                Debug.Log("Highlighter component not found");
            }
        }

        //Method used to Highlight/Unhighlight single method by name, depending on bool value of argument 
    public void HighlightMethod(string className, string methodName, bool isToBeHighlighted, long instanceId = -1)
        {
            var node = classDiagram.FindNode(className);
            if (node)
            {
                ClassTextHighligter classTextHighligter = node.GetComponent<ClassTextHighligter>();
                if (classTextHighligter)
                {
                    if (isToBeHighlighted)
                    {
                        classTextHighligter.HighlightClassLine(methodName);
                    }
                    else
                    {
                        classTextHighligter.UnhighlightClassLine(methodName);
                    }
                }
                else
                {
                    Debug.Log("TextHighlighter component not found");
                }
            }
            else
            {
                Debug.Log("Node " + className + " not found");
            }
        }

        private void HighlightInstancesMethod(string className, string methodName, bool isToBeHighlighted)
        {
            List<CDClassInstance> instances = classDiagram.FindClassByName(className).ClassInfo.Instances;
            foreach (CDClassInstance cdClassInstance in instances)
            {
                HighlightObjectMethod(methodName, cdClassInstance.UniqueID, isToBeHighlighted);
            }
        }

        private void HighlightObjectMethod(string methodName, long cdClassInstanceId, bool isToBeHighlighted)
        {
            var textHighlighter = objectDiagram.FindByID(cdClassInstanceId).VisualObject
                .GetComponent<ObjectTextHighlighter>();
            if (textHighlighter != null)
            {
                if (isToBeHighlighted)
                {
                    textHighlighter.HighlightObjectLine(methodName);
                }
                else
                {
                    textHighlighter.UnHighlightObjectLine(methodName);
                }
            }
        }

        //Method used to Highlight/Unhighlight single edge by name, depending on bool value of argument 
        public void HighlightEdge(string relationshipName, bool isToBeHighlighted, OALCall Call)
        {
            RelationInDiagram relationInDiagram = classDiagram.FindEdgeInfo(relationshipName);

            GameObject edge = relationInDiagram?.VisualObject;

            var callerClassInDiagram =
                DiagramPool.Instance.ClassDiagram.FindClassByName(relationInDiagram?.RelationInfo.FromClass);
            var calledClassInDiagram =
                DiagramPool.Instance.ClassDiagram.FindClassByName(relationInDiagram?.RelationInfo.ToClass);

            if (edge != null)
            {
                if (isToBeHighlighted)
                {
                    edge.GetComponent<UEdge>().ChangeColor(relationColor);
                    edge.GetComponent<UILineRenderer>().LineThickness = 8;
                    HighlightInstancesRelations(Call, callerClassInDiagram, calledClassInDiagram, true);
                }
                else
                {
                    edge.GetComponent<UEdge>().ChangeColor(Color.white);
                    edge.GetComponent<UILineRenderer>().LineThickness = 5;
                    HighlightInstancesRelations(Call, callerClassInDiagram, calledClassInDiagram, false);
                }
            }
            else
            {
                Debug.Log(relationshipName + " NULL Edge ");
            }
        }

        private void HighlightInstancesRelations(OALCall Call, ClassInDiagram callerClassName, ClassInDiagram calledClassName, bool isToBeHighlighted)
        {
            if (Call == null) // unhighlight all
            {
                foreach (var callerInstance in callerClassName.ClassInfo.Instances)
                {
                    foreach (var calledInstance in calledClassName.ClassInfo.Instances)
                    {
                        HighlightObjectRelation(callerInstance.UniqueID, calledInstance.UniqueID, false);
                    }
                }
            }
            else
            {
                var classInDiagram =
                    DiagramPool.Instance.ClassDiagram.FindClassByName(Call.CallerClassName);
                foreach (var callerInstance in classInDiagram.ClassInfo.Instances)
                {
                    HighlightObjectRelation(callerInstance.UniqueID, Call.CalledInstanceId, isToBeHighlighted);
                }
            }
        }

        private void HighlightObjectRelation(long callerInstanceId, long calledInstanceId, bool isToBeHighlighted)
        {
            if (callerInstanceId == calledInstanceId)
            {
                return;
            }

            var objectRelation =
                DiagramPool.Instance.ObjectDiagram.FindRelation(callerInstanceId, calledInstanceId).GameObject;

            if (isToBeHighlighted)
            {
                objectRelation.GetComponent<UEdge>().ChangeColor(relationColor);
                objectRelation.GetComponent<UILineRenderer>().LineThickness = 8;
            }
            else
            {
                objectRelation.GetComponent<UEdge>().ChangeColor(Color.white);
                objectRelation.GetComponent<UILineRenderer>().LineThickness = 5;
            }
        }

        // Couroutine used to Resolve one OALCall consisting of Caller class, caller method, edge, called class, called method
        // Same coroutine is called for play or step mode
        public IEnumerator ResolveCallFunct(OALCall Call)
        {
            int step = 0;
            float speedPerAnim = AnimationData.Instance.AnimSpeed;
            float timeModifier = 1f;
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
                        case 0:
                            HighlightClass(Call.CallerClassName, true);
                            HighlightObjects(Call.CallerClassName, true);
                            break;
                        case 1:
                            HighlightMethod(Call.CallerClassName, Call.CallerMethodName, true);
                            HighlightInstancesMethod(Call.CallerClassName, Call.CallerMethodName, true);
                            break;
                        case 2:
                            yield return StartCoroutine(AnimateFill(Call));
                            timeModifier = 0f;
                            break;
                        case 3:
                            HighlightEdge(Call.RelationshipName, true, Call);
                            timeModifier = 0.5f;
                            break;
                        case 4:
                            HighlightClass(Call.CalledClassName, true, Call.CalledInstanceId);
                            HighlightObject(Call.CalledInstanceId, true);
                            timeModifier = 1f;
                            break;
                        case 5:
                            HighlightMethod(Call.CalledClassName, Call.CalledMethodName, true);
                            HighlightObjectMethod(Call.CalledMethodName, Call.CalledInstanceId, true);
                            timeModifier = 1.25f;
                            break;
                        case 6:
                            HighlightClass(Call.CallerClassName, false);
                            HighlightObjects(Call.CallerClassName, false);
                            HighlightMethod(Call.CallerClassName, Call.CallerMethodName, false);
                            HighlightInstancesMethod(Call.CallerClassName, Call.CallerMethodName, false);
                            HighlightClass(Call.CalledClassName, false, Call.CalledInstanceId);
                            HighlightObject(Call.CalledInstanceId, false);
                            HighlightMethod(Call.CalledClassName, Call.CalledMethodName, false);
                            HighlightObjectMethod(Call.CalledMethodName, Call.CalledInstanceId, false);
                            HighlightEdge(Call.RelationshipName, false, Call);
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
                        if (prevStep)
                        {
                            if (step > 0) step--;
                            step = UnhighlightAllStepAnimation(Call, step);

                            if (step > -1) step--;
                            step = UnhighlightAllStepAnimation(Call, step);
                        }

                        yield return new WaitForFixedUpdate();
                        nextStep = false;
                        prevStep = false;
                    }
                }
            }

            IncrementBarrier();
        }

        private int UnhighlightAllStepAnimation(OALCall Call, int step)
        {
            if (step == 2) step = 1;
            switch (step)
            {
                case 0:
                    HighlightClass(Call.CallerClassName, false);
                    HighlightObjects(Call.CallerClassName, false);
                    break;
                case 1:
                    HighlightMethod(Call.CallerClassName, Call.CallerMethodName, false);
                    HighlightInstancesMethod(Call.CallerClassName, Call.CallerMethodName, false);
                    break;
                case 3:
                    HighlightEdge(Call.RelationshipName, false, Call);
                    break;
                case 4:
                    HighlightClass(Call.CalledClassName, false, Call.CalledInstanceId);
                    HighlightObject(Call.CalledInstanceId, false);
                    break;
                case 5:
                    HighlightMethod(Call.CalledClassName, Call.CalledMethodName, false);
                    HighlightObjectMethod(Call.CalledMethodName, Call.CalledInstanceId, false);
                    break;
            }

            return step;
        }

        public string GetColorCode(string type)
        {
            if (type == "class")
            {
                return ColorUtility.ToHtmlStringRGB(classColor);
            }

            if (type == "method")
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
            if (DiagramPool.Instance.ClassDiagram.GetClassList() != null)
                foreach (Class c in DiagramPool.Instance.ClassDiagram.GetClassList())
                {
                    HighlightClass(c.Name, false);
                    if (c.Methods != null)
                        foreach (Method m in c.Methods)
                        {
                            HighlightMethod(c.Name, m.Name, false);
                        }
                }

            if (DiagramPool.Instance.ClassDiagram.GetRelationList() != null)
                foreach (Relation r in DiagramPool.Instance.ClassDiagram.GetRelationList())
                {
                    HighlightEdge(r.OALName, false, null);
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
}