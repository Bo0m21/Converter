using ExamUniverse.Converter.VCE.Models.FileReader;

namespace ExamUniverse.Converter.VCE.Services.Interfaces
{
    /// <summary>
    ///     Interface for file reader service
    /// </summary>
    public interface IFileReaderService
    {
        /// <summary>
        ///     Read file
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="encryptKeys"></param>
        /// <param name="decryptKeys"></param>
        /// <returns></returns>
        FileModel ReadFile(byte[] bytes, byte[] encryptKeys, byte[] decryptKeys);
    }
}
