using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Profiling;
using UnityEngine.UI;

namespace MonsterMatch.UI
{
    public sealed class TextKeyboardConnector : UIBehaviour
    {
        [SerializeField] private Text m_InputField;
        [SerializeField] private UIKeyboard m_Keyboard;
        [SerializeField] private string m_DeleteKey = "DELETE";
        [SerializeField] private string m_ReturnKey = "RETURN";
        [SerializeField] private Button m_Button;
        [SerializeField] private Button m_OptionalDoneButton;
        [SerializeField] private StringEvent m_TextUpdateReceivers;
        [SerializeField] private StringEvent m_TextDoneReceivers;
        public bool firstClick { get; set; } = true;

        public StringEvent textUpdateReceivers => m_TextUpdateReceivers;

        public StringEvent textDoneReceivers => m_TextDoneReceivers;

        protected override void Start()
        {
            Observable.Merge(m_Button
                        .OnPointerClickAsObservable().AsUnitObservable(),
                    m_Keyboard.OnKeyboardInput.AsObservable().AsUnitObservable())
                .Where(ignored => isActiveAndEnabled)
                .Subscribe(v1 =>
                {
                    Profiler.BeginSample("TextKeyboardConnector.ButtonSubscription");
                    SetKeyboardActive(true);
                    if (firstClick)
                    {
                        firstClick = false;
                        m_InputField.text = "";
                    }

                    // If this is the first letter, default to uppercase
                    if (m_InputField.text.Length == 0)
                    {
                        var qwertyKeyboard = m_Keyboard.Subkeyboards[0];
                        qwertyKeyboard.CycleSubkeyboard();
                        qwertyKeyboard.OnKeyboardInput
                            .AsObservable()
                            .Take(1)
                            .Subscribe(v2 =>
                            {
                                if (qwertyKeyboard.cycleKeyState == UIKeyboard.CycleKeyState.SHIFTED)
                                {
                                    qwertyKeyboard.CycleSubkeyboard();
                                }
                            })
                            .AddTo(this);
                    }

                    Profiler.EndSample();
                })
                .AddTo(this);

            m_Keyboard.OnKeyboardInput
                .AsObservable()
                .Where(ignored => isActiveAndEnabled)
                .Select(item => item.Item2)
                .Subscribe(input =>
                {
                    var done = false;
                    if (input == m_DeleteKey)
                    {
                        if (m_InputField.text.Length > 0)
                        {
                            m_InputField.text = m_InputField.text.Substring(0, m_InputField.text.Length - 1);
                        }
                    }
                    else if (input == m_ReturnKey)
                    {
                        SetKeyboardActive(false);
                        done = true;
                    }
                    else
                    {
                        m_InputField.text += input;
                    }

                    m_TextUpdateReceivers?.Invoke(m_InputField.text);
                    if (done)
                    {
                        m_TextDoneReceivers?.Invoke(m_InputField.text);
                    }
                })
                .AddTo(this);

            // Another send button is supported on e.g. the chat view
            if (m_OptionalDoneButton != null)
            {
                m_OptionalDoneButton
                    .OnPointerClickAsObservable()
                    .Where(ignored => isActiveAndEnabled)
                    .Subscribe(ignored =>
                    {
                        var input = m_InputField.text;
                        SetKeyboardActive(false);
                        m_TextDoneReceivers?.Invoke(input);
                    })
                    .AddTo(this);    
            }
           
        }

        public void SetKeyboardActive(bool isActive)
        {
            Profiler.BeginSample("TextKeyboardConnector.SetKeyboardActive");
            m_Keyboard.gameObject.SetActive(isActive);
            Profiler.EndSample();
        }

        /// <summary>
        /// Always hide the keyboard if this is disabled
        /// </summary>
        protected override void OnDisable()
        {
            SetKeyboardActive(false);
        }
    }
}