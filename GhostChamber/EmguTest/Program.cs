using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EmguTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            FingerTracker fingerTracker = new FingerTracker();

            CaptureForm captureForm = new CaptureForm();
            fingerTracker.mCaptureForm = captureForm;
            Application.Run(captureForm);
        }


    }
}
