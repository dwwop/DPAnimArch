using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System;
using System.Text.RegularExpressions;
using AnimArch.Visualization.Animating;
using AnimArch.Visualization.UI;
using AnimArch.Visualization.ClassDiagrams;

public class FileLoader : MonoBehaviour
{
    void Start()
    {
        FileBrowser.Filter[] filters=new FileBrowser.Filter[2];
        filters[0] = new FileBrowser.Filter("JSON files", ".json");
        filters[1] = new FileBrowser.Filter("XML files", ".xml");
        FileBrowser.SetFilters(true, filters);
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Resources", @"Assets\Resources\", null);
    }
    public void OpenBrowser()
    {
        StartCoroutine(ShowLoadDialogCoroutine(@"Assets\Resources\Animations\","Load Animation","Animation"));
    }
    public void SaveAnimation(Anim newAnim)
    {
        StartCoroutine(SaveAnimationCoroutine(newAnim));
    }
    IEnumerator ShowLoadDialogCoroutine(string path, string tooltip,string type)
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        if (type.Equals("Diagram"))
        {
            FileBrowser.SetDefaultFilter(".xml");
        }
        else
        {
            FileBrowser.SetDefaultFilter(".json");
        }
        yield return FileBrowser.WaitForLoadDialog(false, path,tooltip,"Load");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

        if (FileBrowser.Success)
        {
            // If a file was chosen, read its bytes via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            //byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result)
            if (type.Equals("Animation"))
            {
                //string code = FileBrowserHelpers.ReadTextFromFile(FileBrowser.Result);
                Anim loadedAnim = new Anim(FileBrowserHelpers.GetFilename(FileBrowser.Result).Replace(".json", ""),"");
                loadedAnim.LoadCode(FileBrowser.Result);
                //loadedAnim.Code = GetCleanCode(loadedAnim.Code);
                AnimationData.Instance.AddAnim(loadedAnim);
                AnimationData.Instance.selectedAnim = loadedAnim;
                MenuManager.Instance.UpdateAnimations();
                MenuManager.Instance.SetSelectedAnimation(loadedAnim.AnimationName);
            }   
            else if (type.Equals("Diagram"))
            {
                string fileName = FileBrowserHelpers.GetFilename(FileBrowser.Result);
                Debug.Log(FileBrowser.Result);
                Debug.Log(fileName);
                AnimationData.Instance.SetDiagramPath(FileBrowser.Result);
                ClassDiagram.Instance.LoadDiagram();  
            }
        
        }
    }
    IEnumerator SaveAnimationCoroutine(Anim newAnim)
    {
        FileBrowser.SetDefaultFilter(".json");
        yield return FileBrowser.WaitForSaveDialog(false, @"Assets\Resources\Animations\", "Save Animation", "Save");
        if (FileBrowser.Success)
        {
            string path = FileBrowser.Result;
            string fileName = FileBrowserHelpers.GetFilename(FileBrowser.Result);
            newAnim.AnimationName = FileBrowserHelpers.GetFilename(FileBrowser.Result).Replace(".json", "");
            newAnim.SaveCode(path);
            //FileBrowserHelpers.CreateFileInDirectory(@"Assets\Resources\Animations\",fileName);
            //HandleTextFile.WriteString(path, newAnim.Code/*GetCleanCode(newAnim.Code)*/);
            AnimationData.Instance.AddAnim(newAnim);
            AnimationData.Instance.selectedAnim = newAnim;
            MenuManager.Instance.UpdateAnimations();
            MenuManager.Instance.SetSelectedAnimation(newAnim.AnimationName);
        }

    }
    public void OpenDiagram()
    {
        StartCoroutine(ShowLoadDialogCoroutine(@"Assets\Resources\", "Load Diagram", "Diagram"));
    }

}
