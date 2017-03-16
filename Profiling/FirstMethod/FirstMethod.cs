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
            _diskOpenGlHelper = new DiskOpenGLHelper(this);
            InitializeComponent();
            anT.InitializeContexts();
           
            _diskCalculator = new DiskCalculator(this);
            
            InitStartParams();
           
            firstStyle = tableLayoutPanel1.RowStyles[1].Height;
            secondStyle = tableLayoutPanel1.RowStyles[2].Height;
            
        }

        public bool FirstForm1
        {
            set { FirstForm = value; }
            get { return FirstForm; }
        }

        public int ActQuant
        {
            set { act_quant = value; }
            get { return act_quant; }
        }

        public double DIn
        {
            set { d_in = value; }
            get { return d_in; }
        }

        public double R1
        {
            set { R = value; }
            get { return R; }
        }

        public double O1
        {
            set { O = value; }
            get { return O; }
        }

        public double Rcenter1
        {
            set { Rcenter = value; }
            get { return Rcenter; }
        }

        public bool DLink
        {
            set { d_link = value; }
            get { return d_link; }
        }

        public Point[] Array
        {
            set { array = value; }
            get { return array; }
        }

        public Point[] ArrayO
        {
            set { arrayO = value; }
            get { return arrayO; }
        }

        public Point[] ArraySv
        {
            set { arraySV = value; }
            get { return arraySV; }
        }

        public Point[] Array3D
        {
            set { array3D = value; }
            get { return array3D; }
        }

        public DiskOpenGLHelper DiskOpenGlHelper
        {
            get { return _diskOpenGlHelper; }
        }


        //инициализация начальных переменных программы
        private void InitStartParams()
        {
            

            FirstForm = true;
            View3D = false;
            act_quant = 7;
            d_link = true;
            d_in = 1; // угол

            illumination = false;
            wheel_vis = true;
            drill_vis = true;

    


            O = Math.PI * 1 / 180;
            R = 0.5;

        
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


         
            trackBar2.Value = trackBar2.Maximum / 2;
            trackBar3.Value = trackBar3.Maximum / 3;
            trackBar3_Scroll(null, null);
        }


        #region Трекеры

      

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
      
            R = double.Parse(textBox4.Text) +
                (double.Parse(textBox5.Text) - double.Parse(textBox4.Text)) * trackBar1.Value / trackBar1.Maximum;
            textBox1.Text = R.ToString();
            textBox6.Text = R.ToString();

            DiskCalculator.RefreshPoints();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
     
            O = Math.PI * ((double)trackBar3.Value * 0.88 + 1) / 180;
            textBox3.Text = (double.Parse(trackBar3.Value.ToString()) * 0.88 + 1).ToString();

    
            DiskCalculator.RefreshPoints();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
     
            d_in = double.Parse(textBox6.Text) +
                (double.Parse(textBox7.Text) - double.Parse(textBox6.Text)) * trackBar2.Value / trackBar2.Maximum;
            textBox2.Text = d_in.ToString();

            //пересчитать
            DiskCalculator.RefreshPoints();
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

           
        }


  
   



        public void DrawChart()
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



  
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            FirstForm = true;
            DiskCalculator.RefreshPoints();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            FirstForm = false;
            DiskCalculator.RefreshPoints();
        }


     
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
        

   

        //Построение 3D модели
        
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

            trackBar4_Scroll(null, null);
        }



        //Задаём угол обзора и пересчитываем 3D вид
        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            DiskOpenGlHelper.camR = trackBar4.Value;
            DiskOpenGlHelper.Show3D();

        }


        private readonly DiskOpenGLHelper _diskOpenGlHelper;

        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            DiskOpenGlHelper.camR = float.Parse(trackBar4.Value.ToString());
            DiskOpenGlHelper.Show3D();
        }

        //Поворот камеры
        private void trackBar5_ValueChanged(object sender, EventArgs e)
        {
            DiskOpenGlHelper.camZ = float.Parse(trackBar5.Value.ToString());
            DiskOpenGlHelper.Show3D();
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


            DiskOpenGlHelper.UpdateView();
            DiskOpenGlHelper.camZ = float.Parse(trackBar5.Value.ToString());
            DiskOpenGlHelper.Show3D();

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
