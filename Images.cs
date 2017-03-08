using System;
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
        
        public static string orbParameters = "nFeatures = 1200, scaleFactor = 1.2, nLevels = 8, edgeThreshold = 45, firstLevel = 1, wTak = 2, Fast";
        private const string TestFolder = @"C:\Users\labudova\Google Drive\diplomka\testimages\Konica_Minolta";
        private const string RefFolder = @"C:\Users\labudova\Documents\diplomka\References";
        
        public IEnumerable<Images> GetAllRefImages(Descriptoring.Methods method)
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
                var img = Cv2.ImRead(refImgs[i]);
                var result = Descriptoring.ComputeDescriptorsAndKeypoints(method, img);
                refImagesInfo[i] = new Images() {Descriptors = result.Descriptors, Device = model, ScreenId = int.Parse(screenId), Version = version, Vendor = vendor, Points = result.Points};
            }
            return refImagesInfo;
        }
        public IEnumerable<Images> GetAllTestImages(Descriptoring.Methods method)
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
                var result = Descriptoring.ComputeDescriptorsAndKeypoints(method, img);
                testImagesInfo[i] = new Images() { Descriptors = result.Descriptors, Device = model, ScreenId = int.Parse(fileInfo.Name.Split('_').First()), Vendor = vendor, Version=version, Points = result.Points};
            }
            return testImagesInfo;
        }


    }
}
