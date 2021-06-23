using HMUI;
using SiraUtil.Tools;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ComboSplitter.Services
{
    internal class CustomComboPanelController : IInitializable, ITickable
    {
        ComboUIController _comboPanel;
        Config _config;
        ColorScheme _scheme;
        List<CurvedTextMeshPro> handTexts = null;
        PlayerHeadAndObstacleInteraction _interaction;
        BeatmapObjectManager _manager;
        public int leftCombo = 0;
        public int rightCombo = 0;

        public CustomComboPanelController(Config config, BeatmapObjectManager manager, ComboUIController comboPanel, ColorScheme scheme, PlayerHeadAndObstacleInteraction interaction)
        {
            _config = config;
            _comboPanel = comboPanel;
            _scheme = scheme;
            _interaction = interaction;
            _manager = manager;
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
            if (_interaction.intersectingObstacles.Count > 0) ClearComboOnObstacleCollision();
            UpdatePanels();
        }

        internal void HandleNoteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (noteController.noteData.colorType is ColorType.ColorA) rightCombo++;
            else if (noteController.noteData.colorType is ColorType.ColorB) leftCombo++;

            if (noteController.noteData.colorType is ColorType.None || !noteCutInfo.allIsOK)
            {
                leftCombo = 0;
                rightCombo = 0;
            }
        }

        internal void HandleNoteWasMissedEvent(NoteController noteController)
        {
            if (noteController.noteData.colorType is ColorType.ColorA) rightCombo = 0;
            else if (noteController.noteData.colorType is ColorType.ColorB) leftCombo = 0;
        }

        internal void UpdatePanels()
        {
            UpdateComboForPanel(handTexts[0], leftCombo);
            UpdateComboForPanel(handTexts[1], rightCombo);
        }

        internal void UpdateComboForPanel(CurvedTextMeshPro panel, int combo) =>
            panel.text = combo.ToString();

        internal void ClearComboOnObstacleCollision()
        {
            rightCombo = 0;
            leftCombo = 0;
        }
    }
}