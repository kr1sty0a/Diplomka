using System.Diagnostics;
using System.Linq;
using log4net;

namespace OpenCVSharpSandbox
{
    public class AdaptiveEvaluation
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(AdaptiveEvaluation));
        public Images img;
        public double threshold;
        public double sameKoef;
        public double diffKoef;

        public AdaptiveEvaluation[] GetKoeficientThreshold(Images[] imgs) {
            var watch = new Stopwatch();
            watch.Start();
            var Devices = imgs.Select(x => x.Device).Distinct().ToArray();
            var Versions = imgs.Select(x => x.Version).Distinct().ToArray();
            var adaptiveEv = new AdaptiveEvaluation[imgs.Length];
            var temp = 0;
            foreach (var i in Devices) {
                foreach (var j in Versions) {
                    var refImgs =
                        imgs.Where(x => x.Device == i && x.Version == j).Select(x => x).ToArray();
                    foreach (var refImg1 in refImgs) {
                        double diffKoef = 0;
                        double sameKoef = 1;
                        foreach (var refImg2 in refImgs) {
                            var result = Descriptoring.MatchAndComputeKoeficient(refImg1.Descriptors, refImg2.Descriptors);
                            if (refImg1.ScreenId != refImg2.ScreenId) {
                                if (result > diffKoef) {
                                    diffKoef = result;
                                }
                            }
                            else {
                                if (result < sameKoef) {
                                    sameKoef = result;
                                }
                            }
                        }
                        if (sameKoef > diffKoef) {
                            if (sameKoef == 1) {
                                adaptiveEv[temp] = new AdaptiveEvaluation() {
                                    diffKoef = diffKoef,
                                    sameKoef = sameKoef,
                                    threshold = diffKoef + 0.1,
                                    img = refImg1
                                };
                            }
                            if ((sameKoef - diffKoef) < 0.05) {
                                adaptiveEv[temp] = new AdaptiveEvaluation() {
                                    diffKoef = diffKoef,
                                    sameKoef = sameKoef,
                                    threshold = -2,
                                    img = refImg1
                                };
                            }
                            else {
                                adaptiveEv[temp] = new AdaptiveEvaluation() {
                                    diffKoef = diffKoef,
                                    sameKoef = sameKoef,
                                    threshold = (sameKoef + diffKoef)/2,
                                    img = refImg1
                                };
                            }
                        }
                        else {
                            adaptiveEv[temp] = new AdaptiveEvaluation() {diffKoef = diffKoef, sameKoef = sameKoef, threshold = -1, img = refImg1};
                        }
                        temp += 1;
                    }
                }
                Logger.Info($"Adaptive threshold for device: {i} computed");
            }
            watch.Stop();
            Logger.Info($"Time of computing adaptive threshold: {watch.ElapsedMilliseconds}");
            return adaptiveEv;
        }

        public AdaptiveEvaluation[] GetDistanceThreshold(Images[] imgs) {
            var watch = new Stopwatch();
            watch.Start();
            var Devices = imgs.Select(x => x.Device).Distinct().ToArray();
            var Versions = imgs.Select(x => x.Version).Distinct().ToArray();
            var adaptiveEv = new AdaptiveEvaluation[imgs.Length];
            var temp = 0;
            foreach (var i in Devices) {
                foreach (var j in Versions) {
                    var refImgs =
                        imgs.Where(x => x.Device == i && x.Version == j).Select(x => x).ToArray();
                    foreach (var refImg1 in refImgs) {
                        double diffDist = 255;
                        double sameDist = 0;
                        foreach (var refImg2 in refImgs) {
                            var result = Descriptoring.MatchAndComputeDistance(refImg1.Descriptors, refImg2.Descriptors);
                            if (refImg1.ScreenId != refImg2.ScreenId) {
                                if (result < diffDist) {
                                    diffDist = result;
                                }
                            }
                            else {
                                if (result > sameDist) {
                                    sameDist = result;
                                }
                            }
                        }
                        if (sameDist < diffDist) {
                            if (sameDist == 0) {
                                adaptiveEv[temp] = new AdaptiveEvaluation() {
                                    diffKoef = diffDist,
                                    sameKoef = sameDist,
                                    threshold = diffDist - 25,
                                    img = refImg1
                                };
                            }
                            if ((diffDist - sameDist) < 15) {
                                adaptiveEv[temp] = new AdaptiveEvaluation() {
                                    diffKoef = diffDist,
                                    sameKoef = sameDist,
                                    threshold = -2,
                                    img = refImg1
                                };
                            }
                            else {
                                adaptiveEv[temp] = new AdaptiveEvaluation() {
                                    diffKoef = diffDist,
                                    sameKoef = sameDist,
                                    threshold = (sameDist + diffDist)/2,
                                    img = refImg1
                                };
                            }
                        }
                        else {
                            adaptiveEv[temp] = new AdaptiveEvaluation() {diffKoef = diffDist, sameKoef = sameDist, threshold = -1, img = refImg1};
                        }
                        temp += 1;
                    }
                }
                Logger.Info($"Adaptive threshold for device: {i} computed");
            }
            watch.Stop();
            Logger.Info($"Time of computing adaptive threshold: {watch.ElapsedMilliseconds}");
            return adaptiveEv;
        }
    }
}