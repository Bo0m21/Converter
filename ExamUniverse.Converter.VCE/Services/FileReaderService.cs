using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using ExamUniverse.Converter.VCE.Enums;
using ExamUniverse.Converter.VCE.Extensions;
using ExamUniverse.Converter.VCE.Models.FileReader;
using ExamUniverse.Converter.VCE.Services.Interfaces;

namespace ExamUniverse.Converter.VCE.Services
{
    /// <summary>
    ///     File reader service
    /// </summary>
    public class FileReaderService : IFileReaderService
    {
        private readonly IFormattingService _formattingService;

        public FileReaderService(IFormattingService formattingService)
        {
            _formattingService = formattingService;
        }

        /// <summary>
        ///     Read file
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="decryptKeys"></param>
        /// <returns></returns>
        public FileModel ReadFile(byte[] bytes, byte[] encryptKeys, byte[] decryptKeys)
        {
            FileModel fileModel = new FileModel();
            fileModel.EncryptKeys = encryptKeys;
            fileModel.DecryptKeys = decryptKeys;

            using (BinaryReader binaryReader = new BinaryReader(new MemoryStream(bytes)))
            {
                byte headerFirstByte = binaryReader.ReadByte();
                byte headerSecondByte = binaryReader.ReadByte();

                // Check header bytes
                if (headerFirstByte != 0x85 || headerSecondByte != 0xA8)
                {
                    throw new Exception("Invalid file format");
                }

                #region Read version
                byte versionFirstByte = binaryReader.ReadByte();
                byte versionSecondByte = binaryReader.ReadByte();
                fileModel.Version = versionFirstByte * 10 + versionSecondByte;
                #endregion

                #region Read keys
                int typeCodeLen = binaryReader.ReadInt32();

                // Get keys for exam
                fileModel.Keys = ReadArray(binaryReader);
                byte[] encryptKeysBytes = ReadArray(binaryReader);
                #endregion

                #region Read information file
                byte td3 = binaryReader.ReadByte();

                fileModel.Number = ReadDecryptArray(binaryReader, fileModel).GetString();
                fileModel.Title = ReadDecryptArray(binaryReader, fileModel).GetString();

                fileModel.PassingScore = binaryReader.ReadInt32();
                fileModel.TimeLimit = binaryReader.ReadInt32();

                fileModel.FileVersion = ReadDecryptArray(binaryReader, fileModel).GetString();

                long td7 = binaryReader.ReadInt64();
                long td8 = binaryReader.ReadInt64();

                // delete from 62 version file
                if (fileModel.Version <= 61)
                {
                    long td9 = binaryReader.ReadInt64();
                    long td10 = binaryReader.ReadInt64();

                    byte td11 = binaryReader.ReadByte();
                    byte td12 = binaryReader.ReadByte();

                    string td13 = ReadDecryptArray(binaryReader, fileModel).GetString();
                }

                #endregion

                #region Read styles
                string style = ReadDecryptArray(binaryReader, fileModel).GetString();
                #endregion

                #region Read description
                string description = ReadDecryptArray(binaryReader, fileModel).GetString();
                #endregion

                #region Read sections
                fileModel.SectionsCount = binaryReader.ReadInt32();

                for (int i = 1; i <= fileModel.SectionsCount; i++)
                {
                    SectionModel sectionModel = new SectionModel();

                    sectionModel.Id = binaryReader.ReadInt32();
                    sectionModel.Name = ReadDecryptArray(binaryReader, fileModel).GetString();

                    fileModel.Sections.Add(sectionModel);
                }
                #endregion

                // Read TODO
                string td17 = ReadDecryptArray(binaryReader, fileModel).GetString();

                // Exams count
                int examsCount = binaryReader.ReadInt32();

                // Read all exams
                for (int i = 0; i < examsCount; i++)
                {
                    ExamModel examModel = new ExamModel();
                    examModel.Id = fileModel.Exams.Count + 1;

                    examModel.Type = (ExamType)binaryReader.ReadByte();

                    if (!Enum.IsDefined(typeof(ExamType), examModel.Type))
                    {
                        throw new Exception("Exam type not found");
                    }

                    examModel.Name = ReadDecryptArray(binaryReader, fileModel).GetString();

                    if (examModel.Type == ExamType.Question)
                    {
                        int examQuestionsCount = binaryReader.ReadInt32();
                        ReadExamQuestions(binaryReader, fileModel, examModel, examQuestionsCount);
                    }
                    else if (examModel.Type == ExamType.Section)
                    {
                        int examSectionsCount = binaryReader.ReadInt32();
                        ReadExamSections(binaryReader, fileModel, examModel, examSectionsCount);
                    }

                    fileModel.ExamsCount = fileModel.ExamsCount + 1;
                    fileModel.Exams.Add(examModel);
                }

                // File length
                int fileLength = binaryReader.ReadInt32();

                if (binaryReader.BaseStream.Length != fileLength)
                {
                    throw new Exception("Invalid file format");
                }

                return fileModel;
            }
        }

