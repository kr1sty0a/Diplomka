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
            var evaluation = new ImageEvaluation();
            evaluation.EvaluateAll(true);
        }
    
    }

    
}


