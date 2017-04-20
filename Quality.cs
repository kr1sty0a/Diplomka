using System;

namespace OpenCVSharpSandbox
{
    public class Quality
    {
        public int TP;
        public int TN;
        public int FP;
        public int FN;

        public Quality(int tp, int tn, int fp, int fn) {
            TP = tp;
            TN = tn;
            FP = fp;
            FN = fn;
        }

        public double GetSensitivity() {
            var sensitivity = (double) TP/(double) (TP + FN);
            return sensitivity;
        }

        public double GetSpecificity() {
            var specificity = (double) TN/(double) (TN + FP);
            if (specificity == Double.NaN) {
                throw new DivideByZeroException();
            }
            return specificity;
        }
    }
}
