﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using AqVision.Shape;

namespace IntegrationTesting
{
    public partial class FormShow : Form
    {
        AqVision.Acquistion.AqAcquisitionImage m_Acquisition = new AqVision.Acquistion.AqAcquisitionImage();
        AqVision.Location.AqLocationPattern m_Location = new AqVision.Location.AqLocationPattern();
        public Thread showPic = null;
        public bool m_endThread = false;

        public FormShow()
        {
            InitializeComponent();
            m_Acquisition.Connect();
            listViewRecord.Columns.Add("Serial NO", -2, HorizontalAlignment.Center);
            listViewRecord.Columns.Add("Time", -2, HorizontalAlignment.Center);
            listViewRecord.Columns.Add("Message", -2, HorizontalAlignment.Center);
        }

        ~FormShow()
        {
        }

        private void buttonAcquisition_Click(object sender, EventArgs e)
        {
            try
            {
                aqDisplay1.InteractiveGraphics.Clear();
                aqDisplay1.Update();

                if (ReferenceEquals(showPic, null))
                {
                    showPic = new Thread(new ThreadStart(RegisterVisionAPI));
                    showPic.Start();
                }
                if(showPic.ThreadState == ThreadState.Suspended)
                {
                    showPic.Resume();
                }
            }
            catch (Exception ex)
            {
                m_Acquisition.DisConnect();
                MessageBox.Show(ex.Message);
            }

        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            m_endThread = true;
            Thread.Sleep(4000);
            m_Acquisition.DisConnect();
        }

        public void RegisterVisionAPI()
        {
            bool firstFrame = true;
            while (!m_endThread)
            {
                try
                {
                    aqDisplay1.Invoke(new MethodInvoker(delegate
                        {
                            aqDisplay1.Image = m_Acquisition.Acquisition();
                            if (firstFrame)
                            {
                                firstFrame = false;
                                aqDisplay1.FitToScreen();
                            }
                            aqDisplay1.Update();
                        }));
                }
                catch (SEHException e)
                {
                    AqVision.Interaction.UI2LibInterface.OutputDebugString("SEH Exception: " + e.ToString());
                    MessageBox.Show(e.Message);
                }
                finally
                {
                    AqVision.Interaction.UI2LibInterface.OutputDebugString("Thread Exception");
                    //MessageBox.Show("Thread Exception");
                }
                Thread.Sleep(10);
            }
        }

