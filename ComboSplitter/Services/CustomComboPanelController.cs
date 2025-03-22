using HMUI;
using SiraUtil.Logging;
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
#pragma warning disable CS8618, CS0649
        [InjectOptional] private readonly PauseMenuManager? pauseManager;
        [Inject] private readonly BeatmapObjectManager beatmapObjectManager;
        [Inject] private readonly CoreGameHUDController gameHUDController;
        [Inject] private readonly PlayerHeadAndObstacleInteraction collisionController;
        [Inject] private readonly GameplayCoreSceneSetupData gameplayCoreSceneSetupData;
        [Inject] private readonly GameSongController songController;
        [Inject] private readonly ComboDataProcessor dataProcessor;
        [Inject] private readonly CSConfig config;
        [Inject] private readonly SiraLog logger;
#pragma warning restore CS8618, CS0649

        // Functionality
        private ComboUIController? comboUIController;
        private ColorScheme? colorScheme;
        private CurvedTextMeshPro? leftText;
        private CurvedTextMeshPro? rightText;
        private int leftHandCombo = 0;
        private int rightHandCombo = 0;
        private bool isSetup = false;

        // Stats Tracking
        private PracticeSettings? practiceSettings;
        private float songStartTimeMs = 0f;
        private PlayerSpecificSettings? playerSpecificSettings;
        private string activeSaberType = string.Empty;
        private bool isMapOneHanded = false;
        private int totalLeftNotes = 0;
        private int totalRightNotes = 0;
        private int totalLeftNotesHit = 0;
        private int totalRightNotesHit = 0;
        private int totalLeftHandBadCuts = 0;
        private int totalRightHandBadCuts = 0;
        private int totalLeftHandMisses = 0;
        private int totalRightHandMisses = 0;
        private int totalLeftHandBombCuts = 0;
        private int totalRightHandBombCuts = 0;

        public int LeftCombo => leftHandCombo;
        public int RightCombo => rightHandCombo;
        public bool IsMultiplayer => SceneManager.GetSceneByName("MultiplayerGameplay").isLoaded;

        public void Start()
        {
            if (!config.Enabled) return;

            comboUIController = gameHUDController.GetComponentInChildren<ComboUIController>();
            practiceSettings = gameplayCoreSceneSetupData.practiceSettings;
            playerSpecificSettings = gameplayCoreSceneSetupData.playerSpecificSettings;
            colorScheme = gameplayCoreSceneSetupData.colorScheme;
            this.transform.SetParent(comboUIController?.transform);

            isMapOneHanded = gameplayCoreSceneSetupData.beatmapKey.beatmapCharacteristic.serializedName == "OneSaber";
            activeSaberType = playerSpecificSettings!.leftHanded ? "LeftSaber" : "RightSaber";

            if (practiceSettings != null)
                songStartTimeMs = practiceSettings.startSongTime;

            songController.songDidFinishEvent += LevelDidFinishEvent;
            beatmapObjectManager.noteWasCutEvent += HandleNoteCut;
            beatmapObjectManager.noteWasMissedEvent += HandleNoteMissed;

            GetHandNoteCount(); // Not essential to functionality and can be passed to a background thread
            SetupPanel();
        }

        private async void GetHandNoteCount()
        {
            int[] noteArray = await dataProcessor.GetSpecificHandNoteCountFromBeatmapData(gameplayCoreSceneSetupData.transformedBeatmapData, songStartTimeMs);
            totalLeftNotes = noteArray[0];
            totalRightNotes = noteArray[1];
#if DEBUG
            logger.Info("Total Left Hand Notes: " + totalLeftNotes);
            logger.Info("Total Right Hand Notes: " + totalRightNotes);
#endif
        }

        private void SetupPanel()
        {
            isSetup = false;

            var relativeTransform = comboUIController?.transform.Find("ComboCanvas/NumText");

            if (isMapOneHanded)
            {
                CurvedTextMeshPro tmp = relativeTransform!.GetComponent<CurvedTextMeshPro>();

                if (config.UseSaberColorScheme)
                    tmp!.color = playerSpecificSettings!.leftHanded ? colorScheme!.saberAColor : colorScheme!.saberBColor;

                isSetup = true;
                return;
            }

            GameObject leftTextGo = new GameObject("LeftHandText");
            leftTextGo.layer = 5;
            GameObject rightTextGo = new GameObject("RightHandText");
            rightTextGo.layer = 5;
            relativeTransform!.GetComponent<CurvedTextMeshPro>().enabled = false;
            relativeTransform!.name = "Custom";

            leftText = leftTextGo.AddComponent<CurvedTextMeshPro>();
            leftText.fontStyle = FontStyles.Normal;
            leftText.alignment = TextAlignmentOptions.Right;
            leftText.text = LeftCombo.ToString();

            rightText = rightTextGo.AddComponent<CurvedTextMeshPro>();
            rightText.fontStyle = FontStyles.Normal;
            rightText.alignment = TextAlignmentOptions.Left;
            rightText.text = RightCombo.ToString();

            GameObject div = new GameObject("=== DIVIDER ===");
            var divText = div.AddComponent<CurvedTextMeshPro>();
            divText.fontStyle = FontStyles.Normal;
            divText.alignment = TextAlignmentOptions.Center;
            divText.text = "|";
            div.layer = 5;

            leftTextGo.transform.SetParent(relativeTransform, false);
            leftTextGo.transform.localPosition = new Vector3(-120, -20, 0);
            rightTextGo.transform.SetParent(relativeTransform, false);
            rightTextGo.transform.localPosition = new Vector3(120, -20, 0);
            div.transform.SetParent(relativeTransform, false);
            div.transform.localPosition = new Vector3(0, -15, 0);
            div.transform.localScale = new Vector3(1, 0.6f, 1);

            if (config.UseSaberColorScheme)
            {
                leftText.color = colorScheme!.saberAColor;
                rightText.color = colorScheme!.saberBColor;
            }

            isSetup = true;
        }

        public void Update()
        {
            if (!isSetup || isMapOneHanded) return;

            if (collisionController._intersectingObstacles.Count > 0)
            {
                if (pauseManager != null && pauseManager.enabled) return;

                if (IsMultiplayer)
                {
                    leftHandCombo = 0;
                    rightHandCombo = 0;
                    UpdateTexts();
                    return;
                }

                leftHandCombo = 0;
                rightHandCombo = 0;
            }

            UpdateTexts();
        }

        private void HandleNoteCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            NoteData noteData = noteController.noteData;

            // Good Cuts
            if (noteCutInfo.allIsOK)
            {
                switch (noteData.colorType)
                {
                    case ColorType.ColorA:
                        leftHandCombo++;
                        totalLeftNotesHit++;
                        break;

                    case ColorType.ColorB:
                        rightHandCombo++;
                        totalRightNotesHit++;
                        break;
                }
            }

            // Bombs
            else if (noteData.gameplayType == NoteData.GameplayType.Bomb)
            {
#if DEBUG
                logger.Info("Bomb");
#endif
                switch (noteCutInfo.saberType)
                {
                    case SaberType.SaberA:
                        totalLeftHandBombCuts++;
                        leftHandCombo = 0;
                        break;

                    case SaberType.SaberB:
                        totalRightHandBombCuts++;
                        rightHandCombo = 0;
                        break;
                }
            }

            // Bad Cuts
            else if (!noteCutInfo.allIsOK)
            {
#if DEBUG
                logger.Info("Bad Cut");
#endif
                leftHandCombo = 0;
                rightHandCombo = 0;

                switch (noteCutInfo.saberType)
                {
                    case SaberType.SaberA:
                        totalLeftHandBadCuts++;
                        totalRightHandMisses++;
                        break;

                    case SaberType.SaberB:
                        totalRightHandBadCuts++;
                        totalLeftHandMisses++;
                        break;
                }
            }
        }

        private void HandleNoteMissed(NoteController noteController)
        {
            ColorType colorType = noteController.noteData.colorType;

            if (colorType == ColorType.ColorA)
            { 
                leftHandCombo = 0;
                totalLeftHandMisses++;
            }

            else if (colorType == ColorType.ColorB) 
            {
                rightHandCombo = 0;
                totalRightHandMisses++;
            }
        }

        private void UpdateTexts()
        {
            leftText!.text = LeftCombo.ToString();
            rightText!.text = RightCombo.ToString();
        }

        private void LevelDidFinishEvent()
        {
            PerHandCutData cutData = new PerHandCutData(totalLeftNotesHit, totalRightNotesHit, totalLeftHandBadCuts, totalRightHandBadCuts);
            PerHandMissData missData = new PerHandMissData(totalLeftHandMisses, totalRightHandMisses);
            PerHandBombData bombData = new PerHandBombData(totalLeftHandBombCuts, totalRightHandBombCuts);

            ComboSplitterDataPackage package = new ComboSplitterDataPackage(
                isMapOneHanded,
                activeSaberType,
                totalLeftNotes,
                totalRightNotes,
                cutData,
                missData,
                bombData
                );

            dataProcessor.LevelDidFinishWithDataPackage(package);
        }

        public void OnDestroy()
        {
            beatmapObjectManager.noteWasCutEvent -= HandleNoteCut;
            beatmapObjectManager.noteWasMissedEvent -= HandleNoteMissed;
            songController.songDidFinishEvent -= LevelDidFinishEvent;
        }
    }
}
