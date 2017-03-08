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
using Point = Profiling.Core.Point;

namespace Profiling
{
    public partial class Form1 : Telerik.WinControls.UI.RadForm
    {

#region Переменные


        bool FirstForm; //первая канавка
        bool View3D; //3D вид
        int act_quant; //Количество точек

        double d_in, R, O, Ro, Rcenter; //параметры для расчёта

        bool d_link = true, illumination = true;//для отображения
        bool wheel_vis = true, drill_vis = true;

        Point[] array;
        Point[] arrayO;
        Point[] arraySV;
        Point[] array3D;


        float firstStyle;
        float secondStyle;

#endregion

        public Form1()
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
            d_in = 1; // угол

            illumination = false;
            wheel_vis = true;
            drill_vis = true;

            ///переменные для расчёта стартового


            O = Math.PI * 1 / 180;
            R = 0.5;

            //инициализируем списки
            array = new Point[act_quant];
            arrayO = new Point[act_quant];
            arraySV = new Point[act_quant];
            array3D = new Point[act_quant];

            for (int i = 0; i < act_quant; i++)
            {
                array[i] = new Point();
                arrayO[i] = new Point();
                arraySV[i] = new Point();
                array3D[i] = new Point();
            }


            //настраиваем положение трекбаров
            trackBar2.Value = trackBar2.Maximum / 2;
            trackBar3.Value = trackBar3.Maximum / 3;
            trackBar3_Scroll(null, null);
        }


        #region Трекеры

        //обработчики: берёт значение из интерфейса и передаёт переменной

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            //изменяется переменная R радиус
            R = double.Parse(textBox4.Text) +
                (double.Parse(textBox5.Text) - double.Parse(textBox4.Text)) * trackBar1.Value / trackBar1.Maximum;
            textBox1.Text = R.ToString();
            textBox6.Text = R.ToString();

            //пересчитать точки
            RefreshPoints();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            //изменяется переменная угла скрещивания
            O = Math.PI * ((double)trackBar3.Value * 0.88 + 1) / 180;
            textBox3.Text = (double.Parse(trackBar3.Value.ToString()) * 0.88 + 1).ToString();

            //пересчитать
            RefreshPoints();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            //изменяется межосевое расстояние
            d_in = double.Parse(textBox6.Text) +
                (double.Parse(textBox7.Text) - double.Parse(textBox6.Text)) * trackBar2.Value / trackBar2.Maximum;
            textBox2.Text = d_in.ToString();

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

        public void RefreshPoints()
        {
            //обновление вычислений

            double L, L0, Tt, Rt, dL, Ro;
            int i;
            richTextBox1.Clear();
            string str;

            richTextBox1.Text += "L\t\tTt\t\tRt\n";
            richTextBox1.Text += "\n";

            if (FirstForm) L0 = 1.154338843;   //определяем L0
            else L0 = 1.343481113;

            dL = Math.PI - L0 * 2;             //определяем dL
            dL = dL / (act_quant - 1);

            for (i = 0; i < act_quant; i++)
            {
                L = dL * i + L0;

                Yn(L, O, R, out Tt, out Rt, ref array3D[i], ref arraySV[i]);   //вызов функции Yn
                str = String.Format("{0:0.000000}\t\t{1:0.000000}               {2:0.000000}", Math.Round(L, 6), Math.Round(Tt, 6), Math.Round(Rt, 6)) + "\n";
                richTextBox1.Text += str;

                //заполняем массив точками, по которым будет строиться профиль
                array[i].X = Tt;
                array[i].Y = Rt;
                array[i].Z = 0;
            }

            double a, h, Y, Rv;

            a = array[act_quant / 2].Y - array[0].Y;
            h = array[0].X;
            Ro = (h * h + a * a) / (2 * a);
            Rcenter = Y = array[act_quant / 2].Y - Ro;

            for (i = 0; i < act_quant; i++)
            {
                //заполняется массив точками для построения второго профиля

                arrayO[i].X = array[i].X;
                arrayO[i].Y = array[i].Y - Y;
                Rv = Math.Sqrt(arrayO[i].X * arrayO[i].X + arrayO[i].Y * arrayO[i].Y);   //размерность массива? посмотреть его инициализацию
                arrayO[i].X /= Rv;
                arrayO[i].Y /= Rv;
                arrayO[i].X *= Ro;
                arrayO[i].Y *= Ro;
                arrayO[i].Y += Y;
            }


            //переменные, которые выводятся в результат

            richTextBox1.Text += "\n";
            str = "Шаг 2D сетки = 0.1 ед\n";
            richTextBox1.Text += str;

            richTextBox1.Text += "\n";
            str = "Центр локальной СК: Tt'=0   Rt=" + Math.Round(array[0].Y, 6) + "\n";
            richTextBox1.Text += str;
            str = "Радиус аппроксимирующей окружности = " + Math.Round(Ro, 6) + "\n";
            richTextBox1.Text += str; ;

            str = "Центр аппрокс. окр.: Tt=0    Rt=" + Math.Round(Y, 6) + "\n";
            richTextBox1.Text += str;

            str = "Максимальное отклонение = " + Math.Round(CalcE(), 6);
            richTextBox1.Text += str;

            // RefreshGLLists();
            //  InvalidateRect(Form1->Handle, NULL, 0);     
            DrawChart();


            Show3D();
        }

