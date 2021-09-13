using HMUI;
using IPA.Utilities;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace ComboSplitter.Services
{
    /// <summary>
    /// Base class for handling the combo panel overrides and functionality.
    /// Designed to be compatible across multiplayer and standard modes.
    /// </summary>
    public class CustomComboPanelController : MonoBehaviour
    {
        [Inject] public BeatmapObjectManager beatmapObjectManager;
        [Inject] public ComboUIController comboPanel;
        [Inject] public ColorScheme colorScheme;
        [Inject] public CSConfig config;
        [Inject] public PlayerHeadAndObstacleInteraction collision;
        [InjectOptional] public PauseMenuManager pauseManager; // Single-player specific function

        [SerializeField] public CurvedTextMeshPro leftText;
        [SerializeField] public CurvedTextMeshPro rightText;
        [SerializeField] public int leftCombo = 0;
        [SerializeField] public int rightCombo = 0;
        [SerializeField] public bool isSetup = false;
        [SerializeField] public bool isMultiplayer = false;

        public void Start()
        {
            transform.SetParent(comboPanel.transform);
            SetupComboPanel();
        }

        public void SetupComboPanel()
        {
            GameObject leftTextGo = new GameObject("LeftHandText");
            GameObject rightTextGo = new GameObject("RightHandText");
            var relativeTransform = comboPanel.transform.Find("ComboCanvas/NumText");
            relativeTransform.GetComponent<CurvedTextMeshPro>().enabled = false;
            relativeTransform.name = "Custom";

            leftText = leftTextGo.AddComponent<CurvedTextMeshPro>();
            leftText.fontStyle = FontStyles.Italic;
            leftText.alignment = TextAlignmentOptions.Right;
            leftText.SetField<TMP_Text, float>("m_fontScale", 0.4494382f);
            leftText.text = leftCombo.ToString();

            rightText = rightTextGo.AddComponent<CurvedTextMeshPro>();
            rightText.fontStyle = FontStyles.Italic;
            rightText.alignment = TextAlignmentOptions.Left;
            rightText.SetField<TMP_Text, float>("m_fontScale", 0.4494382f);
            rightText.text = rightCombo.ToString();

            GameObject div = new GameObject("=== DIVIDER ===");
            var divText = div.AddComponent<CurvedTextMeshPro>();
            divText.fontStyle = FontStyles.Italic;
            divText.alignment = TextAlignmentOptions.Center;
            divText.text = "/";
            div.layer = 5;

            leftTextGo.transform.SetParent(relativeTransform, false);
            leftTextGo.transform.localPosition = new Vector3(-120, -20, 0);
            rightTextGo.transform.SetParent(relativeTransform, false);
            rightTextGo.transform.localPosition = new Vector3(120, -20, 0);
            div.transform.SetParent(relativeTransform, false);

            if (config.UseSaberColorScheme)
            {
                leftText.color = colorScheme.saberAColor;
                rightText.color = colorScheme.saberBColor;
            }

            isMultiplayer = SceneManager.GetSceneByName("MultiplayerGameplay").isLoaded;
            AddEvents();
        }

        public void AddEvents()
        {
            beatmapObjectManager.noteWasCutEvent += HandleNoteCut;
            beatmapObjectManager.noteWasMissedEvent += HandleNoteMissed;
            isSetup = true;
        }

        public void LateUpdate()
        {
            if (!isSetup) return;

            if (collision.intersectingObstacles.Count > 0)
            {
                if (isMultiplayer) { leftCombo = 0; rightCombo = 0; }
                else if (!pauseManager.enabled) { leftCombo = 0; rightCombo = 0; }
                else return;
            }

            UpdateTexts();
        }

        public void HandleNoteCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (noteController.noteData.colorType is ColorType.ColorA) leftCombo++;
            else if (noteController.noteData.colorType is ColorType.ColorB) rightCombo++;

            if (noteController.noteData.colorType is ColorType.None || !noteCutInfo.allIsOK)
            {
                leftCombo = 0;
                rightCombo = 0;
            }
        }

        public void HandleNoteMissed(NoteController noteController)
        {
            if (noteController.noteData.colorType is ColorType.ColorA) leftCombo = 0;
            else if (noteController.noteData.colorType is ColorType.ColorB) rightCombo = 0;
        }

        public void UpdateTexts()
        {
            leftText.text = leftCombo.ToString();
            rightText.text = rightCombo.ToString();
        }

        public void OnDestroy()
        {
            beatmapObjectManager.noteWasCutEvent -= HandleNoteCut;
            beatmapObjectManager.noteWasMissedEvent -= HandleNoteMissed;
        }
    }
}
