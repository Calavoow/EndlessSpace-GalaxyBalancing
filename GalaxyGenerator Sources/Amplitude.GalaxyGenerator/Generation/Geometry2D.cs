// <copyright file="Geometry2D.cs" company="AMPLITUDE Studios">Copyright AMPLITUDE Studios. All rights reserved.</copyright>

namespace Amplitude.GalaxyGenerator.Generation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Drawing;

    public static class Geometry2D
    {
        // computes the intersection of line segment [ab] and line segment [mn].
        // The intersection is i.
        // The return result defines the type of intersection according to the IntersectionType enum.
        // i remains untouched if the segments run parallel to each other.
        public enum IntersectionType {Parallel, OutsideSegment, InsideSegment}
        public static IntersectionType Intersection(PointF a, PointF b, PointF c, PointF d, ref PointF i)
        {
            PointF ab = MakeVector(a, b);
            PointF cd = MakeVector(c, d);

            if(AreParallelVectors(ab, cd))
                return IntersectionType.Parallel;
            
            i.X = (ab.X * cd.X * c.Y - ab.X * cd.X * a.Y + ab.Y * cd.X * a.X - ab.X * cd.Y * c.X) / (ab.Y * cd.X - cd.Y * ab.X);
            i.Y = (ab.Y * cd.X * c.Y - ab.X * cd.Y * a.Y + ab.Y * cd.Y * a.X - ab.Y * cd.Y * c.X) / (ab.Y * cd.X - cd.Y * ab.X);

            if ((Value(a, c, d) * Value(b, c, d) <= 0) && (Value(c, a, b) * Value(d, a, b) <= 0))
                return IntersectionType.InsideSegment;
            else
                return IntersectionType.OutsideSegment;
        }

        public static IntersectionType IntersectionCheck(PointF a, PointF b, PointF c, PointF d)
        {
            PointF ab = MakeVector(a, b);
            PointF cd = MakeVector(c, d);

            if (AreParallelVectors(ab, cd))
                return IntersectionType.Parallel;

            if ((Value(a, c, d) * Value(b, c, d) <= 0) && (Value(c, a, b) * Value(d, a, b) <= 0))
                return IntersectionType.InsideSegment;
            else
                return IntersectionType.OutsideSegment;
        }

        // computes the euclidian distance between a and b
        public static float Distance(PointF a, PointF b)
        {
            return (float)System.Math.Sqrt((a.X-b.X)*(a.X-b.X) + (a.Y-b.Y)*(a.Y-b.Y));
        }
        
        // computes the length of v treated as a vector
        public static float Length(PointF v)
        {
            return (float)System.Math.Sqrt(v.X*v.X + v.Y*v.Y);
        }

        // return true if the vector could be normalized (length != 0), false otherwise
        public static bool Normalized(ref PointF v)
        {
            float lg = Length(v);

            if (lg == 0)
            {
                return false;
            }
            else
            {
                MultiplyVector(1/lg, ref v);
                return true;
            }
        }

        public static PointF MakeVector(PointF a, PointF b)
        {
            return new PointF(b.X - a.X, b.Y - a.Y);
        }

        // returns a normal vector for line (ab)
        // it is not normalized
        public static PointF NormalVector(PointF a, PointF b)
        {
            return new PointF(a.Y - b.Y, b.X - a.X);
        }

        public static PointF NormalVector(PointF v)
        {
            return new PointF(-v.Y, v.X);
        }

        public static PointF SumVectors(PointF v, PointF w)
        {
            return new PointF(v.X + w.X, v.Y + w.Y);
        }

        public static PointF Translated(PointF p, PointF v)
        {
            PointF q = new PointF();
            q.X = p.X + v.X;
            q.Y = p.Y + v.Y;
            return q;
        }

        public static PointF MultiplyVector(float q, ref PointF v)
        {
            v.X = q * v.X;
            v.Y = q * v.Y;
            return v;
        }

        public static bool AreParallelVectors(PointF v, PointF w)
        {
            PointF vp = v;
            Normalized(ref vp);

            PointF wp = w;
            Normalized(ref wp);

            if (vp == wp)
                return true;

            return false;
        }

        // computes the result of line(ab) equation for point p
        // this value is 0 if p is on (ab)
        public static float Value(PointF p, PointF a, PointF b)
        {
            PointF d = MakeVector(a, b);

            return d.Y * p.X - d.X * p.Y + d.X * a.Y - d.Y * a.X;
        }

        // returns a point that is the symmetrical opposite of p across line (ab)
        public static PointF Symmetrical(PointF p, PointF a, PointF b)
        {
            PointF h, n, q;

            h = new PointF();

            if (Value(p, a, b) == 0)
                return p;
            else
            {
                n = NormalVector(a, b);
                Intersection(a, b, p, Translated(p, n), ref h);
                q = MakeVector(p, h);
                MultiplyVector(2, ref q);
                return Translated(p, q);
            }
        }

        //will modify p.X and p.Y to match polar coordinates rho,theta with angles in 360 degrees North at 0 clockwise
        public static void FromPolar(ref PointF p, float rho, float theta)
        {
            p.X = rho * (float)Math.Cos((double)(90 - theta) * Math.PI / 180);
            p.Y = rho * (float)Math.Sin((double)(90 - theta) * Math.PI / 180);
        }

        public static float Bearing(PointF a, PointF b)
        {
            double f, g;
            double brg;

            f = b.X - a.X;
            g = b.Y - a.Y;

            if (f != 0)
            {
                if (g > 0)
                    brg = Math.Atan(f / g) * 180 / Math.PI;
                else
                    brg = 180 + Math.Atan(f / g) * 180 / Math.PI;
                if (brg < 0)
                    brg += 360;
            }
            else
                if (f == 0)
                    brg = 0;
                else
                    if (f > 0)
                        brg = 270;
                    else
                        brg = 90;

            return (float)brg;
        }
    }
}
