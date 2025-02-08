namespace ComboSplitter
{
    public struct PerHandCutData
    {
        public int LeftHandCuts;
        public int RightHandCuts;
        public bool OneSaberMap;
        public string ActiveSaberType;

        public PerHandCutData(int leftHandCutCount, int rightHandCutCount, bool oneSaberMap, string activeSaberType)
        {
            LeftHandCuts = leftHandCutCount;
            RightHandCuts = rightHandCutCount;
            OneSaberMap = oneSaberMap;
            ActiveSaberType = activeSaberType;
        }
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
    }
}
