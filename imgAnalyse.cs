using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using static OpenCVSharpSandbox.HelperOperations;
using static System.Console;

namespace OpenCVSharpSandbox
{
    class ImgAnalyse
    {

        public enum Color : int { red, green, blue, unrecognized };

        public struct ResultFromPreviewVerification
        {
            public static Color PreviewColor;
            public static bool Verified;

            public ResultFromPreviewVerification(Color color, bool verified)
            {
                PreviewColor = color;
                Verified = verified;
            }

        }

        private static ResultFromPreviewVerification verifyPreview(Mat preview, Color desiredColor)
        {
            var previewCut = preview.GetRectSubPix(new Size((int)(preview.Cols * 8 / 10), (int)(preview.Rows * 8 / 10)),
                new Point2f(preview.Cols / 2, preview.Rows / 2));
            Cv2.Blur(previewCut, previewCut, new Size(15, 15));
            var pixelCountTotal = previewCut.Width * previewCut.Height;
            var averageColor = previewCut.Sum();
            var averageColorVec = new Vec3d(averageColor.Val0 / (double)pixelCountTotal,
                averageColor.Val1 / (double)pixelCountTotal, averageColor.Val2 / (double)pixelCountTotal);
            var evaluatedColor = EvaluateColorOfPreview(averageColorVec);
            var result = new ResultFromPreviewVerification(evaluatedColor, Equals(desiredColor, evaluatedColor));
            return result;

        }

        private static Color EvaluateColorOfPreview(Vec3d ColorOfPreview)
        {
            if ((ColorOfPreview[0] > 125) && (ColorOfPreview[1] < 125) && (ColorOfPreview[2] < 125))
            {
                return Color.red;
            }
            if ((ColorOfPreview[0] < 125) && (ColorOfPreview[1] > 125) && (ColorOfPreview[2] < 125))
            {
                return Color.green;
            }
            if ((ColorOfPreview[0] < 125) && (ColorOfPreview[1] < 125) && (ColorOfPreview[2] > 125))
            {
                return Color.blue;
            }
                return Color.unrecognized;
        }

        private void Monotonnost()
        {
            var watch = Stopwatch.StartNew();
            var img3 = Cv2.ImRead(@"c:\Users\labudova\Google Drive\diplomka\monotonost\SQ Scan.png", LoadMode.GrayScale);
            Scalar dev;
            Scalar me;
            Cv2.MeanStdDev(img3, out me, out dev);
            double odchylka = dev.Val0;
            if (odchylka > 33.0)
            {
                WriteLine("Obraz neni monotonni");
            }
            else
            {
                WriteLine("Obraz je monotonni");
            }
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            WriteLine("Elapsed time:");
            WriteLine(elapsedMs.ToString());
            ReadKey();
        }

        private static bool MonotonnostWithEdges(Mat img)
        {
            bool monotonnostBool;
            var edges = new Mat();
            Cv2.Canny(img, edges, 400, 450, 3, false);
            var sumaScalar = edges.Sum();
            var suma = sumaScalar[0];
            var prumer = MedianMat(img);
            var pocetMimoInt = 0;
            var pocetCelkem = img.Height * img.Width;
            for (var i = 0; i < img.Height; i++)
            {
                for (var j = 0; j < img.Width; j++)
                {
                    var value = img.Get<byte>(i, j);
                    if ((value < ((int)prumer - 30) | value > ((int)prumer + 30)))
                    {
                        pocetMimoInt += 1;
                    }
                }
            }
            var pomer = (float)pocetMimoInt / (float)(pocetCelkem);
            if ((pomer < 0.1) & (suma < 1100000))
            {
                monotonnostBool = true;
            }
            else
            {
                monotonnostBool = false;
            }
            return monotonnostBool;

        }

        private static void HoughtransformationLines(Mat img)
        {
            var edges = new Mat();
            Cv2.Canny(img, edges, 15, 4.5);
            var lines = Cv2.HoughLines(edges, 1, Math.PI / 180, 50, 50, 10);
            for (int i = 0; i < lines.Length; i++)
            {
                float rho = lines[i].Rho;
                float theta = lines[i].Theta;
                Point pt1, pt2;
                double a = Math.Cos(theta);
                double b = Math.Sin(rho);
                double x0 = a * rho, y0 = b * rho;
                pt1.X = (int)Math.Round(x0 + 1000.0 * (-b));
                pt1.Y = (int)Math.Round(y0 + 1000.0 * (a));
                pt2.X = (int)Math.Round(x0 + 1000.0 * (-b));
                pt2.Y = (int)Math.Round(y0 + 1000.0 * (a));
                Cv2.Line(edges, pt1, pt2, 200.0, 15);
            }
            Cv2.ImShow("Lines", edges);
            Cv2.WaitKey();
        }

