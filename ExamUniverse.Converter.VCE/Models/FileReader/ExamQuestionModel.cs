using System.Collections.Generic;
using ExamUniverse.Converter.VCE.Enums;

namespace ExamUniverse.Converter.VCE.Models.FileReader
{
    /// <summary>
    ///     Exam question model
    /// </summary>
    public class ExamQuestionModel
    {
        public ExamQuestionModel()
        {
            Variants = new List<string>();
            HotAreas = new List<HotAreaModel>();
            DranAndDropTypes = new List<byte>();
            DragAreas = new List<DragAndDropAreaModel>();
            DropAreas = new List<DragAndDropAreaModel>();
            BlockAnswers = new List<string>();
        }

        public int Id { get; set; }
        public int ExamSectionId { get; set; }

        public ExamQuestionType Type { get; set; }

        public int SectionId { get; set; }
        public int Complexity { get; set; }

        public string Question { get; set; }

        public int VariantsCount { get; set; }
        public List<string> Variants { get; set; }

        public string AreaImage { get; set; }

        public int HotAreasCount { get; set; }
        public List<HotAreaModel> HotAreas { get; set; }

        public List<byte> DranAndDropTypes { get; set; }

        public int DragAreasCount { get; set; }
        public List<DragAndDropAreaModel> DragAreas { get; set; }

        public int DropAreasCount { get; set; }
        public List<DragAndDropAreaModel> DropAreas { get; set; }

        public string Reference { get; set; }
        public string Answers { get; set; }

        public int BlockAnswersCount { get; set; }
        public List<string> BlockAnswers { get; set; }
    }
}
