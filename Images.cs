using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    class Images
    {
        public Mat Descriptors;
        public string Device;
        public int ScreenId;
        public string Version;
        private string Vendor;
        private KeyPoint[] Points;
        public static readonly ORB orb = new ORB(1200, 1.2f, 8, 45, 1, 2, ORBScore.Fast);

        private const string TestFolder = @"C:\Users\labudova\Google Drive\diplomka\testimages\Konica_Minolta";
        private const string RefFolder = @"C:\Users\labudova\Documents\diplomka\References";
        
        public IEnumerable<Images> GetAllRefImages()
        {
            var refImgs = HelperOperations.GetAllImgsFromFolder(RefFolder);
            var refImagesInfo = new Images[refImgs.Count];
            for (var i = 0; i < refImgs.Count; i++)
            {
                var imgPath = refImgs[i];               
                var fileInfo = new FileInfo(imgPath);
                var model = fileInfo.Directory.Parent.Parent.Name;
                var version = fileInfo.Directory.Parent.Name;
                var vendor = fileInfo.Directory.Parent.Parent.Parent.Name;
                var screenId = fileInfo.Directory.Name;
                var img = Cv2.ImRead(refImgs[i], LoadMode.Color);
                //var orb = new ORB(1200, 1.2f, 8, 45, 1, 2, ORBScore.Fast);
                var points = orb.Detect(img);
                var descriptors = new Mat();
                orb.Compute(img, ref points, descriptors);
                refImagesInfo[i] = new Images() {Descriptors = descriptors, Device = model, ScreenId = int.Parse(screenId), Version = version, Vendor = vendor, Points = points};
            }
            return refImagesInfo;
        }
        public IEnumerable<Images> GetAllTestImages()
        {
            var testImgs = HelperOperations.GetAllImgsFromFolder(TestFolder);
            var testImagesInfo = new Images[testImgs.Count];
            for (var i = 0; i < testImgs.Count; i++)
            {
                var imgPath = testImgs[i];
                var fileInfo = new FileInfo(imgPath);
                var model = fileInfo.Directory.Name;
                var vendorSplit = model.Split('.');
                string version;
                string vendor;

                if (vendorSplit.Length == 4)
                {
                    version = model;
                    model = fileInfo.Directory.Parent.Name;
                    vendor = fileInfo.Directory.Parent.Parent.Name;
                }
                else
                {
                    version = "5.0.34.1";
                    vendor = fileInfo.Directory.Parent.Name;
                }
                var img = Cv2.ImRead(testImgs[i], LoadMode.Color);
                //var orb = new ORB(1200, 1.2f, 8, 45, 1, 2, ORBScore.Fast);
                var points = orb.Detect(img);
                var descriptors = new Mat();
                orb.Compute(img, ref points, descriptors);
                testImagesInfo[i] = new Images() { Descriptors = descriptors, Device = model, ScreenId = int.Parse(fileInfo.Name.Split('_').First()), Vendor = vendor, Version=version };
            }
            return testImagesInfo;
        }


    }
}
