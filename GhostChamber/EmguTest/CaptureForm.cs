using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Kinect;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.UI;

namespace EmguTest
{
    public partial class CaptureForm : Form
    {
        private KinectSensor mKinect = null;
        private MultiSourceFrameReader mFrameReader = null;
        private Timer mTickTimer;

        public static ushort[] depthData = null;
        public static byte[] pixelData = null;
        public static int depthWidth = 0;
        public static int depthHeight = 0;

        private Mat mFrame;

        public CaptureForm()
        {
            InitializeComponent();
            InitializeKinect();

            mTickTimer = new Timer();
            mTickTimer.Tick += Tick;
            mTickTimer.Interval = 16; // approximately 60 fps
            mTickTimer.Start();
        }

        public void InitializeKinect()
        {
            mKinect = KinectSensor.GetDefault();

            if (mKinect  == null)
            {
                Console.WriteLine("Cannot connect to Kinect.");
            }
            else
            {
                mKinect.Open();
                mFrameReader = mKinect.OpenMultiSourceFrameReader(FrameSourceTypes.Depth);
            }

        }

        public void Tick(Object sender, EventArgs args)
        {
            if (mKinect != null)
            {
                MultiSourceFrame frame = mFrameReader.AcquireLatestFrame();

                if (frame != null)
                {
                    using (var depthFrame = frame.DepthFrameReference.AcquireFrame())
                    {
                        if (depthFrame != null)
                        {
                            if (depthData == null)
                            {
                                depthWidth = depthFrame.FrameDescription.Width;
                                depthHeight = depthFrame.FrameDescription.Height;

                                depthData = new ushort[depthWidth * depthHeight];
                                pixelData = new byte[depthWidth * depthHeight * 3];
                                mFrame = new Mat(depthHeight, depthWidth, DepthType.Cv8U, 3);
                            }

                            ushort minDepth = depthFrame.DepthMinReliableDistance;
                            ushort maxDepth = depthFrame.DepthMaxReliableDistance;

                            depthFrame.CopyFrameDataToArray(depthData);
                            Image<Bgr, Byte> img = mFrame.ToImage<Bgr, Byte>();

                            for (int i = 0; i < depthData.Length; i++)
                            {
                                ushort depth = depthData[i];
                                //byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);
                                byte intensity = (byte) (depth < 1500  && depth > 500 ? 0 : 255);

                                img.Data[i / depthWidth, i % depthWidth, 0] = intensity;
                                img.Data[i / depthWidth, i % depthWidth, 1] = intensity;
                                img.Data[i / depthWidth, i % depthWidth, 2] = intensity;
                            }

                            (Controls["FrameImageBox"] as ImageBox).Image = img;
                        }
                    }
                }
            }

        }
    }
}
