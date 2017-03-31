using System;
using System.Linq;
using log4net;
using OpenCvSharp.CPlusPlus;

namespace OpenCVSharpSandbox
{
    class MatchValidator
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(Program));
        private Vec2f ShiftVector;
        private bool valid;
        public Point2f pointImg1;
        public Point2f pointImg2;


        internal static MatchValidator[] MatchValidation(DMatch[] matches, KeyPoint[] points1, KeyPoint[] points2)
        {
            var Validator = new MatchValidator[matches.Length];
            for (var i = 0; i < matches.Length; i++)
            {
                var point1 = points1[i].Pt;
                var point2 = points2[matches[i].TrainIdx].Pt;
                Validator[i] = new MatchValidator();
                Validator[i].pointImg1 = point1;
                Validator[i].pointImg2 = point2;
                Validator[i].GetShiftVector(point1, point2);
                var distOfMatchedPoints = Validator[i].GetLengthOfVector();
                Validator[i].valid = distOfMatchedPoints < 10;
            }
            return Validator;
        }

        internal static void Validate(DMatch[] matches, KeyPoint[] points1, KeyPoint[] points2)
        {
            var Validator = MatchValidation(matches, points1, points2);
            var validationKoeficient = GetValidationKoef(Validator);
            if (validationKoeficient > 0.5)
            {
                Logger.Info("Image was verified");
            }
            else
            {
                Logger.Info("Image was not verified");
                if (CheckCalibration(Validator))
                {
                    Logger.Warn("Camera is might not be calibrated properly");
                    Logger.Info("Trying to compensate shift");
                    var averageShiftVector = GetAverageShiftVector(Validator);
                    var Points1 = points1.Select(x => x.Pt).ToArray();
                    var Points1Compensation =
                        Points1.Select(
                                x => new Point2f(x.X + averageShiftVector.Item0, x.Y + averageShiftVector.Item1))
                            .ToArray();
                    var newPoints1 = Points1Compensation.Select(x => new KeyPoint(x.X, x.Y, 1.0f)).ToArray();
                    var Validator2Round = MatchValidation(matches, newPoints1, points2);
                    var validationKoeficient2 = GetValidationKoef(Validator2Round);
                    if (validationKoeficient2 > 0.5)
                    {
                        Logger.Info("Image was verified on second round");
                        Logger.Info("Recalibrating camera...");
                    }
                    else
                    {
                        Logger.Error("Image was not verified, compensation was not successful");
                    }
                    for (var i = 0; i < Validator.Length; i++)
                    {
                        Cv2.Circle(imgs.img1, Validator[i].pointImg1, 1, new Scalar(255, 255, 0));
                        Cv2.Circle(imgs.img1, Validator[i].pointImg2, 1, new Scalar(0, 255, 0));
                        Cv2.Circle(imgs.img1, Validator2Round[i].pointImg1, 1, new Scalar(0, 0, 255));
                    }

                }
            }

        }

        public static float ComputeSlope(Point2f point1, Point2f point2)
        {
            var slope = Math.Abs(point1.Y - point2.Y) / Math.Abs(point1.X - point2.X);
            return slope;
        }

        public float ComputeSlope()
        {
            var slope = ShiftVector.Item1/ShiftVector.Item0;
            return slope;
        }

        public void GetShiftVector(Point2f point1, Point2f point2)
        {
            ShiftVector = new Vec2f(point2.X - point1.X, point2.Y - point1.Y);
        }

        public float GetLengthOfVector()
        {
            var length = Math.Sqrt(ShiftVector.Item0*ShiftVector.Item0 + ShiftVector.Item1*ShiftVector.Item1);
            return (float)length;
        }

        public static float GetValidationKoef(MatchValidator[] validator)
        {
            var koef = (float) validator.Where(x => x.valid == true).Select(x => x).ToList().Count/
                       (float) validator.Length;
            return koef;
        }

        public static bool CheckCalibration(MatchValidator[] validator)
        {
            var invalid = validator.Where(x => x.valid == false).Select(x => x.ComputeSlope()).ToArray();
            var averageSlope = invalid.Average();
            var calibrationCheck =
                    invalid.Where(x => (x < averageSlope + 1) && (x > (averageSlope - 1)))
                        .Select(x => x)
                        .ToArray();
            var calProb = (float)calibrationCheck.Length / (float)invalid.Length;
            return calProb > 0.8;

        }

        public static Vec2f GetAverageShiftVector(MatchValidator[] validator)
        {
            var averageSlope = validator.Where(x => x.valid == false).Select(x => x.ComputeSlope()).ToArray().Average();
            var Distance = validator.Where(x => x.valid == false).Select(x => x.GetLengthOfVector()).ToArray();
            var averageDistance = HelperOperations.MedianArray(Distance);
            var calibrationCheckSlope =
                validator.Where(
                        x => ((x.ComputeSlope() < averageSlope + 1) && (x.ComputeSlope() > (averageSlope - 1)))&&
                        (((x.GetLengthOfVector()< averageDistance + 10) && (x.GetLengthOfVector() > averageDistance - 10))))
                    .Select(x => x.ShiftVector)
                    .ToArray();
            var averageShiftVector =
                calibrationCheckSlope.Select(
                    x =>
                        new Vec2f(calibrationCheckSlope.Select(y => y.Item0).Average(),
                            calibrationCheckSlope.Select(y => y.Item1).Average())).First();
            return averageShiftVector;
        }
    }
}
