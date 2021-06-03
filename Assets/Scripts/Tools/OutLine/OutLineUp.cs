using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tianbo.Wang
{
    public class OutLineUp : OutLine
    {
        public override void Init(GameObject go, Vector2 _limitSize)
        {
            base.Init(go, _limitSize);
            enterIcon = Resources.Load<Texture2D>("Images/MouseMoveUpDown");
        }

        public override void RefreshRect(float lineWidth, Color lineColor)
        {
            lineObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, selfRect.GetLocalSize().x);
            lineObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, lineWidth);
            lineObjRect.position = new Vector2(selfRect.GetCenter().x , selfRect.GetCenter().y+ selfRect.GetSize().y / 2f);
            lineObjRect.GetComponent<Image>().color = lineColor;
        }
        protected override void GetStartDragObjPos()
        {
            startDragObjPosY = selfRect.GetCenter().y - selfRect.GetSize().y / 2f;
        }

        protected override bool CheckCanDrag()
        {
            newObjDisY = startDragObjSizeDeltaY + (Input.mousePosition.y - startDragMousePos.y);

            if (limitSize.y > selfRect.GetLocalSize().y + newObjDisY)
            {
                return false;
            }
            return true;
        }

        protected override void DragLine()
        {
            base.DragLine();
            newObjDisY = startDragObjSizeDeltaY + (Input.mousePosition.y - startDragMousePos.y);
            selfRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, startDragObjPosY / (selfRect.parent.GetRectTransform().GetSize().y / 1080f), newObjDisY);
        }
    }
}