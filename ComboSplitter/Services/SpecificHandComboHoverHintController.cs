using BeatSaberMarkupLanguage;
using HMUI;
using IPA.Utilities;
using System.Text;
using TMPro;
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

        private StringBuilder? stringBuilder;
        private ColorScheme? colorScheme;
        private HoverHint? resultsHoverHint;

        private bool oneSaberMap = false;
        private int leftHandCuts = 0;
        private int rightHandCuts = 0;
        private int leftHandMisses = 0;
        private int rightHandMisses = 0;
        private string saberA_HTML = string.Empty;
        private string saberB_HTML = string.Empty;
        private string activeSaberType = string.Empty;

        private static class HoverHintStrings
        {
            internal static string k_LeftHandCuts = "Total Left Hand Cuts: {0}{1}{2}\n";
            internal static string k_RightHandCuts = "Total Right Hand Cuts: {0}{1}{2}\n";
            internal static string k_ComboDrops = "<size=80%>Combo Drops: {0}{1}{2}</size>\n";

            internal static string PrepareCutTextsPerHand(string textToModify, bool useColoring, int cutAmount, string htmlColor)
            {
                return string.Format(textToModify, new string[] {
                    useColoring ? $"<color=#{htmlColor}>" : string.Empty,
                    cutAmount.ToString(),
                    useColoring ? "</color>" : string.Empty,
                });
            }

            internal static string PrepareComboDropTextPerHand(int missAmount)
            {
                return string.Format(k_ComboDrops, new string[] {
                    missAmount > 0 ? "<color=#A20000>" : string.Empty,
                    missAmount.ToString(),
                    missAmount > 0 ? "</color>" : string.Empty 
                });
            }
        }

        public void Start()
        {
            resultsViewController.didActivateEvent += ResultsViewControllerDidActivate;
            dataBus.LevelDidFinishWithHandCutDataEvent += ReceiveCutDataFromPanel;
            dataBus.LevelDidFinishWithHandMissDataEvent += ReceiveMissDataFromPanel;
        }

        public void ReceiveCutDataFromPanel(PerHandCutData cutData)
        {
            this.leftHandCuts = cutData.LeftHandCuts; 
            this.rightHandCuts = cutData.RightHandCuts;
            this.oneSaberMap = cutData.OneSaberMap;
            this.activeSaberType = cutData.ActiveSaberType;
        }

        public void ReceiveMissDataFromPanel(PerHandMissData missData)
        {
            this.leftHandMisses = missData.LeftHandMisses;
            this.rightHandMisses = missData.RightHandMisses;
        }

        private void ResultsViewControllerDidActivate(bool firstActivation, bool addedToHierarchy, bool screenSystemEnabling)
        {
            if (firstActivation)
            {
                Transform goodCutsParent = resultsViewController.transform.Find("Container/ClearedInfo/GoodCuts");
                GameObject goodCutsGO = goodCutsParent.gameObject;
                this.transform.SetParent(goodCutsParent);
                resultsHoverHint = BeatSaberUI.DiContainer.InstantiateComponent<HoverHint>(goodCutsGO);
                resultsHoverHint._hoverHintController._hoverHintPanel._text.horizontalAlignment = HorizontalAlignmentOptions.Center;
            }
            if (addedToHierarchy)
            {
                resultsHoverHint!.enabled = config.Enabled;

                colorScheme = playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme();
                saberA_HTML = ColorUtility.ToHtmlStringRGB(colorScheme.saberAColor);
                saberB_HTML = ColorUtility.ToHtmlStringRGB(colorScheme.saberBColor);

                stringBuilder = new StringBuilder(120);
                string[] lines = new string[4];

                lines[0] = HoverHintStrings.PrepareCutTextsPerHand(HoverHintStrings.k_LeftHandCuts, config.UseColorSchemeInHoverHint, leftHandCuts, saberA_HTML);
                lines[1] = HoverHintStrings.PrepareCutTextsPerHand(HoverHintStrings.k_RightHandCuts, config.UseColorSchemeInHoverHint, rightHandCuts, saberB_HTML);
                lines[2] = HoverHintStrings.PrepareComboDropTextPerHand(leftHandMisses);
                lines[3] = HoverHintStrings.PrepareComboDropTextPerHand(rightHandMisses);

                if (oneSaberMap)
                {
                    if (activeSaberType == "RightSaber")
                    {
                        lines[0] = string.Empty;
                        lines[2] = string.Empty;
                    }
                    else if (activeSaberType == "LeftSaber")
                    {
                        lines[1] = string.Empty;
                        lines[3] = string.Empty;
                    }
                }

                stringBuilder.Append(lines[0]);
                stringBuilder.Append(lines[1]);

                if (config.ShowComboDropsInHoverHint)
                {
                    stringBuilder.Insert(lines[0].Length, lines[2]);
                    stringBuilder.Append(lines[3]);
                }

                resultsHoverHint!.text = stringBuilder.ToString();
            }
        }
    }
}
