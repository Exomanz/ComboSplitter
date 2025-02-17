using HMUI;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        [InjectOptional] private readonly PauseMenuManager? pauseManager; // Single-player specific function
        [Inject] private readonly BeatmapObjectManager beatmapObjectManager;
        [Inject] private readonly CoreGameHUDController gameHUDController;
        [Inject] private readonly ColorScheme colorScheme;
        [Inject] private readonly CSConfig config;
        [Inject] private readonly PlayerHeadAndObstacleInteraction collisionController;
        [Inject] private readonly GameplayCoreSceneSetupData gameplayCoreSceneSetupData;
        [Inject] private readonly GameplayLevelSceneTransitionEvents gameplayLevelSceneTransitionEvents;
        [Inject] private readonly ComboDataBus dataBus;
#pragma warning restore CS8618, CS0649

        private PlayerSpecificSettings? playerSpecificSettings;
        private ComboUIController? comboUIController;
        private CurvedTextMeshPro? leftText;
        private CurvedTextMeshPro? rightText;
        private bool isMapOneHanded = false;
        private bool isSetup = false;
        private int leftHandCombo = 0;
        private int rightHandCombo = 0;
        private int totalLeftNotesHit = 0;
        private int totalRightNotesHit = 0;
        private int totalLeftHandMisses = 0;
        private int totalRightHandMisses = 0;

        public int LeftCombo => leftHandCombo;
        public int RightCombo => rightHandCombo;
        public bool IsMultiplayer => SceneManager.GetSceneByName("MultiplayerGameplay").isLoaded;

        internal void Start()
        {
            if (!config.Enabled) return;

            comboUIController = gameHUDController.GetComponentInChildren<ComboUIController>();
            playerSpecificSettings = gameplayCoreSceneSetupData.playerSpecificSettings;
            this.transform.SetParent(comboUIController?.transform);

            int cuttableLeftNotes = 0;
            int cuttableRightNotes = 0;
            BeatmapDataItem[] allBeatmapItems = gameplayCoreSceneSetupData.transformedBeatmapData.allBeatmapDataItems.ToArray();
            Parallel.For(0, allBeatmapItems.Length, (idx) =>
            {
                BeatmapDataItem item = allBeatmapItems[idx];
                if (item.type!= BeatmapDataItem.BeatmapDataItemType.BeatmapEvent)
                {
                    if (allBeatmapItems[idx] is NoteData noteData && noteData.gameplayType != NoteData.GameplayType.Bomb)
                    {
                        if (noteData.colorType == ColorType.ColorA)
                            cuttableLeftNotes++;
                        else if (noteData.colorType == ColorType.ColorB) 
                            cuttableRightNotes++;
                    }
                }
            });

            isMapOneHanded = gameplayCoreSceneSetupData.beatmapKey.beatmapCharacteristic.serializedName == "OneSaber";

            gameplayLevelSceneTransitionEvents.anyGameplayLevelDidFinishEvent -= LevelDidFinishEvent;
            gameplayLevelSceneTransitionEvents.anyGameplayLevelDidFinishEvent += LevelDidFinishEvent;

            beatmapObjectManager.noteWasCutEvent -= HandleNoteCut;
            beatmapObjectManager.noteWasCutEvent += HandleNoteCut;

            beatmapObjectManager.noteWasMissedEvent -= HandleNoteMissed;
            beatmapObjectManager.noteWasMissedEvent += HandleNoteMissed;

            SetupPanel();
        }

        internal void SetupPanel()
        {
            isSetup = false;

            var relativeTransform = comboUIController?.transform.Find("ComboCanvas/NumText");

            if (isMapOneHanded)
            {
                CurvedTextMeshPro tmp = relativeTransform!.GetComponent<CurvedTextMeshPro>();

                if (config.UseSaberColorScheme)
                    tmp!.color = playerSpecificSettings!.leftHanded ? colorScheme.saberAColor : colorScheme.saberBColor;

                isSetup = true;
                return;
            }

            GameObject leftTextGo = new GameObject("LeftHandText");
            GameObject rightTextGo = new GameObject("RightHandText");
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

            isSetup = true;
        }

        internal void Update()
        {
            if (!isSetup || isMapOneHanded) return; // Retain standard functionality if map is one-handed

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

            else if (noteData.gameplayType == NoteData.GameplayType.Bomb || !noteCutInfo.allIsOK)
            {
                switch (noteCutInfo.saberType)
                {
                    case SaberType.SaberA:
                        leftHandCombo = 0;
                        totalLeftHandMisses++;
                        break;
                    case SaberType.SaberB:
                        rightHandCombo = 0;
                        totalRightHandMisses++;
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

        internal void UpdateTexts()
        {
            leftText!.text = LeftCombo.ToString();
            rightText!.text = RightCombo.ToString();
        }

        private void LevelDidFinishEvent()
        {
            PerHandCutData cutData = new PerHandCutData(totalLeftNotesHit, totalRightNotesHit, isMapOneHanded, playerSpecificSettings!.leftHanded ? "LeftSaber" : "RightSaber");
            PerHandMissData missData = new PerHandMissData(totalLeftHandMisses, totalRightHandMisses);
            dataBus.SendData(cutData, missData);
        }

        internal void OnDestroy()
        {
            beatmapObjectManager.noteWasCutEvent -= HandleNoteCut;
            beatmapObjectManager.noteWasMissedEvent -= HandleNoteMissed;
            gameplayLevelSceneTransitionEvents.anyGameplayLevelDidFinishEvent -= LevelDidFinishEvent;
        }
    }
}
