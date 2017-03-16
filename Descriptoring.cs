using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;


namespace OpenCVSharpSandbox
{
    class Descriptoring
    {
        public static OrbParameters orbParameters = new OrbParameters(1200, 1.2f, 8, 50, 1, 2, ORBScore.Fast);

        //public static readonly ORB orb = new ORB(1200, 1.2f, 8, 50, 1, 2, ORBScore.Fast);
        private static int FastThreshold = 35;
        private static int levelPyr = 1;
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private static float thresholdForDistance = 0.9f;

        public struct DescriptorsAndKeypoints
        {
            public KeyPoint[] Points;
            public Mat Descriptors;
        }

        public struct ResultFromMatching
        {
            public double? ValidRatio;
            public double DistanceOfMatchedDescriptorsKnn;
            public double DistanceOfMatchedDescriptors;
            public double VaseksCoeficient;
            public double RatioDist;
            public DMatch[][] MatchesKnn;
            public DMatch[] Matches;
        }

        public enum Methods
        {
            ORB,
            BRIEF,
            BRISK
        }

        internal static void briefTwoImgs(Mat img1, Mat img2)
        {
            var result = ComputeBriefWithFast(img1, 15, 1);
            var result2 = ComputeBriefWithFast(img2, 15, 1);
            var koeficients = MatchAndValidate(result.Descriptors, result2.Descriptors, result.Points, result2.Points);
            var outImg = new Mat();
            Cv2.DrawMatches(img1, result.Points, img2, result2.Points, koeficients.Matches, outImg);
            var randomNumber = new Random();
            Cv2.ImWrite(
                @"c:\Users\labudova\Documents\diplomka\vysledky_analyz\19_1_2017\1" + randomNumber.Next(0, 1000000) +
                ".png", outImg);
            Cv2.ImShow("vysledek", outImg);
            Cv2.WaitKey();
        }

        public ResultFromMatching ORBtwoImgs(Mat img1, Mat img2, bool draw = false)
        {
            var descriptors = ComputeOrb(img1, orbParameters.Create());
            var descriptors2 = ComputeOrb(img2, orbParameters.Create());
            var result = MatchAndValidate(descriptors.Descriptors, descriptors2.Descriptors, descriptors.Points,
                descriptors2.Points);
            //Logger.Info(result.VaseksCoeficient);

            if (draw == true)
            {
                var outImg = new Mat();
                var outImg2 = new Mat();
                Cv2.DrawMatches(img1, descriptors.Points, img2, descriptors2.Points, result.Matches, outImg);
                Cv2.DrawKeypoints(img2, descriptors2.Points, outImg2);
                var date = System.DateTime.Now;
                var dir = "C:\\Users\\labudova\\Documents\\diplomka\\vysledky_analyz\\" + date.Date.ToString("d-M-yyyy");
                System.IO.Directory.CreateDirectory(dir);
                Cv2.ImWrite(dir + "\\ORB_matches_" + orbParameters.ToString() + "_" +
                            date.Hour + "_" + date.Minute + "_" + date.Second + ".png", outImg);
                Cv2.ImWrite(dir + "\\ORB_keypoints_" + orbParameters.Create() + "_" +
                            date.Hour + "_" + date.Minute + "_" + date.Second + ".png", outImg2);
            }

            //Cv2.ImShow("vysledek", outImg);
            //Cv2.WaitKey();
            return result;
        }

        public ResultFromMatching ORBtwoImgs(Mat img1, Mat img2, OrbParameters orbPar)
        {
            var descriptors = ComputeOrb(img1, orbPar.Create());
            var descriptors2 = ComputeOrb(img2, orbPar.Create());
            var result = MatchAndValidate(descriptors.Descriptors, descriptors2.Descriptors, descriptors.Points,
                descriptors2.Points);
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
            Cv2.ImWrite(
                @"c:\Users\labudova\Documents\diplomka\vysledky_analyz\20_1_2017\BRISK" + randomNumber.Next(0, 1000000) +
                ".png", outImg);
            Cv2.ImShow("vysledek", outImg);
            Cv2.WaitKey();
        }

