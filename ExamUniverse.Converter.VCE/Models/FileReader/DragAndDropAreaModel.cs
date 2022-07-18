namespace ExamUniverse.Converter.VCE.Models.FileReader
{
    /// <summary>
    ///     Drag and drop area model
    /// </summary>
    public class DragAndDropAreaModel
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }

        public string Text { get; set; }
    }
}