        private static void HoughtransformationProbability(Mat img)
        {
            Mat edges = new Mat(img.Height, img.Width, 1);
            Cv2.Canny(img, edges, 15, 4.5);
            CvLineSegmentPoint[] lines = Cv2.HoughLinesP(edges, 1, Math.PI / 180, 1, 100, 3);
            for (int i = 0; i < lines.Length; i++)
            {
                CvLineSegmentPoint l = lines[i];
                Console.WriteLine(l.ToString());
                Cv2.Line(edges, l.P1, l.P2, new Scalar(150), 3);
            }
            Cv2.ImShow("detected lines", edges);
        }
        public static void IconFinder(Mat img, Mat refImg)
        {
            KeyPoint[] points;
            var detector = new FastFeatureDetector(15,false);
            points = detector.Detect(refImg);

            KeyPoint[] points2;
            var detector2 = new FastFeatureDetector(15,false);
            points2 = detector2.Detect(img);
            var brief = new BriefDescriptorExtractor(64);
            var descriptors = new Mat();
            brief.Compute(refImg, ref points, descriptors);

            var brief2 = new BriefDescriptorExtractor(64);
            var descriptors2 = new Mat();
            brief2.Compute(img, ref points2, descriptors2);

            var bfmatcher = new BFMatcher(NormType.Hamming);
            var matches = bfmatcher.Match(descriptors, descriptors2);

            var outImg = new Mat();
            Cv2.DrawMatches(refImg, points, img, points2, matches, outImg);
            var location = GetIconLocation(matches, points, points2);
        }

