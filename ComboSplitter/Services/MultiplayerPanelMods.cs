using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComboSplitter.Services
{
    /*internal class MultiplayerPanelMods : CustomComboPanelController
    {
        public void Awake() => didFinishLoadingEvent += AddEventsOnSetupComplete;

        public void AddEventsOnSetupComplete()
        {
            beatmapObjectManager.noteWasCutEvent += HandleNoteWasCutEvent;
            beatmapObjectManager.noteWasMissedEvent += HandleNoteWasMissedEvent;
            isSetup = true;
        }

        public void LateUpdate()
        {
            if (!isSetup) return;

            if (collision.intersectingObstacles.Count > 0)
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

        }

        public void UpdatePanels()
        {

        }
    }*/
}