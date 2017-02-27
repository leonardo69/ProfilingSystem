using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Profiling.Core;
using ZedGraph;
using Profiling.GUI;
using Tao.OpenGl;
using Tao.FreeGlut;
using Tao.Platform.Windows;

namespace Profiling
{
    public partial class Form2 : Telerik.WinControls.UI.RadForm
    {

#region Переменные


        bool FirstForm; //первая канавка
        bool View3D; //3D вид
        int act_quant; //Количество точек
        bool visDrill1 = true;
        bool visDrill2 = true;
        double w, R1, R2, Ro, h1, h2, O, dZ, U, F1; //параметры  модели

        bool d_link = true, illumination = true;//для отображения
        

        MyPoint[] array1;
        MyPoint[] array3;
        MyPoint[] array4;
        MyPoint[] arraySV;
        MyPoint[] arrayO;
        MyPoint C;


        float firstStyle;
        float secondStyle;

#endregion

        //конструктор (метод который выполняется первым делом и настраивает интерфейс программы)
        public Form2()
        {
            InitializeComponent();
            anT.InitializeContexts();
            InitStartParams();
            firstStyle = tableLayoutPanel1.RowStyles[1].Height;
            secondStyle = tableLayoutPanel1.RowStyles[2].Height;
        }
        
        //инициализация начальных переменных программы
        private void InitStartParams()
        {
            //инициализируем переменные для отображения

            FirstForm = true;
            View3D = false;
            act_quant = 7;
            d_link = true;
           // d_in = 1; // угол

            illumination = false;
      

            ///переменные для расчёта стартового

            w = Math.PI*45/180;
            R1 = 3;
            R2 = 12.5;
            
      

            //инициализируем списки
            array1 = new MyPoint[act_quant];
            array3 = new MyPoint[act_quant];
            array4 = new MyPoint[act_quant];
            arraySV = new MyPoint[act_quant];
            arrayO = new MyPoint[act_quant];

            for (int i = 0; i < act_quant; i++)
            {
                array1[i] = new MyPoint();
                array3[i] = new MyPoint();
                array4[i] = new MyPoint();
                arraySV[i] = new MyPoint();
                arrayO[i] = new MyPoint();
            }


            //настраиваем положение трекбаров
            trackBar1.Value = trackBar2.Maximum / 2;
            trackBar2.Value = trackBar2.Maximum / 2;
            trackBar3.Value = trackBar3.Maximum / 2;
            
            trackBar3_Scroll(null, null);
        }
        
        #region Трекеры

        //обработчики: берёт значение из интерфейса и передаёт переменной

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //изменяется переменная R радиус
            R1 = double.Parse(textBox4.Text) +
                (double.Parse(textBox5.Text) - double.Parse(textBox4.Text)) * trackBar1.Value / trackBar1.Maximum;
            textBox1.Text = R1.ToString();
            textBox6.Text = R1.ToString();
            RefreshPoints();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            
            w = Math.PI * ((double)trackBar3.Value * 0.88 + 1) / 180;
            textBox3.Text = (double.Parse(trackBar3.Value.ToString()) * 0.88 + 1).ToString();
            RefreshPoints();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            
            R2 = double.Parse(textBox6.Text) +
                (double.Parse(textBox7.Text) - double.Parse(textBox6.Text)) * trackBar2.Value / trackBar2.Maximum;
            textBox2.Text = R2.ToString();

            //пересчитать
            RefreshPoints();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                trackBar2.Enabled = true;
                d_link = false;
            }
            else
            {
                trackBar2.Enabled = false;
                d_link = true;
            }

