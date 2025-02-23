using System;
using System.Linq;
using System.Threading.Tasks;

namespace ComboSplitter.Services
{
    public class ComboDataProcessor : IDisposable
    {
        public event Action<ComboSplitterDataPackage> LevelDidFinishWithDataPackageEvent = delegate { };

        public void LevelDidFinishWithDataPackage(ComboSplitterDataPackage package)
        {
            this.LevelDidFinishWithDataPackageEvent.Invoke(package);
        }

        public async Task<int[]> GetSpecificHandNoteCountFromBeatmapData(IReadonlyBeatmapData beatmapData, float startTimeToFilterFrom)
        {
            int[] noteCountArray = new int[2];
            int cuttableLeftNotes = 0;
            int cuttableRightNotes = 0;

            await Task.Run(() =>
            {
                BeatmapDataItem[] allBeatmapDataItems = beatmapData.allBeatmapDataItems.ToArray();
                for (int idx = 0; idx < allBeatmapDataItems.Length; idx++)
                {
                    BeatmapDataItem item = allBeatmapDataItems[idx];

                    if (item.type != BeatmapDataItem.BeatmapDataItemType.BeatmapEvent)
                    {
                        if (item is NoteData noteData &&
                            noteData.gameplayType != NoteData.GameplayType.Bomb &&
                            noteData.time >= startTimeToFilterFrom)
                        {
                            if (noteData.colorType == ColorType.ColorA)
                                cuttableLeftNotes++;

                            else cuttableRightNotes++;
                        }
                    }
                }
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
