using UnityEngine;

namespace Tianbo.Wang
{
    public interface IOutLine
    {
        void Init(GameObject go,Vector2 _limitSize);
        void RefreshRect(float lineWidth, Color lineColor);
        void DestroySelf();
    }
}