        /// <summary>
        ///     Read exam questions
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="fileModel"></param>
        /// <param name="examModel"></param>
        /// <param name="examQuestionCount"></param>
        /// <param name="examSectionId"></param>
        private void ReadExamQuestions(BinaryReader binaryReader, FileModel fileModel, ExamModel examModel, int examQuestionCount, int examSectionId = -1)
        {
            for (int i = 0; i < examQuestionCount; i++)
            {
                ExamQuestionModel examQuestionModel = new ExamQuestionModel();
                examQuestionModel.Id = examModel.ExamQuestionsCount + 1;
                examQuestionModel.ExamSectionId = examSectionId;

                if (fileModel.Version >= 61)
                {
                    int extraQuestionBytes = GetArrayLength(binaryReader);
                }

                byte[] td1 = ReadDecryptArray(binaryReader, fileModel);

                examQuestionModel.Type = (ExamQuestionType)binaryReader.ReadByte();

                if (!Enum.IsDefined(typeof(ExamQuestionType), examQuestionModel.Type))
                {
                    throw new Exception("Exam question type not found");
                }

                examQuestionModel.SectionId = binaryReader.ReadInt32();
                examQuestionModel.Complexity = binaryReader.ReadInt32();

                int td2 = binaryReader.ReadInt32();

                if (examQuestionModel.Type == ExamQuestionType.SingleChoice || examQuestionModel.Type == ExamQuestionType.MultipleChoice)
                {
                    byte[] questionBytes = ReadDecryptArray(binaryReader, fileModel);
                    byte[] answersBytes = ReadDecryptArray(binaryReader, fileModel);

                    examQuestionModel.VariantsCount = binaryReader.ReadInt32();

                    byte td3 = binaryReader.ReadByte();
                    byte td4 = binaryReader.ReadByte();
                    byte td5 = binaryReader.ReadByte();

                    byte[] td6 = ReadDecryptArray(binaryReader, fileModel);

                    _formattingService.FormattingExamQuestion(examQuestionModel, questionBytes, examQuestionModel.VariantsCount);
                    _formattingService.FormattingExamQuestionAnswers(examQuestionModel, answersBytes);
                }
                else if (examQuestionModel.Type == ExamQuestionType.HotArea)
                {
                    byte[] variantsBytes = ReadDecryptArray(binaryReader, fileModel);
                    byte[] answersBytes = ReadDecryptArray(binaryReader, fileModel);
                    byte[] questionBytes = ReadDecryptArray(binaryReader, fileModel);

                    byte td3 = binaryReader.ReadByte();
                    byte td4 = binaryReader.ReadByte();

                    byte[] image = ReadDecryptArray(binaryReader, fileModel);

                    _formattingService.FormattingExamQuestionArea(examQuestionModel, questionBytes);
                    _formattingService.FormattingExamQuestionAreaImage(examQuestionModel, image);
                    _formattingService.FormattingExamQuestionHotAreaVariants(examQuestionModel, variantsBytes);
                    _formattingService.FormattingExamQuestionHotAreaAnswers(examQuestionModel, answersBytes);
                }
                else if (examQuestionModel.Type == ExamQuestionType.DragAndDrop)
                {
                    byte[] variantsBytes = ReadDecryptArray(binaryReader, fileModel);
                    byte[] answersBytes = ReadDecryptArray(binaryReader, fileModel);
                    byte[] questionBytes = ReadDecryptArray(binaryReader, fileModel);

                    byte td3 = binaryReader.ReadByte();
                    byte td4 = binaryReader.ReadByte();

                    byte[] image = ReadDecryptArray(binaryReader, fileModel);

                    _formattingService.FormattingExamQuestionArea(examQuestionModel, questionBytes);
                    _formattingService.FormattingExamQuestionAreaImage(examQuestionModel, image);
                    _formattingService.FormattingExamQuestionDragAndDropAreaVariants(examQuestionModel, variantsBytes, image);
                    _formattingService.FormattingExamQuestionDragAndDropAreaAnswers(examQuestionModel, answersBytes);
                }
                else if (examQuestionModel.Type == ExamQuestionType.FillInTheBlank)
                {
                    byte[] questionBytes = ReadDecryptArray(binaryReader, fileModel);
                    byte[] answersBytes = ReadDecryptArray(binaryReader, fileModel);

                    int answersCount = binaryReader.ReadInt32();

                    byte td4 = binaryReader.ReadByte();
                    byte td5 = binaryReader.ReadByte();
                    byte td6 = binaryReader.ReadByte();

                    _formattingService.FormattingExamQuestionBlock(examQuestionModel, questionBytes);
                    _formattingService.FormattingExamQuestionBlockAnswers(examQuestionModel, answersBytes, answersCount);
                }

                examModel.ExamQuestionsCount = examModel.ExamQuestionsCount + 1;
                examModel.ExamQuestions.Add(examQuestionModel);
            }
        }

