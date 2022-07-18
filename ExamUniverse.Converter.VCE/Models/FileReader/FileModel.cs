using System.Collections.Generic;

namespace ExamUniverse.Converter.VCE.Models.FileReader
{
    /// <summary>
    ///     File model
    /// </summary>
    public class FileModel
    {
        public FileModel()
        {
            Sections = new List<SectionModel>();
            Exams = new List<ExamModel>();
        }

        public int Version { get; set; }

        public byte[] Keys { get; set; }
        public byte[] EncryptKeys { get; set; }
        public byte[] DecryptKeys { get; set; }

        public string Number { get; set; }
        public string Title { get; set; }

        public int PassingScore { get; set; }
        public int TimeLimit { get; set; }

        public string FileVersion { get; set; }

        public int SectionsCount { get; set; }
        public List<SectionModel> Sections { get; set; }

        public int ExamsCount { get; set; }
        public List<ExamModel> Exams { get; set; }
    }
}
