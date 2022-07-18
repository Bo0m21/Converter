using ExamUniverse.Converter.VCE.Enums;

namespace ExamUniverse.Converter.VCE.Models.Formatting
{
    /// <summary>
    ///     Format font model
    /// </summary>
    public class FormatFontModel
    {
        public FormatFontType Type { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
