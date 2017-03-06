using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;


namespace OpenCVSharpSandbox
{
    public struct DescriptorsAndKeypoints
    {
        public KeyPoint[] Points;
        public Mat Descriptors;
    }

    public struct ResultFromMatching
    {
        public double? ValidRatio;
        public double DistanceOfMatchedDescriptors;
        public double VaseksCoeficient;
        public DMatch[][] Matches;
    }
    
    class Descriptoring
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        internal static void briefTwoImgs(Mat img1, Mat img2)
        {
            var result = ComputeBriefWithFast(img1, 15, true, 1);
            var result2 = ComputeBriefWithFast(img2, 15, true, 1);
            var koeficients = MatchAndValidate(result.Descriptors, result2.Descriptors, result.Points, result2.Points);
            var outImg = new Mat();
            Cv2.DrawMatches(img1, result.Points, img2, result2.Points, koeficients.Matches, outImg);
            var randomNumber = new Random();
            Cv2.ImWrite(@"c:\Users\labudova\Documents\diplomka\vysledky_analyz\19_1_2017\1" + randomNumber.Next(0, 1000000) + ".png", outImg);
            Cv2.ImShow("vysledek", outImg);
            Cv2.WaitKey();

        }

        public static ResultFromMatching ORBtwoImgs(Mat img1, Mat img2, int nFeatures, float scaleFactor, int pLevels, int edgeThresh, int firstLevel, int wTak, ORBScore scoretype, bool draw = false)
        {
            var descriptors = ComputeOrb(img1, nFeatures, scaleFactor, pLevels, edgeThresh, firstLevel, wTak, scoretype);
            var descriptors2 = ComputeOrb(img2, nFeatures, scaleFactor, pLevels, edgeThresh, firstLevel, wTak, scoretype);
            var result = MatchAndValidate(descriptors.Descriptors, descriptors2.Descriptors, descriptors.Points,
                    descriptors2.Points);
            //Logger.Info(result.VaseksCoeficient);
            if (draw == true)
            {
                var outImg = new Mat();
                var outImg2 = new Mat();
                Cv2.DrawMatches(img1, descriptors.Points, img2, descriptors2.Points, result.Matches, outImg);
                Cv2.DrawKeypoints(img2,descriptors2.Points, outImg2);
                var date = System.DateTime.Now;
                var dir = "C:\\Users\\labudova\\Documents\\diplomka\\vysledky_analyz\\" + date.Date.ToString("d-M-yyyy");
                System.IO.Directory.CreateDirectory(dir);
                Cv2.ImWrite(dir + "\\ORB_matches_" + nFeatures + "_" + scaleFactor +"_"+ pLevels +"_"+ edgeThresh +"_"+ firstLevel +"_"+ wTak +"_"+ scoretype + "_" +
                    date.Hour + "_" + date.Minute + "_" + date.Second + ".png", outImg);
                Cv2.ImWrite(dir + "\\ORB_keypoints_" + nFeatures + "_" + scaleFactor + "_" + pLevels + "_" + edgeThresh + "_" + firstLevel + "_" + wTak + "_" + scoretype + "_" +
                    date.Hour + "_" + date.Minute + "_" + date.Second + ".png", outImg2);
            }

            //Cv2.ImShow("vysledek", outImg);
            //Cv2.WaitKey();
            return result;
        }
            void BriskTwoImgs(Mat img1, Mat img2)
            {
                var result1 = ComputeBrisk(img1);
                var result2 = ComputeBrisk(img2);
                var matches = MatchAndValidate(result1.Descriptors, result2.Descriptors, result1.Points, result1.Points);
                var outImg = new Mat();
                Cv2.DrawMatches(img1, result1.Points, img2, result2.Points, matches.Matches, outImg);
                var randomNumber = new Random();
                Cv2.ImWrite(@"c:\Users\labudova\Documents\diplomka\vysledky_analyz\20_1_2017\BRISK" + randomNumber.Next(0, 1000000) + ".png", outImg);               
                Cv2.ImShow("vysledek", outImg);
                Cv2.WaitKey();
            }

        internal static double VasekValidator(DMatch[][] matches)
        {
            var c1 = (double)matches.Where(t => t.Length == 2 && t[0].Distance < 0.75 * t[1].Distance)
                        .Select(t => t[0])
                        .ToList()
                        .Count;
            return c1 / matches.Length;
        }


