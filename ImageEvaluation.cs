﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    public class ImageEvaluation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));

        public enum Status
        {
            Fail,
            Ok
        };

        public const float TresholdForValidationKoef = 0.001f;
        private const int ThresholdForValidationDist = 54;

        public struct Pairs
        {
            public Images TestImg;
            public Images RefImg;
            public DMatch[] matches;
            public DMatch[][] matchesKnn;
        }

        public struct ReferenceFound
        {
            public Pairs BestMatchKoef;
            public Pairs BestMatchDist;
            public Status StatusKoef;
            public Status StatusDist;
            public float MaxKoef;
            public float MinDist;

        }

        private struct ReferenceValidation
        {
            public Status StatusAfterValKoef;
            public Status StatusAfterValDist;
            public int ReferenceFinal;
            public double ValidationKoef;
            public double ValidationDist;
        }

        private const string FirstLine =
            "Evaluated file path, Maximal koeficient, Result based on maximal koeficient, status koef without validation, Validation, status," +
            " Minimal distance, Result based on Minimal distance, status distance without validation, Validation, Status, Reference file found";

        private const string FirstLineSimple = "Evaluated file path, Maximal koeficient, Result based on maximal koeficient, status koef without" +
                                               " validation, Minimal distance, Result based on Minimal distance, Status, Reference file found";

        public StringBuilder csv = new StringBuilder();

        public void EvaluateAll(bool simple = false) {
            csv.AppendLine(simple ? FirstLineSimple : FirstLine);
            var images = new Images();
            Images[] refImgCollection = images.GetAllRefImages(Descriptoring.Methods.BRIEF);
            Images[] testImgCollection = images.GetAllTestImages(Descriptoring.Methods.BRIEF);
            var DevicesTest = testImgCollection.Select(x => x.Device).Distinct().ToArray();
            var VersionsTest = testImgCollection.Select(x => x.Version).Distinct().ToArray();
            var DevicesRef = refImgCollection.Select(x => x.Device).Distinct().ToArray();
            var VersionsRef = refImgCollection.Select(x => x.Version).Distinct().ToArray();
            var Devices = DevicesTest.Intersect(DevicesRef).ToList();
            var Versions = VersionsTest.Intersect(VersionsRef).ToList();
            var evaluation = new ImageEvaluation();
            var listSpecificity = new List<double>();
            var listSensitivity = new List<double>();
            foreach (var i in Devices) {
                foreach (var j in Versions) {
                    Quality valuesQuality;
                    if (simple) {
                        valuesQuality = evaluation.EvaluateImageCollectionSimple(i,
                            j,
                            refImgCollection,
                            testImgCollection);
                    }
                    else {
                        valuesQuality = evaluation.EvaluateImageCollection(i,
                            j,
                            refImgCollection,
                            testImgCollection);
                    }
                    listSpecificity.Add(valuesQuality.GetSpecificity());
                    listSensitivity.Add(valuesQuality.GetSensitivity());
                    Logger.Info(
                        $"Method: {Descriptoring.orbParameters.ToString()}, Device {i}, Version {j} Sensitivity: {valuesQuality.GetSensitivity()}, Specificity: {valuesQuality.GetSpecificity()}");
                }
            }
            var specificity = listSpecificity.Average();
            var sensitivity = listSensitivity.Average();
            Logger.Error($"Method: {Descriptoring.orbParameters.ToString()}, Sensitivity: {sensitivity}, Specificity: {specificity}");
        }


        public Quality EvaluateImageCollection(string device, string version, Images[] refImgCollection,
                                               Images[] testImgCollection) {
            var qa = new Quality(0, 0, 0, 0);
            csv.AppendLine($"{Descriptoring.orbParameters}, Device: {device}, Version: {version}");
            var refImgs =
                refImgCollection.Where(x => x.Device == device && x.Version == version).Select(x => x).ToArray();
            var testImgs =
                testImgCollection.Where(x => x.Device == device && x.Version == version).Select(x => x).ToArray();

            foreach (var t in testImgs) {
                var result = FindReference(refImgs, t);
                var validate = ValidateReferenceFound(result);
                var statusFinal = new Status();
                if (validate.ReferenceFinal == t.ScreenId) {
                    statusFinal = Status.Ok;
                }
                else {
                    statusFinal = Status.Fail;
                }
                var newLine =
                    $"{t.ScreenId},{result.MaxKoef:0.###},{result.BestMatchKoef.RefImg.ScreenId},{result.StatusKoef},{validate.ValidationKoef:0.###},{validate.StatusAfterValKoef},{result.MinDist:0.###}," +
                    $"{result.BestMatchDist.RefImg.ScreenId},{result.StatusDist},{validate.ValidationDist:0.###},{validate.StatusAfterValDist},{validate.ReferenceFinal}, {statusFinal}";
                csv.AppendLine(newLine);
                if (validate.ReferenceFinal == t.ScreenId) {
                    qa.TP += 1;
                }
                else if (validate.ReferenceFinal == 0) {
                    var allScreenIds = refImgs.Select(x => x.ScreenId).ToArray();
                    if (allScreenIds.Contains(t.ScreenId)) {
                        qa.TN += 1;
                    }
                    else {
                        qa.FN += 1;
                    }
                }
                else if (validate.ReferenceFinal != t.ScreenId) {
                    qa.FP += 1;
                }
            }
            var date = System.DateTime.Now;
            var path = Path.Combine(Images.WriteFolder, $"ORB_{date:d-M-yyyy h-m-s}.csv");
            File.WriteAllText(path, csv.ToString());
            return qa;

        }

        public static ReferenceFound FindReference(IEnumerable<Images> refImgs, Images testImg) {
            var bestMatchKoef = new Pairs();
            var bestMatchDist = new Pairs();
            var maxKoef = 0d;
            var minDistance = 255d;
            var validationDist = 0d;
            var validationKoef = 0d;

            foreach (var t in refImgs) {
                var bfmatcher = new BFMatcher(NormType.Hamming);
                var matchesKnn = bfmatcher.KnnMatch(testImg.Descriptors, t.Descriptors, 2);
                var matches = bfmatcher.Match(testImg.Descriptors, t.Descriptors);
                var resultKoef = Descriptoring.RatioValidator(matchesKnn);
                var resultDist = Descriptoring.AverageDistanceOfMatchedDescriptors(matches);
                if (maxKoef < resultKoef) {
                    bestMatchKoef = new Pairs() {
                        TestImg = testImg,
                        RefImg = t,
                        matchesKnn = matchesKnn
                    };
                    maxKoef = resultKoef;
                }
                if (minDistance > resultDist) {
                    bestMatchDist = new Pairs() {
                        TestImg = testImg,
                        RefImg = t,
                        matches = matches
                    };
                    minDistance = resultDist;
                }
            }
            var statusKoef = testImg.ScreenId == bestMatchKoef.RefImg.ScreenId ? Status.Ok : Status.Fail;
            var statusDist = testImg.ScreenId == bestMatchDist.RefImg.ScreenId ? Status.Ok : Status.Fail;
            var result = new ReferenceFound() {
                StatusKoef = statusKoef,
                StatusDist = statusDist,
                BestMatchKoef = bestMatchKoef,
                BestMatchDist = bestMatchDist,
                MaxKoef = (float) maxKoef,
                MinDist = (float) minDistance,
            };
            return result;
        }

        private static ReferenceValidation ValidateReferenceFound(ReferenceFound reference) {
            var validationKoef = Descriptoring.MatchValidator(reference.BestMatchKoef.matchesKnn,
                reference.BestMatchKoef.TestImg.Points,
                reference.BestMatchKoef.RefImg.Points);
            var validationDist = Descriptoring.MatchValidator(reference.BestMatchDist.matches,
                reference.BestMatchDist.TestImg.Points,
                reference.BestMatchDist.RefImg.Points);
            var statusAfterValKoef = new Status();
            var statusAfterValDist = new Status();

            if (validationKoef > TresholdForValidationKoef) {
                statusAfterValKoef = Status.Ok;
            }
            else {
                Descriptoring.DrawMatchesImages(reference.BestMatchKoef.TestImg, reference.BestMatchKoef.RefImg, reference.BestMatchKoef.matchesKnn);
                statusAfterValKoef = Status.Fail;
            }
            if (validationDist > TresholdForValidationKoef) {
                statusAfterValDist = Status.Ok;
            }
            else {
                Descriptoring.DrawMatchesImages(reference.BestMatchDist.TestImg, reference.BestMatchDist.RefImg, reference.BestMatchDist.matches);
                statusAfterValDist = Status.Fail;
            }
            var referenceFinal = reference.BestMatchKoef.RefImg.ScreenId;
            if ((statusAfterValKoef == Status.Fail) && (statusAfterValDist == Status.Fail)) {
                referenceFinal = 0;
            }
            else if (statusAfterValKoef == Status.Fail) {
                referenceFinal = reference.BestMatchDist.RefImg.ScreenId;
            }
            return new ReferenceValidation {
                ReferenceFinal = referenceFinal,
                StatusAfterValDist = statusAfterValDist,
                StatusAfterValKoef = statusAfterValKoef,
                ValidationKoef = validationKoef,
                ValidationDist = validationDist
            };
        }

        public Quality EvaluateImageCollectionSimple(string device, string version, Images[] refImgCollection,
                                                     Images[] testImgCollection) {
            var qa = new Quality(0, 0, 0, 0);
            csv.AppendLine($"{Descriptoring.orbParameters}, Device: {device}, Version: {version}");
            var refImgs =
                refImgCollection.Where(x => x.Device == device && x.Version == version).Select(x => x).ToArray();
            var testImgs =
                testImgCollection.Where(x => x.Device == device && x.Version == version).Select(x => x).ToArray();

            foreach (var t in testImgs) {
                var result = FindReference(refImgs, t);
                int ReferenceFinal = 0;
                if (result.MaxKoef > TresholdForValidationKoef) {
                    ReferenceFinal = result.BestMatchKoef.RefImg.ScreenId;
                }
                var statusFinal = new Status();
                if (ReferenceFinal == t.ScreenId) {
                    statusFinal = Status.Ok;
                }
                else {
                    statusFinal = Status.Fail;
                    Descriptoring.DrawMatchesImages(result.BestMatchKoef.TestImg, result.BestMatchKoef.RefImg, result.BestMatchKoef.matchesKnn);
                }
                var newLine =
                    $"{t.ScreenId},{result.MaxKoef:0.###},{result.BestMatchKoef.RefImg.ScreenId},{result.StatusKoef},{result.MinDist:0.###}," +
                    $"{result.BestMatchDist.RefImg.ScreenId},{result.StatusDist},{ReferenceFinal}, {statusFinal}";
                csv.AppendLine(newLine);
                if (ReferenceFinal == t.ScreenId) {
                    qa.TP += 1;
                }
                else if (ReferenceFinal == 0) {
                    var allScreenIds = refImgs.Select(x => x.ScreenId).ToArray();
                    if (allScreenIds.Contains(t.ScreenId)) {
                        qa.TN += 1;
                    }
                    else {
                        qa.FN += 1;
                    }
                }
                else if (ReferenceFinal != t.ScreenId) {
                    qa.FP += 1;
                }
            }
            var date = System.DateTime.Now;
            var path = Path.Combine(Images.WriteFolder, $"ORB_{date:d-M-yyyy h-m}.csv");
            File.WriteAllText(path, csv.ToString());
            return qa;

        }

    }
}