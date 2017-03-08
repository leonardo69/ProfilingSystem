using System;
using System.Drawing;
using Profiling.Core;
using ZedGraph;
using Point = Profiling.Core.Point;

namespace Profiling
{
    public class Calculator
    {
        private Form2 _form2;
        private readonly double Ln = 1.154338843;
        private readonly double Le = 1.98725381;

        public Calculator(Form2 form2)
        {
            _form2 = form2;
        }

        public void CalcXY()
        {
            double L, LL, Xd1, Yd1, r, a1, G, Lt, X1, Y1, Z1, Z, cosLt, sinLt, sqrtT, h1, h2, Si, S0, B, B1, B2,
                Xdn, Ydn, Gn, Ltn, cosLtn, Y1n, Zn, L1, L2, U, X2_, Y2_, Z2_, O, cosO, sinO, X2t, Y2t,
                cosZ2_h2, sinZ2_h2, arctgT;

        
           
            // double R1 , R2 , w;
            double R1 = _form2.R1, R2 = _form2.R2, w = _form2.w;  //TODO: убрать R2=3 
            r = 2 * R1;
            a1 = r + 0.4168 * r / 2;
            _form2.h1 = h1  = R1 / Math.Tan(w);
            B = Math.Atan(h1 / R1);
            B1 = Math.PI / 2 - B;
            _form2.O = O =  B + Math.Asin(R1 * 0.5 * Math.Sin(B) / R2);
            _form2.h2 = h2 = R2 * Math.Tan(O - B);
            B2 = Math.PI / 2 - (O - B);
            _form2.U = U = R2 * Math.Cos(B2) / (R1 * Math.Cos(B1));
            LL = R1 + R2;

            Xdn = a1 - r * Math.Sin(Ln);
            Ydn = r * Math.Cos(Ln);
            Gn = Math.Sqrt(Xdn * Xdn + Ydn * Ydn);
            Ltn = Math.Acos(a1 * Math.Cos(Ln) / Gn);
            cosLtn = Math.Cos(Ltn);

            Y1n = Gn * (Math.Sqrt(R1 * R1 - (Gn * cosLtn) * (Gn * cosLtn)) - Gn * Math.Sin(Ltn)) * cosLtn / R1;
            Zn = h1 * (Math.Acos(Gn * cosLtn / R1) - Math.Atan(Ydn / Xdn) - Ltn);
            S0 = -(Y1n + Zn * Math.Tan(B)) / Math.Tan(B);

            double dL = (Le - Ln) / (_form2.act_quant - 1);
            for (int i = 0; i < _form2.act_quant; i++)
            {
                L = Ln + i * dL;

                _form2.arraySV[i].X = Xd1 = a1 - r * Math.Sin(L);
                _form2.arraySV[i].Y = Yd1 = r * Math.Cos(L);

                G = Math.Sqrt(Xd1 * Xd1 + Yd1 * Yd1);
                Lt = Math.Acos(a1 * Math.Cos(L) / G);

                cosLt = Math.Cos(Lt);
                sinLt = Math.Sin(Lt);
                sqrtT = Math.Sqrt(R1 * R1 - (G * cosLt) * (G * cosLt));

                X1 = G * (G * cosLt * cosLt + sqrtT * sinLt) / R1;
                Y1 = G * (sqrtT - G * sinLt) * Math.Cos(Lt) / R1;
                Z = h1 * (Math.Acos(G * cosLt / R1) - Math.Atan(Yd1 / Xd1) - Lt);

                Si = -(Y1 + Z * Math.Tan(B)) / Math.Tan(B);
                Z1 = Z + Si;

                _form2.array4[i].X = X1;
                _form2.array4[i].Y = Y1;
                _form2.array4[i].Z = Z1;

                L1 = S0 - Si;
                if (L1 < 0) L1 = -L1;

                L1 /= h1;
                L2 = L1 / U;

                sinO = Math.Sin(O);
                cosO = Math.Cos(O);
                X2_ = (X1 - LL) * Math.Cos(L2) + (Y1 * cosO + Z1 * sinO) * Math.Sin(L2);
                Y2_ = (Y1 * cosO + Z1 * sinO) * Math.Cos(L2) + (LL - X1) * Math.Sin(L2);
                Z2_ = -Y1 * sinO + Z1 * cosO;

                _form2.array3[i].X = X2_;
                _form2.array3[i].Y = Y2_;
                _form2.array3[i].Z = Z2_;

                cosZ2_h2 = Math.Cos(Z2_ / h2);
                sinZ2_h2 = Math.Sin(Z2_ / h2);
                X2t = X2_ * cosZ2_h2 + Y2_ * sinZ2_h2;
                Y2t = Y2_ * cosZ2_h2 - X2_ * sinZ2_h2;

                _form2.array1[i].X = Math.Sqrt(X2_ * X2_ + Y2_ * Y2_);
                arctgT = Math.Atan(Y2_ / X2_);
                if ((arctgT < 0) && (L < (Ln + Le) / 2)) arctgT += Math.PI;
                _form2.array1[i].Z = Z2_ - h2 * arctgT;
                _form2.array1[i].Y = 0;
            }

            
            _form2.dZ = _form2.dZ = _form2.array1[_form2.act_quant - 1].Z - _form2.array1[0].Z;
        }

