using OpenCvSharp;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    class imgs
    {
        public static Mat img1 =
            Cv2.ImRead(
                @"C:\Users\labudova\Documents\diplomka\ImagesForTesting\Konica Minolta\C364\5.0.34.1\3_1.png",
                //@"C:\Users\labudova\Documents\diplomka\ImagesForTesting\Konica Minolta\C364\5.0.34.1\2_1.png",
                //@"C:\Users\labudova\Documents\diplomka\ImagesForTesting\Y Soft\TPv4\6.0.0.11\2_1.png",
                LoadMode.Color);

        public static Mat img2 =
            Cv2.ImRead(
                @"C:\RQA\imgs\References\Konica_Minolta\C364\5.0.34.1\3\Main Menu.png",
                //@"C:\RQA\imgs\References\Konica_Minolta\C364\5.0.34.1\2\PIN.png",
                //@"C:\Users\labudova\Documents\diplomka\ImagesForTesting\Y Soft\TPv4\5.0.34.1\2_1 (2).png",
                LoadMode.Color);
    }
}
