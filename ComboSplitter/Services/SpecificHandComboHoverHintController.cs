using BeatSaberMarkupLanguage;
using HMUI;
using SiraUtil.Logging;
using UnityEngine;
using Zenject;

namespace ComboSplitter.Services
{
    public class SpecificHandComboHoverHintController : MonoBehaviour
    {
#pragma warning disable CS8618, CS0649
        [Inject] private readonly CSConfig config;
        [Inject] private readonly PlayerDataModel playerDataModel;
        [Inject] private readonly ResultsViewController resultsViewController;
        [Inject] private readonly ComboDataBus dataBus;
#pragma warning restore CS8618, CS0649

        private ColorScheme? colorScheme;
        private HoverHint? resultsHoverHint;

        private int leftHandCuts = 0;
        private int rightHandCuts = 0;
        private string saberA_HTML = string.Empty;
        private string saberB_HTML = string.Empty;

        public void Start()
        {
            resultsViewController.didActivateEvent += ResultsViewControllerDidActivate;
            dataBus.LevelDidFinishWithHandCutDataEvent += ReceiveDataFromPanel;
        }

        public void ReceiveDataFromPanel(int leftCuts, int rightCuts)
        {
            this.leftHandCuts = leftCuts; 
            this.rightHandCuts = rightCuts;
        }

        private void ResultsViewControllerDidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                Transform goodCutsParent = resultsViewController.transform.Find("Container/ClearedInfo/GoodCuts");
                GameObject goodCutsGO = goodCutsParent.gameObject;
                transform.SetParent(goodCutsParent);
                resultsHoverHint = BeatSaberUI.DiContainer.InstantiateComponent<HoverHint>(goodCutsGO);
            }
            if (addedToHierarchy)
            {
                colorScheme = playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme();
                saberA_HTML = ColorUtility.ToHtmlStringRGB(colorScheme.saberAColor);
                saberB_HTML = ColorUtility.ToHtmlStringRGB(colorScheme.saberBColor);

                string colorHintText =
                    $"Total Left Hand Cuts: <color=#{saberA_HTML}>{leftHandCuts}</color>\n" +
                    $"Total Right Hand Cuts: <color=#{saberB_HTML}>{rightHandCuts}</color>";

                string standardHintText =
                    $"Total Left Hand Cuts: {leftHandCuts}\n" +
                    $"Total Right Hand Cuts: {rightHandCuts}";

                resultsHoverHint!.text = config.UseColorSchemeInHoverHint ? colorHintText : standardHintText;
            }
        }
    }
}
