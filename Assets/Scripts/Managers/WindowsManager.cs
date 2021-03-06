using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tianbo.Wang
{
    public class WindowsManager : SingleMono<WindowsManager>
    {

        /// <summary>
        /// 所有的窗口
        /// </summary>
        public List<WindowsBase> allWindowsBase = new List<WindowsBase>();
        /// <summary>
        /// 所有窗口组成的组
        /// </summary>
        public List<LayoutLine> allWindowsLayoutLines = new List<LayoutLine>();

        public Texture2D mouseIconX;
        public Texture2D mouseIconY;

        /// <summary>
        /// 控制窗口的线宽度
        /// </summary>
        public float lineThinkness = 5;
        /// <summary>
        /// 浮点值是否相等的阈值
        /// </summary>
        public float floatOffset = 1f;

        public bool canMoveWindows = false;


        Vector2 selfSizeDelta = new Vector2(1920, 1080);

        public Transform layoutLinesParent;

        public Transform dragWindowsParnet;

        public RectTransform canvasRect;

        public Color layoutLineColor = new Color(27f / 255f, 28f / 255f, 34f / 255f, 1);
        public Color outLineColor = new Color(72f / 255f, 122f / 255f, 233f / 255f, 1);


        string path = "Prefabs/DragWindowsPrefab";

        /// <summary>
        /// 拖拽的物体预设
        /// </summary>
        public GameObject dragObjPrefab;

        private void Awake()
        {
            dragObjPrefab = Resources.Load<GameObject>(path);
        }

        private void Update()
        {

            if (selfSizeDelta != canvasRect.GetLocalSize())
            {
                Debug.Log("刷新");

                InitWindows();

                selfSizeDelta = canvasRect.GetLocalSize();

                for (int i = 0; i < allWindowsLayoutLines.Count; i++)
                {
                    foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                    {
                        itemKey.ResetChildSigns();
                    }
                }
                RefreshWindowsLayoutLine();
            }
        }


        /// <summary>
        /// 初始化一次窗口
        /// </summary>
        private void InitWindows()
        {
            float offsetY = canvasRect.GetLocalSize().y - selfSizeDelta.y;

            for (int i = 0; i < allWindowsBase.Count; i++)
            {
                if (allWindowsBase[i].windowsName == "资源库")
                {
                    allWindowsBase[i].selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, allWindowsBase[i].selfTrans.GetLocalSize().y - offsetY);
                    allWindowsBase[i].selfTrans.localPosition = new Vector2(allWindowsBase[i].selfTrans.localPosition.x, allWindowsBase[i].selfTrans.localPosition.y - offsetY / 2f);
                }
            }
        }


        public void RegisterWindows(WindowsBase windowsBase)
        {
            if (!allWindowsBase.Contains(windowsBase))
            {
                allWindowsBase.Add(windowsBase);
                RefreshWindowsLayoutLine();
            }
            else
            {
                Debug.Log(windowsBase.name + "已经注册过了");
            }
        }


        public void UnRegisterWindows(WindowsBase windowsBase)
        {
            if (allWindowsBase.Contains(windowsBase))
            {
                allWindowsBase.Remove(windowsBase);
            }
            else
            {
                Debug.Log(windowsBase.name + "已经删除过了");
            }
        }


        /// <summary>
        /// 刷新窗口/组的UI布局，刷新拖拽线的位置
        /// </summary>
        public void RefreshWindowsLayoutLine()
        {

            for (int i = 0; i < allWindowsLayoutLines.Count; i++)
            {
                allWindowsLayoutLines[i].DestroyLine();
            }
            allWindowsLayoutLines.Clear();

            for (int i = 0; i < allWindowsBase.Count; i++)
            {
                for (int j = 0; j < allWindowsBase.Count; j++)
                {
                    if (allWindowsBase[i] != allWindowsBase[j])
                    {
                        int indexI = i;
                        int indexJ = j;
                        CheckNeedToLine(allWindowsBase[indexI], allWindowsBase[indexJ]);
                    }
                }
            }
            FinishAddLine();
            index = 0;
            LimitWindowsSize();
        }
        /// <summary>
        /// 所有的线添加到变量中，进行排序，按照控制窗口少的在前（暂定，后续情况可能回去先问题）
        /// </summary>
        private void FinishAddLine()
        {
            allWindowsLayoutLines.Sort((p, q) =>
            {
                if (p.controlWindows.Count > q.controlWindows.Count)
                {
                    return 1;
                }
                else if (p.controlWindows.Count < q.controlWindows.Count)
                {
                    return -1;
                }
                else
                {
                    return 0;
                }
            });

        }

        int index = 0;
        void LimitWindowsSize()
        {
            index += 1;
            for (int i = 0; i < allWindowsBase.Count; i++)
            {
                float offsetX = allWindowsBase[i].selfTrans.GetLocalSize().x - allWindowsBase[i].minSize.x;
                if (offsetX < 0)
                {
                    for (int j = 0; j < allWindowsLayoutLines.Count; j++)
                    {
                        if (allWindowsLayoutLines[j].controlWindows.ContainsKey(allWindowsBase[i]))
                        {
                            if (allWindowsLayoutLines[j].lineMoveAxis == LineMoveAxis.Horizential)
                            {
                                if (allWindowsLayoutLines[j].controlWindows[allWindowsBase[i]] == LineMoveDir.Right)
                                {
                                    allWindowsLayoutLines[j].MoveLineFunc(offsetX, 0);
                                }
                                if (allWindowsLayoutLines[j].controlWindows[allWindowsBase[i]] == LineMoveDir.Left)
                                {
                                    allWindowsLayoutLines[j].MoveLineFunc(-offsetX, 0);
                                }
                            }
                        }
                    }
                }

                float offsetY = allWindowsBase[i].selfTrans.GetLocalSize().y - allWindowsBase[i].minSize.y;
                if (offsetY < 0)
                {
                    for (int j = 0; j < allWindowsLayoutLines.Count; j++)
                    {
                        if (allWindowsLayoutLines[j].controlWindows.ContainsKey(allWindowsBase[i]))
                        {
                            if (allWindowsLayoutLines[j].lineMoveAxis == LineMoveAxis.Vertical)
                            {
                                if (allWindowsLayoutLines[j].controlWindows[allWindowsBase[i]] == LineMoveDir.Up)
                                {
                                    allWindowsLayoutLines[j].MoveLineFunc(0, offsetY);
                                }
                                if (allWindowsLayoutLines[j].controlWindows[allWindowsBase[i]] == LineMoveDir.Down)
                                {
                                    allWindowsLayoutLines[j].MoveLineFunc(0, -offsetY);
                                }
                            }
                        }
                    }
                }

                if (offsetX < 0 || offsetY < 0)
                {
                    if (index < 10)
                    {
                        LimitWindowsSize();
                    }
                }
            }
        }

        /// <summary>
        /// 窗口插入的时候，缩放目标窗口
        /// </summary>
        /// <param name="dragBeginWindows"></param>
        /// <param name="dragEndWindow"></param>
        /// <param name="dragToChildType"></param>
        /// <param name="dragObjRect"></param>
        public void ReduceRoomByInsertWindow(WindowsBase dragBeginWindows, WindowsBase dragEndWindow, DragToChildType dragToChildType, RectTransform dragObjRect)
        {
            Vector2 tempEndWinSize = dragEndWindow.selfTrans.GetLocalSize();
            Vector2 dragObjSize = dragObjRect.GetLocalSize();

            switch (dragToChildType)
            {
                case DragToChildType.None:
                    break;
                case DragToChildType.Title:
                    if (dragBeginWindows.titleDragObj != dragEndWindow.titleDragObj)
                    {

                    }
                    else
                    {
                        UnFillGap(dragBeginWindows);
                    }
                    break;
                case DragToChildType.Left:
                    if (dragEndWindow.allTitleObjects.Count != 0)
                    {
                        for (int j = 0; j < dragEndWindow.allTitleObjects.Count; j++)
                        {
                            WindowsBase tempWinBase = allWindowsBase.Find((p) =>
                            {
                                if (p.titleDragObj == null || p.name == dragBeginWindows.name)
                                {
                                    return false;
                                }
                                return p.titleDragObj.name == dragEndWindow.allTitleObjects[j].name;
                            });
                            if (tempWinBase != null)
                            {
                                tempWinBase.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempEndWinSize.x - dragObjSize.x);
                                tempWinBase.selfTrans.localPosition += new Vector3(dragObjSize.x / 2f, 0, 0);
                            }
                        }
                    }
                    else
                    {
                        dragEndWindow.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempEndWinSize.x - dragObjSize.x);
                        dragEndWindow.selfTrans.localPosition += new Vector3(dragObjSize.x / 2f, 0, 0);
                    }

                    break;
                case DragToChildType.Right:
                    if (dragEndWindow.allTitleObjects.Count != 0)
                    {
                        for (int j = 0; j < dragEndWindow.allTitleObjects.Count; j++)
                        {
                            WindowsBase tempWinBase = allWindowsBase.Find((p) =>
                            {
                                if (p.titleDragObj == null || p.name == dragBeginWindows.name)
                                {
                                    return false;
                                }
                                return p.titleDragObj.name == dragEndWindow.allTitleObjects[j].name;
                            });
                            if (tempWinBase != null)
                            {
                                tempWinBase.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempEndWinSize.x - dragObjSize.x);
                                tempWinBase.selfTrans.localPosition -= new Vector3(dragObjSize.x / 2f, 0, 0);
                            }
                        }
                    }
                    else
                    {
                        dragEndWindow.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempEndWinSize.x - dragObjSize.x);
                        dragEndWindow.selfTrans.localPosition -= new Vector3(dragObjSize.x / 2f, 0, 0);
                    }


                    break;
                case DragToChildType.Up:
                    if (dragEndWindow.allTitleObjects.Count != 0)
                    {
                        for (int j = 0; j < dragEndWindow.allTitleObjects.Count; j++)
                        {
                            WindowsBase tempWinBase = allWindowsBase.Find((p) =>
                            {
                                if (p.titleDragObj == null || p.name == dragBeginWindows.name)
                                {
                                    return false;
                                }
                                return p.titleDragObj.name == dragEndWindow.allTitleObjects[j].name;
                            });
                            if (tempWinBase != null)
                            {
                                tempWinBase.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempEndWinSize.y - dragObjSize.y);
                                tempWinBase.selfTrans.localPosition -= new Vector3(0, dragObjSize.y / 2f, 0);
                            }
                        }
                    }
                    else
                    {
                        dragEndWindow.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempEndWinSize.y - dragObjSize.y);
                        dragEndWindow.selfTrans.localPosition -= new Vector3(0, dragObjSize.y / 2f, 0);
                    }



                    break;
                case DragToChildType.Down:

                    if (dragEndWindow.allTitleObjects.Count != 0)
                    {
                        for (int j = 0; j < dragEndWindow.allTitleObjects.Count; j++)
                        {
                            WindowsBase tempWinBase = allWindowsBase.Find((p) =>
                            {
                                if (p.titleDragObj == null || p.name == dragBeginWindows.name)
                                {
                                    return false;
                                }
                                return p.titleDragObj.name == dragEndWindow.allTitleObjects[j].name;
                            });
                            if (tempWinBase != null)
                            {
                                tempWinBase.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempEndWinSize.y - dragObjSize.y);
                                tempWinBase.selfTrans.localPosition += new Vector3(0, dragObjSize.y / 2f, 0);
                            }
                        }
                    }
                    else
                    {
                        dragEndWindow.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempEndWinSize.y - dragObjSize.y);
                        dragEndWindow.selfTrans.localPosition += new Vector3(0, dragObjSize.y / 2f, 0);
                    }
                    break;
                default:
                    break;
            }
            dragEndWindow.ResetChildSigns();

        }
        /// <summary>
        /// 填补漏洞
        /// </summary>
        public void FillGap(WindowsBase dragBeginWindows)
        {
            Vector2 tempDragBeginSize = dragBeginWindows.selfTrans.GetLocalSize();
            for (int i = 0; i < allWindowsLayoutLines.Count; i++)
            {
                if (allWindowsLayoutLines[i].controlWindows.ContainsKey(dragBeginWindows))
                {
                    switch (allWindowsLayoutLines[i].controlWindows[dragBeginWindows])
                    {
                        case LineMoveDir.Up:
                            foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                            {
                                if (dragBeginWindows == itemKey)
                                {
                                    continue;
                                }
                                Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();
                                itemKey.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempItemSize.y + tempDragBeginSize.y);
                                itemKey.selfTrans.localPosition -= new Vector3(0, tempDragBeginSize.y / 2f, 0);
                                itemKey.ResetChildSigns();
                            }
                            break;
                        case LineMoveDir.Down:
                            foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                            {
                                if (dragBeginWindows == itemKey)
                                {
                                    continue;
                                }
                                Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();
                                itemKey.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempItemSize.y + tempDragBeginSize.y);
                                itemKey.selfTrans.localPosition += new Vector3(0, tempDragBeginSize.y / 2f, 0);
                                itemKey.ResetChildSigns();

                            }
                            break;
                        case LineMoveDir.Left:
                            foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                            {
                                if (dragBeginWindows == itemKey)
                                {
                                    continue;
                                }
                                Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();
                                itemKey.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempItemSize.x + tempDragBeginSize.x);
                                itemKey.selfTrans.localPosition += new Vector3(tempDragBeginSize.x / 2f, 0, 0);
                                itemKey.ResetChildSigns();

                            }
                            break;
                        case LineMoveDir.Right:
                            foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                            {
                                if (dragBeginWindows == itemKey)
                                {
                                    continue;
                                }
                                Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();
                                itemKey.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempItemSize.x + tempDragBeginSize.x);
                                itemKey.selfTrans.localPosition -= new Vector3(tempDragBeginSize.x / 2f, 0, 0);
                                itemKey.ResetChildSigns();
                            }
                            break;
                    }
                    break;
                }
            }
        }
        /// <summary>
        /// 反向填补漏洞
        /// </summary>
        public void UnFillGap(WindowsBase dragBeginWindows)
        {
            Vector2 tempDragBeginSize = dragBeginWindows.selfTrans.GetLocalSize();
            for (int i = 0; i < allWindowsLayoutLines.Count; i++)
            {
                if (allWindowsLayoutLines[i].controlWindows.ContainsKey(dragBeginWindows))
                {
                    switch (allWindowsLayoutLines[i].controlWindows[dragBeginWindows])
                    {
                        case LineMoveDir.Up:
                            foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                            {
                                if (dragBeginWindows == itemKey)
                                {
                                    continue;
                                }
                                Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();
                                itemKey.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempItemSize.y - tempDragBeginSize.y);
                                itemKey.selfTrans.localPosition += new Vector3(0, tempDragBeginSize.y / 2f, 0);
                                itemKey.ResetChildSigns();
                            }
                            break;
                        case LineMoveDir.Down:
                            foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                            {
                                if (dragBeginWindows == itemKey)
                                {
                                    continue;
                                }
                                Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();
                                itemKey.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, tempItemSize.y - tempDragBeginSize.y);
                                itemKey.selfTrans.localPosition -= new Vector3(0, tempDragBeginSize.y / 2f, 0);
                                itemKey.ResetChildSigns();
                            }
                            break;
                        case LineMoveDir.Left:
                            foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                            {
                                if (dragBeginWindows == itemKey)
                                {
                                    continue;
                                }
                                Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();
                                itemKey.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempItemSize.x - tempDragBeginSize.x);
                                itemKey.selfTrans.localPosition -= new Vector3(tempDragBeginSize.x / 2f, 0, 0);
                                itemKey.ResetChildSigns();
                            }
                            break;
                        case LineMoveDir.Right:
                            foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                            {
                                if (dragBeginWindows == itemKey)
                                {
                                    continue;
                                }
                                Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();
                                itemKey.selfTrans.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, tempItemSize.x - tempDragBeginSize.x);
                                itemKey.selfTrans.localPosition += new Vector3(tempDragBeginSize.x / 2f, 0, 0);
                                itemKey.ResetChildSigns();
                            }
                            break;
                    }
                    break;
                }
            }
        }

        public void CheckNeedToLine(WindowsBase win1, WindowsBase win2)
        {

            Vector2 win1Center = win1.selfTrans.GetCenter();
            Vector2 win2Center = win2.selfTrans.GetCenter();
            Vector2 win1Size = win1.selfTrans.GetSize();
            Vector2 win2Size = win2.selfTrans.GetSize();

            float win1Left = win1Center.x - win1Size.x / 2f;
            float win1Right = win1Center.x + win1Size.x / 2f;
            float win1Up = win1Center.y + win1Size.y / 2f;
            float win1Down = win1Center.y - win1Size.y / 2f;

            float win2Left = win2Center.x - win2Size.x / 2f;
            float win2Right = win2Center.x + win2Size.x / 2f;
            float win2Up = win2Center.y + win2Size.y / 2f;
            float win2Down = win2Center.y - win2Size.y / 2f;

            bool win1IsLeftWin2 = win1Center.x - win2Center.x < 0;
            bool win1IsUpWin2 = win1Center.y - win2Center.y > 0;
            if (Mathf.Abs(win1Left - win2Right) <= floatOffset)//两个横坐标相邻
            {
                GetLineInfo(win1, win2, win1Left, win1IsLeftWin2 ? LineMoveDir.Right : LineMoveDir.Left, !win1IsLeftWin2 ? LineMoveDir.Right : LineMoveDir.Left);
            }

            if (Mathf.Abs(win1Right - win2Left) <= floatOffset)
            {
                GetLineInfo(win1, win2, win1Right, win1IsLeftWin2 ? LineMoveDir.Right : LineMoveDir.Left, !win1IsLeftWin2 ? LineMoveDir.Right : LineMoveDir.Left);
            }

            if (Mathf.Abs(win1Up - win2Down) <= floatOffset)
            {
                GetLineInfo(win1, win2, win1Up, !win1IsUpWin2 ? LineMoveDir.Up : LineMoveDir.Down, win1IsUpWin2 ? LineMoveDir.Up : LineMoveDir.Down);
            }

            if (Mathf.Abs(win1Down - win2Up) <= floatOffset)
            {
                GetLineInfo(win1, win2, win1Down, !win1IsUpWin2 ? LineMoveDir.Up : LineMoveDir.Down, win1IsUpWin2 ? LineMoveDir.Up : LineMoveDir.Down);
            }
        }

        public void GetLineInfo(WindowsBase win1, WindowsBase win2, float tempLineValue, LineMoveDir win1LineMoveDir, LineMoveDir win2LineMoveDir)
        {
            bool isFind = false;
            LineMoveAxis lineMoveAxis = (win1LineMoveDir == LineMoveDir.Left || win1LineMoveDir == LineMoveDir.Right) ? LineMoveAxis.Horizential : LineMoveAxis.Vertical;
            for (int i = 0; i < allWindowsLayoutLines.Count; i++)
            {
                if (Mathf.Abs(allWindowsLayoutLines[i].value - tempLineValue) <= floatOffset && allWindowsLayoutLines[i].lineMoveAxis == lineMoveAxis)
                {
                    isFind = true;
                    if (!allWindowsLayoutLines[i].controlWindows.ContainsKey(win1))
                    {
                        allWindowsLayoutLines[i].controlWindows.Add(win1, win1LineMoveDir);
                    }
                    else
                    {
                        allWindowsLayoutLines[i].controlWindows[win1] = win1LineMoveDir;
                    }
                    if (!allWindowsLayoutLines[i].controlWindows.ContainsKey(win2))
                    {
                        allWindowsLayoutLines[i].controlWindows.Add(win2, win2LineMoveDir);
                    }
                    else
                    {
                        allWindowsLayoutLines[i].controlWindows[win2] = win2LineMoveDir;
                    }
                    if (lineMoveAxis == LineMoveAxis.Horizential)
                    {
                        float tempH = 0;
                        List<float> maxPoint = new List<float>();
                        List<float> minPoint = new List<float>();
                        foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                        {
                            if (allWindowsLayoutLines[i].controlWindows[itemKey] == LineMoveDir.Left)//以左面为准  右面也行
                            {
                                Vector2 tempItemSize = itemKey.selfTrans.GetSize();
                                maxPoint.Add(itemKey.selfTrans.GetCenter().y + tempItemSize.y / 2f);
                                minPoint.Add(itemKey.selfTrans.GetCenter().y - tempItemSize.y / 2f);
                                if (itemKey.isMergeWindowToOther)
                                {
                                    continue;
                                }
                                tempH += tempItemSize.y;
                            }
                        }
                        if (tempH == 0)
                        {
                            foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                            {
                                if (allWindowsLayoutLines[i].controlWindows[itemKey] == LineMoveDir.Left)//以左面为准  右面也行
                                {
                                    Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();
                                    tempH += tempItemSize.y;
                                }
                            }
                        }
                        maxPoint.Sort();
                        minPoint.Sort();
                        float tempCenterY = minPoint[0] + (maxPoint[maxPoint.Count - 1] - minPoint[0]) / 2f;
                        allWindowsLayoutLines[i].SetImageLinePosAndSize(new Vector2(allWindowsLayoutLines[i].value, tempCenterY), new Vector2(lineThinkness, tempH));

                    }
                    else
                    {
                        float tempW = 0;
                        List<float> maxPoint = new List<float>();
                        List<float> minPoint = new List<float>();
                        foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                        {
                            if (allWindowsLayoutLines[i].controlWindows[itemKey] == LineMoveDir.Up)
                            {
                                Vector2 tempItemSize = itemKey.selfTrans.GetSize();
                                maxPoint.Add(itemKey.selfTrans.GetCenter().x + tempItemSize.x / 2f);
                                minPoint.Add(itemKey.selfTrans.GetCenter().x - tempItemSize.x / 2f);
                                if (itemKey.isMergeWindowToOther)
                                {
                                    continue;
                                }
                                tempW += tempItemSize.x;
                            }
                        }
                        if (tempW == 0)
                        {
                            foreach (var itemKey in allWindowsLayoutLines[i].controlWindows.Keys)
                            {
                                if (allWindowsLayoutLines[i].controlWindows[itemKey] == LineMoveDir.Up)
                                {
                                    Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();
                                    tempW += tempItemSize.x;
                                }
                            }
                        }
                        float tempCenterX = minPoint[0] + (maxPoint[maxPoint.Count - 1] - minPoint[0]) / 2f;
                        allWindowsLayoutLines[i].SetImageLinePosAndSize(new Vector2(tempCenterX, allWindowsLayoutLines[i].value), new Vector2(tempW, lineThinkness));

                    }

                    break;
                }
            }
            if (!isFind)
            {
                LayoutLine layoutLine = new LayoutLine(lineMoveAxis, lineMoveAxis.ToString() + allWindowsLayoutLines.Count, layoutLinesParent);
                layoutLine.value = tempLineValue;
                layoutLine.controlWindows.Add(win1, win1LineMoveDir);
                layoutLine.controlWindows.Add(win2, win2LineMoveDir);
                allWindowsLayoutLines.Add(layoutLine);
            }
        }
    }
    public enum LineMoveAxis
    {
        Horizential,
        Vertical
    }

    public enum LineMoveDir
    {
        Up,
        Left,
        Down,
        Right
    }
}