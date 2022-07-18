using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using ExamUniverse.Converter.VCE.Enums;
using ExamUniverse.Converter.VCE.Extensions;
using ExamUniverse.Converter.VCE.Models.FileReader;
using ExamUniverse.Converter.VCE.Models.Formatting;
using ExamUniverse.Converter.VCE.Services.Interfaces;
using ExamUniverse.Converter.VCE.Utilits;

namespace ExamUniverse.Converter.VCE.Services
{
    /// <summary>
    ///     Formatting service
    /// </summary>
    public class FormattingService : IFormattingService
    {
        private string _formatLine;
        private List<string> _formatPatterns;

        private List<FormatFontModel> _formatFonts;
        private List<FormatLineModel> _formatLines;

        public FormattingService()
        {
            _formatLine = "<br>";

            _formatPatterns = new List<string>()
            {
                @"^(-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+) ("".*"")$",
                @"^(-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+)$",
                @"^(-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+)$",
                @"^(-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+) (-?\d+)$"
            };

            _formatFonts = new List<FormatFontModel>()
            {
                new FormatFontModel()
                {
                    Type = FormatFontType.Default,
                    Start = "",
                    End = ""
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.Bold,
                    Start = "<b>",
                    End = "</b>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.Monospaced,
                    Start = "<span style=\"font-family: Courier, monospace\">",
                    End = "</span>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.MonospacedBold,
                    Start = "<span style=\"font-family: Courier, monospace\"><b>",
                    End = "</b></span>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.LargeBold,
                    Start = "<span style=\"font-size: large\"><b>",
                    End = "</b></span>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.Underline,
                    Start = "<u>",
                    End = "</u>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.Italic,
                    Start = "<i>",
                    End = "</i>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.UnderlineHyperlink,
                    Start = "<u><a href=\"UrlData\">UrlData",
                    End = "</a></u>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.BoldUnderline,
                    Start = "<b><u>",
                    End = "</u></b>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.Small,
                    Start = "<span style=\"font-size: small\">",
                    End = "</span>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.Large,
                    Start = "<span style=\"font-size: large\">",
                    End = "</span>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.Hyperlink,
                    Start = "<a href=\"UrlData\">UrlData",
                    End = "</a>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.ItalicHyperlink,
                    Start = "<i><a href=\"UrlData\">UrlData",
                    End = "</a></i>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.BoldHyperlink,
                    Start = "<b><a href=\"UrlData\">UrlData",
                    End = "</a></b>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.BoldItalicHyperlink,
                    Start = "<b><i><a href=\"UrlData\">UrlData",
                    End = "</a><i></b>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.BoldItalicUnderlineHyperlink,
                    Start = "<b><i><u><a href=\"UrlData\">UrlData",
                    End = "</a></u><i></b>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.BoldItalic,
                    Start = "<b><i>",
                    End = "</i></b>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.BoldItalicUnderline,
                    Start = "<b><i><u>",
                    End = "</u></i></b>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.MonospacedSmall,
                    Start = "<span style=\"font-family: Courier, monospace; font-size: small\">",
                    End = "</span>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.MonospacedLarge,
                    Start = "<span style=\"font-family: Courier, monospace; font-size: large\">",
                    End = "</span>"
                },
                new FormatFontModel()
                {
                    Type = FormatFontType.Italic,
                    Start = "<i><u>",
                    End = "</u></i>"
                }
            };

            _formatLines = new List<FormatLineModel>()
            {
                new FormatLineModel()
                {
                    Type = FormatLineType.Delete,
                    Start = "",
                    End = ""
                },
                new FormatLineModel()
                {
                    Type = FormatLineType.Left,
                    Start = "",
                    End = ""
                },
                new FormatLineModel() // TODO az-102
                {
                    Type = FormatLineType.SubParagraph,
                    Start = "",
                    End = ""
                },
                new FormatLineModel()
                {
                    Type = FormatLineType.Center,
                    Start = "<div style=\"text-align: center\">",
                    End = "</div>"
                },
                new FormatLineModel()
                {
                    Type = FormatLineType.Right,
                    Start = "<div style=\"text-align: right\">",
                    End = "</div>"
                },
                new FormatLineModel()
                {
                    Type = FormatLineType.Justify,
                    Start = "<div style=\"text-align: justify\">",
                    End = "</div>"
                },
                new FormatLineModel() // TODO az-304 microsoft.az-304.v2021-05-18.by.sienna.102q.eudump
                {
                    Type = FormatLineType.NextLine,
                    Start = "",
                    End = ""
                },
                new FormatLineModel() // TODO AZ-400 microsoft.az-400.v2021-03-26.by.georgia.165q.eudump
                {
                    Type = FormatLineType.NewUi,
                    Start = "",
                    End = ""
                },
                new FormatLineModel() // TODO AZ-400 microsoft.az-400.v2021-03-26.by.georgia.165q.eudump
                {
                    Type = FormatLineType.NewUi_2,
                    Start = "",
                    End = ""
                },
            };
        }