        private void btn_LoadBitmap_Click(object sender, EventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.gif;*.bmp;*.png;*.tif;*.tiff;*.wmf;*.emf|JPEG Files (*.jpg)|*.jpg;*.jpeg|GIF Files (*.gif)|*.gif|BMP Files (*.bmp)|*.bmp|PNG Files (*.png)|*.png|TIF files (*.tif;*.tiff)|*.tif;*.tiff|EMF/WMF Files (*.wmf;*.emf)|*.wmf;*.emf|All files (*.*)|*.*";
                if (openFileDialog.ShowDialog(this) == DialogResult.OK)
                {
                    aqDisplay1.Image = new Bitmap(openFileDialog.FileName);
                    aqDisplay1.FitToScreen();
                }
            }
            catch (Exception ex)
            {
                m_Acquisition.DisConnect();
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonAddLine_Click(object sender, EventArgs e)
        {
            try
            {
                AqLineSegment lineSegment = new AqLineSegment();
                lineSegment.StartX = 20;
                lineSegment.StartY = 20;
                lineSegment.EndX = 200;
                lineSegment.EndY = 200;
                aqDisplay1.InteractiveGraphics.Add(lineSegment, "AAA 11111", true);
                aqDisplay1.Update();
            }
            catch (Exception ex)
            {
                m_Acquisition.DisConnect();
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonAddRectangle_Click(object sender, EventArgs e)
        {
            try
            {
                buttonStopAcquisition_Click(null, null);

                AqRectangleAffine rectangle = new AqRectangleAffine();
                rectangle.LeftTopPointX = 120;
                rectangle.LeftTopPointY = 120;
                rectangle.RightBottomX = 300;
                rectangle.RightBottomY = 300;
                aqDisplay1.InteractiveGraphics.Add(rectangle, "AAA 22222", true);
                aqDisplay1.Update();
            }
            catch (Exception ex)
            {
                m_Acquisition.DisConnect();
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonAddCircle_Click(object sender, EventArgs e)
        {
            try
            {
                AqGdiPointF LeftTopPoint = new AqGdiPointF(200, 200);
                AqGdiPointF RightBottomPoint = new AqGdiPointF(500, 500);
                AqCircularArc arc = new AqCircularArc(LeftTopPoint, RightBottomPoint, 0, 315);
                aqDisplay1.InteractiveGraphics.Add(arc, "AAA 33333", true);
                aqDisplay1.Update();
            }
            catch (Exception ex)
            {
                m_Acquisition.DisConnect();
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonStopAcquisition_Click(object sender, EventArgs e)
        {
            try
            {
                if(!ReferenceEquals(showPic, null))
                {
                    //if (showPic.ThreadState == ThreadState.Running)
                    {
                        showPic.Suspend();
                    }
                }
            }
            catch (Exception ex)
            {
                m_Acquisition.DisConnect();
                MessageBox.Show(ex.Message);
            }
        }

        private void buttonTraining_Click(object sender, EventArgs e)
        {
            try
            {
                if (aqDisplay1.InteractiveGraphics.Count > 0)
                {
                    m_Location.OriginImage = aqDisplay1.Image;
                    m_Location.RoiRegionTemplate = (AqRectangleAffine)(aqDisplay1.InteractiveGraphics[0]);
                    m_Location.CreateTempateImage();
                    AddMessageToListView((new Rectangle((int)(m_Location.RoiRegionTemplate.LeftTopPointX), (int)(m_Location.RoiRegionTemplate.LeftTopPointY),
                                           (int)(m_Location.RoiRegionTemplate.Width), (int)(m_Location.RoiRegionTemplate.Height))).ToString());
                    aqDisplay1.InteractiveGraphics.Clear();
                    aqDisplay1.Update();
                }
                else
                {
                    AddMessageToListView("aqDisplay1 is empty");
                }
            }
            catch (Exception ex)
            {
                m_Acquisition.DisConnect();
                MessageBox.Show(ex.Message);
            }

        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            try
            {
                string strMessage = m_Location.TemplateRegionCenter.X.ToString() + " " + m_Location.TemplateRegionCenter.X.ToString();
                AddMessageToListView(strMessage);
                m_Location.RunMatcher();
                strMessage = m_Location.MatherResult.ToString() + " " + m_Location.ResultX.ToString() + " " + m_Location.ResultY.ToString() + " (" + m_Location.ResultAngle.ToString() +" )";
                AddMessageToListView(strMessage);

                //aqDisplay1.Image = m_Acquisition.RevBitmap;
                //aqDisplay1.Update();
            }
            catch (Exception ex)
            {
                m_Acquisition.DisConnect();
                MessageBox.Show(ex.Message);
            }
        }

        public void AddMessageToListView(string strMessage)
        {
            ListViewItem item = new ListViewItem(listViewRecord.Items.Count.ToString(), 0);
            item.SubItems.Add(new DateTime().ToString());
            item.SubItems.Add(strMessage);
            listViewRecord.Items.Add(item);
        }

        private void buttonCloseCamera_Click(object sender, EventArgs e)
        {
            m_Acquisition.DisConnect();
        }

        private void button1_Test_Click(object sender, EventArgs e)
        {
            int iState = 0;
            int resultX = 0;
            int resultY = 0;
            string path = "D:\\Bitmap.bmp";
            char[] angle = new char[5];
            AqVision.Interaction.UI2LibInterface.GetImageSpecificLocation1(1218, 1218, 752, 517, 180, 180, path, ref iState, ref resultX, ref resultY, angle);
            string anglestring = new string(angle);
            string strMessage = resultX.ToString() + " " + resultY.ToString() + " ( " + angle[0].ToString() + angle[1].ToString() + angle[2].ToString() + " )" + " >> " + anglestring;
            AddMessageToListView(strMessage);
        }
    }
}
