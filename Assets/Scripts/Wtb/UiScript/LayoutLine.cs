using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tianbo.Wang
{
    [Serializable]
    public class LayoutLine
    {
        public LineMoveAxis lineMoveAxis;
        public float value;
        public Vector2 size;
        public Image lineImage;
        /// <summary>
        /// 当前线段控制的窗口
        /// </summary>
        public Dictionary<WindowsBase, LineMoveDir> controlWindows = new Dictionary<WindowsBase, LineMoveDir>();
        public LayoutLine()
        {

        }
        public LayoutLine(LineMoveAxis _lineMoveAxis, string lineName, Transform parent)
        {
            lineMoveAxis = _lineMoveAxis;
            lineImage = new GameObject(lineName).AddComponent<Image>();
            lineImage.transform.SetParent(parent);
            lineImage.color = WindowsManager.Instance.layoutLineColor;
            if (!WindowsManager.Instance.canMoveWindows)
            {
                return;
            }
            EventTriggerListener.Get(lineImage.gameObject).onBeginDrag = BeginDrag;
            EventTriggerListener.Get(lineImage.gameObject).onEndDrag = EndDrag;
            EventTriggerListener.Get(lineImage.gameObject).onDrag = OnDrag;
            EventTriggerListener.Get(lineImage.gameObject).onEnter = OnMouseEnter;
            EventTriggerListener.Get(lineImage.gameObject).onExit = OnMouseExit;
        }

        private void OnMouseEnter(GameObject go)
        {
            Cursor.SetCursor(lineMoveAxis == LineMoveAxis.Horizential ? WindowsManager.Instance.mouseIconX : WindowsManager.Instance.mouseIconY, new Vector2(60, 60), CursorMode.ForceSoftware);
        }

        private void OnMouseExit(GameObject go)
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
        }

        Vector2 startMousePos;
        private void BeginDrag(GameObject go)
        {
            startMousePos = Input.mousePosition;
        }

        private void OnDrag(GameObject go)
        {
            float offsetX;
            float offsetY;
            offsetX = startMousePos.x - Input.mousePosition.x;
            offsetY = startMousePos.y - Input.mousePosition.y;
            startMousePos = Input.mousePosition;

            if(!CanAddSize(offsetX, offsetY))
            {
                return;
            }

            MoveLineFunc(offsetX, offsetY);

        }

        public bool CanAddSize(float _offsetX, float _offsetY)
        {
            bool canAdd = true;
            foreach (var itemKey in controlWindows.Keys)
            {
                Vector2 tempItemSize = itemKey.selfTrans.GetLocalSize();

                switch (controlWindows[itemKey])
                {
                    case LineMoveDir.Up:
                        tempItemSize -= new Vector2(0, _offsetY);
                        break;
                    case LineMoveDir.Left:
                        tempItemSize += new Vector2(_offsetX, 0);
                        break;
                    case LineMoveDir.Down:
                        tempItemSize += new Vector2(0, _offsetY);
                        break;
                    case LineMoveDir.Right:
                        tempItemSize -= new Vector2(_offsetX, 0);
                        break;
                }
                if (tempItemSize.x < itemKey.minSize.x || tempItemSize.y < itemKey.minSize.y)
                {
                    canAdd = false;
                    break;
                }
            }
            return canAdd;
        }

        /// <summary>
        /// 移动Line的方法
        /// </summary>
        /// <param name="_offsetX"></param>
        /// <param name="_offsetY"></param>
        public void MoveLineFunc(float _offsetX,float _offsetY)
        {

            if (lineMoveAxis == LineMoveAxis.Horizential)
            {
                lineImage.rectTransform.localPosition -= new Vector3(_offsetX, 0, 0);
            }
            else
            {
                lineImage.rectTransform.localPosition -= new Vector3(0, _offsetY, 0);
            }
            foreach (var itemKey in controlWindows.Keys)
            {
                Vector2 tempItemPos = itemKey.selfTrans.GetCenter();
                Vector2 tempItemSize = itemKey.selfTrans.GetSize();

                switch (controlWindows[itemKey])
                {
                    case LineMoveDir.Up:
                        itemKey.selfTrans.localPosition -= new Vector3(0, _offsetY / 2f, 0);
                        itemKey.selfTrans.sizeDelta -= new Vector2(0, _offsetY);
                        break;
                    case LineMoveDir.Left:
                        itemKey.selfTrans.localPosition -= new Vector3(_offsetX / 2f, 0, 0);
                        itemKey.selfTrans.sizeDelta += new Vector2(_offsetX, 0);
                        break;
                    case LineMoveDir.Down:
                        itemKey.selfTrans.localPosition -= new Vector3(0, _offsetY / 2f, 0);
                        itemKey.selfTrans.sizeDelta += new Vector2(0, _offsetY);
                        break;
                    case LineMoveDir.Right:
                        itemKey.selfTrans.localPosition -= new Vector3(_offsetX / 2f, 0, 0);
                        itemKey.selfTrans.sizeDelta -= new Vector2(_offsetX, 0);
                        break;
                }
            }
        }

        private void EndDrag(GameObject go)
        {
            foreach (var itemKey in controlWindows.Keys)
            {
                itemKey.ResetChildSigns();
                WindowsManager.Instance.RefreshWindowsLayoutLine();
            }
        }

        public void SetImageLinePosAndSize(Vector2 pos, Vector2 _size)
        {
            size = _size;
            lineImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _size.x);
            lineImage.rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _size.y);
            lineImage.rectTransform.position = pos;
        }
        public void DestroyLine()
        {
            GameObject.DestroyImmediate(lineImage.gameObject);
        }
    }

}
