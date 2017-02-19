using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Interop;
using Microsoft.Kinect;

namespace GhostChamberPlugin.Experiment
{
    public enum GestureState
    {
        NONE,
        ZOOMING,
        PANNING,
        GRABBING
    }

    public class KinectSkeletonJig : DrawJig, IDisposable
    {
        // We need our Kinect sensor

        private KinectSensor _kinect = null;

        // We need a reader for depth and colour frames

        private MultiSourceFrameReader _frameReader = null;

        // Our skeleton list

        private IList<Microsoft.Kinect.Body> _skeletons = null;

        // A list of lines representing our skeleton(s)

        private List<Line> _lines;

        // Flags to make sure we don't end up both modifying
        // and accessing the _lines member at the same time

        private bool _drawing = false;
        private bool _capturing = false;

        // Should near mode and seated skeleton tracking be enabled

        private bool _nearMode = false;
        internal bool NearMode
        {
            get { return _nearMode; }
        }

        private GestureState gestureState;

        private bool[] zooming;
        private float zoomLeft = 0.0f;
        private float zoomRight = 0.0f;
        private bool zoomRightCaptured = false;
        private const float ZOOM_SCALE = 10.0f;
        private Microsoft.Kinect.Body activeBody = null;

        public KinectSkeletonJig()
        {
            // Initialise members

            _lines = new List<Line>();
            _kinect = KinectSensor.GetDefault();
            gestureState = GestureState.NONE;

            if (_kinect == null)
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage(
                  "\nCannot find Kinect device."
                );
            }
            else
            {
                _drawing = false;
                _capturing = false;

                try
                {
                    _nearMode =
                      (short)Application.GetSystemVariable("KINNEAR") == 1;
                }
                catch
                {
                    _nearMode = false;
                }

                // Initialise the Kinect sensor

                _kinect.Open();

                _frameReader =
                  _kinect.OpenMultiSourceFrameReader(
                    FrameSourceTypes.Body
                  );
            }
        }

        public void Dispose()
        {
            // Uninitialise the Kinect sensor

            if (_kinect != null)
            {
                _kinect.Close();
            }

            // Clear our line list

            ClearLines();
        }

        protected void HandleZoomGesture()
        {
            if (gestureState == GestureState.NONE && _skeletons != null)
            {
                if (zooming == null)
                {
                    zooming = new bool[_kinect.BodyFrameSource.BodyCount];
                }

                for (int  i = 0; i < _kinect.BodyFrameSource.BodyCount; i++)
                {
                    Microsoft.Kinect.Body body = _skeletons[i];

                    //Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;
                    //ed.WriteMessage($"Head Y: {_skeletons[i].Joints[JointType.Head].Position.Y}\n");

                    if (body.Joints[JointType.Head].Position.Y != 0.0f &&
                        (Math.Abs(body.Joints[JointType.HandLeft].Position.Y - 
                        body.Joints[JointType.Head].Position.Y) < 0.2f))
                    {
                        gestureState = GestureState.ZOOMING;
                        zoomLeft = body.Joints[JointType.HandLeft].Position.X;
                        activeBody = body;
                        zooming[i] = true;
                    }
                    else
                    {
                        zooming[i] = false;
                    }
                }
            }
            else if (gestureState == GestureState.ZOOMING)
            {
                if (activeBody != null && 
                    !zoomRightCaptured)
                {
                    if (Math.Abs(activeBody.Joints[JointType.HandRight].Position.Y -
                        activeBody.Joints[JointType.Head].Position.Y) < 0.2f)
                    {
                        zoomRightCaptured = true;
                        zoomRight = activeBody.Joints[JointType.HandRight].Position.X;
                    }
                }

                if (activeBody != null &&
                    zoomRightCaptured)
                {
                    float zoomRight = activeBody.Joints[JointType.HandRight].Position.X;
                    float xRange = ZOOM_SCALE * Math.Abs(zoomLeft - zoomRight);
                    float yRange = xRange;

                    float x = 0.0f;
                    float y = 0.0f;


                    Document doc =
                        Application.DocumentManager.MdiActiveDocument;
                    Database db = doc.Database;
                    Editor ed = doc.Editor;

                    Point2d min2d = new Point2d(x - xRange, y - yRange);
                    Point2d max2d = new Point2d(x + xRange, y + yRange);

                    ViewTableRecord view =
                      new ViewTableRecord();

                    view.CenterPoint = new Point2d(0.0f, 0.0f);
                    view.Width = xRange;
                    view.Height = yRange;
                    //view.CenterPoint =
                    //  min2d + ((max2d - min2d) / 2.0);
                    //view.Height = max2d.Y - min2d.Y;
                    //view.Width = max2d.X - min2d.X;
                    ed.SetCurrentView(view);


                    if (Math.Abs(activeBody.Joints[JointType.HandLeft].Position.Y -
                        activeBody.Joints[JointType.Head].Position.Y) > 0.2f)
                    {
                        gestureState = GestureState.NONE;
                        zoomLeft = 0.0f;
                        zoomRight = 0.0f;
                        zoomRightCaptured = false;
                        activeBody = null;
                    }
                }
            }
        }

