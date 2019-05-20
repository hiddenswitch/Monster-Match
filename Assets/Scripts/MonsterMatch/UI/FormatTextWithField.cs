using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class FormatTextWithField : UIBehaviour
    {
        [SerializeField] private Text m_InputField;
        [SerializeField] private FormattedText m_FormattedText;

        protected override void Start()
        {
            Observable.EveryUpdate()
                .Select(ignored => m_InputField.text)
                .DistinctUntilChanged()
                .Skip(1)
                .Subscribe(value => m_FormattedText.data = value)
                .AddTo(this);
        }
    }
}