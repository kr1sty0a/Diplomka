using System.Windows.Media;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    public class OrbParameters
    {
        
            public static int nFeatures;
            public static float scaleFactor;
            public static int pLevels;
            public static int edgeThresh;
            public static int firstLevel;
            public static int wTak;
            public static ORBScore scoretype;

        
        public OrbParameters(int Nfeatures, float ScaleFactor, int PLevels, int EdgeThresh, int FirstLevel, int WTak, ORBScore ScoreType)
        {
            nFeatures = Nfeatures;
            scaleFactor = ScaleFactor;
            pLevels = PLevels;
            edgeThresh = EdgeThresh;
            firstLevel = FirstLevel;
            wTak = WTak;
            scoretype = ScoreType;
        }


        public ORB Create(int Nfeatures, float ScaleFactor, int PLevels, int EdgeThresh, int FirstLevel, int WTak,
                ORBScore ScoreType)
            {
                return new ORB(Nfeatures, ScaleFactor, PLevels, EdgeThresh, FirstLevel, WTak, ScoreType);
            }

            public string ToString()
            {
            var OrbParametersString = "Nfeatures: " + nFeatures + ", ScaleFactor: " +
                                      scaleFactor + ", PLevels: " + pLevels + ", EdgeThresh: " +
                                      edgeThresh +
                                      ", FirstLevel: " + firstLevel + ", WTak: " + wTak +
                                      "ScoreType: " + scoretype;
            return OrbParametersString;
            }
        }
    }
