using System;

namespace ComboSplitter.Services
{
    public class ComboDataBus : IDisposable
    {
        public event Action<PerHandCutData> LevelDidFinishWithHandCutDataEvent = delegate { };
        public event Action<PerHandMissData> LevelDidFinishWithHandMissDataEvent = delegate { };
        public int LeftHandCuts { get; private set; } = 0;
        public int RightHandCuts { get; private set; } = 0;
        public int LeftHandMisses { get; private set; } = 0;
        public int RightHandMisses { get;private set; } = 0;
        public bool OneSaberMap { get; private set; } = false;
        public string ActiveSaberType { get; private set; } = "RightSaber";

        public void SendData(PerHandCutData perHandCutData, PerHandMissData perHandMissData)
        {
            LeftHandCuts = perHandCutData.LeftHandCuts;
            RightHandCuts = perHandCutData.RightHandCuts;
            LeftHandMisses = perHandMissData.LeftHandMisses;
            RightHandMisses = perHandMissData.RightHandMisses;

            OneSaberMap = perHandCutData.OneSaberMap;
            ActiveSaberType = perHandCutData.ActiveSaberType;

            this.LevelDidFinishWithHandCutDataEvent.Invoke(perHandCutData);
            this.LevelDidFinishWithHandMissDataEvent.Invoke(perHandMissData);
        }

        public void Dispose()
        {
            LeftHandCuts = 0;
            RightHandCuts = 0;
            LeftHandMisses = 0;
            RightHandMisses = 0;
            OneSaberMap = false;
            ActiveSaberType = string.Empty;
        }
    }
}
