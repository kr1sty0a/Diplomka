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
        private int TP;
        private int TN;
        private int FP;
        private int FN;

        private struct Pairs
        {
            public Images TestImg;
            public Images RefImg;
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
            TP = 0;TN = 0;FP = 0;FN = 0;
            var images = new Images();
            Images[] refImgCollection = images.GetAllRefImages(Descriptoring.Methods.ORB);
            Images[] testImgCollection = images.GetAllTestImages(Descriptoring.Methods.ORB);
            var Devices = testImgCollection.Select(x => x.Device).Distinct().ToArray();
            var Versions = testImgCollection.Select(x => x.Version).Distinct().ToArray();
            var evaluationOrb = new ImageEvaluation();
            foreach (var i in Devices)
            {
                foreach (var j in Versions)
                {
                    evaluationOrb.EvaluateImageCollection(i, j, refImgCollection, testImgCollection);
                }
            }
            var specificity = TN / (TN + FP);
            var sensitivity = TP / (TP + FN);
            Logger.Info($"Method: {Descriptoring.orbParameters.ToString()}, Sensitivity: {sensitivity}, Specificity: {specificity}");
        }


        public void EvaluateImageCollection(string device, string version, Images[] refImgCollection,
            Images[] testImgCollection)
        {
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
                else
                {
                    Logger.Error("Something went wrong - Neither of TP,TN,FN,FP");
                }
            }
            var date = System.DateTime.Now;
            var path = Path.Combine(Images.WriteFolder, $"ORB_{date:d-M-yyyy h-m}.csv");
            File.WriteAllText(path, csv.ToString());

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
                var resultMatching = Descriptoring.MatchAndValidate(testImg, t);
                if (maxKoef < resultMatching.VaseksCoeficient)
                {
                    bestMatchKoef = new Pairs()
                    {
                        TestImg = testImg,
                        RefImg = t
                    };
                    maxKoef = resultMatching.VaseksCoeficient;
                    validationKoef = (double) resultMatching.ValidRatio;
                }
                if (minDistance > resultMatching.DistanceOfMatchedDescriptors)
                {
                    bestMatchDist = new Pairs()
                    {
                        TestImg = testImg,
                        RefImg = t,
                    };
                    minDistance = resultMatching.DistanceOfMatchedDescriptors;
                    validationDist = (double) resultMatching.ValidRatio;
                }
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