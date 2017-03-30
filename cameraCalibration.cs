using System;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    class CameraCalibration
    {
        internal static Point[] CornersOfDisplayWithAdaptiveThreshold(Mat img)
        {
            int[] threshold = { 45, 40, 50, 35, 30, 55, 25, 60, 20, 65, 15, 80, 10, 90, 5 };
            var validated = false;
            Point[] corners;
            var image = new Mat();
            var j = 0;

            do
            {
                img.CopyTo(image);
                j += 1;
                corners = CornersOfDisplay(image, threshold[j]);
                if (corners[0] != new Point(0, 0) & corners[1] != new Point(0, img.Height) &
                    corners[2] != new Point(img.Width, 0) & corners[3] != new Point(img.Width, img.Height))
                {
                    validated = DisplayCornersValidation(corners);
                }

            } while ((validated == false) & j < 13);
            return corners;
        }

        private static bool DisplayCornersValidation(Point[] corners)
        {
            var validated = true;
            var distance14 = corners[0].DistanceTo(corners[3]);
            var distance23 = corners[1].DistanceTo(corners[2]);
            var distance13 = corners[0].DistanceTo(corners[2]);
            var distance12 = corners[0].DistanceTo(corners[1]);
            var distance34 = corners[2].DistanceTo(corners[3]);
            var distance24 = corners[1].DistanceTo(corners[3]);
            var widthAver = (distance13 + distance24) / 2;
            var heightAver = (distance12 + distance34) / 2;
            var ratioAver = widthAver / heightAver;
            //var Area = widthAver*ratioAver;

            // TO DO: ziskani rozmeru displeje z databaze

            // konica c364
            //var width = 198;
            //var height = 112;

            // xerox 5335
            var width = 153;
            var height = 91;

            //xeroxCQ
            //var width = 177;
            //var height = 100;


            var dispRatio = (float)width / (float)height;

            if (Math.Abs(distance14 - distance23) > 15) validated = false;
            if (((ratioAver < (dispRatio - 0.1))) || (ratioAver > (dispRatio + 0.1))) validated = false;
            return validated;
        }

        private static Point[] CornersOfDisplay(Mat img, int threshold)
        {
            Cv2.CvtColor(img, img, ColorConversion.RgbToGray);
            Cv2.Normalize(img, img, 0, 255, NormType.MinMax);

            // Display edges calibration
            var center = new Point2f(img.Width / 2, img.Height / 2);

            Cv2.Threshold(img, img, threshold, 255, ThresholdType.Binary);
            Cv2.MedianBlur(img, img, 5);
            Cv2.FloodFill(img, center, new Scalar(120));

            var cx = img.Cols / 2;
            var cy = img.Rows / 2;

            var mat = new MatOfByte(img);
            var indexer = mat.GetIndexer();
            double[] minDistance = { 5000, 5000, 5000, 5000 };
            //var refPoint = new Point();
            Point[] refPoint = { new Point(0, 0), new Point(0, img.Height), new Point(img.Width, 0), new Point(img.Width, img.Height) };
            var corners = new Point[4];

            for (var y = 0; y < img.Height; y++)
            {
                for (var x = 0; x < img.Width; x++)
                {
                    var color = indexer[y, x];
                    if (color == 120)
                    {
                        if (x <= cx & y <= cy) //1.quadrant
                        {
                            //refPoint = new Point(0, 0);
                            var pixel = new Point2f(x, y);
                            var distance = pixel.DistanceTo(refPoint[0]);
                            if (distance < minDistance[0])
                            {
                                minDistance[0] = distance;
                                corners[0] = pixel;
                            }
                        }
                        else if (x <= cx & y > cy) //2.quadrant
                        {
                            //refPoint = new Point(0, img.Height);
                            var pixel = new Point2f(x, y);
                            var distance = pixel.DistanceTo(refPoint[1]);
                            if (distance < minDistance[1])
                            {
                                minDistance[1] = distance;
                                corners[1] = pixel;
                            }
                        }
                        else if (x > cx & y <= cy) //3.quadrant
                        {
                            //refPoint = new Point(img.Width, 0);
                            var pixel = new Point2f(x, y);
                            var distance = pixel.DistanceTo(refPoint[2]);
                            if (distance < minDistance[2])
                            {
                                minDistance[2] = distance;
                                corners[2] = pixel;
                            }
                        }
                        else if (x > cx & y > cy) //4.quadrant
                        {
                            //refPoint = new Point(img.Width, img.Height);
                            var pixel = new Point2f(x, y);
                            var distance = pixel.DistanceTo(refPoint[3]);
                            if (distance < minDistance[3])
                            {
                                minDistance[3] = distance;
                                corners[3] = pixel;
                            }
                        }
                    }
                    indexer[y, x] = color;
                }
            }
            return corners;
        }
        private void HledaniDisplayePomociContour()
        {
            var img2 = Cv2.ImRead(@"c:\Users\labudova\Google Drive\diplomka\fotky_mobil\2_small.jpg", LoadMode.GrayScale);
            var velikost = new Mat();
            img2.Resize(velikost.Size(), 7, 7);
            var edges = new Mat(img2.Height, img2.Width, 1);
            Cv2.Canny(img2, edges, 150, 150, 3);
            var body = Cv2.FindContoursAsMat(edges, ContourRetrieval.CComp, ContourChain.ApproxSimple);
            Cv2.WaitKey();
            Cv2.DestroyAllWindows();
        }
        private static void DontUseRedChannel(ref Mat matrix)
        {
            Mat[] planes;
            var matrixCopy = new Mat();
            matrix.CopyTo(matrixCopy);
            Cv2.CvtColor(matrix, matrix, ColorConversion.RgbToHsv);
            Cv2.Split(matrix, out planes);
            Mat[] imgOut =
            {
                new Mat(planes[0].Height, planes[0].Width, MatType.CV_8UC1),
                new Mat(planes[0].Height, planes[0].Width, MatType.CV_8UC1),
                new Mat(planes[0].Height, planes[0].Width, MatType.CV_8UC1),
            };

            Cv2.Normalize(planes[0], imgOut[0], 0, 255, NormType.MinMax); //zluta

            Cv2.Normalize(planes[1], imgOut[1], 0, 255, NormType.MinMax); //modra

            Cv2.Normalize(planes[2], imgOut[2], 0, 0, NormType.MinMax); //cervena
            var imgRed = new Mat();
            Cv2.Merge(imgOut, imgRed);
            Cv2.ImShow("after merge", imgRed);

            Cv2.CvtColor(imgRed, imgRed, ColorConversion.RgbToGray);
            Cv2.Threshold(imgRed, imgRed, 150, 255, ThresholdType.Binary);
            Cv2.ImShow("after threshold", imgRed);
            if ((int)imgRed.Sum() > 10000)
            {
                Cv2.CvtColor(matrixCopy, matrixCopy, ColorConversion.RgbToGray);
                Cv2.Add(matrixCopy, imgRed, matrix);
            }

            Cv2.ImShow("out", matrix);

        }

        private static void GetContoursOfDisplay(Mat img)
        {
            Cv2.Canny(img, img, 100, 200);
            Mat[] contours;
            Mat hierarchy = new Mat();
            Cv2.FindContours(img, out contours, hierarchy, ContourRetrieval.Tree, ContourChain.ApproxSimple);
            Mat[] contoursPoly = new Mat[contours.Length];
            Rect[] rectangle = new Rect[contours.Length];
            for (int i = 0; i < contours.Length; i++)
            {
                if (contours[i].Width > 50)
                {
                    contoursPoly[i] = new Mat();
                    Cv2.ApproxPolyDP(contours[i], contoursPoly[i], 3, true);
                    rectangle[i] = Cv2.BoundingRect(contoursPoly[i]);
                }

            }
            var drawing = new Mat();
            drawing = Mat.Zeros(img.Size(), MatType.CV_8UC3);
            for (int i = 0; i < contours.Length; i++)
            {
                if (contours[i].Height > 200)
                {
                    Cv2.DrawContours(drawing, contours, i, new Scalar(255, i, 0));
                    Cv2.Rectangle(drawing, rectangle[i].TopLeft, rectangle[i].BottomRight, new Scalar(255, 255, 255));
                }
            }
            //var randomNumber = new Random();
            //Cv2.ImWrite(@"C:\Users\labudova\Documents\diplomka\vysledky_analyz\27_1_2017\1" + randomNumber.Next() + ".png", drawing);
            Cv2.ImShow("out", drawing);
            Cv2.WaitKey();
        }

        

    }
}
