using System.Collections.Generic;
using ExamUniverse.Converter.VCE.Enums;

namespace ExamUniverse.Converter.VCE.Models.FileReader
{
    /// <summary>
    ///     Exam model
    /// </summary>
    public class ExamModel
    {
        public ExamModel()
        {
            ExamSections = new List<ExamSectionModel>();
            ExamQuestions = new List<ExamQuestionModel>();
        }

        public int Id { get; set; }

        public ExamType Type { get; set; }
        public string Name { get; set; }

        public int ExamSectionsCount { get; set; }
        public List<ExamSectionModel> ExamSections { get; set; }

        public int ExamQuestionsCount { get; set; }
        public List<ExamQuestionModel> ExamQuestions { get; set; }
    }
}