        /// <summary>
        ///     Read exam sections
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="fileModel"></param>
        /// <param name="examModel"></param>
        /// <param name="examSectionCount"></param>
        private void ReadExamSections(BinaryReader binaryReader, FileModel fileModel, ExamModel examModel, int examSectionCount)
        {
            for (int i = 0; i < examSectionCount; i++)
            {
                ExamSectionModel examSectionModel = new ExamSectionModel();
                examSectionModel.Id = examModel.ExamSectionsCount + 1;

                examSectionModel.Type = (ExamSectionType)binaryReader.ReadByte();

                if (!Enum.IsDefined(typeof(ExamSectionType), examSectionModel.Type))
                {
                    throw new Exception("Exam section type not found");
                }

                examSectionModel.TimeLimit = binaryReader.ReadInt32();

                if (examSectionModel.Type == ExamSectionType.QuestionSet)
                {
                    var examQuestionsCount = binaryReader.ReadInt32();
                    ReadExamQuestions(binaryReader, fileModel, examModel, examQuestionsCount, examSectionModel.Id);
                }
                else if (examSectionModel.Type == ExamSectionType.Testlet)
                {
                    examSectionModel.Title = ReadDecryptArray(binaryReader, fileModel).GetString();

                    var t1 = binaryReader.ReadInt32();

                    string t2Bytes = ReadDecryptArray(binaryReader, fileModel).GetString();

                    byte[] descriptionBytes = ReadDecryptArray(binaryReader, fileModel);
                    _formattingService.FormattingDescription(examSectionModel, descriptionBytes);

                    var t3 = binaryReader.ReadInt32();

                    var examQuestionsCount = binaryReader.ReadInt32();
                    ReadExamQuestions(binaryReader, fileModel, examModel, examQuestionsCount, examSectionModel.Id);
                }
                else
                {
                    throw new Exception("Section type not found");
                }

                examModel.ExamSectionsCount = examModel.ExamSectionsCount + 1;
                examModel.ExamSections.Add(examSectionModel);
            }
        }

        /// <summary>
        ///     Read array
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        private byte[] ReadArray(BinaryReader binaryReader)
        {
            int messageLen = GetArrayLength(binaryReader);
            return binaryReader.ReadBytes(messageLen);
        }

