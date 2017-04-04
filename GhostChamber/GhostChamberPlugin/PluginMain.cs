using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using GhostChamberPlugin.CommandGestureBindings;
using GhostChamberPlugin.Gestures;
using GhostChamberPlugin.Utilities;
using Microsoft.Kinect;

namespace GhostChamberPlugin
{
	class PluginMain : DrawJig, IDisposable
	{
		// Kinect sensor and depth and color frame reader
		private KinectSensor _kinect = null;
		private MultiSourceFrameReader _frameReader = null;
        private GestureMessenger _messenger = null;

		// List of all active skeletons
		private IList<Microsoft.Kinect.Body> _skeletons = null;

		// Should near mode and seated skeleton tracking be enabled
		private bool _nearMode = false;
		internal bool NearMode
		{
			get { return _nearMode; }
		}

		private GestureType _currentGesture = GestureType.NONE;
		private Dictionary<GestureType, CommandGestureBinding> _gestureMapping = new Dictionary<GestureType, CommandGestureBinding>()
		{
            {GestureType.ZOOM, new ZoomBinding()},
            {GestureType.ORBIT, new OrbitBinding()},
            //{GestureType.PAN, new PanBinding()},
			{GestureType.GRAB, new GrabBinding() }
		};

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

            _messenger = new GestureMessenger();
            _messenger.ConnectToListener();
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
				if (_currentGesture == GestureType.NONE)
				{
					foreach (var binding in _gestureMapping)
					{
						if (binding.Value.IsGestureActive(_skeletons, _kinect.BodyFrameSource.BodyCount))
						{
							_currentGesture = binding.Key;
                            _messenger.SendGestureMessage(_currentGesture);
							//Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"Gesture Type = {_currentGesture}\n");
							break;
						}
					}
				}

				if (_currentGesture != GestureType.NONE)
				{
					_gestureMapping[_currentGesture].Update(_skeletons, _kinect.BodyFrameSource.BodyCount);
					if (!_gestureMapping[_currentGesture].IsGestureActive(_skeletons, _kinect.BodyFrameSource.BodyCount))
					{
						_currentGesture = GestureType.NONE;
                        _messenger.SendGestureMessage(_currentGesture);
                        //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage($"Gesture Type = {_currentGesture}\n");
                    }
				}

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

            _messenger.DisconnectFromListener();
		}
	}

	public class GhostCommands
	{
		[CommandMethod("GHOSTCHAMBER", "GHOSTGO", CommandFlags.Modal)]
		public void GhostPluginStart()
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
