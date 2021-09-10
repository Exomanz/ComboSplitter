using HMUI;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ComboSplitter.Services
{
    internal class SingleplayerCustomComboPanelController : IInitializable, ITickable
    {
        Config _config => Plugin.XConfig;

        [InjectOptional] readonly BeatmapObjectManager _manager;
        ComboUIController _comboPanel;
        ColorScheme _scheme;
        List<CurvedTextMeshPro> handTexts = null;
        PlayerHeadAndObstacleInteraction _interaction;
        PauseMenuManager _pauseManager;

        public int leftCombo = 0;
        public int rightCombo = 0;

        public SingleplayerCustomComboPanelController(BeatmapObjectManager manager, ComboUIController comboPanel, ColorScheme scheme, PlayerHeadAndObstacleInteraction interaction
            , PauseMenuManager pauseManager)
        {
            _comboPanel = comboPanel;
            _scheme = scheme;
            _interaction = interaction;
            _manager = manager;
            _pauseManager = pauseManager;
            handTexts = new List<CurvedTextMeshPro>();
        }

        public void Initialize()
        {
            // Yeah, I accidentally flipped the names of the objects. Cry about it :pepega:
            var leftText = _comboPanel.transform.Find("ComboCanvas/NumText").GetComponent<CurvedTextMeshPro>();
            leftText.name = "LeftHandComboText";
            leftText.alignment = TMPro.TextAlignmentOptions.Left;
            leftText.text = leftCombo.ToString();

            var rightText = GameObject.Instantiate(leftText, leftText.transform.parent);
            rightText.name = "RightHandComboText";
            rightText.text = rightCombo.ToString();
            rightText.alignment = TMPro.TextAlignmentOptions.Right;

            if (_config.UseSaberColorScheme)
            {
                rightText.color = _scheme.saberAColor;
                leftText.color = _scheme.saberBColor;
            }

            // Make the objects accessible (didn't feel like making constructor arguments for them so it's just a list)
            handTexts.Add(leftText);
            handTexts.Add(rightText);

            // Storing the ComboText transform because all of my new objects are relative to it
            var relPosTrans = _comboPanel.transform.Find("ComboText").transform;

            var spacer = new GameObject("=== SPACER ===", new System.Type[] { typeof(CurvedTextMeshPro) }).GetComponent<CurvedTextMeshPro>();
            spacer.transform.SetParent(leftText.transform.parent);
            spacer.transform.position = new Vector3(relPosTrans.position.x + 0.9f, relPosTrans.position.y - 0.24f, relPosTrans.position.z);
            spacer.transform.localScale = new Vector3(1f, 0.6f, 1f);
            spacer.fontStyle = TMPro.FontStyles.Italic;
            spacer.text = "|";
            spacer.gameObject.layer = 5;
            spacer.color = Color.white.ColorWithAlpha(0.75f);

            leftText.transform.position = new Vector3(relPosTrans.transform.position.x + 0.69f, spacer.transform.position.y + 0.15f, relPosTrans.transform.position.z);
            rightText.transform.position = new Vector3(relPosTrans.transform.position.x - 0.77f, spacer.transform.position.y + 0.15f, relPosTrans.transform.position.z);

            _manager.noteWasCutEvent += HandleNoteWasCutEvent;
            _manager.noteWasMissedEvent += HandleNoteWasMissedEvent;
        }

        public void Tick()
        {
            if (_interaction.intersectingObstacles.Count > 0 && !_pauseManager.enabled)
            {
                rightCombo = 0;
                leftCombo = 0;
            }
            UpdatePanels();
        }

        private void HandleNoteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (noteController.noteData.colorType is ColorType.ColorA) rightCombo++;
            else if (noteController.noteData.colorType is ColorType.ColorB) leftCombo++;

            if (noteController.noteData.colorType is ColorType.None || !noteCutInfo.allIsOK)
            {
                leftCombo = 0;
                rightCombo = 0;
            }
        }

        private void HandleNoteWasMissedEvent(NoteController noteController)
        {
            if (noteController.noteData.colorType is ColorType.ColorA) rightCombo = 0;
            else if (noteController.noteData.colorType is ColorType.ColorB) leftCombo = 0;
        }

        private void UpdatePanels()
        {
            handTexts[0].text = leftCombo.ToString();
            handTexts[1].text = rightCombo.ToString();
        }
    }
}