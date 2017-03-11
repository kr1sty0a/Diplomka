using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using log4net.Repository.Hierarchy;
using MoreLinq;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    class ImageEvaluation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private enum Status
        {
            Fail,
            Ok
        };

        public const float TresholdForValidationKoef = 0.01f;
        private const int ThresholdForValidationDist = 54;
       

        private struct Pairs
        {
            public Images TestImg;
            public Images RefImg;
            public DMatch[] matches;
            public DMatch[][] matchesKnn;
        }

        private struct ReferenceFound
        {
            public Pairs BestMatchKoef;
            public Pairs BestMatchDist;
            public Status StatusKoef;
            public Status StatusDist;
            public float MaxKoef;
            public float MinDist;
            public double ValidationKoef;
            public double ValidationDist;
        }

        private struct ReferenceValidation
        {
            public Status StatusAfterValKoef;
            public Status StatusAfterValDist;
            public int ReferenceFound;
        }

        private const string FirstLine =
            "Evaluated file path, Maximal koeficient, Result based on maximal koeficient, status koef without validation, Validation, status," +
            " Minimal distance, Result based on Minimal distance, status distance without validation, Validation, Status, Reference file found";

        public void EvaluateAll()
        {
            //TP = 0;TN = 0;FP = 0;FN = 0;
            var images = new Images();
            Images[] refImgCollection = images.GetAllRefImages(Descriptoring.Methods.ORB);
            Images[] testImgCollection = images.GetAllTestImages(Descriptoring.Methods.ORB);
            var DevicesTest = testImgCollection.Select(x => x.Device).Distinct().ToArray();
            var VersionsTest = testImgCollection.Select(x => x.Version).Distinct().ToArray();
            var DevicesRef = refImgCollection.Select(x => x.Device).Distinct().ToArray();
            var VersionsRef = refImgCollection.Select(x => x.Version).Distinct().ToArray();
            var Devices = DevicesTest.Intersect(DevicesRef).ToList();
            var Versions = VersionsTest.Intersect(VersionsRef).ToList();
            var evaluationOrb = new ImageEvaluation();
            var valuesQuality = new int[4];
            var listSpecificity = new List<double>();
            var listSensitivity = new List<double>();
            foreach (var i in Devices)
            {
                foreach (var j in Versions)
                {
                    valuesQuality = evaluationOrb.EvaluateImageCollection(i, j, refImgCollection, testImgCollection);
                    listSpecificity.Add((double)valuesQuality[1] / (valuesQuality[1] + valuesQuality[2]));
                    listSensitivity.Add((double)valuesQuality[0] / (valuesQuality[0] + valuesQuality[3]));
                    Logger.Info($"Method: {Descriptoring.orbParameters.ToString()}, Device {i}, Version {j} Sensitivity: {listSensitivity.Last()}, Specificity: {listSpecificity.Last()}");

                }
            }
            var specificity = listSpecificity.Average();
            var sensitivity = listSensitivity.Average();
            Logger.Info($"Method: {Descriptoring.orbParameters.ToString()}, Sensitivity: {sensitivity}, Specificity: {specificity}");
        }


        public int[] EvaluateImageCollection(string device, string version, Images[] refImgCollection,
            Images[] testImgCollection)
        {
             int TP = 0;
        int TN = 0;
         int FP = 0;
         int FN = 0;
        var refImgs =
                refImgCollection.Where(x => x.Device == device && x.Version == version).Select(x => x).ToArray();
            var testImgs =
                testImgCollection.Where(x => x.Device == device && x.Version == version).Select(x => x).ToArray();
            var csv = new StringBuilder();
            csv.AppendLine($"{Images.orbParameters}, Device: {device}, Version: {version}");
            csv.AppendLine(FirstLine);
            foreach (var t in testImgs)
            {
                var result = FindReference(refImgs, t);
                var validate = ValidateReferenceFound(result);
                var newLine =
                    $"{t.ScreenId},{result.MaxKoef:0.###},{result.BestMatchKoef.RefImg.ScreenId},{result.StatusKoef},{result.ValidationKoef:0.###},{validate.StatusAfterValKoef},{result.MinDist:0.###}," +
                    $"{result.BestMatchDist.RefImg.ScreenId},{result.StatusDist},{result.ValidationDist:0.###},{validate.StatusAfterValDist},{validate.ReferenceFound}";
                csv.AppendLine(newLine);
                if (validate.ReferenceFound == t.ScreenId)
                {
                    TP += 1;
                }
                else if (validate.ReferenceFound == 0)
                {
                    var allScreenIds = refImgs.Select(x => x.ScreenId = validate.ReferenceFound).ToArray();
                    if (allScreenIds.Contains(validate.ReferenceFound))
                    {
                        TN += 1;
                    }
                    else
                    {
                        FN += 1;
                    }
                }
                else if (validate.ReferenceFound != t.ScreenId)
                {
                    FP += 1;
                }
            }
            var date = System.DateTime.Now;
            var path = Path.Combine(Images.WriteFolder, $"ORB_{date:d-M-yyyy h-m}.csv");
            File.WriteAllText(path, csv.ToString());
            return new []{TP,TN,FP,FN};

        }

        private static ReferenceFound FindReference(IEnumerable<Images> refImgs, Images testImg)
        {
            var bestMatchKoef = new Pairs();
            var bestMatchDist = new Pairs();
            var maxKoef = 0d;
            var minDistance = 255d;
            var validationDist = 0d;
            var validationKoef = 0d;

            foreach (var t in refImgs)
            {
                //var resultMatching = Descriptoring.MatchAndValidate(testImg, t);
                var bfmatcher = new BFMatcher(NormType.Hamming);
                var matchesKnn = bfmatcher.KnnMatch(testImg.Descriptors, t.Descriptors, 2);
                var matches = bfmatcher.Match(testImg.Descriptors, t.Descriptors);
                var resultKoef = Descriptoring.VasekValidator(matchesKnn);
                var resultDist = Descriptoring.AverageDistanceOfMatchedDescriptors(matches);
                if (maxKoef < resultKoef)
                {
                    bestMatchKoef = new Pairs()
                    {
                        TestImg = testImg,
                        RefImg = t,
                        matchesKnn = matchesKnn
                    };
                    maxKoef = resultKoef;
                    //validationKoef = (double) resultMatching.ValidRatio;
                }
                if (minDistance > resultDist)
                {
                    bestMatchDist = new Pairs()
                    {
                        TestImg = testImg,
                        RefImg = t,
                        matches = matches
                    };
                    minDistance = resultDist;
                    //validationDist = (double) resultMatching.ValidRatio;
                }
            }
            validationDist = Descriptoring.MatchValidator(bestMatchDist.matches,bestMatchDist.TestImg.Points, bestMatchDist.RefImg.Points);
            validationKoef = Descriptoring.MatchValidator(bestMatchKoef.matchesKnn, bestMatchKoef.TestImg.Points, bestMatchKoef.RefImg.Points);
            if (validationKoef < TresholdForValidationKoef)
            {
                Descriptoring.DrawMatchesImages(bestMatchKoef.TestImg, bestMatchKoef.RefImg, bestMatchKoef.matchesKnn[0]);
            }
            if (validationDist < TresholdForValidationKoef)
            {
                Descriptoring.DrawMatchesImages(bestMatchDist.TestImg, bestMatchDist.RefImg,bestMatchDist.matches);
            }
            var statusKoef = testImg.ScreenId == bestMatchKoef.RefImg.ScreenId ? Status.Ok : Status.Fail;
            var statusDist = testImg.ScreenId == bestMatchDist.RefImg.ScreenId ? Status.Ok : Status.Fail;
            var result = new ReferenceFound()
            {
                StatusKoef = statusKoef,
                StatusDist = statusDist,
                BestMatchKoef = bestMatchKoef,
                BestMatchDist = bestMatchDist,
                MaxKoef = (float) maxKoef,
                MinDist = (float) minDistance,
                ValidationKoef = validationKoef,
                ValidationDist = validationDist
            };
            return result;
        }

        private static ReferenceValidation ValidateReferenceFound(ReferenceFound reference)
        {
            var statusAfterValKoef = reference.ValidationKoef > TresholdForValidationKoef ? Status.Ok : Status.Fail;
            var statusAfterValDist = reference.ValidationDist > ThresholdForValidationDist ? Status.Ok : Status.Fail;
            var referenceFound = reference.BestMatchKoef.RefImg.ScreenId;
            if ((statusAfterValKoef == Status.Fail) && (statusAfterValDist == Status.Fail))
            {
                referenceFound = 0;
            }
            else if (statusAfterValKoef == Status.Fail)
            {
                referenceFound = reference.BestMatchDist.RefImg.ScreenId;
            }
            return new ReferenceValidation{ReferenceFound = referenceFound, StatusAfterValDist = statusAfterValDist, StatusAfterValKoef = statusAfterValKoef} ;
        }
    }
}