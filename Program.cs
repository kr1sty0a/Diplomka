using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using log4net;
using MoreLinq;
using Size = OpenCvSharp.CPlusPlus.Size;


[assembly: log4net.Config.XmlConfigurator]
namespace OpenCVSharpSandbox
{
    class Program
    {

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        //public static string ReferenceFolder = Properties.Resources.ResourcesRQA;
        //public static string TestingFolder = Properties.Resources.TestFolder;
        public static void Main(string[] args)
        {
            
            Preprocessing.NormaliseByChannels(ref imgs.img2);
            var result1 = Descriptoring.ComputeBrief(imgs.img1);
            var result2 = Descriptoring.ComputeBrief(imgs.img2);
            var bfmatcher = new BFMatcher(NormType.Hamming);
            var matches = bfmatcher.Match(result1.Descriptors, result2.Descriptors);
            
            var res = Descriptoring.MatchAndValidate(result1.Descriptors, result2.Descriptors, result1.Points, result2.Points);
            var imgOut = new Mat();
            //Cv2.DrawMatches(imgs.img1, result1.Points, imgs.img2, result2.Points, matches, imgOut);
            //Cv2.ImShow("obr", imgOut);
            Cv2.ImShow("compensation",imgs.img1);
            Cv2.WaitKey();



            //var evaluation = new ImageEvaluation();
            //evaluation.EvaluateAll();
            //var opt = new Optimalization();
            //opt.OrbOptimalization();
            //opt.BriefOptimalization();

            //opt.MatchTiming(Optimalization.MatcherType.Match,1500);
            //opt.MatchTiming(Optimalization.MatcherType.Knn,1500);
            //opt.MatchTiming(Optimalization.MatcherType.Flann,500);
            //var img = new Images();
            //img.GetAllRefImages(Descriptoring.Methods.BRISK);


            //var imgs2 = new Images();
            //var imgsTest = imgs2.GetAllTestImages(Descriptoring.Methods.BRIEF);
            //var sample = imgsTest.Where(x => x.Device == "C364" && x.Version == "5.0.34.1" && x.ScreenId == 48).Select(x =>x).First();
            //var imgs = new Images();

            //var imgCollection = imgs.GetAllRefImages(Descriptoring.Methods.BRIEF,"c://RQA//imgs//References");
            //var probs = new List<List<double>>();
            //foreach (var imageRef in imgCollection)
            //{
            //    probs.Add(new List<double>{ Descriptoring.MatchAndValidate(sample,imageRef).RatioCoeficient,imageRef.ScreenId});

            //}
            //var prb = probs.ToArray().Where(x => x[1]==40).Select(x=> x).ToArray();

            //SampleFast();
        }

        private static void SampleFast()
        {
            var mat = Cv2.ImRead(@"C:\Users\labudova\Documents\diplomka\ImagesForTesting\Konica Minolta\C364\5.0.34.1\1_2.png", LoadMode.Color);
            var img = new Mat();
            mat.CopyTo(img);
            var fast = new FastFeatureDetector(35);
            var points = fast.Detect(mat);
            Cv2.DrawKeypoints(mat,points,mat);
            var mask = new Mat(mat.Size(), MatType.CV_8UC1, new Scalar(1));
            //var mask = Mat.Ones(mat.Size(),MatType.CV_8UC1);
            var reg = new Rect(new Point2f(150f,150f), new Size(100,100));
            var sum = mask.Sum();
            var part = mask.SubMat(reg);
            part.SetTo(0);
            //mask.SetTo(1);
            //mask[reg] = Mat.Zeros(reg.Size,MatType.CV_8UC1).T();
            sum = mask.Sum();
            fast = new FastFeatureDetector(35);
            var points2 = fast.Detect(img,mask);
            Cv2.DrawKeypoints(img, points2, img);
            Cv2.ImWrite("C://maska.png", mask);
            Cv2.ImShow("mat",mat);
            Cv2.ImShow("mat2", img);
            Cv2.ImShow("mask",mask);
            Cv2.WaitKey();
        }
    }
   
    
    
}


