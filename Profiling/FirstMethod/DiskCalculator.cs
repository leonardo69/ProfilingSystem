using System;
using Profiling.Core;

namespace Profiling
{
   public class DiskCalculator
    {
        private Form1 _form1;

        public DiskCalculator(Form1 form1)
        {
            _form1 = form1;
        }

        public void RefreshPoints()
        {
            //обновление вычислений

            double L, L0, Tt, Rt, dL, Ro;
            int i;
            _form1.richTextBox1.Clear();
            string str;

            _form1.richTextBox1.Text += "L\t\tTt\t\tRt\n";
            _form1.richTextBox1.Text += "\n";

            if (_form1.FirstForm1) L0 = 1.154338843;   //определяем L0
            else L0 = 1.343481113;

            dL = Math.PI - L0 * 2;             //определяем dL
            dL = dL / (_form1.ActQuant - 1);

            for (i = 0; i < _form1.ActQuant; i++)
            {
                L = dL * i + L0;

                Yn(L, _form1.O1, _form1.R1, out Tt, out Rt, ref _form1.Array3D[i], ref _form1.ArraySv[i]);   //вызов функции Yn
                str = String.Format("{0:0.000000}\t\t{1:0.000000}               {2:0.000000}", Math.Round(L, 6), Math.Round(Tt, 6), Math.Round(Rt, 6)) + "\n";
                _form1.richTextBox1.Text += str;

                //заполняем массив точками, по которым будет строиться профиль
                _form1.Array[i].X = Tt;
                _form1.Array[i].Y = Rt;
                _form1.Array[i].Z = 0;
            }

            double a, h, Y, Rv;

            a = _form1.Array[_form1.ActQuant / 2].Y - _form1.Array[0].Y;
            h = _form1.Array[0].X;
            Ro = (h * h + a * a) / (2 * a);
            _form1.Rcenter1 = Y = _form1.Array[_form1.ActQuant / 2].Y - Ro;

            for (i = 0; i < _form1.ActQuant; i++)
            {
                //заполняется массив точками для построения второго профиля

                _form1.ArrayO[i].X = _form1.Array[i].X;
                _form1.ArrayO[i].Y = _form1.Array[i].Y - Y;
                Rv = Math.Sqrt(_form1.ArrayO[i].X *_form1.ArrayO[i].X + _form1.ArrayO[i].Y *_form1.ArrayO[i].Y);   //размерность массива? посмотреть его инициализацию
                _form1.ArrayO[i].X /= Rv;
                _form1.ArrayO[i].Y /= Rv;
                _form1.ArrayO[i].X *= Ro;
                _form1.ArrayO[i].Y *= Ro;
                _form1.ArrayO[i].Y += Y;
            }


            //переменные, которые выводятся в результат

            _form1.richTextBox1.Text += "\n";
            str = "Шаг 2D сетки = 0.1 ед\n";
            _form1.richTextBox1.Text += str;

            _form1.richTextBox1.Text += "\n";
            str = "Центр локальной СК: Tt'=0   Rt=" + Math.Round(_form1.Array[0].Y, 6) + "\n";
            _form1.richTextBox1.Text += str;
            str = "Радиус аппроксимирующей окружности = " + Math.Round(Ro, 6) + "\n";
            _form1.richTextBox1.Text += str; ;

            str = "Центр аппрокс. окр.: Tt=0    Rt=" + Math.Round(Y, 6) + "\n";
            _form1.richTextBox1.Text += str;

            str = "Максимальное отклонение = " + Math.Round((double) CalcE(), 6);
            _form1.richTextBox1.Text += str;

            // RefreshGLLists();
            //  InvalidateRect(Form1->Handle, NULL, 0);     
            _form1.DrawChart();


            _form1.DiskOpenGlHelper.Show3D();
        }

        private double CalcE()   //считаем CalcE
        {
            int i;
            double St, Smax = 0, x, y;

            for (i = 0; i < _form1.ActQuant; i++)
            {
                x = _form1.ArrayO[i].X - _form1.Array[i].X;
                y = _form1.ArrayO[i].Y - _form1.Array[i].Y;
                St = Math.Sqrt(x * x + y * y);
                if (St > Smax) Smax = St;
            }
            return Smax;

        }

