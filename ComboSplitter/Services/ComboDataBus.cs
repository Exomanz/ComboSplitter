using System;

namespace ComboSplitter.Services
{
    public class ComboDataBus : IDisposable
    {
        public event Action<int, int> LevelDidFinishWithHandCutDataEvent = delegate { };
        public int LeftHandCuts { get; private set; } = 0;
        public int RightHandCuts { get; private set; } = 0;

        public void SendData(int leftHandCuts, int rightHandCuts)
        {
            this.LevelDidFinishWithHandCutDataEvent.Invoke(leftHandCuts, rightHandCuts);
            LeftHandCuts = leftHandCuts;
            RightHandCuts = rightHandCuts;
        }

        public void Dispose()
        {
            LeftHandCuts = 0;
            RightHandCuts = 0;
        }
    }
}
