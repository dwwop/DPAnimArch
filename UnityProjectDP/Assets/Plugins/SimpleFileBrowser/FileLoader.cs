using System.Collections;
using UnityEngine;
using SimpleFileBrowser;
using System.IO;
using AnimArch.Parsing;
using AnimArch.Visualization.Animating;
using AnimArch.Visualization.UI;
using AnimArch.Visualization.Diagrams;

public class FileLoader : MonoBehaviour
{
    private void Start()
    {
        var filters = new FileBrowser.Filter[2];
        filters[0] = new FileBrowser.Filter("JSON files", ".json");
        filters[1] = new FileBrowser.Filter("XML files", ".xml");
        FileBrowser.SetFilters(true, filters);
        FileBrowser.SetDefaultFilter(".json");
        FileBrowser.SetExcludedExtensions(".lnk", ".tmp", ".zip", ".rar", ".exe");
        FileBrowser.AddQuickLink("Resources", @"Assets\Resources\", null);
    }

    public void SaveAnimation(Anim newAnim)
    {
        StartCoroutine(SaveAnimationCoroutine(newAnim));
    }

    public void SaveDiagram()
    {
        StartCoroutine(SaveDiagramCoroutine());
    }

    private static IEnumerator LoadAnimationCoroutine()
    {
        // Show a load file dialog and wait for a response from user
        // Load file/folder: file, Initial path: default (Documents), Title: "Load File", submit button text: "Load"

        FileBrowser.SetDefaultFilter(".json");
        yield return FileBrowser.WaitForLoadDialog(false, @"Assets\Resources\Animations\", "Load Animation", "Load");

        // Dialog is closed
        // Print whether a file is chosen (FileBrowser.Success)
        // and the path to the selected file (FileBrowser.Result) (null, if FileBrowser.Success is false)
        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

        if (!FileBrowser.Success) yield break;
        // If a file was chosen, read its bytes via FileBrowserHelpers
        // Contrary to File.ReadAllBytes, this function works on Android 10+, as well
        //byte[] bytes = FileBrowserHelpers.ReadBytesFromFile(FileBrowser.Result)
        //string code = FileBrowserHelpers.ReadTextFromFile(FileBrowser.Result);
        Anim loadedAnim = new Anim(FileBrowserHelpers.GetFilename(FileBrowser.Result).Replace(".json", ""), "");
        loadedAnim.LoadCode(FileBrowser.Result);
        //loadedAnim.Code = GetCleanCode(loadedAnim.Code);
        AnimationData.Instance.AddAnim(loadedAnim);
        AnimationData.Instance.selectedAnim = loadedAnim;
        MenuManager.Instance.UpdateAnimations();
        MenuManager.Instance.SetSelectedAnimation(loadedAnim.AnimationName);
    }


    private static IEnumerator LoadDiagramCoroutine()
    {
        FileBrowser.SetDefaultFilter(".xml");
        yield return FileBrowser.WaitForLoadDialog(false, @"Assets\Resources\", "Load Diagram", "Load");

        if (!FileBrowser.Success) yield break;
        AnimationData.Instance.SetDiagramPath(FileBrowser.Result);
        ClassDiagramBuilder.LoadDiagram();
    }

    private static IEnumerator SaveAnimationCoroutine(Anim newAnim)
    {
        FileBrowser.SetDefaultFilter(".json");
        yield return FileBrowser.WaitForSaveDialog(false, @"Assets\Resources\Animations\", "Save Animation");
        if (!FileBrowser.Success) yield break;
        var path = FileBrowser.Result;
        var fileName = FileBrowserHelpers.GetFilename(FileBrowser.Result);
        newAnim.AnimationName = FileBrowserHelpers.GetFilename(FileBrowser.Result).Replace(".json", "");
        newAnim.SaveCode(path);
        //FileBrowserHelpers.CreateFileInDirectory(@"Assets\Resources\Animations\",fileName);
        //HandleTextFile.WriteString(path, newAnim.Code/*GetCleanCode(newAnim.Code)*/);
        AnimationData.Instance.AddAnim(newAnim);
        AnimationData.Instance.selectedAnim = newAnim;
        MenuManager.Instance.UpdateAnimations();
        MenuManager.Instance.SetSelectedAnimation(newAnim.AnimationName);
    }

    private static IEnumerator SaveDiagramCoroutine()
    {
        FileBrowser.SetDefaultFilter(".json");
        yield return FileBrowser.WaitForSaveDialog(false, @"Assets\Resources\", "Save Diagram");
        if (!FileBrowser.Success) yield break;
        
        var data = Path.GetExtension(FileBrowser.Result) == ".json"
            ? JsonParser.SaveDiagramToJson()
            : XMIParser.ParseDiagramIntoXmi().OuterXml;
        File.WriteAllText(FileBrowser.Result, data);
    }

    public void OpenDiagram()
    {
        StartCoroutine(LoadDiagramCoroutine());
    }

    public void OpenAnimation()
    {
        StartCoroutine(LoadAnimationCoroutine());
    }
}
