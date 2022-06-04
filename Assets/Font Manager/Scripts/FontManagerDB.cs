using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

#if UNITY_EDITOR
using UnityEditor;
#endif // UNITY_EDITOR

[System.Serializable]
public class ManagedTextEntry
{
    public string Path;
    public TMP_FontAsset DefaultFont;
    public float DefaultSize;
}

[CreateAssetMenu(menuName = "Font Manager/Database", fileName = "FontManagerDB")]
public class FontManagerDB : ScriptableObject
{
    [SerializeField] List<ManagedTextEntry> ManagedText = new List<ManagedTextEntry>();

    Dictionary<string, ManagedTextEntry> DynamicText = new Dictionary<string, ManagedTextEntry>();

    float FontProcessing_Default(float defaultSize)
    {
        return defaultSize;
    }

    TMP_FontAsset FontProcessing_Default(TMP_FontAsset defaultfont)
    {
        return defaultfont;
    }

    public void RefreshFonts(Func<float, float> sizeProcessorFn = null, 
                             Func<TMP_FontAsset, TMP_FontAsset> fontProcessorFn = null)
    {
        // setup a map of the current text
        Dictionary<string, ManagedTextEntry> managedTextMap = new Dictionary<string, ManagedTextEntry>();
        foreach (var entry in ManagedText)
        {
            managedTextMap[entry.Path] = entry;
        }

        // if not font processor provided use the default
        if (sizeProcessorFn == null)
            sizeProcessorFn = FontProcessing_Default;
        if (fontProcessorFn == null)
            fontProcessorFn = FontProcessing_Default;

        // traverse all textmesh pro text
        var allTMPText = FindObjectsOfType<TMP_Text>(true);
        foreach (var tmpText in allTMPText)
        {
            var path = BuildPathTo(tmpText.gameObject);

            if (managedTextMap.ContainsKey(path))
            {
                tmpText.fontSize = sizeProcessorFn(managedTextMap[path].DefaultSize);
                tmpText.font = fontProcessorFn(managedTextMap[path].DefaultFont);
            }
            else if (DynamicText.ContainsKey(path))
            {
                tmpText.fontSize = sizeProcessorFn(DynamicText[path].DefaultSize);
                tmpText.font = fontProcessorFn(DynamicText[path].DefaultFont);
            }
        }
    }

    public void BindDynamicText(TMP_Text text)
    {
        var path = BuildPathTo(text.gameObject);
        DynamicText[path] = new ManagedTextEntry()
        {
            Path = path,
            DefaultFont = text.font,
            DefaultSize = text.fontSize
        };
    }

    public void ForceUpdate(TMP_Text text, Func<float, float> sizeProcessorFn = null,
                            Func<TMP_FontAsset, TMP_FontAsset> fontProcessorFn = null)
    {
        var path = BuildPathTo(text.gameObject);

        // if not font processor provided use the default
        if (sizeProcessorFn == null)
            sizeProcessorFn = FontProcessing_Default;
        if (fontProcessorFn == null)
            fontProcessorFn = FontProcessing_Default;

        // is this dynamic text?
        if (DynamicText.ContainsKey(path))
        {
            text.fontSize = sizeProcessorFn(DynamicText[path].DefaultSize);
            text.font = fontProcessorFn(DynamicText[path].DefaultFont);
        }
        else
        {
            foreach(var entry in ManagedText)
            {
                if (entry.Path == path)
                {
                    text.fontSize = sizeProcessorFn(entry.DefaultSize);
                    text.font = fontProcessorFn(entry.DefaultFont);
                    return;
                }
            }
        }
    }

    public void UnbindDynamicText(TMP_Text text)
    {
        var path = BuildPathTo(text.gameObject);
        DynamicText.Remove(path);
    }

    string BuildPathTo(GameObject target)
    {
        string path = target.name;
        var parent = target.transform.parent;

        while (parent != null)
        {
            path = $"{parent.gameObject.name}/{path}";
            parent = parent.parent;
        }
        path = $"{target.scene.name}:{path}";

        return path;
    }

#if UNITY_EDITOR
    public void PopulateManagedText(bool updateIfAlreadyExists = false)
    {
        // setup a map of the current text
        Dictionary<string, ManagedTextEntry> managedTextMap = new Dictionary<string, ManagedTextEntry>();
        foreach(var entry in ManagedText)
        {
            managedTextMap[entry.Path] = entry;
        }

        // traverse all textmesh pro text
        var allTMPText = FindObjectsOfType<TMP_Text>(true);
        foreach(var tmpText in allTMPText)
        {
            var path = BuildPathTo(tmpText.gameObject);

            // add in to the map if not present or if updating is forced
            if (!managedTextMap.ContainsKey(path) || updateIfAlreadyExists)
            {
                managedTextMap[path] = new ManagedTextEntry()
                {
                    Path = path,
                    DefaultFont = tmpText.font,
                    DefaultSize = tmpText.fontSize
                };
            }
        }

        var allPaths = new List<string>(managedTextMap.Keys);
        allPaths.Sort();

        // store a sorted version of the text
        ManagedText = new List<ManagedTextEntry>(managedTextMap.Count);
        foreach(var path in allPaths)
        {
            ManagedText.Add(managedTextMap[path]);
        }

        Undo.RegisterCompleteObjectUndo(this, "Populate managed text");
        EditorUtility.SetDirty(this);
        AssetDatabase.SaveAssetIfDirty(this);
    }
#endif // UNITY_EDITOR
}