        /// <summary>
        ///     Formatting exam question
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="question"></param>
        /// <param name="variantsCount"></param>
        public void FormattingExamQuestion(ExamQuestionModel examQuestionModel, byte[] question, int variantsCount)
        {
            byte[] splitter = new byte[] { 0x2d, 0x38, 0x20, 0x31, 0x20, 0x33, 0x20, 0x31 };
            Separator separator = new Separator(question, splitter);

            byte[] firstSection = separator.Pop();

            if (separator.Peek().SequenceEqual(splitter))
            {
                firstSection = firstSection.Take(firstSection.Length - 5).ToArray();
                byte[] separatorTemp = separator.Pop();
            }

            byte[] questionSection = separator.Pop();

            if (separator.Peek().SequenceEqual(splitter))
            {
                questionSection = questionSection.Take(questionSection.Length - 5).ToArray();
                byte[] separatorTemp = separator.Pop();
            }

            string questionSectionText = FormatText(questionSection);
            examQuestionModel.Question = questionSectionText;

            for (int i = 0; i < variantsCount; i++)
            {
                byte[] variantSection = separator.Pop();

                if (separator.Peek().SequenceEqual(splitter))
                {
                    variantSection = variantSection.Take(variantSection.Length - 5).ToArray();
                    byte[] separatorTemp = separator.Pop();
                }

                string variantSectionText = FormatText(variantSection);
                examQuestionModel.Variants.Add(variantSectionText);
            }

            byte[] referenceSection = separator.Pop();

            if (separator.Peek().SequenceEqual(splitter))
            {
                referenceSection = referenceSection.Take(referenceSection.Length - 5).ToArray();
                byte[] separatorTemp = separator.Pop();
            }

            string referenceSectionText = FormatText(referenceSection);
            examQuestionModel.Reference = referenceSectionText;
        }

        /// <summary>
        ///     Formatting exam question area
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="question"></param>
        public void FormattingExamQuestionArea(ExamQuestionModel examQuestionModel, byte[] question)
        {
            byte[] splitter = new byte[] { 0x2d, 0x38, 0x20, 0x31, 0x20, 0x33, 0x20, 0x31 };
            Separator separator = new Separator(question, splitter);

            byte[] firstSection = separator.Pop();

            if (separator.Peek().SequenceEqual(splitter))
            {
                firstSection = firstSection.Take(firstSection.Length - 5).ToArray();
                byte[] separatorTemp = separator.Pop();
            }

            byte[] questionSection = separator.Pop();

            if (separator.Peek().SequenceEqual(splitter))
            {
                questionSection = questionSection.Take(questionSection.Length - 5).ToArray();
                byte[] separatorTemp = separator.Pop();
            }

            string questionSectionText = FormatText(questionSection);
            examQuestionModel.Question = questionSectionText;

            byte[] referenceSection = separator.Pop();

            if (separator.Peek().SequenceEqual(splitter))
            {
                referenceSection = referenceSection.Take(referenceSection.Length - 5).ToArray();
                byte[] separatorTemp = separator.Pop();
            }

            string referenceSectionText = FormatText(referenceSection);
            examQuestionModel.Reference = referenceSectionText;
        }

