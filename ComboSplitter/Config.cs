using IPA.Config.Stores.Attributes;
using SemVer;
using SiraUtil.Converters;

namespace ComboSplitter
{
    public class Config
    {
        [NonNullable, UseConverter(typeof(VersionConverter))] public virtual Version Version { get; set; } = new Version("0.0.0");
        public virtual bool Enabled { get; set; } = true;
        public virtual bool UseSaberColorScheme { get; set; } = true;
    }
}
