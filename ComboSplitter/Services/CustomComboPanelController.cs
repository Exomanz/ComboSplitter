using HMUI;
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
        [InjectOptional] private readonly PauseMenuManager pauseManager; // Single-player specific function
        [Inject] private readonly BeatmapObjectManager beatmapObjectManager;
        [Inject] private readonly CoreGameHUDController gameHUDController;
        [Inject] private readonly ColorScheme colorScheme;
        [Inject] private readonly CSConfig config;
        [Inject] private readonly PlayerHeadAndObstacleInteraction collisionController;
        [Inject] private readonly GameplayCoreSceneSetupData gameplayCoreSceneSetupData;
        [Inject] private readonly GameplayLevelSceneTransitionEvents gameplayLevelSceneTransitionEvents;
        [Inject] private readonly ComboDataBus dataBus;
#pragma warning restore CS8618, CS0649

        private ComboUIController? comboUIController;
        private CurvedTextMeshPro? leftText;
        private CurvedTextMeshPro? rightText;
        private bool isSetup = false;
        private int totalLeftNotesHit = 0;
        private int totalRightNotesHit = 0;
        private int totalLeftHandMisses = 0;
        private int totalRightHandMisses = 0;

        public int LeftCombo { get; set; } = 0;
        public int RightCombo { get; set; } = 0;
        public bool IsMultiplayer { get; } = SceneManager.GetSceneByName("MultiplayerGameplay").isLoaded;

        internal void Start()
        {
            if (!config.Enabled) return;

            comboUIController = gameHUDController.GetComponentInChildren<ComboUIController>();
            this.transform.SetParent(comboUIController?.transform);

            BeatmapDataItem[] allBeatmapItems = gameplayCoreSceneSetupData.transformedBeatmapData.allBeatmapDataItems.ToArray();
            int cuttableLeftNotes = 0;
            int cuttableRightNotes = 0;
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

            SetupPanel();
            gameplayLevelSceneTransitionEvents.anyGameplayLevelDidFinishEvent -= LevelDidFinishEvent;
            gameplayLevelSceneTransitionEvents.anyGameplayLevelDidFinishEvent += LevelDidFinishEvent;
        }

        internal void SetupPanel()
        {
            isSetup = false;
            GameObject leftTextGo = new GameObject("LeftHandText");
            GameObject rightTextGo = new GameObject("RightHandText");
            var relativeTransform = comboUIController?.transform.Find("ComboCanvas/NumText");
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

            beatmapObjectManager.noteWasCutEvent += HandleNoteCut;
            beatmapObjectManager.noteWasMissedEvent += HandleNoteMissed;
            isSetup = true;
        }

        internal void Update()
        {
            if (!isSetup) return;

            if (collisionController._intersectingObstacles.Count > 0)
            {
                if (pauseManager.enabled) return;

                if (IsMultiplayer)
                {
                    LeftCombo = 0;
                    RightCombo = 0;
                }
                else if (!pauseManager.enabled)
                {
                    LeftCombo = 0;
                    RightCombo = 0;
                }
            }
            UpdateTexts();
        }

        private void HandleNoteCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            NoteData noteData = noteController.noteData;

            if (noteData.colorType == ColorType.ColorA)
            {
                LeftCombo++;
                totalLeftNotesHit++;
            }

            else if (noteData.colorType == ColorType.ColorB)
            {
                RightCombo++;
                totalRightNotesHit++;
            }

            else if (noteData.colorType == ColorType.None)
            {
                if (noteCutInfo.saberType == SaberType.SaberA)
                {
                    LeftCombo = 0;
                    totalLeftHandMisses++;
                }
                else if (noteCutInfo.saberType == SaberType.SaberB)
                {
                    RightCombo = 0;
                    totalRightHandMisses++;
                }
            }

            else if (!noteCutInfo.allIsOK)
            {
                LeftCombo = 0;
                RightCombo = 0;
                totalLeftHandMisses++;
                totalRightHandMisses++;
            }
        }

        private void HandleNoteMissed(NoteController noteController)
        {
            if (noteController.noteData.colorType is ColorType.ColorA)
            { 
                LeftCombo = 0;
                totalLeftHandMisses++;
            }
            else if (noteController.noteData.colorType is ColorType.ColorB) 
            {
                RightCombo = 0;
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
            PerHandCutData cutData = new PerHandCutData(totalLeftNotesHit, totalRightNotesHit);
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