            //пересчитать
        }


        #endregion

        #region  Вычислительная часть


        //Главный метод для расчёта мат модели. Вычисляет значения переменных и выводит на экран в текстбокс

        double Ln = 1.154338843;
        double Le = 1.98725381;


        public void CalcXY()
        {
            double L, LL, Xd1, Yd1, r, a1, G, Lt, X1, Y1, Z1, Z, cosLt, sinLt, sqrtT, h1, h2, Si, S0, B, B1, B2,
               Xdn, Ydn, Gn, Ltn, cosLtn, Y1n, Zn, L1, L2, U, X2_, Y2_, Z2_, O, cosO, sinO, X2t, Y2t,
               cosZ2_h2, sinZ2_h2, arctgT;

           // double R1 , R2 , w;

            r = 2 * R1;
            a1 = r + 0.4168 * r / 2;
            h1  = R1 / Math.Tan(w);
            B = Math.Atan(h1 / R1);
            B1 = Math.PI / 2 - B;
            O =  B + Math.Asin(R1 * 0.5 * Math.Sin(B) / R2);
            h2 = R2 * Math.Tan(O - B);
            B2 = Math.PI / 2 - (O - B);
            U = R2 * Math.Cos(B2) / (R1 * Math.Cos(B1));
            LL = R1 + R2;

            Xdn = a1 - r * Math.Sin(Ln);
            Ydn = r * Math.Cos(Ln);
            Gn = Math.Sqrt(Xdn * Xdn + Ydn * Ydn);
            Ltn = Math.Acos(a1 * Math.Cos(Ln) / Gn);
            cosLtn = Math.Cos(Ltn);

            Y1n = Gn * (Math.Sqrt(R1 * R1 - (Gn * cosLtn) * (Gn * cosLtn)) - Gn * Math.Sin(Ltn)) * cosLtn / R1;
            Zn = h1 * (Math.Acos(Gn * cosLtn / R1) - Math.Atan(Ydn / Xdn) - Ltn);
            S0 = -(Y1n + Zn * Math.Tan(B)) / Math.Tan(B);

            double dL = (Le - Ln) / (act_quant - 1);
            for (int i = 0; i < act_quant; i++)
            {
                L = Ln + i * dL;

                arraySV[i].X = Xd1 = a1 - r * Math.Sin(L);
                arraySV[i].Y = Yd1 = r * Math.Cos(L);

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

                array4[i].X = X1; array4[i].Y = Y1; array4[i].Z = Z1;

                L1 = S0 - Si;
                if (L1 < 0) L1 = -L1;

                L1 /= h1;
                L2 = L1 / U;

                sinO = Math.Sin(O);
                cosO = Math.Cos(O);
                X2_ = (X1 - LL) * Math.Cos(L2) + (Y1 * cosO + Z1 * sinO) * Math.Sin(L2);
                Y2_ = (Y1 * cosO + Z1 * sinO) * Math.Cos(L2) + (LL - X1) * Math.Sin(L2);
                Z2_ = -Y1 * sinO + Z1 * cosO;

                array3[i].X = X2_; array3[i].Y = Y2_; array3[i].Z = Z2_;

                cosZ2_h2 = Math.Cos(Z2_ / h2);
                sinZ2_h2 = Math.Sin(Z2_ / h2);
                X2t = X2_ * cosZ2_h2 + Y2_ * sinZ2_h2;
                Y2t = Y2_ * cosZ2_h2 - X2_ * sinZ2_h2;

                array1[i].X = Math.Sqrt(X2_ * X2_ + Y2_ * Y2_);
                arctgT = Math.Atan(Y2_ / X2_);
                if ((arctgT < 0) && (L < (Ln + Le) / 2)) arctgT += Math.PI;
                array1[i].Z = Z2_ - h2 * arctgT;
                array1[i].Y = 0;
            }
            dZ = array1[act_quant - 1].Z - array1[0].Z;
        }

        public void RefreshPoints()
        {
            
            double L, dL;
            int i;
            int act_quant = this.act_quant;
            richTextBox1.Clear();
            string str=string.Empty;

            str+="L\t\tX0\t\tZ0";
            str+="";

            dL = (Le - Ln) / (act_quant - 1);

            CalcXY();

            for (i = 0; i < act_quant; i++)
            {
                L = dL * i + Ln;

                str += L+"\t\t" + array1[i].X + "\t\t"+ array1[i].Z;
                
                richTextBox1.Text+=str;
            }

            this.C = new MyPoint();
            MyPoint a, b, c ;
            double Ro;
            C.Set(0, 0, 0);

            a = array1[0];
            b = array1[act_quant / 4]; b.Substr(a);
            c = array1[act_quant / 2]; c.Substr(a);

            C.X = 2 * (b.Z * c.X - c.Z * b.X);
            if ((C.X > -0.0000001) && (C.X < 0.00000001)) C.X = 999999999;
            else C.X = (b.Z * (c.Z * c.Z + c.X * c.X) - c.Z * (b.Z * b.Z + b.X * b.X)) / C.X;

            C.Z = 2 * b.Z;
            if ((C.Z > -0.0000001) && (C.Z < 0.00000001)) C.Z = 999999999;
            else C.Z = (b.X * b.X - 2 * b.X * C.X + b.Z * b.Z) / C.Z;

            Ro =  Math.Sqrt(C.X * C.X + C.Z * C.Z);
            C.Add(a);

            for (i = 0; i < act_quant / 2; i++)
            {
                a = array1[i]; a.Substr(C);
                a.Normaliz();
                a.MultConst(Ro);
                a.Add(C);
                arrayO[i].Z = a.Z;
                arrayO[i].X = a.X;
            }

        


            //переменные, которые выводятся в результат

            richTextBox1.Text += "\n";
            str = "Шаг 2D сетки = 0.1 ед\n";
            richTextBox1.Text += str;

            richTextBox1.Text += "\n";
            str = "Радиус аппроксимирующих окружностей = " + Math.Round(Ro, 6) + "\n";
            richTextBox1.Text += str; 

            richTextBox1.Text += "\n";
            str = "Коорд. окр. 1:   Z="+C.Z+"  X="+ C.X;
            richTextBox1.Text += str;

            richTextBox1.Text += "\n";
            str ="Коорд. окр. 2:   Z="+ (-C.Z + 2 * array1[act_quant / 2].Z) + "  X="+ C.X;
            richTextBox1.Text += str;

            str = "Сумма максимальных отклонение = " + Math.Round(CalcE(), 6);
            richTextBox1.Text += str;

             
            DrawChart();

            Show3D();
        }
        
        private double CalcE()   //считаем CalcE
        {
            int i;
            double St, Smax1 = 0, Smax2 = 0, x, y;

            for (i = 0; i < act_quant / 2; i++)
            {
                x = arrayO[i].Z - array1[i].Z;
                y = arrayO[i].X - array1[i].X;
                St = Math.Sqrt(x * x + y * y);
                if ((x < 0) && (St > Smax1)) Smax1 = St;
                if ((x > 0) && (St > Smax2)) Smax2 = St;
            }
            return Smax2 + Smax1;

        }
        
        
        

        //Рисуем графики профилей
        private void DrawChart()
        {
            //Рисуем график
            GraphPane pane = zedGraphControl1.GraphPane;

            // Очистим список кривых на тот случай, если до этого сигналы уже были нарисованы
            pane.CurveList.Clear();
            pane.IsShowTitle = false;

            // Создадим список точек
            PointPairList list = new PointPairList();
            PointPairList list2 = new PointPairList();

            // Заполняем список точек
            for (int i = 0; i < act_quant; i++)
            {
                // добавим в список точку
                list.Add(arrayO[i].X, arrayO[i].Y);
                list2.Add(array[i].X, array[i].Y);
            }

            // Создадим кривую с названием "Sinc", 
            // которая будет рисоваться голубым цветом (Color.Blue),
            // Опорные точки выделяться не будут (SymbolType.None)
            LineItem myCurve = pane.AddCurve("Line1", list, Color.Black, SymbolType.None);
            LineItem myCurve2 = pane.AddCurve("Line2", list2, Color.Blue, SymbolType.None);

            zedGraphControl1.AxisChange();

            // Обновляем график
            zedGraphControl1.Invalidate();
        }



        //GUI (интерфейс)
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            FirstForm = true;
            RefreshPoints();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            FirstForm = false;
            RefreshPoints();
        }


       //GUI Сохранение графика
        private void saveChart_Click(object sender, EventArgs e)
        {

            using (SaveFileDialog sd = new SaveFileDialog())
            {
                sd.Filter = ".png| *.png";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    GraphPane pane = zedGraphControl1.GraphPane;
                    pane.Image.Save(sd.FileName);
                }
            }
        }

        //Сохранение результатов вычисления в файл
        private void saveResult_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog sd = new SaveFileDialog())
            {
                sd.Filter = ".rtf| *.rtf";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    richTextBox1.SaveFile(sd.FileName);
                }
            }
        }

        #endregion
        

        #region OpenGl

        //Построение 3D модели

        //Настройка стартовых параметров
        private void Form1_Load(object sender, EventArgs e)
        {
            Glut.glutInit();
            Glut.glutInitDisplayMode(Glut.GLUT_RGB | Glut.GLUT_DOUBLE | Glut.GLUT_DEPTH);

            // отчитка окна 
            Gl.glClearColor(255, 255, 255, 1);

            // установка порта вывода в соответствии с размерами элемента anT 
            Gl.glViewport(0, 0, anT.Width, anT.Height);


            // настройка проекции 
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(35, (float)anT.Width / (float)anT.Height, 0.1, 200);

            Gl.glMatrixMode(Gl.GL_MODELVIEW);
            Gl.glLoadIdentity();

            // настройка параметров OpenGL для визуализации 
            Gl.glEnable(Gl.GL_DEPTH_TEST);

            Gl.glEnable(Gl.GL_NORMALIZE);
            Gl.glEnable(Gl.GL_BLEND);
            Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA);
            Gl.glEnable(Gl.GL_LINE_SMOOTH);
            Gl.glHint(Gl.GL_LINE_SMOOTH_HINT, Gl.GL_NICEST);

            //  tabControl1.SelectedIndex = 1;
            trackBar4_Scroll(null, null);
        }



        //Задаём угол обзора и пересчитываем 3D вид
        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            //angel = trackBar4.Value;
            camR = trackBar4.Value;
            Show3D();

        }

        private void Show3D()
        {

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();

            RefreshGllList();

            PositeCamera();

            Gl.glCallList(GLList0);
            Gl.glCallList(GLList1);
            PositeCamera();
            Gl.glCallList(GLList2);

            anT.SwapBuffers();

        }

        //Списки 3D обьектов
        //GLList0 = 3d ось, линия пересечения
        //GLList1 = колесо
        //GLList2 = винтовая лестница

        int GLList0, GLList1, GLList2, GLList3, GLList4;

        //Показывает 3D списки
        private void RefreshGllList()
        {

            Gl.glDeleteLists(GLList0, 1);
            Gl.glDeleteLists(GLList1, 1);
            Gl.glDeleteLists(GLList2, 1);
            Gl.glDeleteLists(GLList3, 1);
            Gl.glDeleteLists(GLList4, 1);

            GLList0 = Gl.glGenLists(1);
            Gl.glNewList(GLList0, Gl.GL_COMPILE);
            Draw3DAxis();
            //DrawIntersectLine();
            Gl.glEndList();

            GLList1 = Gl.glGenLists(1);
            Gl.glNewList(GLList1, Gl.GL_COMPILE);
            DrawWireframeDrill1();
            Gl.glEndList();

            GLList2 = Gl.glGenLists(1);
            Gl.glNewList(GLList2, Gl.GL_COMPILE);
            DrawWireframeDrill2();
            Gl.glEndList();

            GLList3 = Gl.glGenLists(1);
            Gl.glNewList(GLList3, Gl.GL_COMPILE);
            DrawIntersectionLine1ForList();
            Gl.glEndList();

            GLList4 = Gl.glGenLists(1);
            Gl.glNewList(GLList4, Gl.GL_COMPILE);
            DrawIntersectionLine2ForList();
            Gl.glEndList();

        }


        //Строим 3Д ось
        private void Draw3DAxis()
        {
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1);

            Gl.glBegin(Gl.GL_LINES);

            Gl.glVertex3d(-R2, 0, 0);
            Gl.glVertex3d(R2 , 0, 0);

            Gl.glVertex3d(0, -R2, 0);
            Gl.glVertex3d(0, R2, 0);

            Gl.glVertex3d(0, 0, -R2);
            Gl.glVertex3d(0, 0, R2);

            Gl.glEnd();
        }


        //Строим винтовую лестницу
        private void DrawWireframeDrill1()
        {
            int i, j, Q1 = 31, Q2;

            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1);

            MyRotate rotate = new MyRotate();
            MyPoint tPoint = new MyPoint();
            MyPoint tPoint2 = new MyPoint();


            MyPoint[] arr1 = new MyPoint[64];
            SetupArray(arr1);  ///проверить ?


            MyPoint[] arr2 = new MyPoint[64];
            SetupArray(arr2);

            double H, tH, t2H, dH;
            H = 20;
            dH = 2 * Math.PI * h1 / (Q1 - 1);

            double B1, dB, L1, L2;
            B1 = -H / h1;
            dB = 2 * Math.PI / (Q1 - 1);

            Q2 =(int) (2 * H / (dB * h1));

            t2H = -H;

            double cH;
            cH = 2 * h1 * Math.Atan(arraySV[0].X / arraySV[0].Y);

            rotate.SetRotate(B1, 0, 0, 1);
            for (j = 0; j < act_quant; j++) rotate.TransformPoint(arraySV[j], arr2[j]);


            for (i = 0; i <= Q2; i++)
            {
                L2 = B1 + dB * (i + 1);

                tH = t2H;
                t2H = -H + dH * (i + 1);

                CopyArray(arr1, arr2);
                rotate.SetRotate(L2, 0, 0, 1);
                for (j = 0; j < act_quant; j++) rotate.TransformPoint(arraySV[j], arr2[j]);

                Gl.glBegin(Gl.GL_LINE_STRIP);
                
                for (j = 0; j < act_quant; j++)
                {
                    Gl.glVertex3d(arr1[j].X, arr1[j].Y, tH);
                }
                Gl.glEnd();
                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (j = 0; j < act_quant; j++)
                {
                    Gl.glVertex3d(-arr1[j].X, -arr1[j].Y, tH);
                }
                Gl.glEnd();

                if (i == (Q2))
                {
                    t2H = H;
                    rotate.SetRotate(-B1, 0, 0, 1);
                    for (j = 0; j < act_quant; j++) rotate.TransformPoint(arraySV[j], arr2[j]);

                    Gl.glBegin(Gl.GL_LINE_STRIP);
                    for (j = 0; j < act_quant; j++)
                    {
                        Gl.glVertex3d(arr2[j].X, arr2[j].Y, t2H);
                    }
                    Gl.glEnd();
                    Gl.glBegin(Gl.GL_LINE_STRIP);
                    for (j = 0; j < act_quant; j++)
                    {
                        Gl.glVertex3d(-arr2[j].X, -arr2[j].Y, t2H);
                    }
                    Gl.glEnd();
                }

                Gl.glBegin(Gl.GL_LINES);
                for (j = 0; j < act_quant; j++)
                {
                    Gl.glVertex3d(arr1[j].X, arr1[j].Y, tH);
                    Gl.glVertex3d(arr2[j].X, arr2[j].Y, t2H);
                }
                Gl.glEnd();
                Gl.glBegin(Gl.GL_LINES);
                for (j = 0; j < act_quant; j++)
                {
                    Gl.glVertex3d(-arr1[j].X, -arr1[j].Y, tH);
                    Gl.glVertex3d(-arr2[j].X, -arr2[j].Y, t2H);
                }
                Gl.glEnd();
            }

            double L0 = -Math.Atan(arraySV[0].Y / arraySV[0].X);
            double h0 = -H, hD;

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (i = 0; dH * i < cH + 0.1; i++)
            {
                L1 = B1 + dB * i - L0;
                Gl.glVertex3d(Math.Cos(L1) * R1, Math.Sin(L1) * R1, -H);
            }
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (i = 0; dH * i < cH + 0.1; i++)
            {
                L1 = B1 + dB * i - L0;
                Gl.glVertex3d(-Math.Cos(L1) * R1, -Math.Sin(L1) * R1, -H);
            }
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (i = 0; dH * i < cH + 0.1; i++)
            {
                L1 = -B1 + dB * i - L0;
                Gl.glVertex3d(Math.Cos(L1) * R1, Math.Sin(L1) * R1, H);
            }
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (i = 0; dH * i < cH + 0.1; i++)
            {
                L1 = -B1 + dB * i - L0;
                Gl.glVertex3d(-Math.Cos(L1) * R1, -Math.Sin(L1) * R1, H);
            }
            Gl.glEnd();

            for (i = 0; ; i++)
            {
                L1 = B1 + dB * i - L0;
                hD = tH = dH * i;

                if (tH > cH)
                {
                    h0 = -H + tH - cH;
                    hD = cH;
                }

                if (h0 > H) break;

                Gl.glBegin(Gl.GL_LINES);
                Gl.glVertex3d(Math.Cos(L1) * R1, Math.Sin(L1) * R1, h0);
                if ((h0 + hD) < H) Gl.glVertex3d(Math.Cos(L1) * R1, Math.Sin(L1) * R1, h0 + hD);
                else Gl.glVertex3d(Math.Cos(L1) * R1, Math.Sin(L1) * R1, H);
                Gl.glEnd();

                Gl.glBegin(Gl.GL_LINES);
                Gl.glVertex3d(-Math.Cos(L1) * R1, -Math.Sin(L1) * R1, h0);
                if ((h0 + hD) < H) Gl.glVertex3d(-Math.Cos(L1) * R1, -Math.Sin(L1) * R1, h0 + hD);
                else Gl.glVertex3d(-Math.Cos(L1) * R1, -Math.Sin(L1) * R1, H);
                Gl.glEnd();
            }





        }


        private void SetupArray(MyPoint[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                MyPoint point = new MyPoint();
                arr[i] = point;
            }
        }

      
       //TODO: добавить методы для рисования 2 винтовой линии, линий пересечения в 3D и 2D
       
        

      

        //Строим линию пересечения
        private void DrawIntersectLine()
        {
            Gl.glColor3f(0, 0, 0.5f);
            Gl.glLineWidth(2);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (int i = 0; i < act_quant; i++) Gl.glVertex3d(array3D[i].X, array3D[i].Y, array3D[i].Z);
            Gl.glEnd();
        }

        //start params
        float camZ = 30, camY = 50, camR = 20;
        float camZt, camYt, camRt;

        private void label5_Click(object sender, EventArgs e)
        {

        }

        bool r_pull = false, l_pull = false;
        int x_mouse_start, y_mouse_start;


        //Метод отвечает за обзор 3Д модели (камеры)
        private void PositeCamera()
        {
            Gl.glLoadIdentity();
            Gl.glTranslated(0, 0, -camR);
            Gl.glRotated(-camY, 1, 0, 0);
            Gl.glRotatef(camZ, 0, 0, 1);
        }

        private void CopyArray(MyPoint[] arr1, MyPoint[] arr2)
        {
            for (int i = 0; i < act_quant; i++) arr1[i] = arr2[i];
        }

        #endregion

        private void Form1_Resize(object sender, EventArgs e)
        {
            UpdateView();
        }

        //Вспомогательный OpenGL
        private void UpdateView()
        {
            Gl.glViewport(0, 0, anT.Width, anT.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(35, (double)anT.Width / anT.Height, 0.1, 200);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        }

        
        //Приближение камеры
        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            camR = float.Parse(trackBar4.Value.ToString());
            Show3D();
        }

        //Поворот камеры
        private void trackBar5_ValueChanged(object sender, EventArgs e)
        {
            camZ = float.Parse(trackBar5.Value.ToString());
            Show3D();
        }



        //Настройка интерфейса
        private void radButton1_Click(object sender, EventArgs e)
        {

            groupBox1.Visible = !groupBox1.Visible;
            groupBox3.Visible = !groupBox3.Visible;
            groupBox4.Visible = !groupBox4.Visible;
            // tableLayoutPanel1
            if (tableLayoutPanel1.RowStyles[1].Height == 0)
            {
                this.tableLayoutPanel1.RowStyles[1].Height = firstStyle;
                this.tableLayoutPanel1.RowStyles[2].Height = secondStyle;
            }
            else
            {
                this.tableLayoutPanel1.RowStyles[1].Height = 0;
                this.tableLayoutPanel1.RowStyles[2].Height = 0;
            }


            UpdateView();
            camZ = float.Parse(trackBar5.Value.ToString());
            Show3D();

        }

        private void radMenuItem6_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void radMenuItem4_Click(object sender, EventArgs e)
        {
            new About().Show();
        }


    



    }
}