        protected override SamplerStatus Sampler(JigPrompts prompts)
        {
            // We don't really need a point, but we do need some
            // user input event to allow us to loop, processing
            // for the Kinect input

            var opts = new JigPromptPointOptions("\nClick to finish: ");
            opts.Cursor = CursorType.Invisible;
            var ppr = prompts.AcquirePoint(opts);
            if (ppr.Status == PromptStatus.OK)
            {
                if (!_drawing)
                {
                    _capturing = true;

                    HandleZoomGesture();

                    // Clear any previous lines

                    ClearLines();

                    var frame = _frameReader.AcquireLatestFrame();
                    if (frame != null)
                    {
                        using (
                          var bodyFrame = frame.BodyFrameReference.AcquireFrame()
                        )
                        {
                            if (bodyFrame != null)
                            {
                                if (_skeletons == null)
                                {
                                    _skeletons =
                                      new Microsoft.Kinect.Body[
                                        _kinect.BodyFrameSource.BodyCount
                                      ];
                                }
                                bodyFrame.GetAndRefreshBodyData(_skeletons);
                            }
                        }
                    }

                    // We'll colour the skeletons from yellow, onwards
                    // (red is a bit dark)

                    short col = 2;

                    // Loop through each of the skeletons

                    if (_skeletons != null)
                    {
                        int index = 0;

                        foreach (var skel in _skeletons)
                        {
                            // Add skeleton vectors for tracked/positioned
                            // skeletons

                            AddLinesForSkeleton(_lines, skel, col++, index);
                            index++;
                        }
                        _capturing = false;
                    }
                }

                // Set the cursor without actually moving it - enough to
                // generate a Windows message

                var pt = System.Windows.Forms.Cursor.Position;
                System.Windows.Forms.Cursor.Position =
                  new System.Drawing.Point(pt.X, pt.Y);

                return SamplerStatus.OK;
            }
            return SamplerStatus.Cancel;
        }

        protected override bool WorldDraw(WorldDraw draw)
        {
            if (!_capturing)
            {
                _drawing = true;

                // Draw each of our lines

                short oidx = draw.SubEntityTraits.Color;

                foreach (var ln in _lines)
                {
                    // Set the colour and lineweight in the subentity
                    // traits based on the original line

                    if (ln != null)
                    {
                        draw.SubEntityTraits.Color = (short)ln.ColorIndex;
                        draw.SubEntityTraits.LineWeight = ln.LineWeight;

                        ln.WorldDraw(draw);
                    }
                }

                draw.SubEntityTraits.Color = oidx;

                _drawing = false;
            }

            return true;
        }

        // Translate from Skeleton Space to WCS

        internal static Point3d PointFromVector(
          CameraSpacePoint p, bool flip = true
        )
        {
            // Rather than just return a point, we're effectively
            // transforming it to the drawing space: flipping the
            // Y and Z axes (which makes it consistent with the
            // point cloud, and makes sure Z is actually up - from
            // the Kinect's perspective Y is up), and reversing
            // the X axis (which is the result of choosing UseDepth
            // rather than UseDepthAndPlayerIndex)

            return new Point3d(flip ? -p.X : p.X, p.Z, p.Y);
        }

        private void AddLinesForSkeleton(
          List<Line> lines, Microsoft.Kinect.Body sk, int idx, int index
        )
        {
            // Hard-code lists of connections between joints

            var links =
              new int[][]
              {
          // Head to left toe
          new int[] { 3, 2, 20, 1, 0, 12, 13, 14, 15 },
          // Hips to right toe
          new int[] { 0, 16, 17, 18, 19 },
          // Left hand to right hand
          new int[] { 21, 7, 6, 5, 4, 2, 8, 9, 10, 11, 23 },
          // Left thumb to palm
          new int[] { 22, 7 },
          // Right thumb to palm
          new int[] { 24, 11 }
              };

            // Populate an array of joints

            var joints = new Point3dCollection();
            for (int i = 0; i < sk.Joints.Count; i++)
            {
                joints.Add(
                  PointFromVector(
                    sk.Joints[(JointType)i].Position, false
                  )
                );
            }

            // For each path of joints, create a sequence of lines

            int limit = sk.Joints.Count - 1;

            foreach (int[] link in links)
            {
                for (int i = 0; i < link.Length - 1; i++)
                {
                    // Only add lines where links are within bounds
                    // (check needed for seated mode)

                    int first = link[i],
                        second = link[i + 1];

                    if (
                      isValidJoint(first, limit) &&
                      isValidJoint(second, limit)
                    )
                    {
                        // Line from this vertex to the next

                        var ln = new Line(joints[first], joints[second]);

                        // Set the color to distinguish between skeletons

                        if (zooming != null && 
                            zooming[index])
                        {
                            ln.ColorIndex = 4;
                        }
                        else
                        {
                            ln.ColorIndex = idx;
                        }

                        // Make tracked skeletons bolder

                        ln.LineWeight =
                          (sk.IsTracked ?
                            LineWeight.LineWeight050 :
                            LineWeight.LineWeight000
                          );

                        lines.Add(ln);
                    }
                }
            }
        }

        private bool isValidJoint(int id, int upperLimit)
        {
            // A joint is valid if it indexes into the collection
            // and if it's above the spine in near mode

            return id <= upperLimit && (!_nearMode || id > 1);
        }

        private void ClearLines()
        {
            // Dispose each of the lines and clear the list

            foreach (Line ln in _lines)
            {
                ln.Dispose();
            }
            _lines.Clear();
        }
    }

    public class SkeletonCommands
    {
        [CommandMethod("ADNPLUGINS", "KINSKEL", CommandFlags.Modal)]
        public void KinectSkeletons()
        {
            var ed =
                Application.DocumentManager.MdiActiveDocument.Editor;

            try
            {
                // Create and use our jig, disposing afterwards

                using (var sj = new KinectSkeletonJig())
                {
                    ed.Drag(sj);
                }
            }
            catch (System.Exception ex)
            {
                ed.WriteMessage(
                    "\nUnable to start Kinect sensor: " + ex.Message
                );
            }
        }
    }
}