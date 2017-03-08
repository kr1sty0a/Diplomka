using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    class ImageEvaluation
    {
        private enum Status
        {
            Fail,
            Ok
        };

        private const float TresholdForValidationKoef = 0.01f;
        private const int ThresholdForValidationDist = 54;

        private struct Pairs
        {
            public DMatch[][] MatchesKnn;
            public DMatch[] Matches;
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
        }

        private const string FirstLine = "Evaluated file path; Maximal koeficient; Result based on maximal koeficient; status koef without validation; Validation; status;" +
                                 " Minimal distance; Result based on Minimal distance; status distance without validation; Validation; Status; Correct reference file";

        public void EvaluateImageCollection(string device, string version, IEnumerable<Images> refImgCollection, IEnumerable<Images> testImgCollection)
        {
            //var images = new Images();
            //var refImgCollection = images.GetAllRefImages();
            //var testImgCollection = images.GetAllTestImages();
            var refImgs =
                refImgCollection.Where(x => x.Device == device && x.Version == version).Select(x => x).ToArray();
            var testImgs =
                testImgCollection.Where(x => x.Device == device && x.Version == version).Select(x => x).ToArray();
            var csv = new StringBuilder();
            csv.AppendLine(Images.orb.ToString());
            csv.AppendLine(FirstLine);
            foreach (var t in testImgs)
            {
                var result = FindReference(refImgs, t);
                var validationKoef = Descriptoring.VasekValidator(result.BestMatchKoef.MatchesKnn);
                var validationDist = Descriptoring.AverageDistanceOfMatchedDescriptors2(result.BestMatchDist.Matches);
                var statusAfterValKoef = validationKoef > TresholdForValidationKoef ? Status.Ok : Status.Fail;
                var statusAfterValDist = validationDist > ThresholdForValidationDist ? Status.Ok : Status.Fail;
                var referenceFound = result.BestMatchKoef.RefImg.ScreenId;
                if ((statusAfterValKoef == Status.Fail) && (statusAfterValDist == Status.Fail))
                {
                    referenceFound = 0;
                }
                var newLine = string.Format(
                    "{0};{1:0.###};{2};{3};{4:0.###};{5};{6:0.###};{7};{8};{9:0.###};{10};{11}", t.ScreenId, result.MaxKoef,result.BestMatchKoef.RefImg.ScreenId, result.StatusKoef, validationKoef, statusAfterValKoef, result.MinDist,
                    result.BestMatchDist.RefImg.ScreenId, result.StatusDist, validationDist, statusAfterValKoef, referenceFound );
                csv.AppendLine(newLine);
            }
            var date = System.DateTime.Now;
            var path = "C: \\Users\\labudova\\Google Drive\\diplomka\\testimages\\vysledky_analyz\\ORB-" + date.ToString("d-M-yyyy h-m") + ".csv";
            System.IO.File.WriteAllText(path, csv.ToString());
        }

        private static ReferenceFound FindReference(IEnumerable<Images> refImgs, Images testImg)
        {
            var bestMatchKoef = new Pairs();
            var bestMatchDist = new Pairs();
            var statusKoef = new Status();
            var statusDist = new Status();

            
                var bfmatcher = new BFMatcher(NormType.Hamming);
                var maxKoef = 0d;
                var minDistance = 255d;
              
                foreach (var t in refImgs)
                {
                    var matchesKnn = bfmatcher.KnnMatch(testImg.Descriptors, t.Descriptors, 2);
                    var matches = bfmatcher.Match(testImg.Descriptors, t.Descriptors);

                    var koef = Descriptoring.VasekValidator(matchesKnn);
                    var distanceOfMatchedDescriptors = Descriptoring.AverageDistanceOfMatchedDescriptors2(matches);
                    if (maxKoef < koef)
                    {
                        bestMatchKoef = new Pairs() { TestImg = testImg, RefImg = t, Matches = matches, MatchesKnn = matchesKnn };
                        maxKoef = koef;
                    }
                    if (minDistance > distanceOfMatchedDescriptors)
                    {
                        bestMatchDist = new Pairs() { TestImg = testImg, RefImg = t, Matches = matches, MatchesKnn = matchesKnn };
                        minDistance = distanceOfMatchedDescriptors;
                    }
                }
                statusKoef = testImg.ScreenId == bestMatchKoef.RefImg.ScreenId ? Status.Ok : Status.Fail;
                statusDist = testImg.ScreenId == bestMatchDist.RefImg.ScreenId ? Status.Ok : Status.Fail;
           
            var result = new ReferenceFound() { StatusKoef = statusKoef, StatusDist = statusDist, BestMatchKoef = bestMatchKoef, BestMatchDist = bestMatchDist, MaxKoef = (float)maxKoef, MinDist = (float)minDistance};
            return (result);
        }
    }
}
