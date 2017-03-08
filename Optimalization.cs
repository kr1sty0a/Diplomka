using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using log4net.Repository.Hierarchy;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    class Optimalization
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        public void OrbOptimalization()
        {
            //var img1 = Cv2.ImRead(allImgs[42], LoadMode.Color);

            ////var img2 = Cv2.ImRead(allImgs[1], LoadMode.Color);
            ////var koeficient = Descriptoring.ORBtwoImgs(img1, img2, 1000, 1.2F, 8, 31, 0, 2, ORBScore.Harris, true);

            //var orbScore = new ORBScore[] { ORBScore.Harris, ORBScore.Fast };
            ////var numOfIterations = 0;
            //var minKoef = 1d;
            //var maxKoef = 0d;
            //var biggestDiff = 0d;
            ////for (var i = 1; i < 3; i++)
            ////{
            ////for (var i = 1; i < allImgs.Count; i++)
            ////{

            ////Logger.Debug(allImgs[i]);
            //int[] sameKonica = new[] { 43, 44, 45, 46, 47, 48, 49, 50, 51 };
            //for (var j = 1000; j < 2001; j += 200)
            //{
            //    for (var k = 0.8f; k < 1.6f; k += 0.1f)
            //    {
            //        for (var l = 1; l < 10; l += 1)
            //        {
            //            for (var m = 40; m < 71; m += 5)
            //            {
            //                var n = 1;
            //                var o = 2;
            //                foreach (var p in orbScore)
            //                {
            //                    var discard = false;
            //                    foreach (int i in sameKonica)
            //                    {
            //                        var img2 = Cv2.ImRead(allImgs[i], LoadMode.Color);
            //                        var koeficient = Descriptoring.ORBtwoImgs(img1, img2, j, k, l, m, n, o, p);
            //                        if (koeficient.ValidRatio < 0.5)
            //                        {
            //                            discard = true;
            //                        }

            //                        if ((koeficient.VaseksCoeficient < minKoef) && (koeficient.ValidRatio > 0.5))
            //                        {
            //                            var ParametersSameMin = new Parameters(j, k, l, m, n, o, p);
            //                            minKoef = koeficient.VaseksCoeficient;

            //                        }
            //                    }
            //                    for (var i = 0; i < allImgs.Count; i++)
            //                    {
            //                        if (!(sameKonica.Contains(i) || i == 42))
            //                        {
            //                            var img2 = Cv2.ImRead(allImgs[i], LoadMode.Color);
            //                            ResultFromMatching koeficient;
            //                            if (i == 0)
            //                            {
            //                                koeficient = Descriptoring.ORBtwoImgs(img1, img2, j, k, l, m, n, o, p, true);
            //                            }
            //                            else
            //                            {
            //                                koeficient = Descriptoring.ORBtwoImgs(img1, img2, j, k, l, m, n, o, p);
            //                            }

            //                            if (koeficient.VaseksCoeficient > maxKoef)
            //                            {
            //                                maxKoef = koeficient.VaseksCoeficient;
            //                                var ParametersMaxDiff = new Parameters(j, k, l, m, n, o, p);
            //                            }
            //                        }
            //                    }

            //                    var Diff = minKoef - maxKoef;
            //                    if ((Diff > 0.72) && (discard == false) && Diff != 1)
            //                    {
            //                        var ParametersBiggestDiff = new Parameters(j, k, l, m, n, o, p);
            //                        biggestDiff = Diff;
            //                        Logger.Error("New maximum difference: " + Diff + ", Parameters: " + j + ", " +
            //                                     k + ", " + l + ", " + m + ", " + n + ", " + o +
            //                                     ", " + p);

            //                    }
            //                    minKoef = 1;
            //                    maxKoef = 0;
            //                }
            //            }
            //        }
            //    }
            //}
        }

        public void FastOptimalization()
        {
            //bool[] nonMaxSupressionBools = { true, false };
            //Stopwatch stopwatch = new Stopwatch();
            //KeyPoint[][] points =
            //{
            //    new KeyPoint[3000], new KeyPoint[3000], new KeyPoint[3000],
            //    new KeyPoint[3000], new KeyPoint[3000], new KeyPoint[3000],
            //    new KeyPoint[3000], new KeyPoint[3000], new KeyPoint[3000],
            //    new KeyPoint[3000]
            //};

            //var best = new object[3] { 0, 0, "" };
            //var second = new object[3] { 0, 0, "" };
            //var third = new object[3] { 0, 0, "" };
            //var fourth = new object[3] { 0, 0, "" };
            //var fifth = new object[3] { 0, 0, "" };
            //var number = 0;
            //var img = new Mat[10];
            //var imageGrayScale = new Mat();
            //for (var l = 0; l < names.Length; l++)
            //{
            //    imageGrayScale = Cv2.ImRead(
            //                @"c:\Users\labudova\Google Drive\diplomka\testimages\KM\" + names[l] + ".png",
            //                LoadMode.Color);
            //    Preprocessing.NormaliseByChannels(ref imageGrayScale);
            //    Cv2.CvtColor(imageGrayScale, imageGrayScale, ColorConversion.RgbToGray);
            //    img[l] = new Mat();
            //    imageGrayScale.CopyTo(img[l]);
            //    imageGrayScale.Dispose();
            //}

            //foreach (bool nonmaxSupr in nonMaxSupressionBools)
            //{
            //    for (var k = 15; k < 66; k += 10)
            //    {
            //        for (var i = 0; i < 2; i++)
            //        {
            //            if (k < 45 | nonmaxSupr == false)
            //            {
            //                Mat[] descriptors =
            //                {
            //                new Mat(), new Mat(), new Mat(), new Mat(), new Mat(),
            //                new Mat(), new Mat(), new Mat(), new Mat(), new Mat()
            //                };
            //                var score = 0;
            //                long timeElapsed = 0;
            //                var scoreDistance = 0;
            //                var NumOfKeypoints = 0;
            //                bool discard = false;
            //                var medianDistanceTotal = 0;

            //                for (var j = 0; j < names.Length; j++)
            //                {
            //                    number += 1;
            //                    Mat image;
            //                    stopwatch.Start();
            //                    image = HelperOperations.ReturnImgFromNextLevPyr(img[j], i+1, 0.5f);

            //                    Cv2.FAST(image, out points[j], k, nonmaxSupr);
            //                    var brief = new BriefDescriptorExtractor(64);
            //                    brief.Compute(image, ref points[j], descriptors[j]);
            //                    stopwatch.Stop();

            //                    timeElapsed += stopwatch.ElapsedMilliseconds;
            //                    var distanceList = Descriptoring.DistanceOfDescriptors(descriptors[j]);
            //                    var medianDistance = HelperOperations.MedianFromList(distanceList);

            //                    medianDistanceTotal += medianDistance;

            //                    Logger.Debug("threshold=" + k);
            //                    Logger.Debug("nonmaxSupression = " + nonmaxSupr);
            //                    Logger.Debug("number of Keypoints = " + points[j].Length);
            //                    Logger.Debug("time to compute (ms) = " + stopwatch.ElapsedMilliseconds);
            //                    Logger.Debug("median distance of descriptors = " + medianDistance);
            //                    var randomNumber = new Random();
            //                    Cv2.DrawKeypoints(image, points[j], image);
            //                    Cv2.ImWrite(
            //                        @"C:\Users\labudova\Documents\diplomka\vysledky_analyz\30_1_2017\" + randomNumber.Next() + ".png",
            //                        image);
            //                    image.Dispose();
            //                    NumOfKeypoints += points[j].Length;
            //                    stopwatch.Reset();
            //                }
            //                double validRatioMin = 1.0;
            //                var timeAverage = (float)timeElapsed / 10;
            //                var distanceOfDescriptorsAverage = medianDistanceTotal / 10;
            //                if (distanceOfDescriptorsAverage > 255) score += 5;
            //                else if (distanceOfDescriptorsAverage > 245) score += 4;
            //                else if (distanceOfDescriptorsAverage < 240) score += 3;
            //                else if (distanceOfDescriptorsAverage < 235) score += 2;
            //                else if (distanceOfDescriptorsAverage < 230) score += 1;
            //                else if (distanceOfDescriptorsAverage < 200) discard = true;
            //                if (timeAverage < 10) score += 5;
            //                else if (timeAverage < 15) score += 4;
            //                else if (timeAverage < 20) score += 3;
            //                else if (timeAverage < 25) score += 2;
            //                else if (timeAverage < 30) score += 1;
            //                else if (timeAverage > 100) discard = true;

            //                var scoreDistanceAverage = (float)scoreDistance / 10;
            //                score += (int)scoreDistanceAverage;
            //                var numOfKeypointsAverage = NumOfKeypoints / 10;
            //                if (numOfKeypointsAverage < 1500) score += 1;
            //                else if (numOfKeypointsAverage < 1200) score += 2;
            //                else if (numOfKeypointsAverage < 900) score += 1;
            //                else if (numOfKeypointsAverage < 700) ;
            //                else if (numOfKeypointsAverage < 400) discard = true;

            //                double distMatches = 0;
            //                for (var j = 1; j < names.Length; j++)
            //                {
            //                    var bfmatcher = new BFMatcher(NormType.Hamming);
            //                    var matches = bfmatcher.KnnMatch(descriptors[0], descriptors[j], 2);
            //                    var validRatio = Descriptoring.MatchValidator(matches, points[0], points[j]);
            //                    if (validRatio < validRatioMin) validRatioMin = validRatio;
            //                    bfmatcher.Dispose();
            //                    distMatches = Descriptoring.AverageDistanceOfMatchedDescriptors(matches);
            //                    var VValidator = Descriptoring.VasekValidator(matches);
            //                    Logger.Debug(names[j] + ", distance of matched descriptors average = " + distMatches);
            //                    Logger.Warn("vaskuv koeficient = " + VValidator);

            //                }

            //                if ((score > (int)best[0]) && (discard != true))
            //                {
            //                    fifth = fourth;
            //                    fourth = third;
            //                    third = second;
            //                    second = best;
            //                    best[0] = score;
            //                    best[1] = number;
            //                    best[2] = "threshold = " + k + "nonmaxSupression = " + nonmaxSupr +
            //                            "number of Keypoints = " + numOfKeypointsAverage + "time to compute (ms) = " + timeAverage +
            //                            "median distance of descriptors = " + distanceOfDescriptorsAverage;
            //                    Logger.Debug("best = " + best);

            //                }
            //                else if ((score > (int)second[0]) && (discard != true))
            //                {
            //                    fifth = fourth;
            //                    fourth = third;
            //                    third = second;
            //                    second[0] = score;
            //                    second[1] = number;
            //                    second[2] = "threshold = " + k + "nonmaxSupression = " + nonmaxSupr +
            //                            "number of Keypoints = " + numOfKeypointsAverage + "time to compute (ms) = " + timeAverage +
            //                            "median distance of descriptors = " + distanceOfDescriptorsAverage;
            //                    Logger.Debug("second = " + second);
            //                }
            //                else if ((score > (int)third[0]) && (discard != true))
            //                {
            //                    fifth = fourth;
            //                    fourth = third;
            //                    third[0] = score;
            //                    third[1] = number;
            //                    third[2] = "threshold = " + k + "nonmaxSupression = " + nonmaxSupr +
            //                            "number of Keypoints = " + numOfKeypointsAverage + "time to compute (ms) = " + timeAverage +
            //                            "median distance of descriptors = " + distanceOfDescriptorsAverage;
            //                    Logger.Debug("third = " + third);
            //                }
            //                else if ((score > (int)fourth[0]) && (discard != true))
            //                {
            //                    fifth = fourth;
            //                    fourth[0] = score;
            //                    fourth[1] = number;
            //                    fourth[2] = "threshold = " + k + "nonmaxSupression = " + nonmaxSupr +
            //                            "number of Keypoints = " + numOfKeypointsAverage + "time to compute (ms) = " + timeAverage +
            //                            "median distance of descriptors = " + distanceOfDescriptorsAverage;
            //                    Logger.Debug("fourth = " + fourth);
            //                }
            //                else if ((score > (int)fifth[0]) && (discard != true))
            //                {

            //                    fifth[0] = score;
            //                    fifth[1] = number;
            //                    fifth[2] = "threshold = " + k + "nonmaxSupression = " + nonmaxSupr +
            //                            "number of Keypoints = " + numOfKeypointsAverage + "time to compute (ms) = " + timeAverage +
            //                            "median distance of descriptors = " + distanceOfDescriptorsAverage;
            //                    Logger.Debug("fifth = " + fifth);
            //                }

            //                Logger.Info("threshold = " + k);
            //                Logger.Info("score = " + score);
            //                Logger.Info("average number of keypoints = " + numOfKeypointsAverage);
            //                Logger.Info("average time to compute (ms) = " + timeAverage);
            //                Logger.Info("score of distance of descriptors = " + distanceOfDescriptorsAverage);
            //                Logger.Info("nonmaxSupression = " + nonmaxSupr);
            //                if (i == 1) Logger.Info("1 level image pyramid");
            //                if (i == 2) Logger.Info("2 level image pyramid");

            //                Logger.Info("valid ration minimum = " + validRatioMin);
            //                Logger.Info("\\\\\\\\");

            //            }
            //        }
            //    }
            //}
        }

    }
}
