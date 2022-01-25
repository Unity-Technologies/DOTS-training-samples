using UnityEngine;

namespace Onboarding.BezierPath
{
    public class BezierCurveFitting
    {
        /// <summary>
        /// Non Linear Least square fitting of a dataset with a bezier spline
        /// WARNING: THE OUTPUT is not the 4 control points of a regular bezier equation
        /// but instead 4 constants to be used with the DEVELOPED version of the bezier equation !!!!
        /// f(t) = (dt³ + ct² + bt + a)
        /// 
        /// In our use case, the curve initial guess for P0, and P1 is perfect, as we know
        /// the curve passes by these points
        /// Also, we know that curve is continuous and the function will converge.
        /// Thus, this function is not intended for general purpose curve fitting, it assumes all the above, 
        /// as well as at least 4 points are passed in
        /// 
        /// (P1 & P2) are obtained by partially differentiating the sum of square distance
        /// between each sample 'i' and the parametric curve with respect to P1 and P2 at the corresponding 't'
        /// and then solving for the two unknowns P1 and  P2.
        /// 
        /// </summary>
        // --------------------------------------------------------------------------------------------
        public static void LeastSquareFit_FastBezier(Vector2[] lengthMapping, int start, int end, out float a, out float b, out float c, out float d)
        {

            double distStart = lengthMapping[start].x;
            double distRange = lengthMapping[end].x - distStart;

            // P0 and P1 are set, since we already know where they land
            double P0asDouble = lengthMapping[start].y;
            double P3asDouble = lengthMapping[end].y;

            // 
            double SumOfB1Squared = 0, SumOfB2Squared = 0, C1 = 0, C2 = 0, SumOfB1TimesB2 = 0;
            for (int i = start; i <= end; ++i)
            {
                double t = (lengthMapping[i].x - distStart) / distRange;
                double t2 = t * t;
                double t3 = t2 * t;
                double t4 = t3 * t;
                double _1_t = 1 - t;
                double _1_t2 = _1_t * _1_t;
                double _1_t3 = _1_t2 * _1_t;
                double _1_t4 = _1_t3 * _1_t;

                // Bezier : 
                //      f(t) = P0(1-t)³ + 3P1t(1-t)² + 3P2t²(1-t) + P3t³
                // so
                //      f(t) = P0*B0 + P1*B1 + P2*B2 + P3*B3
                // with :
                //      B0 = (1-t)³ 
                //      B1 = 3t(1-t)²
                //      B2 = 3t²(1-t)
                //      B3 = t³

                // Update sum of B1 et B2 for the P1 and P2
                SumOfB1Squared += 9 * t2 * _1_t4; // += B1² => 9t²(1-t)^4
                SumOfB2Squared += 9 * t4 * _1_t2; // += B2² => 9t^4(1-t)²
                SumOfB1TimesB2 += 9 * t3 * _1_t3; // += B1xB2 => 9t³(1-t)³

                double partialResidual = (lengthMapping[i].y - _1_t3 * P0asDouble - t3 * P3asDouble);
                // C1 = C1 + B1*( sample[i] - B0*P0 - B3*P3 );
                C1 += 3 * t * _1_t2 * partialResidual;

                // C2 = C2 + B2*( sample[i] - B0*P0 - B3*P3 );
                C2 += 3 * t2 * _1_t * partialResidual;
            }

            // the easy ones we already know
            a = (float)P0asDouble;
            d = (float)P3asDouble;

            // Solve for the other 2
            double denom = (SumOfB1Squared * SumOfB2Squared - SumOfB1TimesB2 * SumOfB1TimesB2);
            b = (float)((SumOfB2Squared * C1 - SumOfB1TimesB2 * C2) / denom);
            c = (float)((SumOfB1Squared * C2 - SumOfB1TimesB2 * C1) / denom);

            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            // NOT PART OF THE BASIC LEAST SQUARE ALGORITHM
            // !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

            // Develop the Bezier equation, so it's faster at runtime to consume

            //   a(1-t)³ + 3bt(1-t)² + 3ct²(1-t) + dt³
            // = a(1-3t+3t²-t³) + 3bt(t²-2t+1) + 3c(t²-t³) + dt³
            // = a-3at+3at²-at³ + 3bt³-6bt²+3bt + 3ct²-3ct³ + dt³
            // = a + t(-3a+3b) + t²(3a-6b+3c) + t³(-a+3b-3c+d)
            float dP1 = -3 * a + 3 * b;
            float dP2 = 3 * a - 6 * b + 3 * c;
            float dP3 = -a + 3 * b - 3 * c + d;

            b = dP1;
            c = dP2;
            d = dP3;
        }

        public static void LeastSquareFit_Bezier(Vector2[] samples, 
                                          out float c0, 
                                          out float c1, 
                                          out float c2, 
                                          out float c3)
        {
            // C0 and C1 are set right away, since the Bezier
            // goes through them
            double C0 = samples[0].y;
            double C3 = samples[samples.Length-1].y;

            // B0 = (1-x)³ 
            // B1 = 3x(1-x)²
            // B2 = 3x²(1-x)
            // B3 = x³
            //
            // B1² = 9x²(1-x)^4
            // B2² = 9x^4(1-x)²
            // B1 x B2 => 9x³(1-x)³
            //
            // a = SUM(i, 1, n) [ B1(x)² ]
            // b = SUM(i, 1, n) [ B1(x)B2(x) ]
            // c = SUM(i, 1, n) [ B1(x)(y - B0(x)C0 - B3(x)C3) ]
            // e = SUM(i, 1, n) [ B2(x)² ]
            // f = SUM(i, 1, n) [ B2(x)(y - B0(x)C0 - B3(x)C3)) ]
            //

            double a = 0, b = 0, c = 0, e = 0, f = 0;

            for (int i = 0; i < samples.Length; ++i)
            {
                double x = samples[i].x;
                double x_pow2 = x * x;
                double x_pow3 = x_pow2 * x;
                double x_pow4 = x_pow3 * x;
                double one_minus_x = 1 - x;
                double one_minus_x_pow2 = one_minus_x * one_minus_x;
                double one_minus_x_pow3 = one_minus_x_pow2 * one_minus_x;
                double one_minus_x_pow4 = one_minus_x_pow3 * one_minus_x;

                // Update SUM(i, 1, n) [ B1(x)² ]
                a += 9 * x_pow2 * one_minus_x_pow4;
                // Update SUM(i, 1, n) [ B1(x)B2(x) ]
                e += 9 * x_pow4 * one_minus_x_pow2;
                // Update SUM(i, 1, n) [ B1(x)B2(x) ]
                b += 9 * x_pow3 * one_minus_x_pow3;

                double r = (samples[i].y - one_minus_x_pow3 * C0 - x_pow3 * C3);

                // Update SUM(i, 1, n) [ B1(x)(y - B0(x)C0 - B3(x)C3) ]
                c += 3 * x * one_minus_x_pow2 * r;

                // Update SUM(i, 1, n) [ B2(x)(y - B0(x)C0 - B3(x)C3)) ]
                f += 3 * x_pow2 * one_minus_x * r;
            }

            // the easy ones we already know
            c0 = (float)C0;
            c3 = (float)C3;

            // Solve for the other 2
            double denom = (a * e - b * b);
            c1 = (float)((c * e - b * f) / denom);
            c2 = (float)((a * f - b * c) / denom);
        }
    }
}
