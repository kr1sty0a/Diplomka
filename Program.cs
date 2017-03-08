using System.Collections.Generic;
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
            IEnumerable<Images> refImgCollection = images.GetAllRefImages(Descriptoring.Methods.ORB);
            IEnumerable<Images> testImgCollection = images.GetAllTestImages(Descriptoring.Methods.ORB);
            var evaluationOrb = new ImageEvaluation();
            evaluationOrb.EvaluateImageCollection("C364","5.0.34.1",refImgCollection, testImgCollection);

        }
    
    }

    
}