        //Связанана с RefreshPoints
        private double CalcE()   //считаем CalcE
        {
            int i;
            double St, Smax = 0, x, y;

            for (i = 0; i < act_quant; i++)
            {
                x = arrayO[i].X - array[i].X;
                y = arrayO[i].Y - array[i].Y;
                St = Math.Sqrt(x * x + y * y);
                if (St > Smax) Smax = St;
            }
            return Smax;

        }

        //Yn просмотреть  и передачу по ссылке

        //Связана с RefreshPoints
        double Yn(double L, double w, double R, out double Tt, out double Rt, ref Point Point3D, ref Point PSV)
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


        //Связана с Yn
        double F(double L, double w, double R, double Yn, out double Tt, out double Rt, Point P3D, Point PSV)
        {
            double a1, r, Xtd, Ytd, G, Lt, d, h, Rn, tga, X_, Y_, Z_, bn, X, Y, Z, O, A, B, C, D;

            if (FirstForm) a1 = 2.4168 * R;
            else a1 = 2.841212649 * R;

            if (d_link)
            {
                if (FirstForm) d = 8 * R;
                else d = 8.424412649 * R;
            }
            else d = d_in;

            r = 2 * R;
            h = R / Math.Tan(w);
            O = Math.PI / 2 - w;
            if (FirstForm) tga = 1 / (8 * Math.Tan(w));
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
            if (FirstForm)
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

        int GLList0, GLList1, GLList2;

        //Показывает 3D списки
        private void RefreshGllList()
        {

            Gl.glDeleteLists(GLList0, 1);
            Gl.glDeleteLists(GLList1, 1);
            Gl.glDeleteLists(GLList2, 1);

            GLList0 = Gl.glGenLists(1);
            Gl.glNewList(GLList0, Gl.GL_COMPILE);
            Draw3DAxis();
            DrawIntersectLine();
            Gl.glEndList();

            GLList1 = Gl.glGenLists(1);
            Gl.glNewList(GLList1, Gl.GL_COMPILE);
            DrawWheel();
            Gl.glEndList();

            GLList2 = Gl.glGenLists(1);
            Gl.glNewList(GLList2, Gl.GL_COMPILE);
            DrawWireframeDrill();
            Gl.glEndList();

        }


        //Строим 3Д ось
        private void Draw3DAxis()
        {
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1);

            Gl.glBegin(Gl.GL_LINES);

            Gl.glVertex3d(-R * 2, 0, 0);
            Gl.glVertex3d(R * 2, 0, 0);

            Gl.glVertex3d(0, -R * 2, 0);
            Gl.glVertex3d(0, R * 2, 0);

            Gl.glVertex3d(0, 0, -R * 2);
            Gl.glVertex3d(0, 0, R * 2);

            Gl.glEnd();
        }


        //Строим винтовую лестницу
        private void DrawWireframeDrill()
        {
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1);

            Rotate rotate = new Rotate();
            Point tPoint = new Point();
            Point tPoint2 = new Point();


            Point[] arr1;// = new Point[64];
            // SetupArray(arr1);


            Point[] arr2 = new Point[64];
            SetupArray(arr2);

