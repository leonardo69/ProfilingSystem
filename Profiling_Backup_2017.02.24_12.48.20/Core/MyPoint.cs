using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiling.Core
{
    //Участвует в построении профиля
    class MyPoint
    {
        public double X, Y, Z;

        public void Set (double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;

        }

        public void Substr (MyPoint point2)
        {
            this.X -= point2.X;
            this.Y -= point2.Y;
            this.Z -= point2.Z;
        }

        public void Add(MyPoint point2)
        {
            this.X += point2.X;
            this.Y += point2.Y;
            this.Z += point2.Z;
        }

        public void MultConst(double k)
        {
            this.X *= k;
            this.Y *= k;
            this.Z *= k;

        }

        public MyPoint Clone()
        {
            MyPoint newPoint = new MyPoint();
            newPoint.X = this.X;
            newPoint.Y = this.Y;
            newPoint.Z = this.Z;

            return newPoint;

         //   return new MyPoint(this.X, this.Y, this.Z);
        }

        public void VectorMult(MyPoint p1, MyPoint p2)
        {
            X = (p1.Y * p2.Z) - (p2.Y * p1.Z);
            Y = -(p1.X * p2.Z) + (p2.X * p1.Z);
            Z = (p1.X * p2.Y) - (p2.X * p1.Y);
        }



        //вычисление нормали к плосоксти, заданной 3 точками

        public void NPlane(MyPoint p1, MyPoint p2, MyPoint p3)
        {
            MyPoint v1, v2;
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
