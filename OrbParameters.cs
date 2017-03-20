using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    public class OrbParameters
    {
        private static int nFeatures;
        private static float scaleFactor;
        private static int pLevels;
        private static int edgeThresh;
        private static int firstLevel;
        public int wTak;
        private static ORBScore scoretype;


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

        public ORB Create()
        {
            return new ORB(nFeatures, scaleFactor, pLevels, edgeThresh, firstLevel, wTak, scoretype);
        }

        public new string ToString()
            {
            var OrbParametersString = "Nfeatures: " + nFeatures + ", ScaleFactor: " +
                                      scaleFactor + ", PLevels: " + pLevels + ", EdgeThresh: " +
                                      edgeThresh +
                                      ", FirstLevel: " + firstLevel + ", WTak: " + wTak +
                                      ", ScoreType: " + scoretype;
            return OrbParametersString;
            }
        }
    }