        internal static double MatchValidator(DMatch[][] matches, KeyPoint[] points1, KeyPoint[] points2)
        {
            var valid = 0;
            var invalid = 0;
            for (var i = 0; i < matches.Length; i++)
            {
                var point1 = points1[i].Pt;
                var point2 = points2[matches[i][0].TrainIdx].Pt;
                var distOfMatchedPoints = point1.DistanceTo(point2);
                if (distOfMatchedPoints < 10)
                {
                    valid += 1;
                }
                else
                {
                    invalid += 1;
                }
            }
            return (double)valid / ((double)valid + (double)invalid);
        }
        public static double AverageDistanceOfMatchedDescriptors(DMatch[][] matches)
        {
            float distanceTotal = 0;
            for (var i = 0; i < matches.Length; i++)
            {
                distanceTotal += matches[i][0].Distance;
            }
            var average = distanceTotal / matches.Length;
            return average;
        }

        internal static List<int> DistanceOfDescriptors(Mat descriptors)
        {
            List<int> distancesList = new List<int>(64);
            for (var j = 0; j < (descriptors.Width * 8); j++)
            {
                distancesList.Add(0);
            }
            for (var i = 0; i < descriptors.Rows - 1; i++)
            {
                for (var j = i + 1; j < descriptors.Rows; j++)
                {

                    double distance = 0;
                    var mat1 = descriptors.SubMat(new Range(i, i + 1), new Range(0, descriptors.Width));
                    var mat2 = descriptors.SubMat(new Range(j, j + 1), new Range(0, descriptors.Width));

                    distance = Cv2.Norm(mat1, mat2, NormType.Hamming);
                    distancesList[(int)distance] += 1;
                }
            }

            return distancesList;
        }

        internal static ResultFromMatching MatchAndValidate(Mat descriptors1, Mat descriptors2, KeyPoint[] points, KeyPoint[] points2)
        {
            var bfmatcher = new BFMatcher(NormType.Hamming);
            var matches = bfmatcher.KnnMatch(descriptors1, descriptors2, 2);
            var validRatio = MatchValidator(matches, points, points2);
            var distanceOfMatchedDescriptors = AverageDistanceOfMatchedDescriptors(matches);
            var vasekValidace = VasekValidator(matches);
            return new ResultFromMatching {DistanceOfMatchedDescriptors = distanceOfMatchedDescriptors,
                ValidRatio = validRatio, Matches = matches, VaseksCoeficient = vasekValidace};
        }

        private static DescriptorsAndKeypoints ComputeBriefWithFast(Mat img, int fastThreshold, bool nonMaxSupression, int levelImgPyr)
        {       
            Cv2.CvtColor(img, img, ColorConversion.RgbToGray);
            img = HelperOperations.ReturnImgFromNextLevPyr(img, levelImgPyr, 0.5f);
            KeyPoint[] points;
            Cv2.FAST(img, out points, fastThreshold, nonMaxSupression);
            var brief = new BriefDescriptorExtractor(64);
            var descriptors = new Mat();
            brief.Compute(img, ref points, descriptors);
            img.Dispose();
            brief.Dispose();
            return new DescriptorsAndKeypoints {Descriptors = descriptors, Points = points};
        }

        internal static DescriptorsAndKeypoints ComputeOrb(Mat img, int nFeatures, float scaleFactor, int pLevels, int edgeThresh, int firstLevel, int wTak, ORBScore scoretype )
        {
            var orb = new ORB(nFeatures, scaleFactor, pLevels, edgeThresh, firstLevel, wTak, scoretype);
            var descriptors = new Mat();
            var points = orb.Detect(img);
            orb.Compute(img, ref points, descriptors);
            return new DescriptorsAndKeypoints { Descriptors = descriptors, Points = points };
        }

        internal static DescriptorsAndKeypoints ComputeBrisk(Mat img)
        {
            var brisk = new BRISK();
            var descriptors = new Mat();
            KeyPoint[] points;
            points = brisk.Detect(img);
            brisk.Compute(img, ref points, descriptors);
            return new DescriptorsAndKeypoints { Descriptors = descriptors, Points = points };
        }

        public static double AverageDistanceOfMatchedDescriptors2(DMatch[] matches)
        {
            float distanceTotal = 0;
            for (var i = 0; i < matches.Length; i++)
            {
                distanceTotal += matches[i].Distance;
            }
            var average = distanceTotal / matches.Length;
            return average;
        }

    }
}
