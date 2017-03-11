using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.Util;
using Emgu.CV.UI;

namespace EmguTest
{
    class FingerTracker
    {
        public VideoCapture mCapture;
        public Mat mFrame;
        public CaptureForm mCaptureForm;

        public FingerTracker()
        {
            CvInvoke.UseOpenCL = false;
            mCaptureForm = null;
            mFrame = new Mat();

            try
            {
                mCapture = new VideoCapture();
                mCapture.ImageGrabbed += ProcessFrame;
                mCapture.Start();
            }
            catch (NullReferenceException exception)
            {
                MessageBox.Show(exception.Message);
            }
        }

        private void ProcessFrame(object sender, EventArgs args)
        {
            if (mCapture != null &&
                mCapture.Ptr != IntPtr.Zero)
            {
                mCapture.Retrieve(mFrame, 0);

                if (mCaptureForm != null)
                {
                    (mCaptureForm.Controls["FrameImageBox"] as ImageBox).Image = mFrame;
                }
            }
        }
    }
}
