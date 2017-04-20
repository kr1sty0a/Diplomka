using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    public class Visualisation
    {
        public static void DrawBriskPoints() {
            var brisk = new BRISK(50, 0, 3f);
            var descriptors = new Mat();
            KeyPoint[] points;
            points = brisk.Detect(imgs.img2);
            var imgOut = new Mat();
            Cv2.DrawKeypoints(imgs.img2, points, imgOut);
            Cv2.ImShow("brisk", imgOut);
            Cv2.WaitKey();
        }

        public static void DrawBriefPoints() {
            KeyPoint[] points;
            var imgPyr = new Mat();
            Cv2.PyrDown(imgs.img2, imgPyr);
            using (var fastDetector = new FastFeatureDetector(35))
            {
                points = fastDetector.Detect(imgPyr);
            }
            var imgOut = new Mat();
            Cv2.DrawKeypoints(imgPyr, points, imgOut);
            Cv2.ImShow("brisk", imgOut);
            Cv2.WaitKey();
        }
    }
}