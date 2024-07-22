using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.Eventing.Reader;
using System.Drawing.Imaging;
using System.IO;
using Encoder = System.Drawing.Imaging.Encoder;
using System.ComponentModel;

namespace EbookFormatter
{
    internal class EbookFormatEngine
    {
        // static variables
        int BackgroundThresholdRgbValue = 5;
        int BackgroundSampleLine = 0;
        int JpegQualityFactor = 100;
        int LeftAndRight = 2;
        string OutputFoldername = "output";

        public string[] arrayInputImageFilenames { set; get; }
        int medianLeftBoarder;
        int medianRightBoarder;
        public StringBuilder logger { set;  get; }

        enum Directions
        {
            Left,
            Right
        };

        public EbookFormatEngine()
        {
            logger = new StringBuilder();
            logger.AppendLine($"Ebook Formatter is initalized");
        }

        public void GetBoarders()
        {
            int[] medianBoarders = new int[LeftAndRight];
            foreach (Directions direction in Enum.GetValues(typeof(Directions)))
            {
                medianBoarders[(int)direction] = GetBoarder(direction);
            }
            medianLeftBoarder = medianBoarders[(int)Directions.Left];
            logger.AppendLine($"Left boarder index: {medianLeftBoarder}");
            medianRightBoarder = medianBoarders[(int)Directions.Right];
            logger.AppendLine($"Right boarder index: {medianRightBoarder}");
        }

        public void SliceEbook(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            string outputFolderPath = CreateOutputDirectory(OutputFoldername);

            int masterPageIndex = 0;
            int masterPageMax = arrayInputImageFilenames.Length * 2;

            foreach (var sourceFilename in arrayInputImageFilenames)
            {
                logger.AppendLine($"{sourceFilename}: master page index {masterPageIndex}");

                Bitmap sourceBitmap = new Bitmap(sourceFilename);
                foreach (Directions direction in Enum.GetValues(typeof(Directions)))
                {
                    SliceAndSavePage(direction, outputFolderPath, masterPageIndex, sourceBitmap);
                }
                masterPageIndex += 2;
                sourceBitmap.Dispose();

                // report progress
                worker.ReportProgress(masterPageIndex * 100 / masterPageMax);
            }
        }

        private int GetBoarder(Directions direction)
        {
            List<int> listBoarders = new List<int>();
            foreach (var imgFilename in arrayInputImageFilenames)
            {
                Bitmap bmpFromFile = new Bitmap(imgFilename);
                int bmpWidth = bmpFromFile.Width;
                Color background;
                int positionPixel;
                int incrementPixel;
                int deltaColor;

                if (direction == Directions.Left)
                {
                    background = bmpFromFile.GetPixel(0, BackgroundSampleLine);
                    positionPixel = 1;
                    incrementPixel = 1;
                }
                else
                {
                    background = bmpFromFile.GetPixel(bmpWidth - 1, BackgroundSampleLine);
                    positionPixel = (bmpWidth - 1) - 1;
                    incrementPixel = -1; 
                }

                Color current;
                int boarderSearchEndpoint = bmpWidth / 2;

                // find a pixel position which has larger piexl difference from background than threshold
                while (positionPixel != boarderSearchEndpoint)
                {
                    current = bmpFromFile.GetPixel(positionPixel, BackgroundSampleLine);
                    deltaColor = Math.Abs(background.R - current.R) + Math.Abs(background.G - current.G) + Math.Abs(background.B - current.B);
                    if (deltaColor > BackgroundThresholdRgbValue)
                    {
                        // once the position is found, add the position to the list
                        listBoarders.Add(positionPixel);
                        logger.AppendLine($"{imgFilename}: {direction}, boarder index is {positionPixel.ToString()}");
                        break;
                    }
                    else
                    {
                        positionPixel += incrementPixel;
                    }
                }
                if (positionPixel == boarderSearchEndpoint)
                {
                    logger.AppendLine($"Wanning: {imgFilename}: {direction}, boarder is not iedntified, boarder is set to search endpoint");
                }
                bmpFromFile.Dispose();
            }
            
            // calculate the median of the pixel location where the difference from background more than theshold is found 
            int medianBoarder = GetMedian(listBoarders);
            return GetMedian(listBoarders);
        }

