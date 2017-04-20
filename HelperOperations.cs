using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    class HelperOperations
    {
        internal static int MedianFromList(List<int> list) {
            var total = list.Sum();
            var centralValIndex = total/2;
            var cumsum = list[0];
            var MedianDistance = 0;
            for (var i = 1; i < list.Count; i++) {
                cumsum += list[i];
                if (cumsum > centralValIndex) return i;
            }
            return MedianDistance;
        }

        internal static float MedianFromList(List<float> list) {
            var total = list.Sum();
            var centralValIndex = total/2;
            var cumsum = list[0];
            var MedianDistance = 0;
            for (var i = 1; i < list.Count; i++) {
                cumsum += list[i];
                if (cumsum > centralValIndex) return i;
            }
            return MedianDistance;
        }

        public static float MedianMat(Mat matrix) {
            var numOfValues = matrix.Height*matrix.Width;
            var array = new float[numOfValues];
            int index = 0;
            var mat = new MatOfByte(matrix);
            var indexer = mat.GetIndexer();
            for (int i = 0; i < matrix.Height; i++) {
                for (int j = 0; j < matrix.Width; j++) {
                    var pixelIntensity = indexer[i, j];
                    array[index] = pixelIntensity;
                    index += 1;
                }
            }
            float median;
            Array.Sort(array);
            if (numOfValues%2 == 0) {
                median = array[(numOfValues/2)];
            }
            else {
                median = array[(numOfValues/2) + 1];
            }
            return median;
        }

        internal static Mat ArrayToMat(float[] array, int matHeight, int matWidth) {
            if (matHeight*matWidth != array.Length) {
                throw new InvalidDataException("Incorrect dimension," + array.Length.ToString() + " should be dividable by matHeigh and matWitdh");
            }
            var matrix = new Mat(matHeight, matWidth, MatType.CV_32FC1);
            var indexer = matrix.GetGenericIndexer<float>();
            for (int l = 0; l < array.Length; l++) {
                int row = l/matWidth;
                int col = l%matWidth;
                indexer[row, col] = array[l];

            }
            return matrix;
        }

        internal static float[] MatToArray(Mat matrix) {
            var array = new float[matrix.Height*matrix.Width];
            var index = 0;
            var indexer = matrix.GetGenericIndexer<float>();
            for (var i = 0; i < matrix.Height; i++) {
                for (var j = 0; j < matrix.Width; j++) {
                    array[index] = indexer[i, j];
                    index += 1;
                }
            }
            return array;
        }

        public static float MedianArray(float[] array) {
            var numOfValues = array.Length;
            float median;
            Array.Sort(array);
            if (numOfValues%2 == 0) {
                median = array[(numOfValues/2)];
            }
            else {
                median = array[(numOfValues/2) + 1];
            }
            return median;
        }

        public static Mat GetImgFromPyramid(Mat img, int level) {
            var image = new Mat();
            if (level == 0) {
                img.CopyTo(image);
            }
            else if (level == 1) {
                image = new Mat();
                Cv2.PyrDown(img, image);
            }
            else if (level == 2) {
                image = new Mat();
                var imageTemp2 = new Mat();
                Cv2.PyrDown(img, imageTemp2);
                Cv2.PyrDown(imageTemp2, image);
                imageTemp2.Dispose();
            }
            else {
                image = new Mat();
                var imageTemp2 = new Mat();
                var imageTemp3 = new Mat();
                Cv2.PyrDown(img, imageTemp2);
                Cv2.PyrDown(imageTemp2, imageTemp3);
                Cv2.PyrDown(imageTemp3, image);
                img.Dispose();
                imageTemp2.Dispose();
                imageTemp3.Dispose();
            }
            return image;
        }

        // TODO: Create method to get image from certain level of image pyramid, do it recursively

        public static Mat ReturnImgFromNextLevPyr(Mat img, int level, float ratio) {
            while (true) {
                if (level == 1) {
                    return img;
                }
                else {
                    var type = img.Type();
                    var imgPyr = new Mat((int) (img.Height*ratio), (int) (img.Width*ratio), type);
                    Cv2.PyrDown(img, imgPyr, imgPyr.Size());
                    img = imgPyr;
                    level -= 1;
                }
            }
        }

        public static List<string> GetAllImgsFromFolder(string rootFolderPath) {
            List<string> dirs = new List<string>(System.IO.Directory.EnumerateDirectories(rootFolderPath));
            for (var i = 0; i < dirs.Count; i++) {
                var tempPath = dirs[i];
                List<string> tempDirs = new List<string>(System.IO.Directory.EnumerateDirectories(tempPath));
                foreach (string t in tempDirs) {
                    dirs.Add(t);
                }
            }
            List<string> imgFiles = new List<string>();
            for (var i = 0; i < dirs.Count; i++) {
                string[] tempImgFiles = System.IO.Directory.GetFiles(dirs[i], "*.png");
                for (var j = 0; j < tempImgFiles.Length; j++) {
                    imgFiles.Add(tempImgFiles[j]);
                }
            }
            if (imgFiles.Count == 0) {
                var ImgFiles = System.IO.Directory.GetFiles(rootFolderPath, "*.png");
                return ImgFiles.ToList();
            }
            return imgFiles;
        }
    }
}