        internal static double VasekValidator(DMatch[][] matches)
        {
            var c1 = (double) matches.Where(t => t.Length == 2 && t[0].Distance < 0.75*t[1].Distance)
                .Select(t => t[0])
                .ToList()
                .Count;
            return c1/matches.Length;
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
            return (double) valid/((double) valid + (double) invalid);
        }
        internal static double MatchValidator(DMatch[] matches, KeyPoint[] points1, KeyPoint[] points2)
        {
            var valid = 0;
            var invalid = 0;
            for (var i = 0; i < matches.Length; i++)
            {
                var point1 = points1[i].Pt;
                var point2 = points2[matches[i].TrainIdx].Pt;
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
            var average = distanceTotal/matches.Length;
            return average;
        }

        public static double AverageDistanceOfMatchedDescriptors(DMatch[] matches)
        {
            float distanceTotal = 0;
            for (var i = 0; i < matches.Length; i++)
            {
                distanceTotal += matches[i].Distance;
            }
            var average = distanceTotal/matches.Length;
            return average;
        }

        internal static List<int> DistanceOfDescriptors(Mat descriptors)
        {
            List<int> distancesList = new List<int>(64);
            for (var j = 0; j < (descriptors.Width*8); j++)
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
                    distancesList[(int) distance] += 1;
                }
            }

            return distancesList;
        }

        internal static double RatioOfDescriptorsWithSmallDistance(DMatch[] matches)
        {
            //var maximum = matches.Select(x => x.Distance).Average();
            var matchesWithSmallDist = matches.Where(x => x.Distance < 50).Select(x => x).ToList().Count;
            return matchesWithSmallDist/(double)matches.Length;
        }

        internal static ResultFromMatching MatchAndValidate(Images img1, Images img2, bool draw = false)
        {
            var bfmatcher = new BFMatcher(NormType.Hamming);
            var matchesKnn = bfmatcher.KnnMatch(img1.Descriptors, img2.Descriptors, 2);
            var bfmatcher2 = new BFMatcher(NormType.Hamming,true);
            var matches = bfmatcher2.Match(img1.Descriptors, img2.Descriptors);
            var validRatio = MatchValidator(matchesKnn, img1.Points, img2.Points);
            var distanceOfMatchedDescriptors = AverageDistanceOfMatchedDescriptors(matchesKnn);
            var distanceOfMatchedDescriptors2 = AverageDistanceOfMatchedDescriptors(matches);
            var ratioDist = 0d;  
            if (matches.Length > 0)
            {
                ratioDist = RatioOfDescriptorsWithSmallDistance(matches);
            }
            else
            {
                Logger.Error("No matches");
            }
            
            var vasekValidace = VasekValidator(matchesKnn);
            if (draw && (vasekValidace < ImageEvaluation.TresholdForValidationKoef))
            {
                var image1 = Cv2.ImRead(img1.path);
                var image2 = Cv2.ImRead(img2.path);
                var outImg = new Mat();
                Cv2.DrawMatches(image1, img1.Points, image2, img2.Points, matchesKnn, outImg);
                var date = System.DateTime.Now;
                var dir = Images.WriteFolder + "\\" + date.Date.ToString("dd-MM-yyyy");
                Directory.CreateDirectory(dir);
                var filePath = Path.Combine(dir, $"{img1.Device}_{img1.ScreenId}_{img2.ScreenId}_{date.TimeOfDay:hh'-'mm'-'ss}.png");
                Cv2.ImWrite(filePath, outImg);
                if (vasekValidace < ImageEvaluation.TresholdForValidationKoef)
                {
                    Logger.Info(
                        $"Saving image from matching, koef: {ImageEvaluation.TresholdForValidationKoef:F}, Path: {filePath}");
                }
            }
            return new ResultFromMatching
            {
                DistanceOfMatchedDescriptorsKnn = distanceOfMatchedDescriptors,
                DistanceOfMatchedDescriptors = distanceOfMatchedDescriptors2,
                ValidRatio = validRatio,
                VaseksCoeficient = vasekValidace,
                RatioDist = ratioDist
            };
        }

