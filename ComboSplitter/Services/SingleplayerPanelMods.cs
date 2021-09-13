using Zenject;

namespace ComboSplitter.Services
{
    /*internal class SingleplayerPanelMods : CustomComboPanelController
    {
#pragma warning disable CS0649
        [Inject] private PauseMenuManager pauseManager;
#pragma warning restore CS0649

        public void Awake() => didFinishLoadingEvent += AddEventsOnSetupComplete;

        public void AddEventsOnSetupComplete()
        {
            beatmapObjectManager.noteWasCutEvent += HandleNoteWasCutEvent;
            beatmapObjectManager.noteWasMissedEvent += HandleNoteWasMissedEvent;
            isSetup = transform;
        }

        public void LateUpdate()
        {
            if (!isSetup) return;

            if (collision.intersectingObstacles.Count > 0 && !pauseManager.enabled)
            {
                leftCombo = 0;
                rightCombo = 0;
            }
            UpdatePanels();
        }

        public void HandleNoteWasCutEvent(NoteController noteController, in NoteCutInfo noteCutInfo)
        {
            if (noteController.noteData.colorType is ColorType.ColorA) leftCombo++;
            else if (noteController.noteData.colorType is ColorType.ColorB) rightCombo++;

            if (noteController.noteData.colorType is ColorType.None || !noteCutInfo.allIsOK)
            {
                leftCombo = 0;
                rightCombo = 0;
            }
        }

        public void HandleNoteWasMissedEvent(NoteController noteController)
        {
            if (noteController.noteData.colorType is ColorType.ColorA) leftCombo = 0;
            else if (noteController.noteData.colorType is ColorType.ColorB) rightCombo = 0;
        }

        public void UpdatePanels()
        {
            leftText.text = leftCombo.ToString();
            rightText.text = rightCombo.ToString();
        }
    }*/
}