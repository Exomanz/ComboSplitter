namespace ComboSplitter
{
    public class CSConfig
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual bool UseSaberColorScheme { get; set; } = true;
        public virtual bool ShowResultsHoverHint { get; set; } = true;
        public virtual bool UseColorSchemeInHoverHint { get; set; } = true;
        public virtual bool ShowPercentageInHoverHint { get; set; } = true;
        public virtual bool ShowMissInfoInHoverHint { get; set; } = true;
        public virtual bool ExtendMissInfoInHoverHint { get; set; } = true;
    }
}
