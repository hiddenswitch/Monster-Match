using MonsterMatch.UI;
using UniRx;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MonsterMatch
{
    public sealed class SaveGameController : UIBehaviour
    {
        private const string playerPrefsKey = "save";
        public static SaveGameController instance { get; private set; }
        [SerializeField] private CreateProfileController m_CreateProfileController;
        [SerializeField] private PlayerProfileController m_PlayerProfileController;
        [SerializeField] private ProfileCreatorProfilesTextController m_ProfileCreatorProfilesTextController;
        [SerializeField] private FormattedText m_NameFormattedText;
        [SerializeField] private Text m_NameText;

        [Header("Save Options")] [SerializeField]
        private int m_ThrottleFrames = 120;

        private Subject<Unit> m_RequestSaveSubject = new Subject<Unit>();

        protected override void Awake()
        {
            base.Awake();

            instance = this;
            m_RequestSaveSubject
                .BatchFrame(m_ThrottleFrames, FrameCountType.Update)
                .Subscribe(ignored => { InnerSave(); })
                .AddTo(this);

            Observable.ReturnUnit()
                .DelayFrame(1)
                .Subscribe(ignored => { Load(); })
                .AddTo(this);
        }

        public void Save()
        {
            m_RequestSaveSubject.OnNext(Unit.Default);
        }

        private void InnerSave()
        {
            var save = new Save();
            // Save the character creation
            save.drawing = Drawing.FromDrawingView(m_CreateProfileController.drawingView);
            save.profileTextIndex = m_ProfileCreatorProfilesTextController.profileTextIndex;
            save.profileGender = m_PlayerProfileController.playerGender;
            save.profileName = m_PlayerProfileController.playerName;
            var json = JsonUtility.ToJson(save);
            // TODO: Save the chat state and the judgements
            PlayerPrefs.SetString(playerPrefsKey, json);
        }

        public void Load()
        {
            var json = PlayerPrefs.GetString(playerPrefsKey);
            if (string.IsNullOrEmpty(json))
            {
                return;
            }

            var save = JsonUtility.FromJson<Save>(json);
            save.drawing.PostDrawing(m_CreateProfileController.drawingView);
            m_ProfileCreatorProfilesTextController.profileTextIndex = save.profileTextIndex;
            m_PlayerProfileController.playerGender = save.profileGender;
            m_PlayerProfileController.playerName = save.profileName;
            m_NameText.text = save.profileName;
            m_NameFormattedText.data = save.profileName;
        }

        private void OnApplicationQuit()
        {
            InnerSave();
        }
    }
}