        public void RefreshPoints()
        {

            double L, dL;
            int i;
            int act_quant = 51;
            _form2.richTextBox1.Clear();
            string str;

            _form2.richTextBox1.Text += "L\t\tX0\t\tZ0";
            _form2.richTextBox1.Text += "\n\n";

            dL = (Le - Ln) / (act_quant - 1);

            CalcXY();

            for (i = 0; i < act_quant; i++)
            {
                L = dL * i + Ln;
                str =String.Format("{0:0.000000}\t\t{1:0.000000}\t\t{2:0.000000}", Math.Round(L,6),Math.Round(_form2.array1[i].X,6),Math.Round(_form2.array1[i].Z,6))+"\n";
                _form2.richTextBox1.Text += str;

            }

            Point C = new Point();
            Point a, b, c;
            double Ro;
            C.Set(0, 0, 0);

            a = _form2.array1[0].Clone();
            b = _form2.array1[act_quant / 4].Clone(); b.Substr(a);
            c = _form2.array1[act_quant / 2].Clone(); c.Substr(a);

            C.X = 2 * (b.Z * c.X - c.Z * b.X);
            if ((C.X > -0.0000001) && (C.X < 0.00000001)) C.X = 999999999;
            else C.X = (b.Z * (c.Z * c.Z + c.X * c.X) - c.Z * (b.Z * b.Z + b.X * b.X)) / C.X;

            C.Z = 2 * b.Z;
            if ((C.Z > -0.0000001) && (C.Z < 0.00000001)) C.Z = 999999999;
            else C.Z = (b.X * b.X - 2 * b.X * C.X + b.Z * b.Z) / C.Z;

            Ro = Math.Sqrt(C.X * C.X + C.Z * C.Z);
            C.Add(a);

            for (i = 0; i < act_quant / 2; i++)
            {
                a = _form2.array1[i].Clone(); a.Substr(C);
                a.Normaliz();
                a.MultConst(Ro);
                a.Add(C);
                _form2.arrayO[i].Z = a.Z;
                _form2.arrayO[i].X = a.X;
            }


            _form2.richTextBox1.Text += "\n";
            str = "Шаг 2D сетки = 0.1 ед\n";
            _form2.richTextBox1.Text += str;

            _form2.richTextBox1.Text += "\n";
            str = "Радиус аппроксимирующих окружностей = " + Math.Round(Ro, 6) + "\n";
            _form2.richTextBox1.Text += str;

            str = "Коорд. окр. 1:   Z=" +Math.Round(C.Z,6) + "  X=" + Math.Round(C.X,6)+ "\n";
            _form2.richTextBox1.Text += str;


            int numberElement = act_quant/2;
            double temp = _form2.array1[numberElement].Z;
            double doub = 2*temp;
            double cs = -C.Z;

            double coord2 = -C.Z + 2*_form2.array1[act_quant/2].Z;
            str = "Коорд. окр. 2:   Z=" + Math.Round(coord2,6) + "  X=" + Math.Round(C.X,6)+ "\n";
            _form2.richTextBox1.Text += str;

            str = "Сумма максимальных отклонение = " + Math.Round((double) CalcE(), 6);
            _form2.richTextBox1.Text += str;


            DrawChart();

            _form2.Show3D();
        }

        private double CalcE()   
        {
            int i;
            double St, Smax1 = 0, Smax2 = 0, x, y;

            for (i = 0; i < _form2.act_quant / 2; i++)
            {
                x = _form2.arrayO[i].Z - _form2.array1[i].Z;
                y = _form2.arrayO[i].X - _form2.array1[i].X;
                St = Math.Sqrt(x * x + y * y);
                if ((x < 0) && (St > Smax1)) Smax1 = St;
                if ((x > 0) && (St > Smax2)) Smax2 = St;
            }
            return Smax2 + Smax1;

        }

        private void DrawChart()
        {
            //Рисуем график
            GraphPane pane = _form2.zedGraphControl1.GraphPane;

            pane.CurveList.Clear();
            pane.IsShowTitle = false;

            // Создадим список точек
            PointPairList list = new PointPairList();
            PointPairList list2 = new PointPairList();
            PointPairList list3 = new PointPairList();
           
            for (int i = 0; i < _form2.act_quant; i++)
            {
                list3.Add(_form2.array1[i].Z, _form2.array1[i].X);
            }

            for (int i = 0; i < _form2.act_quant/2; i++)
            {
                list.Add(_form2.arrayO[i].Z, _form2.arrayO[i].X);
            }

            for (int i = 0; i < _form2.act_quant/2; i++)
            {
                list2.Add(-_form2.arrayO[i].Z + 2 *_form2.array1[_form2.act_quant / 2].Z, _form2.arrayO[i].X);
            }
   
            LineItem myCurve = pane.AddCurve("Line1", list, Color.Black, SymbolType.None);
            LineItem myCurve2 = pane.AddCurve("Line2", list2, Color.Blue, SymbolType.None);
            LineItem myCurve3 = pane.AddCurve("Line3", list3, Color.Brown, SymbolType.None);
            _form2.zedGraphControl1.AxisChange();

            // Обновляем график
            _form2.zedGraphControl1.Invalidate();
        }
    }
}