using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using Tianbo.Wang;

public class CreateNewWindows
{
    [MenuItem("GameObject/UI/CreateUiWindows")]
    public static void CreateUiWindows()
    {
        Transform parent = Selection.activeTransform;
        GameObject window = new GameObject("Windows");
        RectTransform rectTrans = window.AddComponent<RectTransform>();
        if (parent != null)
        {
            rectTrans.SetParent(parent);
        }
        rectTrans.anchorMin = Vector2.zero;
        rectTrans.anchorMax = Vector2.one;
        rectTrans.pivot = new Vector2(0.5f, 0.5f);
        rectTrans.anchoredPosition = Vector2.zero;
        rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
        rectTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 500);

        GameObject titleGroup = new GameObject("TitleGroup");
        RectTransform titleGroupRect = titleGroup.AddComponent<RectTransform>();
        titleGroupRect.SetParent(rectTrans);
        titleGroupRect.localPosition = Vector2.zero;
        titleGroupRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 0);
        titleGroupRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
        titleGroupRect.anchorMin = new Vector2(0, 1);
        titleGroupRect.anchorMax = new Vector2(1, 1);
        titleGroupRect.pivot = new Vector2(0.5f, 1);
        HorizontalLayoutGroup titleGroupHoLg = titleGroup.AddComponent<HorizontalLayoutGroup>();
        titleGroupHoLg.childForceExpandWidth = titleGroupHoLg.childForceExpandHeight = true;
        titleGroupHoLg.childControlWidth = true;
        titleGroupHoLg.childControlHeight = false;
        titleGroupHoLg.childScaleWidth = false;
        titleGroupHoLg.childScaleHeight = false;
        GameObject titileObj = new GameObject("Title");
        RectTransform titileObjRect = titileObj.AddComponent<RectTransform>();
        titileObjRect.pivot = new Vector2(0.5f, 1);
        Image titleImage = titileObj.AddComponent<Image>();
        titleImage.color = Color.black;
        titileObjRect.SetParent(titleGroupRect);
        titileObjRect.localPosition = Vector2.zero;
        titileObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 500);
        titileObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
        GameObject titleText = new GameObject("Text");
        RectTransform titleTextRect = titleText.AddComponent<RectTransform>();
        titleTextRect.anchorMin = titleTextRect.anchorMax = titleTextRect.pivot = new Vector2(0, 0.5f);
        Text tempText = titleText.AddComponent<Text>();
        tempText.text = "新建窗口";
        tempText.fontSize = 36;
        tempText.alignment = TextAnchor.MiddleLeft;
        titleTextRect.SetParent(titileObjRect);
        titleTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 300);
        titleTextRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 100);
        titleTextRect.anchoredPosition = Vector2.zero;


        window.AddComponent<Image>();
        WindowsBase windowsBase = window.AddComponent<WindowsBase>();
        windowsBase.titleDragObj = titileObj;
        windowsBase.windowsName = "新建窗口";
    }

}
