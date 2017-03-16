using System;
using System.Windows.Forms;
using Profiling.Core;
using Profiling.GUI;
using Tao.FreeGlut;
using Tao.OpenGl;
using Telerik.WinControls.UI;

namespace Profiling
{
    public partial class Form2 : RadForm
    {
        private bool FirstForm; //первая канавка
        private bool View3D; //3D вид
        public int act_quant; //Количество точек
        private bool visDrill1 = true;
        private bool visDrill2 = true;
        public double w; //параметры  модели
        public double R1; //параметры  модели
        public double R2; //параметры  модели
        private double Ro; //параметры  модели
        public double h1; //параметры  модели
        public double h2; //параметры  модели
        public double O; //параметры  модели
        public double dZ; //параметры  модели
        public double U; //параметры  модели
        private double F1; //параметры  модели

        private bool d_link = true; //для отображения


        public Point[] array1;
        public Point[] array3;
        public Point[] array4;
        public Point[] arraySV;
        public Point[] arrayO;


        private readonly float firstStyle;
        private readonly float secondStyle;


        public Form2()
        {
            InitializeComponent();
            anT.InitializeContexts();
            WormCalculator = new WormCalculator(this);
            WormOpenGlHelper = new WormOpenGLHelper(this);
            InitStartParams();
            firstStyle = tableLayoutPanel1.RowStyles[1].Height;
            secondStyle = tableLayoutPanel1.RowStyles[2].Height;
        }

        public WormOpenGLHelper WormOpenGlHelper { get; }

        public WormCalculator WormCalculator { get; }


        private void InitStartParams()
        {
            act_quant = 51;

            w = Math.PI*45/180;
            R1 = 3;
            R2 = 12.5;


            InitDataArrays();

            InitTrackBars();

            trackBar3_Scroll(null, null);
        }

        private void InitDataArrays()
        {
            array1 = new Point[act_quant];
            array3 = new Point[act_quant];
            array4 = new Point[act_quant];
            arraySV = new Point[act_quant];
            arrayO = new Point[act_quant];


            for (var i = 0; i < act_quant; i++)
            {
                array1[i] = new Point();
                array3[i] = new Point();
                array4[i] = new Point();
                arraySV[i] = new Point();
                arrayO[i] = new Point();
            }
        }

        private void InitTrackBars()
        {
            trackBar1.Value = trackBar1.Maximum/2;
            R1 = double.Parse(textBox4.Text) +
                 (double.Parse(textBox5.Text) - double.Parse(textBox4.Text))*trackBar1.Value/trackBar1.Maximum;
            textBox1.Text = R1.ToString();
            textBox6.Text = R1.ToString();

            trackBar2.Value = trackBar2.Maximum/2;
            R2 = double.Parse(textBox6.Text) +
                 (double.Parse(textBox7.Text) - double.Parse(textBox6.Text))*trackBar2.Value/trackBar2.Maximum;
            textBox2.Text = R2.ToString();

            trackBar3.Value = trackBar3.Maximum/2;
        }

        #region TrackBars events

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            R1 = double.Parse(textBox4.Text) +
                 (double.Parse(textBox5.Text) - double.Parse(textBox4.Text))*trackBar1.Value/trackBar1.Maximum;
            textBox1.Text = R1.ToString();
            textBox6.Text = R1.ToString();

            R2 = double.Parse(textBox6.Text) +
                 (double.Parse(textBox7.Text) - double.Parse(textBox6.Text))*trackBar2.Value/trackBar2.Maximum;
            textBox2.Text = R2.ToString();

            WormCalculator.RefreshPoints();
        }

        private void trackBar3_Scroll(object sender, EventArgs e)
        {
            w = Math.PI*(trackBar3.Value*0.88 + 1)/180;
            textBox3.Text = (double.Parse(trackBar3.Value.ToString())*0.88 + 1).ToString();
            WormCalculator.RefreshPoints();
        }

