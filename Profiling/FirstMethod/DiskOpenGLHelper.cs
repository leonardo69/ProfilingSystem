using System;
using System.Linq;
using Profiling.Core;
using Tao.OpenGl;

namespace Profiling
{
    public class DiskOpenGLHelper
    {
        private Form1 _form1;
        private int GLList0;
        private int GLList1;
        private int GLList2;
        public float camZ = 30;
        private float camY = 50;
        public float camR = 20;
        private float camZt;
        private float camYt;
        private float camRt;
        private bool r_pull = false;
        private bool l_pull = false;
        private int x_mouse_start;
        private int y_mouse_start;

        public DiskOpenGLHelper(Form1 form1)
        {
            _form1 = form1;
        }

        public void Show3D()
        {

            Gl.glClear(Gl.GL_COLOR_BUFFER_BIT | Gl.GL_DEPTH_BUFFER_BIT);
            Gl.glLoadIdentity();

            RefreshGllList();

            PositeCamera();

            Gl.glCallList((int) GLList0);
            Gl.glCallList((int) GLList1);
            PositeCamera();
            Gl.glCallList((int) GLList2);

            _form1.anT.SwapBuffers();

        }

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

        private void Draw3DAxis()
        {
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1);

            Gl.glBegin(Gl.GL_LINES);

            Gl.glVertex3d(-_form1.R1 * 2, 0, 0);
            Gl.glVertex3d(_form1.R1 * 2, 0, 0);

            Gl.glVertex3d(0, -_form1.R1 * 2, 0);
            Gl.glVertex3d(0, _form1.R1 * 2, 0);

            Gl.glVertex3d(0, 0, -_form1.R1 * 2);
            Gl.glVertex3d(0, 0, _form1.R1 * 2);

            Gl.glEnd();
        }

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
            int Q = (int)(_form1.O1 * 180 / Math.PI + 5);


            h = _form1.R1 / Math.Tan(_form1.O1);
            H = _form1.Array[_form1.ActQuant / 2].Y;
            dH = (2 * H) / (Q - 1);

            t2H = -H;
            L2 = t2H / h;
            rotate.SetRotate(L2, 0, 0, 1);
            for (j = 0; j < _form1.ActQuant; j++) rotate.TransformPoint(_form1.ArraySv[j], arr2[j]);

            dL = Math.Atan(_form1.ArraySv[0].Y /_form1.ArraySv[0].X);
            dL = (Math.PI - dL) * 2 / (12 - 1);

            for (i = 0; i < Q; i++)
            {
                tH = t2H;
                t2H = -H + (i + 1) * dH;

                L2 = t2H / h;
                //  CopyArray(arr1, arr2);
                arr1 = arr2.Select(o => o.Clone()).ToArray();
                rotate.SetRotate(L2, 0, 0, 1);
                for (j = 0; j < _form1.ActQuant; j++) rotate.TransformPoint(_form1.ArraySv[j], arr2[j]);

                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (j = 0; j < _form1.ActQuant; j++)
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
                    for (j = 0; j < _form1.ActQuant; j++)
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

        private void DrawWheel()
        {
            double d;

            if (_form1.DLink)
            {
                if (_form1.FirstForm1) d = 8 *_form1.R1;
                else d = 8.424412649 *_form1.R1;
            }
            else d = _form1.DIn;

            Gl.glTranslated(d, 0, 0);
            Gl.glRotated(-_form1.O1 * 180 / Math.PI, 1, 0, 0);

            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1);
            Gl.glBegin(Gl.GL_LINES);
            Gl.glVertex3d(0, -_form1.Array[_form1.ActQuant / 2].Y, 0);
            Gl.glVertex3d(0, _form1.Array[_form1.ActQuant / 2].Y, 0);
            Gl.glEnd();

            DrawWireframeWheel();


        }

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
                for (i = 0; i < _form1.ActQuant; i++)
                    Gl.glVertex3d(_form1.Array[i].Y * Math.Cos(L1), _form1.Array[i].X, _form1.Array[i].Y * Math.Sin(L1));
                Gl.glEnd();

                Gl.glBegin(Gl.GL_LINES);
                for (i = 0; i < _form1.ActQuant; i++)
                {
                    Gl.glVertex3d(_form1.Array[i].Y * Math.Cos(L1), _form1.Array[i].X, _form1.Array[i].Y * Math.Sin(L1));
                    Gl.glVertex3d(_form1.Array[i].Y * Math.Cos(L2), _form1.Array[i].X, _form1.Array[i].Y * Math.Sin(L2));
                }
                Gl.glEnd();
            }
        }

        private void DrawIntersectLine()
        {
            Gl.glColor3f(0, 0, 0.5f);
            Gl.glLineWidth(2);
            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (int i = 0; i < _form1.ActQuant; i++) Gl.glVertex3d(_form1.Array3D[i].X, _form1.Array3D[i].Y, _form1.Array3D[i].Z);
            Gl.glEnd();
        }

        private void PositeCamera()
        {
            Gl.glLoadIdentity();
            Gl.glTranslated(0, 0, -camR);
            Gl.glRotated(-camY, 1, 0, 0);
            Gl.glRotatef(camZ, 0, 0, 1);
        }

        private void CopyArray(Point[] arr1, Point[] arr2)
        {
            for (int i = 0; i < _form1.ActQuant; i++) arr1[i] = arr2[i];
        }

        public void Form1_Resize(object sender, EventArgs e)
        {
            UpdateView();
        }

        public void UpdateView()
        {
            Gl.glViewport(0, 0, _form1.anT.Width, _form1.anT.Height);
            Gl.glMatrixMode(Gl.GL_PROJECTION);
            Gl.glLoadIdentity();
            Glu.gluPerspective(35, (double) _form1.anT.Width /_form1.anT.Height, 0.1, 200);
            Gl.glMatrixMode(Gl.GL_MODELVIEW);
        }
    }
}