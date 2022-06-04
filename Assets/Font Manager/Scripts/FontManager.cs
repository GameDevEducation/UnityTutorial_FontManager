using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FontManager : MonoBehaviour
{
    [SerializeField] FontManagerDB FontManagerDB;

    public static FontManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError($"Found additional FontManager on {gameObject.name}");
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void BindDynamicText(TMP_Text text)
    {
        Instance.FontManagerDB.BindDynamicText(text);
    }

    public static void UnbindDynamicText(TMP_Text text)
    {
        Instance.FontManagerDB.UnbindDynamicText(text);
    }

    public void SetFonts_Default()
    {
        FontManagerDB.RefreshFonts();
    }

    public void SetFonts_Larger()
    {
        FontManagerDB.RefreshFonts(FontProcessor_Larger);
    }

    float FontProcessor_Larger(float defaultSize)
    {
        return defaultSize * 1.25f;
    }
}