        private static Point2f GetIconLocation(DMatch[] matches, KeyPoint[] points1, KeyPoint[] points2) {
            var Validator = new MatchValidator[matches.Length];
            for (var i = 0; i < matches.Length; i++) {
                var point1 = points1[i].Pt;
                var point2 = points2[matches[i].TrainIdx].Pt;
                Validator[i] = new MatchValidator();
                Validator[i].pointImg1 = point1;
                Validator[i].pointImg2 = point2;
                Validator[i].GetShiftVector(point1, point2);
            }
            // stage 1:
            var averageVec = MatchValidator.GetAverageShiftVector(Validator);
            var averageSlope = averageVec.Item1 / averageVec.Item0;
            var correctSlope = Validator.Where(
                        x => ((x.ComputeSlope() < averageSlope + 0.02) && (x.ComputeSlope() > (averageSlope - 0.02))))
                        .Select(x => x)
                    .ToArray();
            var averageDistance = correctSlope.Select(x => x.GetLengthOfVector()).Average();
            var correctDistanceAndSlope =
                correctSlope.Where(x => (x.GetLengthOfVector() > (averageDistance - 10)) && (x.GetLengthOfVector() < (averageDistance + 10))).Select(x=>x).ToArray();
            var validatedPoints =
                correctDistanceAndSlope.Select(
                        x => new Point2f(correctDistanceAndSlope.Select(y => y.pointImg2.X).Average(), correctDistanceAndSlope.Select(z => z.pointImg2.Y).Average())).First();
            return validatedPoints;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="matches"></param>  Matches of reference and analyzed image keypoints
        /// <param name="points1"></param> keypoints found in analyzed image
        /// <param name="points2"></param> 
        /// <returns></returns>  Location of reference in analyzed image,  bool: true: location was found with sufficient probability, false: location was not found
        private static Tuple<Point, bool> SubmatrixLocation(DMatch[][] matches, KeyPoint[] points1, KeyPoint[] points2)
        {
            var Coordinates = new Point2f[matches.Length];
            var distances = new double[matches.Length];

            float sumx = 0;
            float sumy = 0;
            for (var i = 0; i < matches.Count(); i++)
            {
                var point = points2[matches[i][0].TrainIdx].Pt;
                sumx += point.X;
                sumy += point.Y;
                Coordinates[i] = new Point2f(point.X, point.Y);
            }
            var xLoc = sumx / matches.Length;
            var yLoc = sumy / matches.Length;
            var pointLoc = new Point(xLoc, yLoc);

            for (var i = 0; i < matches.Count(); i++)
            {
                for (var j = 0; j < matches.Count(); j++)
                {
                    if (i != j)
                    {
                        distances[i] += Coordinates[i].DistanceTo(Coordinates[j]);
                    }
                }
            }
            sumx = 0;
            sumy = 0;

            var finalPoints = new List<Point2f>();
            var averageDistance = distances.Average();

            for (var i = 0; i < distances.Length; i++)
            {
                if (distances[i] < averageDistance)
                {
                    sumx += Coordinates[i].X;
                    sumy += Coordinates[i].Y;
                    finalPoints.Add(Coordinates[i]);
                }
            }

            xLoc = sumx / finalPoints.Count;
            yLoc = sumy / finalPoints.Count;
            pointLoc = new Point(xLoc, yLoc);
            sumx = 0;
            sumy = 0;


            var finalPoints2 = new List<Point2f>();

            for (var i = 0; i < finalPoints.Count; i++)
            {
                if (finalPoints[i].DistanceTo(pointLoc) < 100)
                {
                    sumx += finalPoints[i].X;
                    sumy += finalPoints[i].Y;
                    finalPoints2.Add(finalPoints[i]);
                }
            }
            xLoc = sumx / finalPoints2.Count;
            yLoc = sumy / finalPoints2.Count;
            pointLoc = new Point(xLoc, yLoc);
            var count = 0;
            var keypointsAroundPointLoc1 = new List<Point>();
            var keypointsAroundPointLoc2 = new List<Point>();


            for (var j = 0; j < matches.Count(); j++)
            {
                var point2 = points2[matches[j][0].TrainIdx].Pt;
                var distance = point2.DistanceTo(pointLoc);
                //var distance = Coordinates[j].DistanceTo(pointLoc);
                if (distance < 50)
                {
                    var point1 = points1[matches[j][0].QueryIdx].Pt;
                    keypointsAroundPointLoc1.Add(point1);
                    keypointsAroundPointLoc2.Add(Coordinates[j]);
                    count += 1;

                }
            }

            var ratio = (float)count / (float)matches.Length;
            var xIndexes1 = new int[keypointsAroundPointLoc1.Count()];
            var yIndexes1 = new int[keypointsAroundPointLoc1.Count()];
            var xIndexes2 = new int[keypointsAroundPointLoc1.Count()];
            var yIndexes2 = new int[keypointsAroundPointLoc1.Count()];
            bool valid = true;
            if (ratio > 0.1)
            {

                var xAroundPoint1 = new int[keypointsAroundPointLoc1.Count()];
                var yAroundPoint1 = new int[keypointsAroundPointLoc1.Count()];
                var xAroundPoint2 = new int[keypointsAroundPointLoc1.Count()];
                var yAroundPoint2 = new int[keypointsAroundPointLoc1.Count()];

                for (var i = 0; i < keypointsAroundPointLoc1.Count(); i++)
                {
                    xAroundPoint1[i] = keypointsAroundPointLoc1[i].X;
                    yAroundPoint1[i] = keypointsAroundPointLoc1[i].Y;
                    xAroundPoint2[i] = keypointsAroundPointLoc2[i].X;
                    yAroundPoint2[i] = keypointsAroundPointLoc2[i].Y;
                    xIndexes1[i] = i;
                    yIndexes1[i] = i;
                    xIndexes2[i] = i;
                    yIndexes2[i] = i;
                }


                Array.Sort(xAroundPoint1, xIndexes1);
                Array.Sort(yAroundPoint1, yIndexes1);
                Array.Sort(xAroundPoint2, xIndexes2);
                Array.Sort(yAroundPoint2, yIndexes2);
                var xComparison = new float[(keypointsAroundPointLoc1.Count() / 5)];
                var yComparison = new float[(keypointsAroundPointLoc1.Count() / 5)];
                for (var i = 0; i < (keypointsAroundPointLoc1.Count() - (keypointsAroundPointLoc1.Count() % 5)); i += 5)
                {
                    xComparison[(i / 5)] = 0;
                    var tempForComparisonX = new int[5];
                    Array.Copy(xIndexes2, i, tempForComparisonX, 0, 5);
                    yComparison[(i / 5)] = 0;
                    var tempForComparisonY = new int[5];
                    Array.Copy(yIndexes2, i, tempForComparisonY, 0, 5);
                    for (var j = 0; j < 5; j++)
                    {
                        if (tempForComparisonX.Contains(xIndexes1[i + j])) xComparison[(i / 5)] += 1;
                        if (tempForComparisonY.Contains(yIndexes1[i + j])) yComparison[(i / 5)] += 1;
                    }
                }
                if (xComparison.Length < 3 && yComparison.Length < 3) valid = false;
                if ((xComparison.Average() < 2) && (yComparison.Average() < 2)) valid = false;
                Console.WriteLine("X comparison = " + xComparison.Average() + " ,length = " + xComparison.Length.ToString());
                Console.WriteLine("Y comparison = " + yComparison.Average() + " ,length = " + yComparison.Length.ToString());
            }
            else valid = false;

            Console.WriteLine("probability = " + ratio.ToString());
            if ((pointLoc == new Point(0, 0)) || (ratio < 0.2)) valid = false;



            Console.WriteLine("valid = " + valid.ToString());
            var outPut = new Tuple<Point, bool>(pointLoc, valid);
            return outPut;
        }
    }
}