        internal static ResultFromMatching MatchAndValidate(Mat Descriptor1, Mat Descriptor2, KeyPoint[] Points1,
            KeyPoint[] Points2)
        {
            var bfmatcher = new BFMatcher(NormType.Hamming,true);
            var matches = bfmatcher.KnnMatch(Descriptor1, Descriptor2, 2);
            var validRatio = MatchValidator(matches, Points1, Points2);
            var distanceOfMatchedDescriptors = AverageDistanceOfMatchedDescriptors(matches);
            var vasekValidace = VasekValidator(matches);
            return new ResultFromMatching
            {
                DistanceOfMatchedDescriptors = distanceOfMatchedDescriptors,
                ValidRatio = validRatio,
                MatchesKnn = matches,
                VaseksCoeficient = vasekValidace
            };
        }

        internal static DescriptorsAndKeypoints ComputeBriefWithFast(Mat img, int fastThreshold, int levelImgPyr)
        {
            Cv2.CvtColor(img, img, ColorConversion.RgbToGray);
            img = HelperOperations.ReturnImgFromNextLevPyr(img, levelImgPyr, 0.5f);
            KeyPoint[] points;
            Cv2.FAST(img, out points, fastThreshold, true);
            var brief = new BriefDescriptorExtractor(64);
            var descriptors = new Mat();
            brief.Compute(img, ref points, descriptors);
            img.Dispose();
            brief.Dispose();
            return new DescriptorsAndKeypoints {Descriptors = descriptors, Points = points};
        }

        internal static DescriptorsAndKeypoints ComputeOrb(Mat img, ORB orb)
        {
            //var orb = orbParameters.Create();
            var descriptors = new Mat();
            var points = orb.Detect(img);
            orb.Compute(img, ref points, descriptors);
            return new DescriptorsAndKeypoints {Descriptors = descriptors, Points = points};
        }

        internal static DescriptorsAndKeypoints ComputeBrisk(Mat img)
        {
            var brisk = new BRISK();
            var descriptors = new Mat();
            KeyPoint[] points;
            points = brisk.Detect(img);
            brisk.Compute(img, ref points, descriptors);
            return new DescriptorsAndKeypoints {Descriptors = descriptors, Points = points};
        }


        public DescriptorsAndKeypoints ComputeDescriptorsAndKeypoints(Methods method, Mat img)
        {
            var result = new DescriptorsAndKeypoints();
            if (method == Methods.ORB)
            {
                result = ComputeOrb(img, orbParameters.Create());
            }
            else if (method == Methods.BRIEF)
            {
                result = ComputeBriefWithFast(img, FastThreshold, levelPyr);
            }
            else if (method == Methods.BRISK)
            {
                result = ComputeBrisk(img);
            }
            else
            {
                throw new NotImplementedException();
            }
            return (result);
        }

        public static void DrawMatchesImages(Images img1, Images img2, DMatch[] matchesKnn)
        {
                var image1 = Cv2.ImRead(img1.path);
                var image2 = Cv2.ImRead(img2.path);
                var outImg = new Mat();
                Cv2.DrawMatches(image1, img1.Points, image2, img2.Points, matchesKnn, outImg);
                var date = System.DateTime.Now;
                var dir = Images.WriteFolder + "\\" + date.Date.ToString("dd-MM-yyyy");
                Directory.CreateDirectory(dir);
                var filePath = Path.Combine(dir, $"{img1.Device}_{img1.ScreenId}_{img2.ScreenId}_{date.TimeOfDay:hh'-'mm'-'ss}.png");
                Cv2.ImWrite(filePath, outImg);
        }
        public static void DrawMatchesImages(Images img1, Images img2, DMatch[][] matchesKnn)
        {
            var image1 = Cv2.ImRead(img1.path);
            var image2 = Cv2.ImRead(img2.path);
            var outImg = new Mat();
            var newMatches = new DMatch[matchesKnn.Length];
            newMatches = matchesKnn.Select(x => x[0]).ToArray();
            Cv2.DrawMatches(image1, img1.Points, image2, img2.Points, newMatches, outImg);
            var date = System.DateTime.Now;
            var dir = Images.WriteFolder + "\\" + date.Date.ToString("dd-MM-yyyy");
            Directory.CreateDirectory(dir);
            var filePath = Path.Combine(dir, $"{img1.Device}_{img1.ScreenId}_{img2.ScreenId}_{date.TimeOfDay:hh'-'mm'-'ss}.png");
            Cv2.ImWrite(filePath, outImg);
        }
    }
}
