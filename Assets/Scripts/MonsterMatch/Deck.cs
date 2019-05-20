using System.Collections.Generic;
using System.Linq;
using HiddenSwitch;

namespace MonsterMatch
{
    public sealed class Deck
    {
        private IList<bool> m_Deck;

        public Deck(int size, int startingPositives)
        {
            m_Deck = Enumerable.Range(0, startingPositives).Select(i => true)
                .Concat(Enumerable.Range(0, size - startingPositives).Select(i => false))
                .Shuffled().ToList();
        }

        public bool Draw()
        {
            if (m_Deck.Count == 0)
            {
                return false;
            }

            var value = m_Deck[m_Deck.Count - 1];
            m_Deck.RemoveAt(m_Deck.Count - 1);
            return value;
        }

        public void RemoveNegative()
        {
            for (var i = 0; i < m_Deck.Count; i++)
            {
                if (m_Deck[i])
                {
                    continue;
                }

                m_Deck.RemoveAt(i);
                return;
            }
        }

        public void ShuffleIn(bool positive)
        {
            m_Deck.Insert(UnityEngine.Random.Range(0, m_Deck.Count), positive);
        }

        public bool this[int index]
        {
            get => m_Deck[index];
            set => m_Deck[index] = value;
        }
    }
}