using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
public class UIManager : MonoBehaviour
{
    
    public Toggle[] togs;
    public RectTransform[] content;

    public Button btnLink;

    private void Start()
    {
        btnLink.onClick.AddListener(() => Application.OpenURL("https://github.com/ZeroUltra/UnityAlphaVideo"));
        for (int i = 0; i < togs.Length; i++)
        {
            int index = i;
            togs[index].onValueChanged.AddListener((ison) => OnTogChange(index, ison));
            if (index == 0) { togs[index].isOn = true; togs[index].image.color = Color.green; }
            else togs[index].isOn = false;
        }
    }

    private void OnTogChange(int index, bool ison)
    {
        if (ison)
        {
            togs[index].image.color = Color.green;
            content[index].DOScaleX(1, 0.4f);
        }
        else
        {

            togs[index].image.color = Color.white;
            content[index].localScale = new Vector3(0, 1, 1);
        }
    }
}