using ExamUniverse.Converter.VCE.Enums;

namespace ExamUniverse.Converter.VCE.Models.Formatting
{
    /// <summary>
    ///     Format line model
    /// </summary>
    public class FormatLineModel
    {
        public FormatLineType Type { get; set; }
        public string Start { get; set; }
        public string End { get; set; }
    }
}
