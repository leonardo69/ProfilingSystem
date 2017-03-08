using System;
using Profiling.Core;
using Tao.OpenGl;

namespace Profiling
{
    public class OpenGLHelper
    {
        private Form2 _form2;
        public int GLList0;
        private int GLList1;
        private int GLList2;
        private int GLList3;
        private int GLList4;

        public OpenGLHelper(Form2 form2)
        {
            _form2 = form2;
        }

        public void RefreshGllList()
        {

            Gl.glDeleteLists(GLList0, 1);
            Gl.glDeleteLists(GLList1, 1);
            Gl.glDeleteLists(GLList2, 1);
            Gl.glDeleteLists(GLList3, 1);
            Gl.glDeleteLists(GLList4, 1);

            GLList0 = Gl.glGenLists(1);
            Gl.glNewList(GLList0, Gl.GL_COMPILE);
            Draw3DAxis();
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

        private void Draw3DAxis()
        {
            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1);

            Gl.glBegin(Gl.GL_LINES);

            Gl.glVertex3d(-_form2.R2, 0, 0);
            Gl.glVertex3d(_form2.R2 , 0, 0);

            Gl.glVertex3d(0, -_form2.R2, 0);
            Gl.glVertex3d(0, _form2.R2, 0);

            Gl.glVertex3d(0, 0, -_form2.R2);
            Gl.glVertex3d(0, 0, _form2.R2);

            Gl.glEnd();
        }

        private void DrawWireframeDrill1()
        {
            int i, j, Q1 = 31, Q2;

            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1);

            Rotate rotate = new Rotate();
            Point tPoint = new Point();
            Point tPoint2 = new Point();


            Point[] arr1 = new Point[64];
            SetupArray(arr1);  //проверить ?


            Point[] arr2 = new Point[64];
            SetupArray(arr2);

            double H, tH, t2H, dH;
            H = 20;
            dH = 2 * Math.PI *_form2.h1 / (Q1 - 1);

            double B1, dB, L1, L2;
            B1 = -H /_form2.h1;
            dB = 2 * Math.PI / (Q1 - 1);

            Q2 =(int) (2 * H / (dB *_form2.h1));

            t2H = -H;

            double cH;
            cH = 2 *_form2.h1 * Math.Atan(_form2.arraySV[0].X /_form2.arraySV[0].Y);

            rotate.SetRotate(B1, 0, 0, 1);
            for (j = 0; j < _form2.act_quant; j++) rotate.TransformPoint(_form2.arraySV[j], arr2[j]);


            for (i = 0; i <= Q2; i++)
            {
                L2 = B1 + dB * (i + 1);

                tH = t2H;
                t2H = -H + dH * (i + 1);

                CopyArray(arr1, arr2);
                rotate.SetRotate(L2, 0, 0, 1);
                for (j = 0; j < _form2.act_quant; j++) rotate.TransformPoint(_form2.arraySV[j], arr2[j]);

                Gl.glBegin(Gl.GL_LINE_STRIP);
                
                for (j = 0; j < _form2.act_quant; j++)
                {
                    Gl.glVertex3d(arr1[j].X, arr1[j].Y, tH);
                }
                Gl.glEnd();
                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (j = 0; j < _form2.act_quant; j++)
                {
                    Gl.glVertex3d(-arr1[j].X, -arr1[j].Y, tH);
                }
                Gl.glEnd();

                if (i == (Q2))
                {
                    t2H = H;
                    rotate.SetRotate(-B1, 0, 0, 1);
                    for (j = 0; j < _form2.act_quant; j++) rotate.TransformPoint(_form2.arraySV[j], arr2[j]);

                    Gl.glBegin(Gl.GL_LINE_STRIP);
                    for (j = 0; j < _form2.act_quant; j++)
                    {
                        Gl.glVertex3d(arr2[j].X, arr2[j].Y, t2H);
                    }
                    Gl.glEnd();
                    Gl.glBegin(Gl.GL_LINE_STRIP);
                    for (j = 0; j < _form2.act_quant; j++)
                    {
                        Gl.glVertex3d(-arr2[j].X, -arr2[j].Y, t2H);
                    }
                    Gl.glEnd();
                }

                Gl.glBegin(Gl.GL_LINES);
                for (j = 0; j < _form2.act_quant; j++)
                {
                    Gl.glVertex3d(arr1[j].X, arr1[j].Y, tH);
                    Gl.glVertex3d(arr2[j].X, arr2[j].Y, t2H);
                }
                Gl.glEnd();
                Gl.glBegin(Gl.GL_LINES);
                for (j = 0; j < _form2.act_quant; j++)
                {
                    Gl.glVertex3d(-arr1[j].X, -arr1[j].Y, tH);
                    Gl.glVertex3d(-arr2[j].X, -arr2[j].Y, t2H);
                }
                Gl.glEnd();
            }

