using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    class ImageEvaluation
    {
        public ORB orbParameters = new ORB(1200, 1.2f, 8, 45, 1, 2, ORBScore.Fast);

        public struct Pairs
        {
            public KeyPoint[] points1;
            public KeyPoint[] points2;
            public DMatch[][] matches2;
            public DMatch[] matches1;
            public Images img1;
            public Images img2;
        }

        public void EvaluateImageCollection(string device, string version)
        {
            var images = new Images();
            var refImgCollection = images.GetAllRefImages();
            var testImgCollection = images.GetAllTestImages();
            var refImgs =
                refImgCollection.Where(x => x.Device == device && x.Version == version).Select(x => x).ToArray();
            var testImgs =
                testImgCollection.Where(x => x.Device == device && x.Version == version).Select(x => x).ToArray();

            var csv = new StringBuilder();
            var firstLine =
                "Evaluated file path; Maximal koeficient; Result based on maximal koeficient; status koef without validation; Validation; status; Minimal distance; Result based on Minimal distance; status distance without validation; Validation; Status; Correct reference file";
            csv.AppendLine(firstLine);

            for (var k = 0; k < (testImgs.Length - 1); k++)
            {
                var bfmatcher = new BFMatcher(NormType.Hamming);

                var MaxKoef = 0d;
                var MinDistance = 255d;
                //var bestMatchKoef = new Images();
                //var bestMatchDist = new Images();
                var bestMatchKoef = new Pairs();

                for (var i = 0; i < refImgs.Length; i++)
                {
                    var matchesKnn = bfmatcher.KnnMatch(testImgs[k].Descriptors, refImgs[i].Descriptors, 2);
                    var matches = bfmatcher.Match(testImgs[k].Descriptors, refImgs[i].Descriptors);

                    var koef = Descriptoring.VasekValidator(matchesKnn);
                    var distanceOfMatchedDescriptors = Descriptoring.AverageDistanceOfMatchedDescriptors2(matches);
                    if (MaxKoef < koef)
                    {
                        bestMatchKoef = new Pairs() {};
                        //bestMatchKoef = refImgs[i];
                        MaxKoef = koef;
                    }
                    if (MinDistance > distanceOfMatchedDescriptors)
                    {
                        bestMatchDist = refImgs[i];
                        MinDistance = distanceOfMatchedDescriptors;
                    }

                }

                string statusKoef;
                string statusDist;

                if (testImgs[k].ScreenId == bestMatchKoef.ScreenId)
                {
                    statusKoef = "OK";
                }
                else
                {
                    statusDist = "FAIL";
                }
            }
        }
    }
}
