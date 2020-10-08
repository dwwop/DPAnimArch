using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SimpleFileBrowser;
using System;
using System.Text.RegularExpressions;

public class FileLoader : MonoBehaviour
{
    void Start()
    {
        FileBrowser.SetFilters(true, new FileBrowser.Filter("Text Files", ".txt"));
        FileBrowser.SetDefaultFilter(".txt");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Resources", @"Assets\Resources\", null);
    }
    public void OpenBrowser()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }
    public void SaveAnimation(Anim newAnim)
    {
        StartCoroutine(SaveAnimationCoroutine(newAnim));
    }
    IEnumerator ShowLoadDialogCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"
        yield return FileBrowser.WaitForLoadDialog(false, @"Assets\Resources\Animations\","Load Animation","Load");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

        if (FileBrowser.Success)
        {
            // If a file was chosen, read its bytes via FileBrowserHelpers
            // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
            //byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result)
            string code = FileBrowserHelpers.ReadTextFromFile(FileBrowser.Result);
            Anim loadedAnim = new Anim(FileBrowserHelpers.GetFilename(FileBrowser.Result).Replace(".txt", ""), code);
            //loadedAnim.Code = GetCleanCode(loadedAnim.Code);
            AnimationData.Instance.AddAnim(loadedAnim);
            AnimationData.Instance.selectedAnim = loadedAnim;
            MenuManager.Instance.UpdateAnimations();
            MenuManager.Instance.SetSelectedAnimation(loadedAnim.AnimationName);
        }
    }
    IEnumerator SaveAnimationCoroutine(Anim newAnim)
    {
        yield return FileBrowser.WaitForSaveDialog(false, @"Assets\Resources\Animations\", "Save Animation", "Save");
        if (FileBrowser.Success)
        {
            string path = FileBrowser.Result;
            string fileName = FileBrowserHelpers.GetFilename(FileBrowser.Result);
            newAnim.AnimationName = fileName.Replace(".txt", "");
            //FileBrowserHelpers.CreateFileInDirectory(@"Assets\Resources\Animations\",fileName);
            HandleTextFile.WriteString(path, newAnim.Code/*GetCleanCode(newAnim.Code)*/);
            AnimationData.Instance.AddAnim(newAnim);
            AnimationData.Instance.selectedAnim = newAnim;
            MenuManager.Instance.UpdateAnimations();
            MenuManager.Instance.SetSelectedAnimation(newAnim.AnimationName);
        }

    }
   /* private string[] GetCleanCode(string[] uncleanCode)
    {
        List<String> code = new List<string>(uncleanCode);
        List<String> cleanCode = new List<String>();
        for (int i = 0; i < code.Count; i++)
        {
            if (code[i].Length > 1)
            {
                cleanCode.Add(Regex.Replace(code[i], " ", ""));
            }
        }
        return cleanCode.ToArray();
    }*/

}
