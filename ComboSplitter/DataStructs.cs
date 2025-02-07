namespace ComboSplitter
{
    public struct PerHandCutData
    {
        public int LeftHandCuts;
        public int RightHandCuts;

        public PerHandCutData(int leftHandCutCount, int rightHandCutCount)
        {
            LeftHandCuts = leftHandCutCount;
            RightHandCuts = rightHandCutCount;
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
