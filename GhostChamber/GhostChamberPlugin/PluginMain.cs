using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using GhostChamberPlugin.Gestures;
using Microsoft.Kinect;

namespace GhostChamberPlugin
{
	class PluginMain : DrawJig, IDisposable
	{
		// Kinect sensor and depth and color frame reader
		private KinectSensor _kinect = null;
		private MultiSourceFrameReader _frameReader = null;

		// List of all active skeletons
		private IList<Microsoft.Kinect.Body> _skeletons = null;

		// Should near mode and seated skeleton tracking be enabled
		private bool _nearMode = false;
		internal bool NearMode
		{
			get { return _nearMode; }
		}

		private ZoomGesture _zoom = new ZoomGesture();

		public PluginMain()
		{
			_kinect = KinectSensor.GetDefault();
			if (_kinect == null)
			{
				Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nCannot find Kinect device.");
			}
			else
			{
				try
				{
					_nearMode = (short)Application.GetSystemVariable("KINNEAR") == 1;
				}
				catch
				{
					_nearMode = false;
				}
				// Initialise the Kinect sensor
				_kinect.Open();
				_frameReader = _kinect.OpenMultiSourceFrameReader(FrameSourceTypes.Body);
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
				_zoom.HandleZoomGesture(_skeletons, _kinect.BodyFrameSource.BodyCount);

				var frame = _frameReader.AcquireLatestFrame();
				if (frame != null)
				{
					using (var bodyFrame = frame.BodyFrameReference.AcquireFrame())
					{
						if (bodyFrame != null)
						{
							if (_skeletons == null)
							{
								_skeletons = new Microsoft.Kinect.Body[_kinect.BodyFrameSource.BodyCount];
							}
							bodyFrame.GetAndRefreshBodyData(_skeletons);
						}
					}
				}

				// Set the cursor without actually moving it - enough to
				// generate a Windows message
				var pt = System.Windows.Forms.Cursor.Position;
				System.Windows.Forms.Cursor.Position = new System.Drawing.Point(pt.X, pt.Y);
				return SamplerStatus.OK;
			}
			return SamplerStatus.Cancel;
		}

		protected override bool WorldDraw(WorldDraw draw)
		{
			// Do nothing
			return true;
		}

		public void Dispose()
		{
			// Uninitialise the Kinect sensor
			if (_kinect != null)
			{
				_kinect.Close();
			}
		}
	}

	public class GhostCommands
	{
		[CommandMethod("GHOSTPLUGINS", "GHOSTGO", CommandFlags.Modal)]
		public void KinectSkeletons()
		{
			var editor = Application.DocumentManager.MdiActiveDocument.Editor;
			try
			{
				// Create and use our jig, disposing afterwards
				using (var plugin = new PluginMain())
				{
					editor.Drag(plugin);
				}
			}
			catch (System.Exception ex)
			{
				editor.WriteMessage("\nUnable to start Kinect sensor: " + ex.Message);
			}
		}
	}
}