            double L0 = -Math.Atan(_form2.arraySV[0].Y /_form2.arraySV[0].X);
            double h0 = -H, hD;

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (i = 0; dH * i < cH + 0.1; i++)
            {
                L1 = B1 + dB * i - L0;
                Gl.glVertex3d(Math.Cos(L1) *_form2.R1, Math.Sin(L1) *_form2.R1, -H);
            }
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (i = 0; dH * i < cH + 0.1; i++)
            {
                L1 = B1 + dB * i - L0;
                Gl.glVertex3d(-Math.Cos(L1) *_form2.R1, -Math.Sin(L1) *_form2.R1, -H);
            }
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (i = 0; dH * i < cH + 0.1; i++)
            {
                L1 = -B1 + dB * i - L0;
                Gl.glVertex3d(Math.Cos(L1) *_form2.R1, Math.Sin(L1) *_form2.R1, H);
            }
            Gl.glEnd();

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (i = 0; dH * i < cH + 0.1; i++)
            {
                L1 = -B1 + dB * i - L0;
                Gl.glVertex3d(-Math.Cos(L1) *_form2.R1, -Math.Sin(L1) *_form2.R1, H);
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
                Gl.glVertex3d(Math.Cos(L1) *_form2.R1, Math.Sin(L1) *_form2.R1, h0);
                if ((h0 + hD) < H) Gl.glVertex3d(Math.Cos(L1) *_form2.R1, Math.Sin(L1) *_form2.R1, h0 + hD);
                else Gl.glVertex3d(Math.Cos(L1) *_form2.R1, Math.Sin(L1) *_form2.R1, H);
                Gl.glEnd();

                Gl.glBegin(Gl.GL_LINES);
                Gl.glVertex3d(-Math.Cos(L1) *_form2.R1, -Math.Sin(L1) *_form2.R1, h0);
                if ((h0 + hD) < H) Gl.glVertex3d(-Math.Cos(L1) *_form2.R1, -Math.Sin(L1) *_form2.R1, h0 + hD);
                else Gl.glVertex3d(-Math.Cos(L1) *_form2.R1, -Math.Sin(L1) *_form2.R1, H);
                Gl.glEnd();
            }





        }

        private void DrawWireframeDrill2()
        {
            int i, j, Q1 = 51, Q2;

            Gl.glColor3f(0, 0, 0);
            Gl.glLineWidth(1);
            

            Rotate rotate = new Rotate();
            Point tPoint = new Point();
            Point tPoint2 = new Point();


            Point[] arr1 = new Point[64];
            SetupArray(arr1);  


            Point[] arr2 = new Point[64];
            SetupArray(arr2);

            double H, h, dH;
            H = 20;
            dH = 2 * Math.PI *_form2.h2 / Q1;

            double B1, dB;
            B1 = -H /_form2.h2;
            dB = 2 * Math.PI / Q1;

            Q2 =(int)(2 * H / (dB *_form2.h2));

            double L1, L2;

            for (i = 0; i < Q2; i++)
            {
                L1 = B1 + dB * i;
                L2 = B1 + dB * (i + 1);

                h = -H + dH * i;


                Gl.glBegin(Gl.GL_LINE_STRIP);
                for (j = 0; j < _form2.act_quant; j++)
                {
                    Gl.glVertex3d(_form2.array1[j].X * Math.Cos(L1), _form2.array1[j].X * Math.Sin(L1), _form2.array1[j].Z + h);
                }
                Gl.glEnd();

                if (i < (Q2 - 1))
                {
                    Gl.glBegin(Gl.GL_LINES);
                    for (j = 0; j < _form2.act_quant; j++)
                    {
                        Gl.glVertex3d(_form2.array1[j].X * Math.Cos(L1), _form2.array1[j].X * Math.Sin(L1), _form2.array1[j].Z + h);
                        Gl.glVertex3d(_form2.array1[j].X * Math.Cos(L2), _form2.array1[j].X * Math.Sin(L2), _form2.array1[j].Z - H + dH * (i + 1));
                    }
                    Gl.glEnd();
                }

                if (dB * i >= 2 * Math.PI)
                {
                    Gl.glBegin(Gl.GL_LINES);
                    Gl.glVertex3d(_form2.array1[0].X * Math.Cos(L1), _form2.array1[0].X * Math.Sin(L1), _form2.array1[0].Z + h);
                    Gl.glVertex3d(_form2.array1[0].X * Math.Cos(L1), _form2.array1[0].X * Math.Sin(L1), _form2.array1[0].Z + h - dH * Q1 + _form2.dZ);
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

        public void DrawDrill1()
        {
            Gl.glRotatef((float)(-Math.Atan(_form2.arraySV[0].Y /_form2.arraySV[0].X) * 180 / Math.PI), 0, 0, 1);
            Gl.glCallList(GLList1);
        }

        public void DrawDrill2()
        {
            Gl.glTranslated((_form2.R1 + _form2.R2), 0, 0);
            Gl.glRotatef((float)(_form2.O * 180 / Math.PI), 1, 0, 0);
            Gl.glCallList(GLList2);
        }

        private void DrawIntersectionLine1ForList()
        {
            Gl.glColor3f(0, 0, 1);
            Gl.glLineWidth(2);
            Gl.glNormal3d(0, 0, 0);

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (int i = 0; i < _form2.act_quant; i++)
            {
                Gl.glVertex3d(_form2.array4[i].X, _form2.array4[i].Y, _form2.array4[i].Z);
            }
            Gl.glEnd();
        }

        private void DrawIntersectionLine2ForList()
        {
            Gl.glColor3f(0, 0, 1);
            Gl.glLineWidth(2);
            Gl.glNormal3d(0, 0, 0);

            Gl.glBegin(Gl.GL_LINE_STRIP);
            for (int i = 0; i < _form2.act_quant; i++)
            {
                Gl.glVertex3d(_form2.array3[i].X, _form2.array3[i].Y, _form2.array3[i].Z);
            }
            Gl.glEnd();
        }

        public void DrawIntersectionLine1FromList()
        {
            Gl.glCallList(GLList3);
        }

        public void DrawIntersectionLine2FromList()
        {
            Gl.glTranslated((_form2.R1 + _form2.R2), 0, 0);
            Gl.glRotatef((float)(_form2.O * 180 / Math.PI), 1, 0, 0);
            Gl.glCallList(GLList4);
        }

        private void CopyArray(Point[] arr1, Point[] arr2)
        {
            for (int i = 0; i < _form2.act_quant; i++) arr1[i] = arr2[i].Clone();
        }
    }
}