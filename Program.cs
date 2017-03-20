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
            //var evaluation = new ImageEvaluation();
            //evaluation.EvaluateAll();
            var opt = new Optimalization();
            //opt.OrbOptimalization();

            //opt.MatchTiming(Optimalization.MatcherType.Match,1500);
            //opt.MatchTiming(Optimalization.MatcherType.Knn,1500);
            //opt.MatchTiming(Optimalization.MatcherType.Flann,500);
            var img = new Images();
            img.GetAllRefImages(Descriptoring.Methods.BRISK);
        }
    
    }

    
}


