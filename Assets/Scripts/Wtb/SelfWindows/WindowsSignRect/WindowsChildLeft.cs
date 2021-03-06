using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tianbo.Wang
{
    public class WindowsChildLeft : WindowsChildBase
    {
        public WindowsChildLeft() : base()
        {

        }

        public WindowsChildLeft(RectTransform parent) : base(parent)
        {
            selfChildType = DragToChildType.Left;

        }
        public override void SetPosAndSize()
        {
            base.SetPosAndSize();

            parentCenterPoint = parentRect.GetCenter();
            parentSize = parentRect.GetSize();
            selfRect.sizeDelta = new Vector2(parentSize.x / times, 0);
            selfRect.anchoredPosition = Vector2.zero;
            childCenter = selfRect.GetCenter();
            childSize = selfRect.GetLocalSize();
            if (titleDragObj != null)
            {
                selfRect.sizeDelta = new Vector2(parentSize.x / times, -titleDragObj.transform.GetRectTransform().GetSize().y);
                selfRect.anchoredPosition = Vector2.zero - new Vector2(0, titleDragObj.transform.GetRectTransform().GetSize().y / 2f);
            }


        }

        public override void SetAnchoredAndPivote()
        {
            base.SetAnchoredAndPivote();
            selfRect.anchorMin = new Vector2(0, 0);
            selfRect.anchorMax = new Vector2(0, 1);
            selfRect.pivot = new Vector2(0, 0.5f);
        }

    }
}