using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Tianbo.Wang
{
    public class WindowsBase : MonoBehaviour
    {
        /// <summary>
        /// 拖拽的物体
        /// </summary>
        public GameObject titleDragObj;
        /// <summary>
        /// 展示给用户的名字
        /// </summary>
        public string windowsName;
        [HideInInspector]
        public RectTransform selfTrans;

        Vector3 dragTitleStartMousePos;
        Vector3 dragTitleStartPos;

        /// <summary>
        /// 拖拽的临时窗口
        /// </summary>
        RectTransform dragObjRect;



        WindowsBase dragEndWindow;

        DragToChildType dragToChildType;



        public Vector2 minSize = new Vector2(400, 600);

        public List<GameObject> allTitleObjects = new List<GameObject>();

        /// <summary>
        /// 刚开始拖拽的时候判断当前窗口是否与其他窗口合并中（拉到标题栏与其它窗口合并）
        /// </summary>
        public bool curWinInOtherWin = false;

        /// <summary>
        /// 结束拖拽判断当前的窗口整到别的窗口上面的状态
        /// </summary>
        public bool isMergeWindowToOther = false;

        /// <summary>
        /// 是否独立在其他窗口之外
        /// </summary>
        public bool isHaveOutLine = false;

        public float outLineWidth = 2;

        protected virtual void Awake()
        {
            selfTrans = GetComponent<RectTransform>();
            WindowsManager.Instance.RegisterWindows(this);
            InitChildRect();
            if (titleDragObj != null)
            {
                titleDragObj.name = transform.name + "_Title";
                if (!WindowsManager.Instance.canMoveWindows)
                {
                    return;
                }
                titleDragObj.GetComponentInChildren<Text>().text = windowsName;
                allTitleObjects.Clear();
                allTitleObjects.Add(titleDragObj);
                EventTriggerListener.Get(titleDragObj).onBeginDrag += BeginDragTitle;
                EventTriggerListener.Get(titleDragObj).onDrag += DragTitle;
                EventTriggerListener.Get(titleDragObj).onEndDrag += EndDragTitle;
                EventTriggerListener.Get(titleDragObj).onClick += ClickTitle;
            }

        }


        private void ClickTitle(GameObject go)
        {
            for (int i = 0; i < WindowsManager.Instance.allWindowsBase.Count; i++)
            {
                if (WindowsManager.Instance.allWindowsBase[i].titleDragObj != null && WindowsManager.Instance.allWindowsBase[i].titleDragObj.name == go.name)
                {
                    WindowsManager.Instance.allWindowsBase[i].transform.SetAsLastSibling();
                }
            }
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="go"></param>
        private void BeginDragTitle(GameObject go)
        {
            isMergeWindowToOther = false;
            curWinInOtherWin = false;
            for (int i = 0; i < allTitleObjects.Count; i++)
            {
                WindowsBase tempWindows = WindowsManager.Instance.allWindowsBase.Find((p) =>
                {
                    if (p.titleDragObj == null)
                    {
                        return false;
                    }
                    return p.titleDragObj.name == allTitleObjects[i].name;
                });
                if (tempWindows != null && tempWindows != this)
                {
                    curWinInOtherWin = true;
                    break;
                }
            }

            dragTitleStartMousePos = Input.mousePosition;

            dragObjRect = Instantiate(WindowsManager.Instance.dragObjPrefab, WindowsManager.Instance.dragWindowsParnet).GetComponent<RectTransform>();
            dragObjRect.anchorMin = Vector2.zero;
            dragObjRect.anchorMax = Vector2.one;
            dragObjRect.pivot = Vector2.one / 2f;
            dragObjRect.transform.GetComponentInChildren<Text>(true).text = windowsName;
            dragObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, selfTrans.GetSize().x / 2f);
            dragObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, selfTrans.GetSize().y / 2f);
            dragObjRect.position = dragTitleStartMousePos;
            dragObjRect.transform.SetAsLastSibling();
            dragTitleStartPos = dragObjRect.position;
        }
        /// <summary>
        /// 正在拖拽标题部分执行的事件
        /// </summary>
        /// <param name="go"></param>
        private void DragTitle(GameObject go)
        {

            for (int i = 0; i < WindowsManager.Instance.allWindowsBase.Count; i++)
            {
                if (WindowsManager.Instance.allWindowsBase[i].isHaveOutLine)
                {
                    continue;
                }
                if (WindowsManager.Instance.allWindowsBase[i] == this)
                {
                    ChangeDragObjSizeAndPos(i);
                    dragEndWindow = WindowsManager.Instance.allWindowsBase[i];
                    continue;
                }
                if (MouseEnterAndExit.IsMouseEnter(WindowsManager.Instance.allWindowsBase[i].selfTrans))
                {
                    bool isSet = false;
                    for (int j = 0; j < WindowsManager.Instance.allWindowsBase[i].windowsChildBases.Count; j++)
                    {
                        if (WindowsManager.Instance.allWindowsBase[i].windowsChildBases[j].IsMouseEnter())
                        {
                            isSet = true;
                            SetRectPosAndSizeEqual(dragObjRect, WindowsManager.Instance.allWindowsBase[i].windowsChildBases[j]);
                            dragToChildType = WindowsManager.Instance.allWindowsBase[i].windowsChildBases[j].selfChildType;
                            dragEndWindow = WindowsManager.Instance.allWindowsBase[i];
                            break;
                        }
                    }
                    if (!isSet)
                    {
                        ChangeDragObjSizeAndPos(i);
                        dragEndWindow = WindowsManager.Instance.allWindowsBase[i];
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// 最终自身要变成的形状  localSize
        /// </summary>
        Vector2 endDragSizeDelta;
        /// <summary>
        /// 最终自身要去到的位置
        /// </summary>
        Vector2 endDragPos;
        /// <summary>
        /// 当鼠标放到标题上，或者鼠标在空白部分的时候
        /// </summary>
        /// <param name="i"></param>
        private void ChangeDragObjSizeAndPos(int i)
        {
            if (WindowsManager.Instance.allWindowsBase[i].titleDragObj != null && MouseEnterAndExit.IsMouseEnter(WindowsManager.Instance.allWindowsBase[i].titleDragObj.transform as RectTransform))
            {
                RectTransform titleRect = WindowsManager.Instance.allWindowsBase[i].titleDragObj.GetComponent<RectTransform>();
                Vector2 titleSize = titleRect.GetSize();
                dragObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, titleSize.x);
                dragObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, titleSize.y);
                dragObjRect.position = new Vector3((dragTitleStartPos + Input.mousePosition - dragTitleStartMousePos).x, titleRect.GetCenter().y, 0);
                endDragSizeDelta = WindowsManager.Instance.allWindowsBase[i].selfTrans.GetLocalSize();
                endDragPos = WindowsManager.Instance.allWindowsBase[i].selfTrans.position;
                dragToChildType = DragToChildType.Title;
            }
            else
            {
                dragObjRect.position = dragTitleStartPos + Input.mousePosition - dragTitleStartMousePos;
                dragObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Mathf.Max(minSize.x, selfTrans.GetSize().x / 2f));
                dragObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(minSize.y, selfTrans.GetSize().y / 2f));
                endDragSizeDelta = selfTrans.GetLocalSize() - new Vector2(outLineWidth, outLineWidth) * 2;
                endDragPos = dragObjRect.position;
                dragToChildType = DragToChildType.None;
                transform.SetAsLastSibling();
                WindowsManager.Instance.allWindowsBase.Sort((p, q) =>
                {
                    Vector2 pSize = p.selfTrans.GetSize();
                    Vector2 qSize = q.selfTrans.GetSize();
                    if (pSize.x * pSize.y > qSize.x * qSize.y)
                    {
                        return 1;
                    }
                    else if (pSize.x * pSize.y == qSize.x * qSize.y)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                });
            }
        }
        /// <summary>
        /// 鼠标挪到标志位上的时候，将自身的形状与标志位重叠
        /// </summary>
        /// <param name="changeRect"></param>
        /// <param name="childBase"></param>
        void SetRectPosAndSizeEqual(RectTransform changeRect, WindowsChildBase childBase)
        {
            childBase.Refresh();
            changeRect.position = childBase.childCenter;
            changeRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, childBase.childSize.x);
            changeRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, childBase.childSize.y);

            endDragSizeDelta = changeRect.GetLocalSize();
            endDragPos = changeRect.position;
        }
        /// <summary>
        /// 拖拽松手后
        /// </summary>
        /// <param name="go"></param>
        private void EndDragTitle(GameObject go)
        {
            isMergeWindowToOther = dragToChildType == DragToChildType.Title;
            if (!curWinInOtherWin)
            {
                WindowsManager.Instance.FillGap(this);
            }


            if (dragToChildType != DragToChildType.None && dragToChildType != DragToChildType.Title)
            {
                for (int j = 0; j < dragEndWindow.windowsChildBases.Count; j++)
                {
                    if (dragEndWindow.windowsChildBases[j].selfChildType == dragToChildType)
                    {
                        SetRectPosAndSizeEqual(dragObjRect, dragEndWindow.windowsChildBases[j]);  //填补漏洞后刷新子物体标志位置
                        break;
                    }
                }
            }
            else if (dragToChildType == DragToChildType.Title)
            {
                if (dragEndWindow != this)
                {
                    endDragSizeDelta = dragEndWindow.selfTrans.GetLocalSize();
                    endDragPos = dragEndWindow.selfTrans.position;
                }
            }
            //为当前物体赋值
            selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, endDragSizeDelta.x);
            selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, endDragSizeDelta.y);

            selfTrans.position = endDragPos;

            ResetChildSigns();

            WindowsManager.Instance.ReduceRoomByInsertWindow(this, dragEndWindow, dragToChildType, dragObjRect);
            WindowsManager.Instance.RefreshWindowsLayoutLine();
            DestroyImmediate(dragObjRect.gameObject);

            if (dragToChildType == DragToChildType.None)
            {
                //限制不要出界面外
                float curMaxHight = Screen.height - selfTrans.GetSize().y / 2f - 44 - outLineWidth;  //44 是上面的TopArea的高度,再减去outLineWidth，给描边留点位置
                float curMinHight = selfTrans.GetSize().y / 2f;

                float curMaxWidth = Screen.width - selfTrans.GetSize().x / 2f;
                float curMinWidth = selfTrans.GetSize().x / 2f;

                selfTrans.position = new Vector3(Mathf.Clamp(selfTrans.position.x, curMinWidth, curMaxWidth), Mathf.Clamp(selfTrans.position.y, curMinHight, curMaxHight), selfTrans.position.z);
                InstantiateOutlines();
                isHaveOutLine = true;
            }
            else
            {
                DestroyOutLines();
                isHaveOutLine = false;
            }

            if (curWinInOtherWin)
            {
                CurWinDestroyOtherWinTitle();
            }

            if (dragToChildType == DragToChildType.Title)
            {
                AddTitle();
            }

        }


        /// <summary>
        /// 解除合并标题
        /// </summary>
        private void CurWinDestroyOtherWinTitle()
        {
            for (int i = 0; i < allTitleObjects.Count; i++)
            {
                WindowsBase tempWindows = WindowsManager.Instance.allWindowsBase.Find((p) =>
                {
                    if (p.titleDragObj == null)
                    {
                        return false;
                    }
                    return p.titleDragObj.name == allTitleObjects[i].name;
                });
                if (tempWindows != null && tempWindows != this)
                {
                    for (int j = tempWindows.allTitleObjects.Count - 1; j >= 0; j--)
                    {
                        if (tempWindows.allTitleObjects[j].name == titleDragObj.name && tempWindows.allTitleObjects[j].name != tempWindows.titleDragObj.name)
                        {
                            DestroyImmediate(tempWindows.allTitleObjects[j]);
                            tempWindows.allTitleObjects.RemoveAt(j);
                        }
                    }
                }
                if (allTitleObjects[i].name != titleDragObj.name)
                {
                    DestroyImmediate(allTitleObjects[i]);
                }
            }
            allTitleObjects.Clear();
            allTitleObjects.Add(titleDragObj);
        }
        /// <summary>
        /// 拖入标题栏，双方都增加新标题（可拖拽）
        /// </summary>
        private void AddTitle()
        {
            if (dragEndWindow != this)
            {
                for (int i = 0; i < dragEndWindow.allTitleObjects.Count; i++)
                {
                    WindowsBase tempOtherWin = WindowsManager.Instance.allWindowsBase.Find((p) => { return p.name == dragEndWindow.allTitleObjects[i].name.Split('_')[0]; });
                    if (tempOtherWin != null)
                    {
                        if (allTitleObjects.Find((p) => { return p.name == tempOtherWin.titleDragObj.name; }) == null)
                        {
                            GameObject beginWinAddEndTitle = Instantiate(tempOtherWin.titleDragObj, titleDragObj.transform.parent);
                            beginWinAddEndTitle.name = tempOtherWin.transform.name + "_Title";
                            beginWinAddEndTitle.transform.SetAsFirstSibling();
                            EventTriggerListener.Get(beginWinAddEndTitle).onBeginDrag += tempOtherWin.BeginDragTitle;
                            EventTriggerListener.Get(beginWinAddEndTitle).onDrag += tempOtherWin.DragTitle;
                            EventTriggerListener.Get(beginWinAddEndTitle).onEndDrag += tempOtherWin.EndDragTitle;
                            EventTriggerListener.Get(beginWinAddEndTitle).onClick += tempOtherWin.ClickTitle;
                            allTitleObjects.Add(beginWinAddEndTitle);
                        }

                        if (tempOtherWin.allTitleObjects.Find((p) => { return p.name == titleDragObj.name; }) == null)
                        {
                            GameObject endWinAddSelfTitle = Instantiate(titleDragObj, tempOtherWin.titleDragObj.transform.parent);
                            endWinAddSelfTitle.name = transform.name + "_Title";
                            EventTriggerListener.Get(endWinAddSelfTitle).onBeginDrag += BeginDragTitle;
                            EventTriggerListener.Get(endWinAddSelfTitle).onDrag += DragTitle;
                            EventTriggerListener.Get(endWinAddSelfTitle).onEndDrag += EndDragTitle;
                            EventTriggerListener.Get(endWinAddSelfTitle).onClick += ClickTitle;
                            tempOtherWin.allTitleObjects.Add(endWinAddSelfTitle);
                        }

                    }
                }
            }
        }



        /// <summary>
        /// 刷新子物体 自身的标志物体
        /// </summary>
        public void ResetChildSigns()
        {
            for (int i = 0; i < windowsChildBases.Count; i++)
            {
                windowsChildBases[i].Refresh();
            }
        }

        protected virtual void OnDestroy()
        {
            if (titleDragObj != null)
            {
                EventTriggerListener.Get(titleDragObj).onBeginDrag -= BeginDragTitle;
                EventTriggerListener.Get(titleDragObj).onDrag -= DragTitle;
                EventTriggerListener.Get(titleDragObj).onEndDrag -= EndDragTitle;
                EventTriggerListener.Get(titleDragObj).onClick -= ClickTitle;
            }
            WindowsManager.Instance.UnRegisterWindows(this);
        }

        #region 标志物体
        public List<WindowsChildBase> windowsChildBases = new List<WindowsChildBase>();
        WindowsChildBase windowsChildUp;
        WindowsChildBase windowsChildDown;
        WindowsChildBase windowsChildLeft;
        WindowsChildBase windowsChildRight;
        /// <summary>
        /// 生成子物体（用于标志其他窗口停靠的位置）
        /// </summary>
        void InitChildRect()
        {
            windowsChildBases.Clear();
            windowsChildUp = new WindowsChildUp(selfTrans);
            windowsChildBases.Add(windowsChildUp);
            windowsChildDown = new WindowsChildDown(selfTrans);
            windowsChildBases.Add(windowsChildDown);
            windowsChildLeft = new WindowsChildLeft(selfTrans);
            windowsChildBases.Add(windowsChildLeft);
            windowsChildRight = new WindowsChildRight(selfTrans);
            windowsChildBases.Add(windowsChildRight);
        }
        #endregion

        #region outline

        List<OutLine> outLines = new List<OutLine>();
        public void InstantiateOutlines()
        {
            DestroyOutLines();
            outLines.Clear();
            outLines.Add(OutLineManager.Instance.GetOutLine(this, "OutLineLeft"));
            outLines.Add(OutLineManager.Instance.GetOutLine(this, "OutLineRight"));
            outLines.Add(OutLineManager.Instance.GetOutLine(this, "OutLineUp"));
            outLines.Add(OutLineManager.Instance.GetOutLine(this, "OutLineDown"));

            outLines.Add(OutLineManager.Instance.GetOutLine(this, "OutLineLeftUp"));
            outLines.Add(OutLineManager.Instance.GetOutLine(this, "OutLineRightUp"));
            outLines.Add(OutLineManager.Instance.GetOutLine(this, "OutLineLeftDown"));
            outLines.Add(OutLineManager.Instance.GetOutLine(this, "OutLineRightDown"));

            for (int i = 0; i < outLines.Count; i++)
            {
                outLines[i].Init(gameObject, minSize);
                outLines[i].RefreshRect(outLineWidth, WindowsManager.Instance.outLineColor);
                outLines[i].DragLineAction = RefreshLinePosition;
            }
        }

        private void DestroyOutLines()
        {
            for (int i = 0; i < outLines.Count; i++)
            {
                outLines[i].DestroySelf();
            }
        }

        void RefreshLinePosition()
        {
            for (int i = 0; i < outLines.Count; i++)
            {
                if (outLines[i].lineObj == null)
                {
                    break;
                }
                outLines[i].RefreshRect(outLineWidth, WindowsManager.Instance.outLineColor);
            }
            ResetChildSigns();
        }
        #endregion

    }

}