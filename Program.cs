using System;
using System.Collections.Generic;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Shapes;
using log4net;


[assembly: log4net.Config.XmlConfigurator]
namespace OpenCVSharpSandbox
{
    class Program
    {

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        public struct Parameters
        {
            public static int nFeatures;
            public static float scaleFactor;
            public static int pLevels;
            public static int edgeThresh;
            public static int firstLevel;
            public static int wTak;
            public static ORBScore scoretype;

            public Parameters(int Nfeatures, float ScaleFactor, int PLevels, int EdgeThresh, int FirstLevel, int WTak, ORBScore ScoreType)
            {
                nFeatures = Nfeatures;
                scaleFactor = ScaleFactor;
                pLevels = PLevels;
                edgeThresh = EdgeThresh;
                firstLevel = FirstLevel;
                wTak = WTak;
                scoretype = ScoreType;
            }
        }

        public static void Main(string[] args)
        {






            var bestMatch = 0;
            if ((bestMatchKoef == bestMatchDist))
            {
                var bfmatcherRes = new BFMatcher(NormType.Hamming);
                var imgRes = Cv2.ImRead(refImgs[bestMatchKoef], LoadMode.Color);
                var orbRes = new ORB(1200, 1.2f, 8, 45, 1, 2, ORBScore.Fast);
                var pointsEval = orbRes.Detect(imgRes);
                var descRes = new Mat();
                orbRes.Compute(imgRes, ref pointsEval, descRes);
                var matchesRes = bfmatcherRes.KnnMatch(descImg, descRes, 2);
                //var matchesRes = bfmatcherRes.Match(descImg, descRes);
               

                //}
                //if (bestMatchKoef != bestMatchDist)
                //{
                //var bfmatcherResDist = new BFMatcher(NormType.Hamming);
                //var imgResDist = Cv2.ImRead(refImgs[bestMatchDist], LoadMode.Color);
                //var orbResDist = new ORB(1200, 1.2f, 8, 45, 1, 2, ORBScore.Fast);
                //var pointsResDist = orbResDist.Detect(imgRes);
                //var descResDist = new Mat();
                //orbRes.Compute(imgResDist, ref pointsEval, descResDist);
                //var matchesResDist = bfmatcherResDist.KnnMatch(descImg, descResDist, 2);
                //var validateDist = Descriptoring.MatchValidator(matchesResDist, points2, pointsResDist);

                //string statusKoef;
                //string statusDist;
                //string referenceFound;
                //if (validate > 0.01)
                //{
                //    var index = evalFiles.IndexOf(allImgs[k]);
                //    referenceFound = refFiles[index];
                //    var file = refImgs[bestMatchKoef];
                //    file = System.IO.Path.GetDirectoryName(file);
                //    if (file == referenceFound)
                //    {
                //        statusKoef = "OK";
                //    }
                //    else
                //    {
                //        statusKoef = "FAIL";
                //    }
                //}
                //else
                //{
                //    var index = evalFiles.IndexOf(allImgs[k]);
                //    referenceFound = refFiles[index];
                //    if (referenceFound == "unrecognized")
                //    {
                //        statusKoef = "OK";
                //    }
                //    else
                //    {
                //        statusKoef = "FAIL";
                //    }
                //}
                //var indexWithoutValidation = evalFiles.IndexOf(allImgs[k]);
                //var referenceFoundWithoutValidation = refFiles[indexWithoutValidation];
                //var fileWithoutValidation = refImgs[bestMatchKoef];
                //var fileDirectory = System.IO.Path.GetDirectoryName(fileWithoutValidation);

                //string statusKoefWithoutVal;
                //string statusDistaceWithoutVal;
                //if (referenceFoundWithoutValidation == fileDirectory)
                //{
                //    statusKoefWithoutVal = "OK";
                //}
                //else
                //{
                //    statusKoefWithoutVal = "FAIL";
                //}
                //fileWithoutValidation = refImgs[bestMatchDist];
                //fileDirectory = System.IO.Path.GetDirectoryName(fileWithoutValidation);
                //if (referenceFoundWithoutValidation == fileDirectory)
                //{
                //    statusDistaceWithoutVal = "OK";
                //}
                //else
                //{
                //    statusDistaceWithoutVal = "FAIL";
                //}

                //if (validateDist > 0.01)
                //{
                //    var index = evalFiles.IndexOf(allImgs[k]);
                //    referenceFound = refFiles[index];
                //    var file = refImgs[bestMatchDist];
                //    file = System.IO.Path.GetDirectoryName(file);
                //    if (file == referenceFound)
                //    {
                //        statusDist = "OK";
                //    }
                //    else
                //    {
                //        statusDist = "FAIL";
                //    }
                //}
                //else
                //{
                //    //System.Console.WriteLine("Picture was unrecognized");
                //    var index = evalFiles.IndexOf(allImgs[k]);
                //    referenceFound = refFiles[index];
                //    if (referenceFound == "unrecognized")
                //    {
                //        statusDist = "OK";
                //    }
                //    else
                //    {
                //        statusDist = "FAIL";
                //    }
                //}
                //var newLine = string.Format("{0};{1:0.###};{2};{3};{4:0.###};{5};{6:0.###};{7};{8};{9:0.###};{10};{11}", evalFiles[k], MaxKoef,
                //    refImgs[bestMatchKoef], statusKoefWithoutVal, validate, statusKoef, MinDistance, refImgs[bestMatchDist], statusDistaceWithoutVal, validateDist, statusDist, referenceFound);
                //csv.AppendLine(newLine);

            }
            var date = System.DateTime.Now;
            var path = "C: \\Users\\labudova\\Google Drive\\diplomka\\testimages\\vysledky_analyz\\ORB-" + date.Date.ToString("d-M-yyyy") + ".csv";
            System.IO.File.WriteAllText(path, csv.ToString());













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
            //                                var ParametersMaxDiff = new Parameters(j,k,l,m,n,o,p);
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
    
    }

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


