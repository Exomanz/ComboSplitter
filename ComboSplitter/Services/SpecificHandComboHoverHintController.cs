using BeatSaberMarkupLanguage;
using HMUI;
using SiraUtil.Logging;
using System;
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
        [Inject] private readonly ComboDataProcessor dataBus;
        [Inject] private readonly SiraLog logger;
#pragma warning restore CS8618, CS0649

        private ComboSplitterDataPackage dataPackage;
        private StringBuilder? stringBuilder;
        private ColorScheme? colorScheme;
        private HoverHint? resultsHoverHint;

        private string saberA_HTML = string.Empty;
        private string saberB_HTML = string.Empty;
        private bool oneSaberMap = false;
        private string activeSaberType = string.Empty;
        private int totalCuttableLeftNotes = 0;
        private int totalCuttableRightNotes = 0;
        private int leftHandCuts = 0;
        private int rightHandCuts = 0;
        private int leftHandBadCuts = 0;
        private int rightHandBadCuts = 0;
        private int leftHandMisses = 0;
        private int rightHandMisses = 0;
        private int leftHandBombCuts = 0;
        private int rightHandBombCuts = 0;

        private static class HoverHintStrings
        {
            internal static string k_LeftHandCuts = "Total Left Hand Cuts : {0} {1}\n";
            internal static string k_RightHandCuts = "Total Right Hand Cuts : {0} {1}\n";
            internal static string k_MissInfo = "<size=80%>{0} Misses {1}</size>\n";
            internal static string k_ColorFormatString = "<color=#{1}>{0}</color>";
            internal static string k_ExtendedInfoString = "| {0} Bombs | {1} Bad Cuts"; 

            internal static string PrepareCutTextsPerHand(string textToModify, bool useColoring, int cutAmount, bool showPercentage, double percentage, string htmlColor)
            {
                string cutAmountString = cutAmount.ToString();
                string fCutText = useColoring ? string.Format(k_ColorFormatString, cutAmountString, htmlColor) : cutAmountString;

                return string.Format(textToModify, new string[] {
                    fCutText,
                    showPercentage ? $"<size=80%>({percentage}%)</size>" : string.Empty,
                });
            }

            internal static string PrepareComboDropTextPerHand(int missCount, bool extendMissInfo, int bombCutCount, int badCutCount)
            {
                static string GenerateFormatString(bool check, int count)
                {
                    string f;

                    if (check)
                    {
                        f = string.Format(k_ColorFormatString, count.ToString(), "A20000");
                    }
                    else f = count.ToString();

                    return f;
                }

                string fMissString = GenerateFormatString(missCount > 0, missCount);
                string fBombString = GenerateFormatString(bombCutCount > 0, bombCutCount);
                string fBadCutString = GenerateFormatString(badCutCount > 0, badCutCount);

                string extendedInfo = string.Format(k_ExtendedInfoString, fBombString, fBadCutString);

                return string.Format(k_MissInfo, new string[] {
                    fMissString,
                    extendMissInfo ? extendedInfo : string.Empty,
                });
            }
        }

        public void Start()
        {
            resultsViewController.didActivateEvent += ResultsViewControllerDidActivate;
            dataBus.LevelDidFinishWithDataPackageEvent += ReceiveDataFromComboPanel;
        }

        private void ReceiveDataFromComboPanel(ComboSplitterDataPackage package)
        {
            this.dataPackage = package;
            this.oneSaberMap = package.IsMapOneSaber;
            this.activeSaberType = package.ActiveSaberType;
            this.totalCuttableLeftNotes = package.TotalLeftCuttableNotes;
            this.totalCuttableRightNotes = package.TotalRightCuttableNotes;
            PerHandCutData cutData = package.CutData;
            this.leftHandCuts = cutData.LeftHandCuts;
            this.rightHandCuts = cutData.RightHandCuts;
            this.leftHandBadCuts = cutData.LeftHandBadCuts;
            this.rightHandBadCuts = cutData.RightHandBadCuts;
            PerHandMissData missData = package.MissData;
            this.leftHandMisses = missData.LeftHandMisses;
            this.rightHandMisses = missData.RightHandMisses;
            PerHandBombData bombData = package.BombData;
            this.leftHandBombCuts = bombData.LeftHandBombCutCount;
            this.rightHandBombCuts = bombData.RightHandBombCutCount;
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
                resultsHoverHint!.enabled = config.ShowResultsHoverHint && config.Enabled; 

                double leftHandCutPercentage = Math.Floor((float)leftHandCuts / (float)totalCuttableLeftNotes * 100);
                double rightHandCutPercentage = Math.Floor((float)rightHandCuts / (float)totalCuttableRightNotes * 100);
                int[] finalMissCountPerHand = CalculateFinalMissCount();
#if DEBUG
                logger.Info(dataPackage.ToString());
                logger.Info("Left Hand Cut Percentage: " + leftHandCutPercentage);
                logger.Info("Right Hand Cut Percentage: " + rightHandCutPercentage);
#endif
                colorScheme = playerDataModel.playerData.colorSchemesSettings.GetSelectedColorScheme();
                saberA_HTML = ColorUtility.ToHtmlStringRGB(colorScheme.saberAColor);
                saberB_HTML = ColorUtility.ToHtmlStringRGB(colorScheme.saberBColor);

                string[] lines = new string[4];

                lines[0] = HoverHintStrings.PrepareCutTextsPerHand(HoverHintStrings.k_LeftHandCuts, config.UseColorSchemeInHoverHint, leftHandCuts, config.ShowPercentageInHoverHint, leftHandCutPercentage, saberA_HTML);
                lines[1] = HoverHintStrings.PrepareCutTextsPerHand(HoverHintStrings.k_RightHandCuts, config.UseColorSchemeInHoverHint, rightHandCuts, config.ShowPercentageInHoverHint, rightHandCutPercentage, saberB_HTML);
                lines[2] = HoverHintStrings.PrepareComboDropTextPerHand(finalMissCountPerHand[0], config.ExtendMissInfoInHoverHint, leftHandBombCuts, leftHandBadCuts);
                lines[3] = HoverHintStrings.PrepareComboDropTextPerHand(finalMissCountPerHand[1], config.ExtendMissInfoInHoverHint, rightHandBombCuts, rightHandBadCuts);

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

                stringBuilder = new StringBuilder(150);
                stringBuilder.Append(lines[0]);
                stringBuilder.Append(lines[1]);

                if (config.ShowMissInfoInHoverHint)
                {
                    stringBuilder.Insert(lines[0].Length, lines[2]);
                    stringBuilder.Append(lines[3]);
                }

                resultsHoverHint!.text = stringBuilder.ToString();
            }
        }

        // We gotta do some specific calculation depending on what settings are active.
        // Too many to throw into the other method.
        private int[] CalculateFinalMissCount()
        {
            int[] missArray = new int[2];
            if (!config.ShowMissInfoInHoverHint) return missArray;
            
            int leftHandMisses = this.leftHandMisses;
            int rightHandMisses = this.rightHandMisses;

            if (!config.ExtendMissInfoInHoverHint)
            {
                leftHandMisses += leftHandBadCuts + leftHandBombCuts;
                rightHandMisses += rightHandBadCuts + rightHandBombCuts;
            }

            missArray[0] = leftHandMisses;
            missArray[1] = rightHandMisses;

            return missArray;
        }

        public void OnDestroy()
        {
            resultsViewController.didActivateEvent -= ResultsViewControllerDidActivate;
            dataBus.LevelDidFinishWithDataPackageEvent -= ReceiveDataFromComboPanel;
        }
    }
}
