using System;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public sealed class StampItem
    {
        [SerializeField] private string m_SpriteName;
        [SerializeField] private Vector2 m_LocalPosition;
        [SerializeField] private Vector2 m_LocalScale;

        public string spriteName
        {
            get => m_SpriteName;
            set => m_SpriteName = value;
        }

        public Vector2 localPosition
        {
            get => m_LocalPosition;
            set => m_LocalPosition = value;
        }

        public Vector2 localScale
        {
            get => m_LocalScale;
            set => m_LocalScale = value;
        }
    }
}