using System;
using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    public class Preprocessing
    {
        private static void SharpenImage(ref Mat img)
        {
            float[] kernel = {-1, -1, -1, -1, 9, -1, -1, -1, -1};
            var kernelMat = HelperOperations.ArrayToMat(kernel, 3, 3);
            Cv2.Filter2D(img, img, -1, kernelMat);
        }

        private static Mat SobelEdges(Mat img)
        {
            //SHARPEN
            SharpenImage(ref img);


            //SOBEL OPERATOR
            float[] kernelX = {-1, 0, 1, -2, 0, 2, -1, 0, 1};
            var kernelMatX = HelperOperations.ArrayToMat(kernelX, 3, 3);

            float[] kernelY = {1, 2, 1, 0, 0, 0, -1, -2, -1};
            var kernelMatY = HelperOperations.ArrayToMat(kernelY, 3, 3);

            var gradientX = new Mat();
            var gradientY = new Mat();

            Cv2.Filter2D(img, gradientX, -1, kernelMatX);
            Cv2.Filter2D(img, gradientY, -1, kernelMatY);

            var edges = new Mat();
            Cv2.BitwiseOr(gradientX, gradientY, edges);

            Cv2.Threshold(edges, edges, 130, 255, ThresholdType.Binary);
            Cv2.MedianBlur(edges, edges, 3);

            return edges;
        }


        private static void RedukceMoireSumu()
        {
            using (
                var img = Cv2.ImRead(@"c:\Users\labudova\Google Drive\diplomka\testimages\screens\Help Screen.png",
                    LoadMode.GrayScale))
            {
                //img.Resize(360);
                // expand input image to optimal size
                Mat padded = new Mat();
                int m = Cv2.GetOptimalDFTSize(img.Rows);
                int n = Cv2.GetOptimalDFTSize(img.Cols); // on the border add zero values
                Cv2.CopyMakeBorder(img, padded, 0, m - img.Rows, 0, n - img.Cols, BorderType.Constant, Scalar.All(0));

                // Add to the expanded another plane with zeros
                Mat paddedF32 = new Mat();
                padded.ConvertTo(paddedF32, MatType.CV_32F);
                Mat[] planes = {paddedF32, Mat.Zeros(padded.Size(), MatType.CV_32F)};
                Mat complex = new Mat();
                Cv2.Merge(planes, complex);

                // this way the result may fit in the source matrix
                Mat dft = new Mat();
                Cv2.Dft(complex, dft);

                // compute the magnitude and switch to logarithmic scale
                // => log(1 + sqrt(Re(DFT(I))^2 + Im(DFT(I))^2))
                Mat[] dftPlanes;
                Cv2.Split(dft, out dftPlanes); // planes[0] = Re(DFT(I), planes[1] = Im(DFT(I))

                // planes[0] = magnitude
                Mat magnitude = new Mat();
                Cv2.Magnitude(dftPlanes[0], dftPlanes[1], magnitude);

                // spektrum pro upravu
                var powerSpec = magnitude.Clone();
                //Cv2.Normalize(powerSpec,powerSpec,0,1,NormType.MinMax);

                magnitude += Scalar.All(1); // switch to logarithmic scale
                Cv2.Log(magnitude, magnitude);

                // crop the spectrum, if it has an odd number of rows or columns
                Mat spectrum = magnitude[
                    new Rect(0, 0, magnitude.Cols & -2, magnitude.Rows & -2)];
                //Cv2.ImShow("Neupravene spektrum", spectrum);
                // rearrange the quadrants of Fourier image  so that the origin is at the image center
                int cx = spectrum.Cols/2;
                int cy = spectrum.Rows/2;

                Mat q0 = new Mat(spectrum, new Rect(0, 0, cx, cy)); // Top-Left - Create a ROI per quadrant
                Mat q1 = new Mat(spectrum, new Rect(cx, 0, cx, cy)); // Top-Right
                Mat q2 = new Mat(spectrum, new Rect(0, cy, cx, cy)); // Bottom-Left
                Mat q3 = new Mat(spectrum, new Rect(cx, cy, cx, cy)); // Bottom-Right

                // swap quadrants (Top-Left with Bottom-Right)
                Mat tmp = new Mat();
                q0.CopyTo(tmp);
                q3.CopyTo(q0);
                tmp.CopyTo(q3);

                // swap quadrant (Top-Right with Bottom-Left)
                q1.CopyTo(tmp);
                q2.CopyTo(q1);
                tmp.CopyTo(q2);

                // Transform the matrix with float values into a
                Cv2.Normalize(spectrum, spectrum, 0, 1, NormType.MinMax);


                // Show the result
                Cv2.ImShow("Input Image", img);
                Cv2.ImShow("Spectrum Magnitude", spectrum);

                // calculating the idft
                //Mat inverseTransform = new Mat();
                //Cv2.Dft(dft, inverseTransform, DftFlag2.Inverse | DftFlag2.RealOutput);
                //Cv2.Normalize(inverseTransform, inverseTransform, 0, 1, NormType.MinMax);
                //Cv2.ImShow("Reconstructed by Inverse DFT", inverseTransform);
                //Cv2.WaitKey();

                const int T = 4; //treshold
                const int k = 2; //okoli
                double[] G5 =
                {
                    0.0768836536133642, 0.0487705754992860, 0.0392105608476768, 0.0487705754992860, 0.0768836536133642,
                    0.0487705754992860, 0.0198013266932447, 0.00995016625083189, 0.0198013266932447, 0.0487705754992860,
                    0.0392105608476768, 0.00995016625083189, 0, 0.00995016625083189, 0.0392105608476768,
                    0.0487705754992860, 0.0198013266932447, 0.00995016625083189, 0.0198013266932447, 0.0487705754992860,
                    0.0768836536133642, 0.0487705754992860, 0.0392105608476768, 0.0487705754992860, 0.0768836536133642
                }; //kernel
                var okoli = new Mat();
                for (var i = (k + 1); i < (powerSpec.Rows - k - 1); i++)
                {
                    for (var j = (k + 1); j < (powerSpec.Width - k - 1); j++)
                    {
                        Cv2.GetRectSubPix(powerSpec, new Size(5, 5), new Point2f(i, j), okoli);
                        var okoliArray = HelperOperations.MatToArray(okoli);
                        var centreVal = okoliArray[12];
                        //var centreVal = okoli.Get<float>(i,j);
                        //var med = MedianMat(okoli);
                        var med = HelperOperations.MedianArray(okoliArray);
                        if (centreVal/med > T)
                        {
                            //var okoliArray = MatToArray(okoli);
                            Console.WriteLine(i.ToString() + "," + j.ToString());
                            Console.WriteLine(centreVal.ToString());
                            Console.WriteLine("----------------");
                            for (int ind = 0; ind < okoliArray.Length; ind++)
                            {
                                int row = ind/okoli.Width;
                                int col = ind%okoli.Width;
                                powerSpec.Set<float>((i + row), (j + col), okoliArray[ind]*(float) G5[ind]);
                            }
                        }
                    }
                }

                //vykresleni upraveneho spektra
                powerSpec += Scalar.All(1); // switch to logarithmic scale
                Cv2.Log(powerSpec, powerSpec);

                // crop the spectrum, if it has an odd number of rows or columns
                Mat powerSpecMat = powerSpec[
                    new Rect(0, 0, powerSpec.Cols & -2, powerSpec.Rows & -2)];

                // rearrange the quadrants of Fourier image  so that the origin is at the image center
                int dx = powerSpecMat.Cols/2;
                int dy = powerSpecMat.Rows/2;

                Mat qa0 = new Mat(powerSpecMat, new Rect(0, 0, dx, dy)); // Top-Left - Create a ROI per quadrant
                Mat qa1 = new Mat(powerSpecMat, new Rect(dx, 0, dx, dy)); // Top-Right
                Mat qa2 = new Mat(powerSpecMat, new Rect(0, dy, dx, dy)); // Bottom-Left
                Mat qa3 = new Mat(powerSpecMat, new Rect(dx, dy, dx, dy)); // Bottom-Right

                // swap quadrants (Top-Left with Bottom-Right)
                Mat tmp1 = new Mat();
                qa0.CopyTo(tmp1);
                qa3.CopyTo(qa0);
                tmp1.CopyTo(qa3);

                // swap quadrant (Top-Right with Bottom-Left)
                qa1.CopyTo(tmp1);
                qa2.CopyTo(qa1);
                tmp1.CopyTo(qa2);

                Cv2.Normalize(powerSpecMat, powerSpecMat, 0, 1, NormType.MinMax);

                Cv2.ImShow("Upravene spektrum", powerSpecMat);
                Cv2.WaitKey();
                Cv2.DestroyAllWindows();

                //var planesForIdft = new Mat();
                //var prepareForMerge = new Mat[2];
                //prepareForMerge[0] = powerSpec;
                //prepareForMerge[1] = dftPlanes[1];
                //Cv2.Merge(prepareForMerge, planesForIdft);
                //// calculating the idft
                //Mat inverseTransform2 = new Mat();
                //Cv2.Dft(planesForIdft, inverseTransform2, DftFlag2.Inverse | DftFlag2.RealOutput);
                //Cv2.Normalize(inverseTransform2, inverseTransform2, 0, 1, NormType.MinMax);
                //Cv2.ImShow("Reconstructed by Inverse DFT 2 ", inverseTransform2);

                //Cv2.DestroyAllWindows();
                //Cv2.Split(img, out planes);
                //var m = MedianMat(img);
                //double minVal;
                //double maxVal;
                //img.MinMaxLoc(out minVal, out maxVal);
                //Console.WriteLine(minVal.ToString());
                //Console.WriteLine(maxVal.ToString());
                //Console.WriteLine(m.ToString());
                //Console.ReadKey();
                //Cv2.ImShow("B", planes[0]);
                //Cv2.ImShow("R", planes[1]);
                //Cv2.ImShow("G", planes[2]);
                //Cv2.WaitKey();
                //Cv2.DestroyAllWindows();
            }
        }

        public static void NormaliseByChannels(ref Mat matrix)
        {
            Mat[] planes;
            Cv2.Split(matrix, out planes);
            Mat[] imgOut =
            {
                new Mat(planes[0].Height, planes[0].Width, MatType.CV_8UC1),
                new Mat(planes[0].Height, planes[0].Width, MatType.CV_8UC1),
                new Mat(planes[0].Height, planes[0].Width, MatType.CV_8UC1),
            };
            for (var k = 0; k < 3; k++)
            {
                Cv2.Normalize(planes[k], imgOut[k], 0, 255, NormType.MinMax);
                planes[k].Dispose();
            }
            Cv2.Merge(imgOut, matrix);
        }

        private static Mat ExtrakceTextu(Mat img)
        {
            var edges = new Mat();

            Cv2.Canny(img, edges, 200, 300);

            Cv2.Normalize(img, img, 1, 255, NormType.MinMax);
            var cannyEdges = new Mat();
            //edges.CopyTo(cannyEdges);

            float[] structureElement = {0, 1, 0, 1, 1, 1, 0, 1, 0};
            var structureElementMat = HelperOperations.ArrayToMat(structureElement, 3, 3);
            structureElementMat.ConvertTo(structureElementMat, MatType.CV_8U);
            Cv2.Dilate(edges, edges, structureElementMat);
            Cv2.Dilate(edges, edges, structureElementMat);
            Cv2.BitwiseAnd(edges, img, img);
            var origImg = new Mat();
            img.CopyTo(origImg);
            var matrix = new MatOfByte(img);
            var indexer = matrix.GetIndexer();
            for (var i = 0; i < img.Height; i++)
            {
                for (var j = 0; j < img.Width; j++)
                {
                    int pixelVal = indexer[i, j];
                    if (pixelVal == 0)
                    {
                        indexer[i, j] = 170; //idealne zase median
                        //indexer[i, j] = 255;
                    }
                    else if (pixelVal > 170)
                    {
                        indexer[i, j] = 170;
                    }
                }
            }

            Cv2.Threshold(img, img, 150, 255, ThresholdType.Binary);
            //line substraction
            Cv2.Canny(origImg, cannyEdges, 10, 200);
            var lines = Cv2.HoughLinesP(cannyEdges, 1, Math.PI/180, 1, 100, 3);
            for (int i = 0; i < lines.Length; i++)
            {
                var l = lines[i];
                Cv2.Line(img, l.P1, l.P2, new Scalar(255), 8);
            }
            var sobelEdges = SobelEdges(origImg);
            lines = Cv2.HoughLinesP(sobelEdges, 1, Math.PI/180, 1, 100, 2);
            for (int i = 0; i < lines.Length; i++)
            {
                var l = lines[i];
                Cv2.Line(img, l.P1, l.P2, new Scalar(255), 8);
            }
            return img;
        }

        public static Mat Negative(Mat img, bool normalize = true)
        {
            var imgOut = new Mat();
            if (img.Channels() == 3)
            {
                if (normalize == true)
                {
                    Preprocessing.NormaliseByChannels(ref img);
                }
                var refImg = new Mat(img.Height, img.Width, MatType.CV_8UC3, new Scalar(255, 255, 255));
                Cv2.Subtract(refImg, img, imgOut);
            }
            else
            {
                if (normalize == true)
                {
                    Cv2.Normalize(img, img, 0, 255, NormType.MinMax);
                }
                var refImg = new Mat(img.Height, img.Width, MatType.CV_8UC1, new Scalar(255));
                Cv2.Subtract(refImg, img, imgOut);
            }
            return imgOut;
        }

        private static Mat BlurSegmentation(Mat img)
        {
            var imgOut = new Mat();
            Cv2.PyrMeanShiftFiltering(img, imgOut, 5, 30, 1);
            return imgOut;
        }
    }
}