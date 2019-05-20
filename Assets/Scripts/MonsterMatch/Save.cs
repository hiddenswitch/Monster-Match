using System;
using UnityEngine;

namespace MonsterMatch
{
    [Serializable]
    public sealed class Save
    {
        [SerializeField] private Drawing m_Drawing;
        [SerializeField] private int m_ProfileTextIndex;
        [SerializeField] private string m_ProfileName;
        [SerializeField] private string m_ProfileGender;

        public Drawing drawing
        {
            get => m_Drawing;
            set => m_Drawing = value;
        }

        public string profileName
        {
            get => m_ProfileName;
            set => m_ProfileName = value;
        }

        public string profileGender
        {
            get => m_ProfileGender;
            set => m_ProfileGender = value;
        }

        public int profileTextIndex
        {
            get => m_ProfileTextIndex;
            set => m_ProfileTextIndex = value;
        }
    }
}