        /// <summary>
        ///     Read decrypt array
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <param name="fileModel"></param>
        /// <returns></returns>
        private byte[] ReadDecryptArray(BinaryReader binaryReader, FileModel fileModel)
        {
            int messageLen = GetArrayLength(binaryReader);

            if (messageLen == 0)
            {
                return Array.Empty<byte>();
            }

            messageLen = messageLen - 1;

            byte[] selectedKey = default;
            byte slectKey = binaryReader.ReadByte();

            if (slectKey < 0x80)
            {
                if (fileModel.Keys == null || fileModel.Keys.Length == 0)
                {
                    throw new Exception("Key cannot be empty");
                }

                selectedKey = fileModel.Keys;
            }
            else
            {
                if (fileModel.DecryptKeys == null || fileModel.DecryptKeys.Length == 0)
                {
                    throw new Exception("Key cannot be empty");
                }

                selectedKey = fileModel.DecryptKeys;
            }

            int globalOffset = 0;

            if (fileModel.Version >= 61)
            {
                messageLen = messageLen - 4;
                globalOffset = binaryReader.ReadInt32();
            }

            byte[] key = selectedKey.Skip(globalOffset).Take(32).ToArray();
            byte[] iv = selectedKey.Skip(globalOffset).Skip(32).Take(16).ToArray();

            byte[] encryptedBytes = binaryReader.ReadBytes(messageLen);
            byte[] decryptedBytes = DecryptBytes_Aes(encryptedBytes, key, iv);
            return decryptedBytes;
        }

        /// <summary>
        ///     Get array length
        /// </summary>
        /// <param name="binaryReader"></param>
        /// <returns></returns>
        private int GetArrayLength(BinaryReader binaryReader)
        {
            List<byte> bytes = new List<byte>();
            byte vce_char = binaryReader.ReadByte();

            int v1 = GetArrayLength_Xor1(vce_char, vce_char ^ 0x80);
            int v2 = GetArrayLength_Xor2(0x80, v1, 0);

            int counter = 0;

            do
            {
                vce_char = binaryReader.ReadByte();
                int v4 = (v2 ^ vce_char) & 0xff;
                int v5 = GetArrayLength_Xor1(vce_char, v2 ^ vce_char);
                v2 = GetArrayLength_Xor2(v2, v5, counter + 1);

                bytes.Add((byte)v4);
                counter++;
            } while (counter != 4);

            return BitConverter.ToInt32(bytes.ToArray(), 0);
        }

        /// <summary>
        ///     Get array length Xor1
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <returns></returns>
        private int GetArrayLength_Xor1(int a1, int a2)
        {
            return (a1 ^ a2) & 0xff;
        }

        /// <summary>
        ///     Get array length Xor2
        /// </summary>
        /// <param name="a1"></param>
        /// <param name="a2"></param>
        /// <param name="a3"></param>
        /// <returns></returns>
        private int GetArrayLength_Xor2(int a1, int a2, int a3)
        {
            return a1 + a2 | a3;
        }

        /// <summary>
        ///     Decrypt bytes aes
        /// </summary>
        /// <param name="encryptedBytes"></param>
        /// <param name="key"></param>
        /// <param name="iv"></param>
        /// <returns></returns>
        private byte[] DecryptBytes_Aes(byte[] encryptedBytes, byte[] key, byte[] iv)
        {
            using (AesCryptoServiceProvider aesCrypto = new AesCryptoServiceProvider())
            {
                aesCrypto.Padding = PaddingMode.None;

                aesCrypto.Key = key;
                aesCrypto.IV = iv;

                using (ICryptoTransform decryptor = aesCrypto.CreateDecryptor(aesCrypto.Key, aesCrypto.IV))
                {
                    using (MemoryStream msEncrypt = new MemoryStream(encryptedBytes))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(msEncrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (MemoryStream msDecrypt = new MemoryStream())
                            {
                                cryptoStream.CopyTo(msDecrypt);
                                return msDecrypt.ToArray();
                            }
                        }
                    }
                }
            }
        }
    }
}
