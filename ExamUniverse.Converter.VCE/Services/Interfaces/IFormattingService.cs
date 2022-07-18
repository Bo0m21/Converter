using ExamUniverse.Converter.VCE.Models.FileReader;

namespace ExamUniverse.Converter.VCE.Services.Interfaces
{
    /// <summary>
    ///     Interface for formatting service
    /// </summary>
    public interface IFormattingService
    {
        /// <summary>
        ///     Formatting exam question
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="question"></param>
        /// <param name="variantsCount"></param>
        void FormattingExamQuestion(ExamQuestionModel examQuestionModel, byte[] question, int variantsCount);

        /// <summary>
        ///     Formatting exam question area
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="question"></param>
        void FormattingExamQuestionArea(ExamQuestionModel examQuestionModel, byte[] question);

        /// <summary>
        ///     Formatting exam question block
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="question"></param>
        void FormattingExamQuestionBlock(ExamQuestionModel examQuestionModel, byte[] question);

        /// <summary>
        ///     Formatting exam question area image
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="image"></param>
        void FormattingExamQuestionAreaImage(ExamQuestionModel examQuestionModel, byte[] image);

        /// <summary>
        ///     Formatting exam question hot area variants
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="variants"></param>
        void FormattingExamQuestionHotAreaVariants(ExamQuestionModel examQuestionModel, byte[] variants);

        /// <summary>
        ///     Formatting exam question drag and drop area variants
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="variants"></param>
        /// <param name="image"></param>
        void FormattingExamQuestionDragAndDropAreaVariants(ExamQuestionModel examQuestionModel, byte[] variants, byte[] image);

        /// <summary>
        ///     Formatting question answers
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="answers"></param>
        void FormattingExamQuestionAnswers(ExamQuestionModel examQuestionModel, byte[] answers);

        /// <summary>
        ///     Formatting exam question hot area answers
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="answers"></param>
        void FormattingExamQuestionHotAreaAnswers(ExamQuestionModel examQuestionModel, byte[] answers);

        /// <summary>
        ///     Formatting exam question drag and drop area answers
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="answers"></param>
        void FormattingExamQuestionDragAndDropAreaAnswers(ExamQuestionModel examQuestionModel, byte[] answers);

        /// <summary>
        ///     Formatting question block answers
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="answers"></param>
        /// <param name="answersCount"></param>
        void FormattingExamQuestionBlockAnswers(ExamQuestionModel examQuestionModel, byte[] answers, int answersCount);

        /// <summary>
        ///     Formatting description
        /// </summary>
        /// <param name="examSectionModel"></param>
        /// <param name="description"></param>
        void FormattingDescription(ExamSectionModel examSectionModel, byte[] description);
    }
}
