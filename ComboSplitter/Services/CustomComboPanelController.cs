using HMUI;
using SiraUtil.Logging;
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
        [InjectOptional] protected PauseMenuManager pauseManager; // Single-player specific function
        [Inject] protected BeatmapObjectManager beatmapObjectManager;
        [Inject] protected ComboUIController comboUIController;
        [Inject] protected ColorScheme colorScheme;
        [Inject] protected CSConfig config;
        [Inject] protected PlayerHeadAndObstacleInteraction collisionController;
        [Inject] protected GameplayCoreSceneSetupData gameplayCoreSceneSetupData;
        [Inject] protected SiraLog log;

        public int LeftCombo { get; set; } = 0;
        public int RightCombo { get; set; } = 0;
        public bool IsMultiplayer { get; } = SceneManager.GetSceneByName("MultiplayerGameplay").isLoaded;

        private CurvedTextMeshPro leftText;
        private CurvedTextMeshPro rightText;
        private bool isSetup = false;

        internal void Start()
        {
            this.transform.SetParent(comboUIController.transform);

            BeatmapDataItem[] allBeatmapItems = gameplayCoreSceneSetupData.transformedBeatmapData.allBeatmapDataItems.ToArray();
            int cuttableLeftNotes = 0;
            int cuttableRightNotes = 0;
            ParallelLoopResult result = Parallel.For(0, allBeatmapItems.Length, (idx) =>
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
        }

        internal void SetupPanel()
        {
            isSetup = false;
            GameObject leftTextGo = new GameObject("LeftHandText");
            GameObject rightTextGo = new GameObject("RightHandText");
            var relativeTransform = comboUIController.transform.Find("ComboCanvas/NumText");
            relativeTransform.GetComponent<CurvedTextMeshPro>().enabled = false;
            relativeTransform.name = "Custom";

            leftText = leftTextGo.AddComponent<CurvedTextMeshPro>();
            leftText.fontStyle = FontStyles.Italic;
            leftText.alignment = TextAlignmentOptions.Right;
            leftText.text = LeftCombo.ToString();

            rightText = rightTextGo.AddComponent<CurvedTextMeshPro>();
            rightText.fontStyle = FontStyles.Italic;
            rightText.alignment = TextAlignmentOptions.Left;
            rightText.text = RightCombo.ToString();

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
                if (IsMultiplayer) { LeftCombo = 0; RightCombo = 0; }
                else if (!pauseManager.enabled) { LeftCombo = 0; RightCombo = 0; }
            }
            UpdateTexts();
        }

        private void HandleNoteCut(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            NoteData noteData = noteController.noteData;

            if (noteData.colorType == ColorType.ColorA)
                LeftCombo++;
            else if (noteData.colorType == ColorType.ColorB)
                RightCombo++;

            if (noteData.colorType == ColorType.None || !noteCutInfo.allIsOK)
            {
                LeftCombo = 0;
                RightCombo = 0;
            }
        }

        private void HandleNoteMissed(NoteController noteController)
        {
            if (noteController.noteData.colorType is ColorType.ColorA) LeftCombo = 0;
            else if (noteController.noteData.colorType is ColorType.ColorB) RightCombo = 0;
        }

        internal void UpdateTexts()
        {
            leftText.text = LeftCombo.ToString();
            rightText.text = RightCombo.ToString();
        }

        internal void OnDestroy()
        {
            beatmapObjectManager.noteWasCutEvent -= HandleNoteCut;
            beatmapObjectManager.noteWasMissedEvent -= HandleNoteMissed;
        }
    }
}
