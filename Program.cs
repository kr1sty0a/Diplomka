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
            //var desc = new Descriptoring();
            //var img1 = testImgCollection[97];
            //var img2 = refImgCollection[20];
            //var validate = Descriptoring.MatchAndValidate(img1,img2,true);
           


            
            var evaluationOrb = new ImageEvaluation();
            evaluationOrb.EvaluateImageCollection("C364", "5.0.34.1", refImgCollection, testImgCollection);
            evaluationOrb.EvaluateImageCollection("MP C300", "5.0.34.1", refImgCollection, testImgCollection);
            evaluationOrb.EvaluateImageCollection("5335", "5.0.34.1", refImgCollection, testImgCollection);
            evaluationOrb.EvaluateImageCollection("TPv4", "5.0.34.1", refImgCollection, testImgCollection);
        }
    
    }

    
}