        private int GetMedian(List<int> listBoarder)
        {
            listBoarder.Sort();
            if (listBoarder.Count % 2 == 0)
            {
                return listBoarder[listBoarder.Count / 2];
            }
            else
            {
                return (listBoarder[listBoarder.Count / 2] + listBoarder[listBoarder.Count / 2 + 1]) / 2;
            }
        }

        private void SliceAndSavePage(Directions direction, string outputFolderPath, int masterPageIndex, Bitmap sourceBitmap)
        {
            // setting image codec as JPEG
            ImageCodecInfo imgcdcinfJPEG = GetJpegCodecInfo();
            var encparamsJPG = GetJpegEncodingParams(JpegQualityFactor);

            Rectangle rectanglePage;
            string fullpathPage;
            int widthPageLeft = GetPageWidth(Directions.Left);
            int widthPageRight = GetPageWidth(Directions.Right);
            int heightBitmap = sourceBitmap.Height;

            if (direction == Directions.Left)
            {
                rectanglePage = new Rectangle(medianLeftBoarder, 0, widthPageLeft, heightBitmap);
                fullpathPage = Path.Combine(outputFolderPath, $"{masterPageIndex + 1:0000}.jpg");
            }
            else
            {
                rectanglePage = new Rectangle(medianLeftBoarder + widthPageLeft, 0, widthPageRight, heightBitmap);
                fullpathPage = Path.Combine(outputFolderPath, $"{masterPageIndex:0000}.jpg");
            }
            logger.AppendLine($"{direction.ToString()}: {fullpathPage} created, width: {rectanglePage.Width}, height: {rectanglePage.Height}");
            if (rectanglePage.Width == 0)
            {
                logger.AppendLine($"Warning: page width is 0");
            }
            // create the new bitmap, trimming each page by rect and save as jpg
            Bitmap bitmapPage = sourceBitmap.Clone(rectanglePage, sourceBitmap.PixelFormat);
            bitmapPage.Save(fullpathPage, imgcdcinfJPEG, encparamsJPG);

            bitmapPage.Dispose();
        }

        private string CreateOutputDirectory(string outputFoldername)
        {
            string currentFolderPath = Path.GetDirectoryName(arrayInputImageFilenames[0]);
            string outputFolderPath = Path.Combine(currentFolderPath, outputFoldername);
            Directory.CreateDirectory(outputFolderPath);
            return outputFolderPath;
        }

        private int GetPageWidth(Directions direction)
        {
            int widthPage;

            // simply half width between left and right borders
            widthPage = (medianRightBoarder - medianLeftBoarder) / 2;

            // if valid image width is odd, make left page 1 dot larger
            if (((medianRightBoarder + medianLeftBoarder) % 2 == 1) && (direction == Directions.Left))
            {
                widthPage = (medianRightBoarder - medianLeftBoarder) / 2 + 1;
            }

            return widthPage;
        }

        private EncoderParameters GetJpegEncodingParams(int qualityParameterJPEG)
        {
            var encparamJPG = new EncoderParameter(Encoder.Quality, qualityParameterJPEG);
            var encparamsJPG = new EncoderParameters(1);
            encparamsJPG.Param[0] = encparamJPG;
            return encparamsJPG;
        }

        private ImageCodecInfo GetJpegCodecInfo()
        {
            // it does not assume the case JPEG coder is not present - if not, that's gonna be Windows OS wide issue
            ImageCodecInfo imgcdcinfJPEG = null;
            foreach (var imgcdcinfEach in ImageCodecInfo.GetImageEncoders())
            {
                if (imgcdcinfEach.FormatID == ImageFormat.Jpeg.Guid)

                    imgcdcinfJPEG = imgcdcinfEach;
            }
            return imgcdcinfJPEG;
        }

    }
}
