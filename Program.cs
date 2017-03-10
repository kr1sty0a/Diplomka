using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;
using log4net;


[assembly: log4net.Config.XmlConfigurator]
namespace OpenCVSharpSandbox
{
    class Program
    {

        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
       
        public static void Main(string[] args)
        {
            
            var images = new Images();
            Images[] refImgCollection = images.GetAllRefImages(Descriptoring.Methods.ORB);
            Images[] testImgCollection = images.GetAllTestImages(Descriptoring.Methods.ORB);
            //var img = Cv2.ImRead()
            var desc = new Descriptoring();
            var img1 = testImgCollection[0];
            var img2 = testImgCollection[2];
            var validate = Descriptoring.MatchAndValidate(img1.Descriptors, img2.Descriptors,
                img1.Points, img2.Points);
           


            
            var evaluationOrb = new ImageEvaluation();
            evaluationOrb.EvaluateImageCollection("C364", "5.0.34.1", refImgCollection, testImgCollection);
            evaluationOrb.EvaluateImageCollection("MP C300", "5.0.34.1", refImgCollection, testImgCollection);
            evaluationOrb.EvaluateImageCollection("5335", "5.0.34.1", refImgCollection, testImgCollection);
            evaluationOrb.EvaluateImageCollection("TPv4", "5.0.34.1", refImgCollection, testImgCollection);
            evaluationOrb.EvaluateImageCollection("TPv4", "6.0.0.11", refImgCollection, testImgCollection);
        }
    
    }

    
}


