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
using Emgu.CV.Util;
using Emgu.CV.UI;

namespace EmguTest
{
    public partial class CaptureForm : Form
    {
        private KinectSensor mKinect = null;
        private MultiSourceFrameReader mFrameReader = null;
        private Timer mTickTimer;
        private IList<Microsoft.Kinect.Body> mSkeletons = null;
        private DepthSpacePoint[] mLeftHandPoints = null;
        private DepthSpacePoint[] mRightHandPoints = null;

        public static ushort[] depthData = null;
        public static byte[] pixelData = null;
        public static int depthWidth = 0;
        public static int depthHeight = 0;

        private Mat mFrame;

        const double THRESHOLD = 80.0;
        const double THRESHOLD_MAX_VALUE = 255;

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
                mFrameReader = mKinect.OpenMultiSourceFrameReader(FrameSourceTypes.Depth | FrameSourceTypes.Body);
            }
        }

        private bool IsDefectUnique(Point point, List<Point> pointList)
        {
            foreach (Point p in pointList)
            {
                if (Math.Sqrt( (point.X - p.X) * (point.X - p.X) + 
                               (point.Y - p.Y) * (point.Y - p.Y)) < 15.0)
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsDefectOutsideHandRadius(Point startPoint)
        {
            if (mKinect != null &&
                mRightHandPoints != null &&
                mLeftHandPoints != null)
            {
                for (int i = 0; i < mKinect.BodyFrameSource.BodyCount; i++)
                {
                    if (GetPointDistance(mRightHandPoints[i], startPoint) < 15 &&
                        GetPointDistance(mLeftHandPoints[i], startPoint) < 15)
                    {
                        return false;
                    }
                }
            }

            return true;
        }

        private double GetPointDistance(DepthSpacePoint p1, Point p2)
        {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }
        private void TrackFingers()
        {

        }

        public void UpdateBody()
        {
            if (mKinect != null)
            {
                MultiSourceFrame frame = mFrameReader.AcquireLatestFrame();
                if (frame != null)
                {
                    using (var bodyFrame = frame.BodyFrameReference.AcquireFrame())
                    {
                        if (bodyFrame != null)
                        {
                            if (mSkeletons == null)
                            {
                                mSkeletons = new Microsoft.Kinect.Body[mKinect.BodyFrameSource.BodyCount];
                                mRightHandPoints = new DepthSpacePoint[mKinect.BodyFrameSource.BodyCount];
                                mLeftHandPoints = new DepthSpacePoint[mKinect.BodyFrameSource.BodyCount];
                            }
                            bodyFrame.GetAndRefreshBodyData(mSkeletons);
                        }
                    }
                }

                // Update hand positions
                if (mSkeletons != null &&
                    mRightHandPoints != null &&
                    mLeftHandPoints != null)
                {
                    for (int i = 0; i < mKinect.BodyFrameSource.BodyCount; i++)
                    {
                        mRightHandPoints[i] = mKinect.CoordinateMapper.MapCameraPointToDepthSpace(mSkeletons[i].Joints[JointType.HandRight].Position);
                        mLeftHandPoints[i] = mKinect.CoordinateMapper.MapCameraPointToDepthSpace(mSkeletons[i].Joints[JointType.HandLeft].Position);
                    }
                }
            }
        }

        public void Tick(Object sender, EventArgs args)
        {
            if (mKinect != null)
            {
                MultiSourceFrame frame = mFrameReader.AcquireLatestFrame();

                TrackFingers();

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
                                mFrame = new Mat(depthHeight, depthWidth, DepthType.Cv8U, 1);
                            }

                            ushort minDepth = depthFrame.DepthMinReliableDistance;
                            ushort maxDepth = depthFrame.DepthMaxReliableDistance;

                            depthFrame.CopyFrameDataToArray(depthData);
                            Image<Gray, Byte> img = mFrame.ToImage<Gray, Byte>();

                            for (int i = 0; i < depthData.Length; i++)
                            {
                                ushort depth = depthData[i];
                                //byte intensity = (byte)(depth >= minDepth && depth <= maxDepth ? depth : 0);
                                byte intensity = (byte) (depth < 1000  && depth > 10 ? 0 : 255);

                                img.Data[i / depthWidth, i % depthWidth, 0] = intensity;
                            }

                            mFrame = img.Mat;

                            // DISPLAY Depth image
                            //(Controls["FrameImageBox"] as ImageBox).Image = img;

                            //*********************
                            // Gaussian Blur
                            //*********************
                            CvInvoke.GaussianBlur(img, img, new Size(5, 5), 0);

                            //*********************
                            // Threshold
                            //*********************
                            //mFrame = img.Mat;

                            //Mat thresholds = new Mat(); ;

                            //CvInvoke.Threshold(mFrame, thresholds, THRESHOLD, THRESHOLD_MAX_VALUE, ThresholdType.Binary);

                            //// DISPLAY Thresholds
                            //(Controls["FrameImageBox"] as ImageBox).Image = img;

                            //*********************
                            // Contours
                            //*********************
                            Mat hierarchy = new Mat();

                            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
                            VectorOfVectorOfPointF significantContours = new VectorOfVectorOfPointF();
                            CvInvoke.FindContours(mFrame, contours, hierarchy, RetrType.Tree, ChainApproxMethod.ChainApproxNone);

                            Image<Gray, Byte> contourImage = new Image<Gray, Byte>(mFrame.Size);

                            for (int i = 0; i < contours.Size; i++)
                            {
                                if (CvInvoke.ContourArea(contours[i]) > 500.0)
                                {
                                    VectorOfPointF bigContour = new VectorOfPointF();
                                    System.Drawing.PointF[] points = new System.Drawing.PointF[contours[i].Size];
                                    Point[] intPoints = contours[i].ToArray();

                                    for (int j = 0; j < intPoints.Length; j++)
                                    {
                                        points[j] = intPoints[j];
                                    }

                                    bigContour.Push(points);
                                    significantContours.Push(bigContour);
                                }
                            }

                            //if (contours.Size > 0)
                            //{
                            //    CvInvoke.DrawContours(contourImage, significantContours, -1, new MCvScalar(255, 0, 0));
                            //}

                            //(Controls["FrameImageBox"] as ImageBox).Image = contourImage;

                            //*********************
                            // Convex Hulls
                            //*********************
                            for (int i = 0; i < significantContours.Size; i++)
                            {
                                System.Drawing.PointF[] hullPoints;
                                VectorOfPoint contourPoints = new VectorOfPoint(Array.ConvertAll(significantContours[i].ToArray(), Point.Round));
                                VectorOfInt convexHull = new VectorOfInt();

                                hullPoints = CvInvoke.ConvexHull(significantContours[i].ToArray());
                                CvInvoke.ConvexHull(contourPoints, convexHull);

                                CvInvoke.Polylines(mFrame, Array.ConvertAll(hullPoints, Point.Round), true, new MCvScalar(255, 255, 255));

                                // How many defects tho?
                                //VectorOfVectorOfInt defects = new VectorOfVectorOfInt();
                                Mat defects = new Mat();
                                CvInvoke.ConvexityDefects(contourPoints /*significantContours[i]*/,
                                                          convexHull /*new VectorOfPointF(hullPoints)*/,
                                                          defects);

                                if (!defects.IsEmpty)
                                {
                                    Matrix<int> m = new Matrix<int>(defects.Rows, defects.Cols, defects.NumberOfChannels);
                                    defects.CopyTo(m);

                                    List<Point> validPoints = new List<Point>();

                                    // Draw tha defacts
                                    for (int d = 0; d < m.Rows; d++)
                                    {
                                        int startIndex = m.Data[d, 0];
                                        int endIndex = m.Data[d, 1];
                                        int farthestIndex = m.Data[d, 2];

                                        Point farthestPoint = contourPoints[farthestIndex];
                                        Point startPoint = contourPoints[startIndex];

                                        if (IsDefectUnique(startPoint, validPoints) &&
                                            IsDefectOutsideHandRadius(startPoint))
                                        {
                                            validPoints.Add(startPoint);
                                        }
                                        
                                        //if (true/*endIndex - startIndex > 10*/)
                                        //{
                                        //    CvInvoke.Circle(mFrame, startPoint, 3, new MCvScalar(255, 0, 0), 2);
                                        //}
                                    }

                                    // Draw valid indices
                                    foreach (Point p in validPoints)
                                    {
                                        CvInvoke.Circle(mFrame, p, 3, new MCvScalar(255, 0, 0), 2);
                                    }
                                }
                            }

                            (Controls["FrameImageBox"] as ImageBox).Image = mFrame;
                        }
                    }
                }
            }

        }
    }
}
