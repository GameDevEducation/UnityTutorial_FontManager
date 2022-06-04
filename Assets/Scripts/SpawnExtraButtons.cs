using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnExtraButtons : MonoBehaviour
{
    [SerializeField] RectTransform ButtonRoot;
    [SerializeField] GameObject ButtonPrefab;
    [SerializeField] int NumToSpawn = 3;

    // Start is called before the first frame update
    void Start()
    {
        for (int index = 0; index < NumToSpawn; ++index)
        {
            var newButton = Instantiate(ButtonPrefab, ButtonRoot);

            FontManager.BindDynamicText(newButton.GetComponentInChildren<TMPro.TMP_Text>());
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