        private void trackBar2_Scroll(object sender, EventArgs e)
        {
            R2 = double.Parse(textBox6.Text) +
                 (double.Parse(textBox7.Text) - double.Parse(textBox6.Text))*trackBar2.Value/trackBar2.Maximum;
            textBox2.Text = R2.ToString();

            WormCalculator.RefreshPoints();
        }

        #endregion

        #region  Callculation Part

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            FirstForm = true;
            WormCalculator.RefreshPoints();
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            FirstForm = false;
            WormCalculator.RefreshPoints();
        }


        private void saveChart_Click(object sender, EventArgs e)
        {
            using (var sd = new SaveFileDialog())
            {
                sd.Filter = ".png| *.png";
                if (sd.ShowDialog() == DialogResult.OK)
                {
                    var pane = zedGraphControl1.GraphPane;
                    pane.Image.Save(sd.FileName);
                }
            }
        }

        private void saveResult_Click(object sender, EventArgs e)
        {
            using (var sd = new SaveFileDialog())
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
            Glu.gluPerspective(35, anT.Width/(float) anT.Height, 0.1, 200);

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

        private void trackBar4_Scroll(object sender, EventArgs e)
        {
            camR = trackBar4.Value;
        }

        public void Show3D()
        {
            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();

            WormOpenGlHelper.RefreshGllList();

            PositeCamera();

            Gl.glCallList(WormOpenGlHelper.GLList0);

            WormOpenGlHelper.DrawIntersectionLine1FromList();
            WormOpenGlHelper.DrawIntersectionLine2FromList();

            PositeCamera();
            WormOpenGlHelper.DrawDrill1();
            PositeCamera();
            WormOpenGlHelper.DrawDrill2();

            anT.SwapBuffers();
        }

  


        private float camZ = 30;
        private readonly float camY = 50;
        private float camR = 20;
        private float camZt;
        private float camYt;
        private float camRt;

   

        private bool r_pull = false;
        private bool l_pull = false;
        private int x_mouse_start;
        private int y_mouse_start;


        private void PositeCamera()
        {
            Gl.glLoadIdentity();
            Gl.glTranslated(0, 0, -camR);
            Gl.glRotated(-camY, 1, 0, 0);
            Gl.glRotatef(camZ, 0, 0, 1);
        }

        #endregion

        private void Form1_Resize(object sender, EventArgs e)
        {
            UpdateView();
        }

        private void UpdateView()
        {
            Gl.glViewport(0, 0, anT.Width, anT.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(35, (double) anT.Width/anT.Height, 0.1, 200);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        }

        private void trackBar4_ValueChanged(object sender, EventArgs e)
        {
            camR = float.Parse(trackBar4.Value.ToString());
            Show3D();
        }

        private void trackBar5_ValueChanged(object sender, EventArgs e)
        {
            camZ = float.Parse(trackBar5.Value.ToString());
            Show3D();
        }

        private void radButton1_Click(object sender, EventArgs e)
        {
            groupBox1.Visible = !groupBox1.Visible;
            groupBox3.Visible = !groupBox3.Visible;
            groupBox4.Visible = !groupBox4.Visible;
            // tableLayoutPanel1
            if (tableLayoutPanel1.RowStyles[1].Height == 0)
            {
                tableLayoutPanel1.RowStyles[1].Height = firstStyle;
                tableLayoutPanel1.RowStyles[2].Height = secondStyle;
            }
            else
            {
                tableLayoutPanel1.RowStyles[1].Height = 0;
                tableLayoutPanel1.RowStyles[2].Height = 0;
            }


            UpdateView();
            camZ = float.Parse(trackBar5.Value.ToString());
            Show3D();
        }

        private void radMenuItem6_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void radMenuItem4_Click(object sender, EventArgs e)
        {
            new About().Show();
        }

        private void radMenuItem8_Click(object sender, EventArgs e)
        {
            SWCore sw = new SWCore();
            sw.Open3DModel("test");
        }
    }
}