        /// <summary>
        ///     Formatting exam question block
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="question"></param>
        public void FormattingExamQuestionBlock(ExamQuestionModel examQuestionModel, byte[] question)
        {
            byte[] splitter = new byte[] { 0x2d, 0x38, 0x20, 0x31, 0x20, 0x33, 0x20, 0x31 };
            Separator separator = new Separator(question, splitter);

            byte[] firstSection = separator.Pop();

            if (separator.Peek().SequenceEqual(splitter))
            {
                firstSection = firstSection.Take(firstSection.Length - 5).ToArray();
                byte[] separatorTemp = separator.Pop();
            }

            byte[] questionSection = separator.Pop();

            if (separator.Peek().SequenceEqual(splitter))
            {
                questionSection = questionSection.Take(questionSection.Length - 5).ToArray();
                byte[] separatorTemp = separator.Pop();
            }

            string questionSectionText = FormatText(questionSection);
            examQuestionModel.Question = questionSectionText;

            byte[] referenceSection = separator.Pop();

            if (separator.Peek().SequenceEqual(splitter))
            {
                referenceSection = referenceSection.Take(referenceSection.Length - 5).ToArray();
                byte[] separatorTemp = separator.Pop();
            }

            string referenceSectionText = FormatText(referenceSection);
            examQuestionModel.Reference = referenceSectionText;
        }

        /// <summary>
        ///     Formatting exam question area image
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="image"></param>
        public void FormattingExamQuestionAreaImage(ExamQuestionModel examQuestionModel, byte[] image)
        {
            string imageBase64 = Convert.ToBase64String(image, 0, image.Length);
            examQuestionModel.AreaImage = "<img src=\"data:image;base64," + imageBase64 + "\" >";
        }

        /// <summary>
        ///     Formatting exam question hot area variants
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="variants"></param>
        public void FormattingExamQuestionHotAreaVariants(ExamQuestionModel examQuestionModel, byte[] variants)
        {
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(variants));

            examQuestionModel.HotAreasCount = binaryReader.ReadInt32();

            for (int i = 0; i < examQuestionModel.HotAreasCount; i++)
            {
                int startX = binaryReader.ReadInt32();
                int startY = binaryReader.ReadInt32();

                int endX = binaryReader.ReadInt32();
                int endY = binaryReader.ReadInt32();

                examQuestionModel.HotAreas.Add(new HotAreaModel()
                {
                    X = startX,
                    Y = startY,
                    Width = endX - startX,
                    Height = endY - startY
                });
            }
        }

        /// <summary>
        ///     Formatting exam question drag and drop area variants
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="variants"></param>
        /// <param name="image"></param>
        public void FormattingExamQuestionDragAndDropAreaVariants(ExamQuestionModel examQuestionModel, byte[] variants, byte[] image)
        {
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(variants));

            var count = binaryReader.ReadInt32();

            for (int i = 0; i < count; i++)
            {
                byte type = binaryReader.ReadByte();
                examQuestionModel.DranAndDropTypes.Add(type);

                int number = binaryReader.ReadInt32();

                int startX = binaryReader.ReadInt32();
                int startY = binaryReader.ReadInt32();

                int endX = binaryReader.ReadInt32();
                int endY = binaryReader.ReadInt32();

                if (type == 1)
                {
                    Bitmap imageBitmap = new Bitmap(new MemoryStream(image));
                    Bitmap imageCropped = imageBitmap.Clone(new Rectangle(startX, startY, endX - startX, endY - startY), imageBitmap.PixelFormat);

                    byte[] imageCroppedBytes;

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        imageCropped.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);
                        imageCroppedBytes = memoryStream.ToArray();
                    }

                    string imageBase64 = Convert.ToBase64String(imageCroppedBytes, 0, imageCroppedBytes.Length);
                    string imageHtml = "<img src=\"data:image;base64," + imageBase64 + "\" >";

