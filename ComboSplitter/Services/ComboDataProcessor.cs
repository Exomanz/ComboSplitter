using IPA.Utilities;
using SiraUtil.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace ComboSplitter.Services
{
    public class ComboDataProcessor : IDisposable
    {
        [Inject] private readonly SiraLog logger;
        public event Action<ComboSplitterDataPackage> LevelDidFinishWithDataPackageEvent = delegate { };

        public readonly List<BeatmapDataItem> notes = new List<BeatmapDataItem>();

        public void LevelDidFinishWithDataPackage(ComboSplitterDataPackage package)
        {
            this.LevelDidFinishWithDataPackageEvent.Invoke(package);
        }

        public async Task<int[]> GetSpecificHandNoteCountFromBeatmapData(IReadonlyBeatmapData beatmapData, float startTimeToFilterFrom)
        {
            int[] noteCountArray = new int[2];
            int cuttableLeftNotes = 0;
            int cuttableRightNotes = 0;
#if DEBUG
            StringBuilder sb = new StringBuilder();
            const string f_string = "Idx: {0} | Note Type: {1}/{2} | Beat: {3} \n";
            notes.Clear();
#endif
            await Task.Run(async () =>
            {
                BeatmapDataItem[] allBeatmapDataItems = beatmapData.allBeatmapDataItems.ToArray();
                for (int idx = 0; idx < allBeatmapDataItems.Length; idx++)
                {
                    BeatmapDataItem item = allBeatmapDataItems[idx];

                    if (item is not BeatmapObjectData objectData)
                        continue;
#if DEBUG
                    notes.Add(item);
#endif
                    if (item is NoteData noteData &&
                        noteData.gameplayType != NoteData.GameplayType.Bomb &&
                        noteData.time >= startTimeToFilterFrom)
                    {
#if DEBUG
                        sb.Append(string.Format(f_string, notes.IndexOf(item), noteData.gameplayType, noteData.colorType, noteData.beat));
#endif
                        _ = noteData.colorType == ColorType.ColorA ?
                            cuttableLeftNotes++ :
                            cuttableRightNotes++;
                    }

                    else if (item is SliderData sliderData &&
                        sliderData.sliceCount > 0 &&
                        sliderData.time >= startTimeToFilterFrom)
                    {
                        int inc = sliderData.sliceCount - 1;
#if DEBUG
                        for (int i = 0; i < inc; i++)
                            sb.Append(string.Format(f_string, notes.IndexOf(item), NoteData.GameplayType.BurstSliderElement, sliderData.colorType, sliderData.beat));
#endif
                        _ = sliderData.colorType == ColorType.ColorA ?
                        cuttableLeftNotes += inc :
                        cuttableRightNotes += inc;
                    }

                    else continue;
                }
#if DEBUG
                using StreamWriter sw = new StreamWriter(Path.Combine(UnityGame.UserDataPath, "cs_debug.txt"), false);
                await sw.WriteAsync(sb.ToString());
#endif
            });

            noteCountArray[0] = cuttableLeftNotes;
            noteCountArray[1] = cuttableRightNotes;

            return noteCountArray;
        }

        public void Dispose()
        {
        }
    }
}
