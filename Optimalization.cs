using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    class Optimalization
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        public enum MatcherType
        {
            Knn,
            Match,
            Flann
        }
        public string path = Path.Combine(Images.TestFolder, "Konica Minolta", "C364", "5.0.34.1");
        private int screenSelected = 7;

        public void OrbOptimalization()
        {
            StringBuilder csv = new StringBuilder();
           
            



            var orbScore = new ORBScore[] {ORBScore.Harris, ORBScore.Fast};
            ////var numOfIterations = 0;
            //var minKoef = 1d;
            //var maxKoef = 0d;
            //var biggestDiff = 0d;
            ////for (var i = 1; i < 3; i++)
            ////{
            ////for (var i = 1; i < allImgs.Count; i++)
            ////{

            ////Logger.Debug(allImgs[i]);
            //int[] sameKonica = new[] {43, 44, 45, 46, 47, 48, 49, 50, 51};
            //for (var j = 300; j < 2001; j += 200)
            //{
            var j = 500;
            var k = 1.2f;
            var l = 8;
            var m = 31;
            var n = 1;
            //var o = 2;
            var p = ORBScore.Fast;

            //for (var k = 0.8f; k < 1.6f; k += 0.1f)
            //{
            //for (var l = 1; l < 10; l += 1)
            //{
                //for (var m = 10; m < 80; m += 5)
                //{
                for (var o = 2; o < 5; o++)
                {
                    //var o = 2;
                    //foreach (var p in orbScore)
                    //{
                    var discard = false;
                //foreach (int i in sameKonica)
                //{
                Descriptoring.orbParameters = new OrbParameters(j, k, l, m, n, o, p);
                //Descriptoring.orbParameters = new OrbParameters(2000, 1.4F, 12, 31, 0, 4, ORBScore.Fast); // now used
                //Descriptoring.orbParameters = new OrbParameters(1200, 1.2f, 8, 50, 1, 2, ORBScore.Fast); //optimal
                csv.AppendLine(Descriptoring.orbParameters.ToString());
                csv.AppendLine(
                    "image,Distance of Match, Distance of KnnMatch, Koeficient computed from KnnMatch, Valid ratio computed from knnMatch, Ratio of desc with small distance, ratio of cross matches vs matches  ");
                var Imgs = new Images();
                var testImgs = Imgs.GetAllTestImages(Descriptoring.Methods.ORB, path);
                var sameImgs =
                    testImgs.Where(x => x.ScreenId == screenSelected).Select(x => x).ToArray();
                var diffImgs =
                    testImgs.Where(x => x.ScreenId != screenSelected).Select(x => x).ToArray();
                for (var temp = 1; temp < sameImgs.Length; temp++)
                {
                    var result = Descriptoring.MatchAndValidate(sameImgs[0], sameImgs[temp]);
                    csv.AppendLine(
                        $"{sameImgs[temp].path},{result.DistanceOfMatchedDescriptors},{result.DistanceOfMatchedDescriptorsKnn}," +
                        $"{result.RatioCoeficient},{result.ValidRatio},{result.RatioDist},{result.RatioCross}");
                }
                csv.AppendLine("different images");
                foreach (var diffImg in diffImgs)
                {
                    var result = Descriptoring.MatchAndValidate(sameImgs[0], diffImg);
                    csv.AppendLine(
                        $"{diffImg.path},{result.DistanceOfMatchedDescriptors},{result.DistanceOfMatchedDescriptorsKnn}," +
                        $"{result.RatioCoeficient},{result.ValidRatio},{result.RatioDist},{result.RatioCross}");
                }
                    ////Console.WriteLine(l.ToString());


                    //    if (koeficient.ValidRatio < 0.5)
                    //    {
                    //        discard = true;
                    //    }

                    //    if ((koeficient.RatioCoeficient < minKoef) && (koeficient.ValidRatio > 0.5))
                    //    {
                    //        var ParametersSameMin = new Parameters(j, k, l, m, n, o, p);
                    //        minKoef = koeficient.RatioCoeficient;

                    //    }
                    //}
                    //for (var i = 0; i < allImgs.Count; i++)
                    //{
                    //    if (!(sameKonica.Contains(i) || i == 42))
                    //    {
                    //        var img2 = Cv2.ImRead(allImgs[i], LoadMode.Color);
                    //        ResultFromMatching koeficient;
                    //        if (i == 0)
                    //        {
                    //            koeficient = Descriptoring.ORBtwoImgs(img1, img2, j, k, l, m, n, o, p, true);
                    //        }
                    //        else
                    //        {
                    //            koeficient = Descriptoring.ORBtwoImgs(img1, img2, j, k, l, m, n, o, p);
                    //        }

                    //        if (koeficient.RatioCoeficient > maxKoef)
                    //        {
                    //            maxKoef = koeficient.RatioCoeficient;
                    //            var ParametersMaxDiff = new Parameters(j, k, l, m, n, o, p);
                    //        }
                    //    }
                    //}

                    //var Diff = minKoef - maxKoef;
                    //if ((Diff > 0.72) && (discard == false) && Diff != 1)
                    //{
                    //    var ParametersBiggestDiff = new Parameters(j, k, l, m, n, o, p);
                    //    biggestDiff = Diff;
                    //    Logger.Error("New maximum difference: " + Diff + ", Parameters: " + j + ", " +
                    //                 k + ", " + l + ", " + m + ", " + n + ", " + o +
                    //                 ", " + p);

                    //}
                    //minKoef = 1;
                    //maxKoef = 0;
                    //            }
                    //        }
                    //    }
                    //}
                    //}
                //}
            }
            var date = System.DateTime.Now;
            var pathResult = Path.Combine(Images.WriteFolder, $"ORB_{date:d-M-yyyy h-m}.csv");
            File.WriteAllText(pathResult, csv.ToString());
        }

        public void FastOptimalization()
        {
            bool[] nonMaxSupressionBools = { true, false };
            Stopwatch stopwatch = new Stopwatch();
            //KeyPoint[][] points =
            //{
            //    new KeyPoint[3000], new KeyPoint[3000], new KeyPoint[3000],
            //    new KeyPoint[3000], new KeyPoint[3000], new KeyPoint[3000],
            //    new KeyPoint[3000], new KeyPoint[3000], new KeyPoint[3000],
            //    new KeyPoint[3000]
            //};
            var points = new KeyPoint[][] {};
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
            //                    var VValidator = Descriptoring.RatioValidator(matches);
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

        public void BriefOptimalization()
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine(
                                "image,Distance of Match, Distance of KnnMatch, Koeficient computed from KnnMatch, " +
                                "Valid ratio computed from knnMatch, Ratio of desc with small distance, cross matches");
            bool[] nonMaxSupressionBools = {true, false};
            //foreach (bool nonmaxSupr in nonMaxSupressionBools)
            //{
            //for (var k = 15; k < 66; k += 5)
            //{
                //for (var i = 1; i < 4; i++)
                //{

                var nonmaxSupr = true;
                var k = 35;
                var i = 2;

                if (k < 45 | nonmaxSupr == false)
                {

                    Descriptoring.FastThreshold = k;
                    Descriptoring.fastBool = nonmaxSupr;
                    Descriptoring.levelPyr = 2;
                    var Imgs = new Images();
                    var testImgs = Imgs.GetAllTestImages(Descriptoring.Methods.BRIEF, path);
                    var sameImgs =
                        testImgs.Where(x => x.ScreenId == screenSelected).Select(x => x).ToArray();
                    var diffImgs =
                        testImgs.Where(x => x.ScreenId != screenSelected).Select(x => x).ToArray();
                    csv.AppendLine(
                        $"nonMaxSupr: {nonmaxSupr}, Fast threshold: {Descriptoring.FastThreshold}, level of image pyramid: {Descriptoring.levelPyr}");
                    for (var l = 1; l < sameImgs.Length; l++)
                    {
                        var result = Descriptoring.MatchAndValidate(sameImgs[0], sameImgs[l]);
                        csv.AppendLine(
                            $"{sameImgs[l].path},{result.DistanceOfMatchedDescriptors},{result.DistanceOfMatchedDescriptorsKnn}," +
                            $"{result.RatioCoeficient},{result.ValidRatio},{result.RatioDist},{result.RatioCross}");
                    }


                    csv.AppendLine("different images");
                    foreach (var diffImg in diffImgs)
                    {
                        var result = Descriptoring.MatchAndValidate(sameImgs[0], diffImg);
                        csv.AppendLine(
                            $"{diffImg.path},{result.DistanceOfMatchedDescriptors},{result.DistanceOfMatchedDescriptorsKnn}," +
                            $"{result.RatioCoeficient},{result.ValidRatio},{result.RatioDist},{result.RatioCross}");
                    }
            //}
                }
            var date = System.DateTime.Now;
            var pathResult = Path.Combine(Images.WriteFolder, $"BRIEF_{date:d-M-yyyy h-m}.csv");
            File.WriteAllText(pathResult, csv.ToString());
        }

        public void MatchTiming(MatcherType method, int numberOfKeypoints)
        {
            Descriptoring.orbParameters = new OrbParameters(numberOfKeypoints, 1.2f, 8, 50, 1, 2, ORBScore.Fast);
            var path = Path.Combine(Images.TestFolder, "Konica Minolta", "C364", "5.0.34.1");
            var Imgs = new Images();
            var testImgs = Imgs.GetAllTestImages(Descriptoring.Methods.ORB, path);
            //var testImgs = Imgs.GetAllTestImages(Descriptoring.Methods.BRIEF, path);
            var timer = new Stopwatch();
            timer.Start();
            if (method == MatcherType.Match)
            {
                for (var i = 1; i < testImgs.Length; i++)
                {
                    var matcher = new BFMatcher(NormType.Hamming);
                    matcher.Match(testImgs[0].Descriptors, testImgs[i].Descriptors);
                }

            }
            else if (method == MatcherType.Knn)
            {
                for (var i = 1; i < testImgs.Length; i++)
                {
                    var matcher = new BFMatcher(NormType.Hamming);
                    matcher.KnnMatch(testImgs[0].Descriptors, testImgs[i].Descriptors,1);
                }
            }
            else if (method == MatcherType.Flann)
            {
                throw new NotImplementedException();
            }
            timer.Stop();
            Logger.Info($"Elapsed time: {timer.ElapsedMilliseconds} ");
        }

        public void BriskOptimalization()
        {
            StringBuilder csv = new StringBuilder();
            csv.AppendLine(
                                "image,Distance of Match, Distance of KnnMatch, Koeficient computed from KnnMatch, " +
                                "Valid ratio computed from knnMatch, Ratio of desc with small distance, cross matches");
            int i = 50;
            int j = 0;
            float k = 3f;
            //for (var i = 10; i < 80; i++)
            //{
                //for (var j = 0; j < 7; j++)
                //{
                //for (var k = 0.5f; k < 5; k =k +0.5f)
                //{
                csv.AppendLine( $"thresh = {i}, octaves = {j}, patternScale = {k}"
                               );

                        Descriptoring.brisk = new BRISK(i, j, k);
                        var path = Path.Combine(Images.TestFolder, "Konica Minolta", "C364", "5.0.34.1");
                        var Imgs = new Images();
                        var testImgs = Imgs.GetAllTestImages(Descriptoring.Methods.BRISK, path);
                        var sameImgs =
                        testImgs.Where(x => x.ScreenId == screenSelected).Select(x => x).ToArray();
                        var diffImgs =
                            testImgs.Where(x => x.ScreenId != screenSelected).Select(x => x).ToArray();
                        for (var l = 1; l < sameImgs.Length; l++)
                        {
                            var result = Descriptoring.MatchAndValidate(sameImgs[0], sameImgs[l]);
                            csv.AppendLine(
                            $"{sameImgs[l].path},{result.DistanceOfMatchedDescriptors},{result.DistanceOfMatchedDescriptorsKnn}," +
                            $"{result.RatioCoeficient},{result.ValidRatio},{result.RatioDist},{result.RatioCross}");
                        }
                        csv.AppendLine("different images");
                        for (var m = 0; m < diffImgs.Length; m++)
                        {
                            var result = Descriptoring.MatchAndValidate(sameImgs[0], diffImgs[m]);
                            csv.AppendLine(
                        $"{diffImgs[m].path},{result.DistanceOfMatchedDescriptors},{result.DistanceOfMatchedDescriptorsKnn}," +
                        $"{result.RatioCoeficient},{result.ValidRatio},{result.RatioDist},{result.RatioCross}");
                        }
                //    }
                //}
            //}

            var date = System.DateTime.Now;
            var pathResult = Path.Combine(Images.WriteFolder, $"BRISK_{date:d-M-yyyy h-m}.csv");
            File.WriteAllText(pathResult, csv.ToString());
        }

        public void BriskOptimalization2()
        {
            StringBuilder csv = new StringBuilder();
            //csv.AppendLine(
            //                    "image,Distance of Match, Distance of KnnMatch, Koeficient computed from KnnMatch, " +
            //                    "Valid ratio computed from knnMatch, Ratio of desc with small distance, cross matches");
            var bestParametersKoef = new float[] {};
            var bestParametersDist = new float[] { };
            //int i = 30;
            //int j = 3;
            //float k = 1f;
            for (var i = 10; i < 80; i+=5)
            {
                for (var j = 0; j < 7; j++)
                {
                    for (var k = 0.5f; k < 5; k = k + 0.5f)
                    {
                        csv.AppendLine($"thresh = {i}, octaves = {j}, patternScale = {k}"
                               );

                Descriptoring.brisk = new BRISK(i, j, k);
                var path = Path.Combine(Images.TestFolder, "Konica Minolta", "C364", "5.0.34.1");
                var Imgs = new Images();
                var testImgs = Imgs.GetAllTestImages(Descriptoring.Methods.BRISK, path);
                var sameImgs =
                testImgs.Where(x => x.ScreenId == screenSelected).Select(x => x).ToArray();
                var diffImgs =
                    testImgs.Where(x => x.ScreenId != screenSelected).Select(x => x).ToArray();
                var bigDist = 0d;
                var smallDist = 255d;
                var bigKoef = 0d;
                var smallKoef = 1d;
                var differenceKoef = 0d;
                        var differenceDist = 0d;
                for (var l = 1; l < sameImgs.Length; l++)
                {
                    var result = Descriptoring.MatchAndValidate(sameImgs[0], sameImgs[l]);
                    if (result.DistanceOfMatchedDescriptors > bigDist) {
                        bigDist = result.DistanceOfMatchedDescriptors;
                    }
                    if (result.RatioCoeficient < smallKoef)
                    {
                                smallKoef = result.RatioCoeficient;
                    }
                            //csv.AppendLine(
                            //$"{sameImgs[l].path},{result.DistanceOfMatchedDescriptors},{result.DistanceOfMatchedDescriptorsKnn}," +
                            //$"{result.RatioCoeficient},{result.ValidRatio},{result.RatioDist},{result.RatioCross}");

              }
                //csv.AppendLine("different images");
                for (var m = 0; m < diffImgs.Length; m++)
                {
                    var result = Descriptoring.MatchAndValidate(sameImgs[0], diffImgs[m]);
                            //    csv.AppendLine(
                            //$"{diffImgs[m].path},{result.DistanceOfMatchedDescriptors},{result.DistanceOfMatchedDescriptorsKnn}," +
                            //$"{result.RatioCoeficient},{result.ValidRatio},{result.RatioDist},{result.RatioCross}");
                            if (result.DistanceOfMatchedDescriptors < smallDist)
                            {
                                smallDist = result.DistanceOfMatchedDescriptors;
                            }
                            if (result.RatioCoeficient > bigKoef)
                            {
                                bigKoef = result.RatioCoeficient;
                            }


                        }
                        var diffKoef = smallKoef - bigKoef;
                        if (diffKoef > differenceKoef) {
                            differenceKoef = diffKoef;
                            bestParametersKoef = new float[] {i,j,k};

                        }
                        var diffDist = bigDist - smallDist;
                        if (diffDist > differenceDist) {
                            differenceDist = diffDist;
                            bestParametersDist = new float[] { i, j, k };
                        }
                        csv.AppendLine($"Diff dist: {diffDist}, Diff koef {diffKoef}");
                       
                    }
                    }
                }
            var date = System.DateTime.Now;
            var pathResult = Path.Combine(Images.WriteFolder, $"BRISK_{date:d-M-yyyy h-m}.csv");
            File.WriteAllText(pathResult, csv.ToString());
            Logger.Info($"best parameters according koef = {bestParametersKoef}, best parameters according dist = {bestParametersDist}");

        }
    }
}
