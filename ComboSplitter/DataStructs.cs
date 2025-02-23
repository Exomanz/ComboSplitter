namespace ComboSplitter
{
    public struct ComboSplitterDataPackage
    {
        public bool IsMapOneSaber;
        public string ActiveSaberType;
        public int TotalLeftCuttableNotes;
        public int TotalRightCuttableNotes;
        public PerHandCutData CutData;
        public PerHandMissData MissData;
        public PerHandBombData BombData;

        public ComboSplitterDataPackage(bool isMapOneSaber, string activeSaberType, int totalLeftCuttableNotes, int totalRightCuttableNotes, PerHandCutData cutData, PerHandMissData missData, PerHandBombData bombData)
        {
            IsMapOneSaber = isMapOneSaber;
            ActiveSaberType = activeSaberType;
            TotalLeftCuttableNotes = totalLeftCuttableNotes;
            TotalRightCuttableNotes = totalRightCuttableNotes;
            CutData = cutData;
            MissData = missData;
            BombData = bombData;
        }

#if DEBUG
        public override readonly string ToString()
        {
            return $"[Contents of ComboSplitterDataPackage]\n" +
                $"--------------------\n" +
                $"IsMapOneSaber: {IsMapOneSaber}\n" +
                $"ActiveSaberType: '{ActiveSaberType}'\n" +
                $"Total Left Cuttable Notes: {TotalLeftCuttableNotes}\n" +
                $"Total Right Cuttable Notes: {TotalRightCuttableNotes}\n" +
                $"--------\n" +
                $"Contents of CutData:\n{CutData}\n" +
                $"--------\n" +
                $"Contents of MissData:\n{MissData}\n" +
                $"--------\n" +
                $"Contents of BombData:\n{BombData}\n" +
                $"--------\n";
        }
#endif
    }


    public struct PerHandCutData
    {
        public int LeftHandCuts;
        public int RightHandCuts;
        public int LeftHandBadCuts;
        public int RightHandBadCuts;

        public PerHandCutData(int leftHandCutCount, int rightHandCutCount, int leftHandBadCuts, int rightHandBadCuts)
        {
            LeftHandCuts = leftHandCutCount;
            RightHandCuts = rightHandCutCount;
            LeftHandBadCuts = leftHandBadCuts;
            RightHandBadCuts = rightHandBadCuts;
        }

#if DEBUG
        public override readonly string ToString()
        {
            return $"Left Hand Cuts: {LeftHandCuts}\n" +
                $"Right Hand Cuts: {RightHandCuts}\n" +
                $"Left Hand Bad Cuts: {LeftHandBadCuts}\n" +
                $"Right Hand Bad Cuts: {RightHandBadCuts}";
        }
#endif
    }

    public struct PerHandMissData
    {
        public int LeftHandMisses;
        public int RightHandMisses;

        public PerHandMissData(int leftHandMissCount, int rightHandMissCount)
        {
            LeftHandMisses = leftHandMissCount;
            RightHandMisses = rightHandMissCount;
        }

#if DEBUG
        public override readonly string ToString()
        {
            return $"Left Hand Misses: {LeftHandMisses}\n" +
                $"Right Hand Misses: {RightHandMisses}";
        }
#endif
    }

    public struct PerHandBombData
    {
        public int LeftHandBombCutCount;
        public int RightHandBombCutCount;

        public PerHandBombData(int leftHandBombCutCount, int rightHandBombCutCount)
        {
            LeftHandBombCutCount = leftHandBombCutCount;
            RightHandBombCutCount = rightHandBombCutCount;
        }

#if DEBUG
        public override readonly string ToString()
        {
            return $"Left Hand Bomb Cut Count: {LeftHandBombCutCount}\n" +
                $"Right Hand Bomb Cut Count: {RightHandBombCutCount}";
        }
#endif
    }
}
