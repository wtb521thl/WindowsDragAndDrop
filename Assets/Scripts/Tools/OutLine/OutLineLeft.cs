using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tianbo.Wang
{

    public class OutLineLeft : OutLine
    {
        public override void Init(GameObject go, Vector2 _limitSize)
        {
            base.Init(go, _limitSize);
            enterIcon = Resources.Load<Texture2D>("Images/MouseMoveLeftRight");
        }

        public override void RefreshRect(float lineWidth, Color lineColor)
        {
            lineObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, lineWidth);
            lineObjRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, selfRect.GetLocalSize().y);
            lineObjRect.position = new Vector2(selfRect.GetCenter().x - selfRect.GetSize().x / 2f, selfRect.GetCenter().y);
            lineObjRect.GetComponent<Image>().color = lineColor;
        }

        protected override void GetStartDragObjPos()
        {
            startDragObjPosX = selfRect.parent.GetRectTransform().GetSize().x - (selfRect.GetCenter().x + selfRect.GetSize().x / 2f);
        }

        protected override bool CheckCanDrag()
        {
            newObjDisX = startDragObjSizeDeltaX - (Input.mousePosition.x - startDragMousePos.x);
            if (limitSize.x > selfRect.GetLocalSize().x + newObjDisX)
            {
                return false;
            }
            return true;
        }

        protected override void DragLine()
        {
            base.DragLine();

            selfRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, startDragObjPosX/ (selfRect.parent.GetRectTransform().GetSize().x/1920f), newObjDisX);
        }
    }
}