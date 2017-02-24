using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiling.Core
{
    //Участвует в построении 3Д модели
    class MyRotate
    {
        //размерность матрицы

        double[] matrix = new double[16];


        //Операции матрицы поворота
        public void SetRotate(double angle, double x, double y, double z)
        {
            double c, s;
            c = Math.Cos(angle);
            s = Math.Sin(angle);

            matrix[15] = 1;
            matrix[3] = matrix[7] = matrix[11] = matrix[12] = matrix[13] = matrix[14] = 0;

            matrix[0] = x * x * (1 - c) + c;
            matrix[4] = x * y * (1 - c) - z * s;
            matrix[8] = x * z * (1 - c) + y * s;

            matrix[1] = y * x * (1 - c) + z * s;
            matrix[5] = y * y * (1 - c) + c;
            matrix[9] = y * z * (1 - c) - x * s;

            matrix[2] = x * z * (1 - c) - y * s;
            matrix[6] = y * z * (1 - c) + x * s;
            matrix[10] = z * z * (1 - c) + c;

        }

        //Пересчитать новое положение точек
        public void TransformPoint(MyPoint in_point, MyPoint out_point)
        {
            out_point.X = matrix[0] * in_point.X +
                            matrix[4] * in_point.Y +
                            matrix[8] * in_point.Z +
                            matrix[12];
            out_point.Y = matrix[1] * in_point.X +
                            matrix[5] * in_point.Y +
                            matrix[9] * in_point.Z +
                            matrix[13];
            out_point.Z = matrix[2] * in_point.X +
                            matrix[6] * in_point.Y +
                            matrix[10] * in_point.Z +
                            matrix[14];
        }



    }
}
