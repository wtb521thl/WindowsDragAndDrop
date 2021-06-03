using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tianbo.Wang
{
    public class OutLineLeftDown : OutLine
    {
        public override void Init(GameObject go, Vector2 _limitSize)
        {
            base.Init(go, _limitSize);
            enterIcon = Resources.Load<Texture2D>("Images/MouseMoveLeftDownRightUp");
        }

        public override void RefreshRect(float lineWidth, Color lineColor)
        {
            lineObjRect.sizeDelta = new Vector2(lineWidth * 2, lineWidth * 2);
            lineObjRect.position = selfRect.GetCenter() - new Vector3(selfRect.GetSize().x / 2f, selfRect.GetSize().y / 2f, 0);
            lineObjRect.GetComponent<Image>().color = lineColor;
        }

        protected override void GetStartDragObjPos()
        {
            startDragObjPosX = selfRect.parent.GetRectTransform().GetSize().x - (selfRect.GetCenter().x + selfRect.GetSize().x / 2f);

            startDragObjPosY = selfRect.parent.GetRectTransform().GetSize().y - (selfRect.GetCenter().y + selfRect.GetSize().y / 2f);
        }


        protected override bool CheckCanDrag()
        {
            newObjDisX = startDragObjSizeDeltaX - (Input.mousePosition.x - startDragMousePos.x);

            newObjDisY = startDragObjSizeDeltaY - (Input.mousePosition.y - startDragMousePos.y);

            if (limitSize.x > selfRect.GetLocalSize().x + newObjDisX)
            {
                return false;
            }

            if (limitSize.y > selfRect.GetLocalSize().y + newObjDisY)
            {
                return false;
            }

            return true;
        }

        protected override void DragLine()
        {
            base.DragLine();
            //newObjDisX = startDragObjSizeDeltaX - (Input.mousePosition.x - startDragMousePos.x);
            selfRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, startDragObjPosX / (selfRect.parent.GetRectTransform().GetSize().x / 1920f), newObjDisX);

            //newObjDisY = startDragObjSizeDeltaY - (Input.mousePosition.y - startDragMousePos.y);
            selfRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, startDragObjPosY / (selfRect.parent.GetRectTransform().GetSize().y / 1080f), newObjDisY);


            if (limitSize.x > selfRect.GetSize().x + newObjDisX)
            {
                selfRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, startDragObjPosX / (selfRect.parent.GetRectTransform().GetSize().x / 1920f), limitSize.x);
            }

            if (limitSize.y > selfRect.GetSize().y + newObjDisY)
            {
                selfRect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, startDragObjPosY / (selfRect.parent.GetRectTransform().GetSize().y / 1080f), limitSize.y);
            }
        }
    }
}