                    examQuestionModel.DragAreasCount = examQuestionModel.DragAreasCount + 1;
                    examQuestionModel.DragAreas.Add(new DragAndDropAreaModel()
                    {
                        X = startX,
                        Y = startY,
                        Width = endX - startX,
                        Height = endY - startY,
                        Text = imageHtml
                    });
                }
                else if (type == 2)
                {
                    examQuestionModel.DropAreasCount = examQuestionModel.DropAreasCount + 1;
                    examQuestionModel.DropAreas.Add(new DragAndDropAreaModel()
                    {
                        X = startX,
                        Y = startY,
                        Width = endX - startX,
                        Height = endY - startY
                    });
                }
                else
                {
                    throw new Exception("Drag and drop area not found");
                }
            }
        }

        /// <summary>
        ///     Formatting question answers
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="answers"></param>
        public void FormattingExamQuestionAnswers(ExamQuestionModel examQuestionModel, byte[] answers)
        {
            examQuestionModel.Answers = answers.GetStringWithReplaced();
        }

        /// <summary>
        ///     Formatting exam question hot area answers
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="answers"></param>
        public void FormattingExamQuestionHotAreaAnswers(ExamQuestionModel examQuestionModel, byte[] answers)
        {
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(answers));

            int count = binaryReader.ReadInt32();

            List<string> numbers = new List<string>();

            for (int i = 0; i < count; i++)
            {
                byte check = binaryReader.ReadByte();

                if (check == 1)
                {
                    string numberText = (i + 1).ToString();
                    numbers.Add(numberText);
                }
            }

            examQuestionModel.Answers = string.Join(",", numbers);
        }

        /// <summary>
        ///     Formatting exam question drag and drop area answers
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="answers"></param>
        public void FormattingExamQuestionDragAndDropAreaAnswers(ExamQuestionModel examQuestionModel, byte[] answers)
        {
            BinaryReader binaryReader = new BinaryReader(new MemoryStream(answers));

            List<string> dragNumbers = new List<string>();
            List<string> dropNumbers = new List<string>();

            int count = binaryReader.ReadInt32();

            int index = 0;

            for (int i = 0; i < count; i++)
            {
                int number = binaryReader.ReadInt32();

                if (number == -1)
                {
                    index = index + 1;
                    dragNumbers.Add((index - 1).ToString());
                    continue;
                }

                if (examQuestionModel.DranAndDropTypes[index] == 1)
                {
                    index = index + 1;
                    dragNumbers.Add(number.ToString());
                }
                else if (examQuestionModel.DranAndDropTypes[index] == 2)
                {
                    index = index + 1;
                    dropNumbers.Add(number.ToString());
                }
                else
                {
                    throw new Exception("Drag and drop area not found");
                }
            }

            if (examQuestionModel.DranAndDropTypes.Count - index > 0)
            {
                List<string> exceptNumbers = dropNumbers.Except(dragNumbers).ToList();
                dragNumbers.AddRange(exceptNumbers);

                dragNumbers = dragNumbers.OrderBy(x => Convert.ToInt32(x)).ToList();
            }

            List<string> numbers = new List<string>();

            for (int i = 0; i < dropNumbers.Count; i++)
            {
                string dropNumber = dropNumbers[i];

                for (int j = 0; j < dragNumbers.Count; j++)
                {
                    string dragNumber = dragNumbers[j];

                    if (dropNumber == dragNumber)
                    {
                        string numberText = (j + 1).ToString();
                        numbers.Add(numberText);
                    }
                }
            }

            examQuestionModel.Answers = string.Join(",", numbers);
        }

        /// <summary>
        ///     Formatting question block answers
        /// </summary>
        /// <param name="examQuestionModel"></param>
        /// <param name="answers"></param>
        /// <param name="answersCount"></param>
        public void FormattingExamQuestionBlockAnswers(ExamQuestionModel examQuestionModel, byte[] answers, int answersCount)
        {
            byte[] splitter = new byte[] { 0x02, 0x07 };
            Separator separator = new Separator(answers, splitter);

            List<string> answersList = new List<string>();

            byte[] sectionTemp = separator.Pop();

            for (int i = 0; i < answersCount; i++)
            {
                byte[] separatorTemp = separator.Pop();

                byte[] answerSection = separator.Pop();

                if (i + 1 < answersCount)
                {
                    answerSection = answerSection.Take(answerSection.Length - 3).ToArray();
                }
                else
                {
                    answerSection = answerSection.ToArray();
                }

                answersList.Add(answerSection.GetString());
            }

            // TODO переписать это для того чтобы сохранять один лист ответов
            examQuestionModel.BlockAnswersCount = answersList.Count;
            examQuestionModel.BlockAnswers = answersList;
        }

        /// <summary>
        ///     Formatting description
        /// </summary>
        /// <param name="examSectionModel"></param>
        /// <param name="description"></param>
        public void FormattingDescription(ExamSectionModel examSectionModel, byte[] description)
        {
            byte[] splitter = new byte[] { 0x2d, 0x38, 0x20, 0x31, 0x20, 0x33, 0x20, 0x31 };
            Separator separator = new Separator(description, splitter);

            if (separator.Peek().SequenceEqual(splitter))
            {
                byte[] separatorTemp = separator.Pop();
            }

            byte[] descriptionSection = separator.Pop();
            string descriptionSectionText = FormatText(descriptionSection);
            examSectionModel.Description = descriptionSectionText;
        }

        /// <summary>
        ///     Format text
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        private string FormatText(byte[] bytes)
        {
            byte[] splitterBlock = new byte[] { 0x0d, 0x0a };
            byte[] splitterLine = new byte[] { 0x29, 0x20 };

            Separator separator = new Separator(bytes, new byte[][] { splitterBlock, splitterLine });
            List<DataModel> dataArray = GetDataBySplitters(separator, splitterBlock, splitterLine);

            List<string> resultString = new List<string>();

            string formatLineStart = "";
            string formatLineEnd = "";

            string formatTextStart = "";
            string formatTextEnd = "";

            for (int i = 0; i < dataArray.Count; i++)
            {
                DataModel data = dataArray[i];

                if (data.Type == DataType.Text)
                {
                    string textDataString = data.Data.GetString();
                    resultString.Add(textDataString);
                    resultString.Add(_formatLine);
                }
                else if (data.Type == DataType.Format)
                {
                    string formatDataString = data.Data.GetString();
                    List<string> formatData = SplitStringByPatterns(formatDataString);

                    if (formatData.Count == 0)
                    {

                    }
                    else if (formatData.Count == 6)
                    {
                        int formatFontNumber = Convert.ToInt32(formatData[0]);
                        FormatFontType formatFontType = (FormatFontType)formatFontNumber;

                        int numberNotFound1 = Convert.ToInt32(formatData[1]);

                        int formatLineNumber = Convert.ToInt32(formatData[2]);
                        FormatLineType formatLineType = (FormatLineType)formatLineNumber;

                        int numberNotFound2 = Convert.ToInt32(formatData[3]);
                        int numberNotFound3 = Convert.ToInt32(formatData[4]);

                        string url = formatData[5];

                        // Format line
                        FormatLineModel formatLine = _formatLines.FirstOrDefault(fl => fl.Type == formatLineType);

                        if (formatLine.Type == FormatLineType.Delete)
                        {
                            if (resultString.LastOrDefault() == _formatLine)
                            {
                                resultString.RemoveAt(resultString.Count - 1);
                            }
                        }
                        else
                        {
                            resultString.Add(formatLineEnd);

                            formatLineStart = formatLine.Start;
                            formatLineEnd = formatLine.End;

                            resultString.Add(formatLineStart);
                        }

                        // Format text
                        FormatFontModel formatText = _formatFonts.FirstOrDefault(ft => ft.Type == formatFontType);

                        // TODO Set default font because idn how create 17 font
                        if (formatFontNumber >= 17)
                        {
                            formatText = _formatFonts.FirstOrDefault();
                        }

                        resultString.Add(formatTextEnd);

                        formatTextStart = formatText.Start;
                        formatTextEnd = formatText.End;

                        resultString.Add(formatTextStart.Replace("UrlData", url));
                    }
                    else if (formatData.Count == 7)
                    {

                    }
                    else if (formatData.Count == 10)
                    {
                        resultString.Add("• ");
                        resultString.Add(_formatLine);
                    }
                    else
                    {
                        throw new Exception("Format type not found");
                    }
                }
                else if (data.Type == DataType.Image)
                {
                    string imageWidth = data.Property1 != null ? data.Property1.GetStringWithReplaced().Replace("=", "=\"") + "\"" : "";
                    string imageHeight = data.Property2 != null ? data.Property1.GetStringWithReplaced().Replace("=", "=\"") + "\"" : "";

                    string imageBase64 = Convert.ToBase64String(data.Data, 0, data.Data.Length);
                    resultString.Add("<img src=\"data:image;base64," + imageBase64 + "\" " + imageWidth + " " + imageHeight + " >");
                }
            }

            return string.Join("", resultString).TrimStart(_formatLine).TrimEnd(_formatLine);
        }

        /// <summary>
        ///     Get data by splitters
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="splitterBlock"></param>
        /// <param name="splitterLine"></param>
        /// <returns></returns>
        private List<DataModel> GetDataBySplitters(Separator separator, byte[] splitterBlock, byte[] splitterLine)
        {
            List<DataModel> dataModels = new List<DataModel>();
            byte[] data = Array.Empty<byte>();

            while (separator.HasValue)
            {
                byte[] temp = separator.Pop();
                string tempString = temp.GetString();

                if (temp.SequenceEqual(splitterLine))
                {
                    dataModels.Add(new DataModel()
                    {
                        Data = data,
                        Type = DataType.Text
                    });

                    data = Array.Empty<byte>();
                    continue;
                }
                else if (temp.SequenceEqual(splitterBlock))
                {
                    dataModels.Add(new DataModel()
                    {
                        Data = data,
                        Type = DataType.Format
                    });

                    data = Array.Empty<byte>();
                    continue;
                }
                else if (tempString.Contains("TJPEGImage") || tempString.Contains("TPngImage"))
                {
                    byte[] imageWidthProperty = Array.Empty<byte>();
                    byte[] imageHeightProperty = Array.Empty<byte>();

                    SkipSplitters(separator, splitterBlock, splitterLine);

                    if (separator.Peek().GetString().Contains("width"))
                    {
                        imageWidthProperty = separator.Pop();
                        SkipSplitters(separator, splitterBlock, splitterLine);
                    }

                    if (separator.Peek().GetString().Contains("height"))
                    {
                        imageHeightProperty = separator.Pop();
                        SkipSplitters(separator, splitterBlock, splitterLine);
                    }

                    int imageLength = separator.ReadInt32();
                    byte[] imageData = separator.ReadBytes(imageLength);

                    dataModels.Add(new DataModel()
                    {
                        Data = imageData,
                        Type = DataType.Image,

                        Property1 = imageWidthProperty,
                        Property2 = imageHeightProperty
                    });

                    data = Array.Empty<byte>();
                    continue;
                }

                data = temp;
            }

            return dataModels;
        }

        /// <summary>
        ///     Skip splitters
        /// </summary>
        /// <param name="separator"></param>
        /// <param name="splitterBlock"></param>
        /// <param name="splitterLine"></param>
        private void SkipSplitters(Separator separator, byte[] splitterBlock, byte[] splitterLine)
        {
            while (separator.HasValue)
            {
                byte[] temp = separator.Peek();

                if (!temp.SequenceEqual(splitterBlock) && !temp.SequenceEqual(splitterLine))
                {
                    break;
                }

                separator.Pop();
            }
        }

        /// <summary>
        ///     Split string by patterns
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private List<string> SplitStringByPatterns(string content)
        {
            List<string> results = new List<string>();

            for (int i = 0; i < _formatPatterns.Count; i++)
            {
                Match matches = Regex.Match(content, _formatPatterns[i]);

                while (matches.Success)
                {
                    for (int j = 0; j < matches.Groups.Count; j++)
                    {
                        if (j == 0)
                        {
                            continue;
                        }

                        results.Add(matches.Groups[j].Value);
                    }

                    matches = matches.NextMatch();
                }
            }

            return results;
        }
    }
}
