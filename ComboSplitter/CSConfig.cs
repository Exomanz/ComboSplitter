namespace ComboSplitter
{
    public class CSConfig
    {
        public virtual bool Enabled { get; set; } = true;
        public virtual bool UseSaberColorScheme { get; set; } = true;
        public virtual bool UseColorSchemeInHoverHint { get; set; } = true;
        public virtual bool ShowComboDropsInHoverHint { get; set; } = true;
    }
}