        private double Yn(double L, double w, double R, out double Tt, out double Rt, ref Point Point3D, ref Point PSV)
        {
            double TT, RT, Ybeg, Yend, Ymid, Fbeg, Fend, Fmid, B;
            //выходные параметры TT, RT 



            Ybeg = -100 * R;
            Yend = 100 * R;
            Fbeg = F(L, w, R, Ybeg, out TT, out RT, Point3D, PSV);
            Fend = F(L, w, R, Yend, out TT, out RT, Point3D, PSV);

            while (((Yend - Ybeg) < -0.0001) || ((Yend - Ybeg) > 0.0001))
            {
                Ymid = (Yend + Ybeg) / 2;
                Fmid = F(L, w, R, Ymid, out TT, out RT, Point3D, PSV);
                if (Fmid == 0)
                {
                    Ybeg = Yend = Ymid;
                    break;
                }

                if ((Fbeg > 0) == (Fmid < 0))
                {
                    Yend = Ymid;
                    Fend = F(L, w, R, Yend, out TT, out RT, Point3D, PSV);
                }
                else
                {
                    Ybeg = Ymid;
                    Fbeg = F(L, w, R, Ybeg, out TT, out RT, Point3D, PSV);
                }
            }

            Ymid = (Yend + Ybeg) / 2;
            Fmid = F(L, w, R, Ymid, out TT, out RT, Point3D, PSV);
            Tt = TT;
            Rt = RT;
            return Ymid;
        }

        private double F(double L, double w, double R, double Yn, out double Tt, out double Rt, Point P3D, Point PSV)
        {
            double a1, r, Xtd, Ytd, G, Lt, d, h, Rn, tga, X_, Y_, Z_, bn, X, Y, Z, O, A, B, C, D;

            if (_form1.FirstForm1) a1 = 2.4168 * R;
            else a1 = 2.841212649 * R;

            if (_form1.DLink)
            {
                if (_form1.FirstForm1) d = 8 * R;
                else d = 8.424412649 * R;
            }
            else d = _form1.DIn;

            r = 2 * R;
            h = R / Math.Tan(w);
            O = Math.PI / 2 - w;
            if (_form1.FirstForm1) tga = 1 / (8 * Math.Tan(w));
            else tga = 0.1187026370 / Math.Tan(w);
            Xtd = a1 - r * Math.Sin(L);
            Ytd = r * Math.Cos(L);

            PSV.X = Xtd;
            PSV.Y = Ytd;
            PSV.Z = 0;

            G = Math.Sqrt(Xtd * Xtd + Ytd * Ytd);

            Lt = Math.Acos(a1 * Math.Cos(L) / G);
            double cosLt = Math.Cos(Lt);
            double sinLt = Math.Sin(Lt);

            Rn = Math.Sqrt(Yn * Yn + d * d);

            X_ = G * (G * cosLt * cosLt + Math.Sqrt(Rn * Rn - G * G * cosLt * cosLt) * sinLt) / Rn;
            Y_ = G * (Math.Sqrt(Rn * Rn - G * G * cosLt * cosLt) - G * sinLt) * cosLt / Rn;
            Z_ = h * (Math.Acos(G * cosLt / Rn) - Math.Atan(Ytd / Xtd) - Lt);

            bn = Math.Atan(Yn / d);

            X = X_ * Math.Cos(bn) - Y_ * Math.Sin(bn);
            Y = X_ * Math.Sin(bn) + Y_ * Math.Cos(bn);
            Z = Z_ + h * bn;

            P3D.X = X; P3D.Y = Y; P3D.Z = Z;
            A = Yn * (1 - tga / Math.Tan(O));
            if (_form1.FirstForm1)
            {
                B = -7 * R;
                C = -7 * R / (8 * Math.Tan(w));
            }
            else
            {
                B = -7.424412649 * R;
                C = -0.8812973625 * R / Math.Tan(w);
            }

            D = -R * Yn * (1 - tga / Math.Tan(O));

            Tt = Y * Math.Sin(O) - Z * Math.Cos(O);
            Rt = Math.Sqrt((Tt * Math.Sin(O) - Y) * (Tt * Math.Sin(O) - Y) + (d - X) * (d - X) +
                           (Tt * Math.Cos(O) + Z) * (Tt * Math.Cos(O) + Z));

            return A * X + B * Y + C * Z + D;
        }
    }
}