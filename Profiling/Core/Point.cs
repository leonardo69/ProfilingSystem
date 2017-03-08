using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Collections.Generic;

namespace Profiling.Core
{
   
    public class Point
    {
        public double X, Y, Z;

        public void Set (double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public void Substr (Point point2)
        {
            X -= point2.X;
            Y -= point2.Y;
            Z -= point2.Z;
        }

        public void Add(Point point2)
        {
            X += point2.X;
            Y += point2.Y;
            Z += point2.Z;
        }

        public void MultConst(double k)
        {
            X *= k;
            Y *= k;
            Z *= k;

        }

        public Point Clone()
        {
            Point newPoint = new Point();
            newPoint.X = X;
            newPoint.Y = Y;
            newPoint.Z = Z;

            return newPoint;

        }

        public void VectorMult(Point p1, Point p2)
        {
            X = (p1.Y * p2.Z) - (p2.Y * p1.Z);
            Y = -(p1.X * p2.Z) + (p2.X * p1.Z);
            Z = (p1.X * p2.Y) - (p2.X * p1.Y);
        }



        //вычисление нормали к плосоксти, заданной 3 точками

        public void NPlane(Point p1, Point p2, Point p3)
        {
            Point v1, v2;
            v1 = v2 = p1;
            v1.Substr(p2); v1.Normaliz();
            v2.Substr(p3); v2.Normaliz();
            VectorMult(v1, v2);
            Normaliz();
        }


        public void Normaliz()
        {
            double R = Math.Sqrt(X * X + Y * Y + Z * Z);
            X /= R; Y /= R; Z /= R;
        }

    }
}
