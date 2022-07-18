using ExamUniverse.Converter.VCE.Enums;

namespace ExamUniverse.Converter.VCE.Models.Formatting
{
    /// <summary>
    ///     Data model
    /// </summary>
    public class DataModel
    {
        public byte[] Data { get; set; }
        public DataType Type { get; set; }

        public byte[] Property1 { get; set; }
        public byte[] Property2 { get; set; }
    }
}