            //   O = Math.PI * 1 / 180;

            double h, dH, H, tH, t2H, L2, dL;
            int i, j;
            int Q = (int)(O * 180 / Math.PI + 5);


            h = R / Math.Tan(O);
            H = array[act_quant / 2].Y;
            dH = (2 * H) / (Q - 1);

            t2H = -H;
            L2 = t2H / h;
            rotate.SetRotate(L2, 0, 0, 1);
            for (j = 0; j < act_quant; j++) rotate.TransformPoint(arraySV[j], arr2[j]);

            dL = Math.Atan(arraySV[0].Y / arraySV[0].X);
            dL = (Math.PI - dL) * 2 / (12 - 1);

            for (i = 0; i < Q; i++)
            {
                tH = t2H;
                t2H = -H + (i + 1) * dH;

                L2 = t2H / h;
                //  CopyArray(arr1, arr2);
                arr1 = arr2.Select(o => o.Clone()).ToArray();
                rotate.SetRotate(L2, 0, 0, 1);
                for (j = 0; j < act_quant; j++) rotate.TransformPoint(arraySV[j], arr2[j]);

                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (j = 0; j < act_quant; j++)
                {
                    Gl.glVertex3d(arr1[j].X, arr1[j].Y, tH);
                }
                Gl.glEnd();

                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (j = 0; j < 12; j++)
                {
                    rotate.SetRotate(dL * j, 0, 0, 1);
                    rotate.TransformPoint(arr1[0], tPoint);
                    Gl.glVertex3d(tPoint.X, tPoint.Y, tH);
                }
                Gl.glEnd();

                if (i < (Q - 1))
                {
                    Gl.glBegin(Gl.GL_LINES);
                    for (j = 0; j < act_quant; j++)
                    {
                        Gl.glVertex3d(arr1[j].X, arr1[j].Y, tH);
                        Gl.glVertex3d(arr2[j].X, arr2[j].Y, t2H);
                    }
                    Gl.glEnd();

                    Gl.glBegin(Gl.GL_LINES);
                    for (j = 0; j < 12; j++)
                    {
                        rotate.SetRotate(dL * j, 0, 0, 1);
                        rotate.TransformPoint(arr1[0], tPoint);
                        rotate.TransformPoint(arr2[0], tPoint2);
                        Gl.glVertex3d(tPoint.X, tPoint.Y, tH);
                        Gl.glVertex3d(tPoint2.X, tPoint2.Y, t2H);
                    }
                    Gl.glEnd();
                }
            }

        }


        private void SetupArray(Point[] arr)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                Point point = new Point();
                arr[i] = point;
            }
        }

        //Строим колесо (подготовка)
        private void DrawWheel()
        {
            double d;

            if (d_link)
            {
                if (FirstForm) d = 8 * R;
                else d = 8.424412649 * R;
            }
            else d = d_in;

            Gl.glTranslated(d, 0, 0);
            Gl.glRotated(-O * 180 / Math.PI, 1, 0, 0);

            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3d(0, -array[act_quant / 2].Y, 0);
            Gl.glVertex3d(0, array[act_quant / 2].Y, 0);
            Gl.glEnd();

            DrawWireframeWheel();


        }

        //Строим колесо (само построение)

        private void DrawWireframeWheel()
        {
            int i, j, Q = 50;
            double L1, L2;

            L2 = 0;

            for (j = 0; j < Q; j++)
            {
                L1 = L2;
                L2 = Math.PI * 2 * (j + 1) / Q;

                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (i = 0; i < act_quant; i++)
                    Gl.glVertex3d(array[i].Y * Math.Cos(L1), array[i].X, array[i].Y * Math.Sin(L1));
                Gl.glEnd();

                Gl.glBegin(Gl.GL_LINES);
                for (i = 0; i < act_quant; i++)
                {
                    Gl.glVertex3d(array[i].Y * Math.Cos(L1), array[i].X, array[i].Y * Math.Sin(L1));
                    Gl.glVertex3d(array[i].Y * Math.Cos(L2), array[i].X, array[i].Y * Math.Sin(L2));
                }
                Gl.glEnd();
            }
        }

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

        private void CopyArray(Point[] arr1, Point[] arr2)
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
