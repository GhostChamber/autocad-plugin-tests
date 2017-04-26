using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using GhostChamberPlugin.CommandGestureBindings;
using GhostChamberPlugin.Utilities;
using Microsoft.Kinect;

namespace GhostChamberPlugin
{
    /**
     * The PluginMain class extends DrawJig and IDisposable. This class is the entry point to the plugin.
     */
    class PluginMain : DrawJig, IDisposable
	{
        private KinectSensor kinect = null;                             /**< The reference to the Kinect Sensor. */
		private MultiSourceFrameReader frameReader = null;              /**< The depth and color frame reader  */
        private GestureMessenger messenger = null;                      /**< The GestureMessenger is used by the GhostStreamer system to assign which gesture is being used. */
		private IList<Body> skeletons = null;                           /**< List of all active skeletons. */
        private bool nearMode = false;                                  /**< Whether near mode and seated skeleton tracking be enabled*/
        
        /**
         * Set the property NearMode.
         */
        internal bool NearMode
		{
			get { return nearMode; }
		}

		private GestureType currentGesture = GestureType.NONE;          /**< The gesture currently being read by the Kinect. */
		private Dictionary<GestureType, CommandGestureBinding> gestureMapping = new Dictionary<GestureType, CommandGestureBinding>()
		{
			{GestureType.ZOOM, new ZoomBinding()},
			{GestureType.ORBIT, new OrbitBinding()},
			{GestureType.GRAB, new GrabBinding()},
			{GestureType.SNAP_BACK, new SnapBackBinding()}
		};                                                              /**< The Dictionary Container for GestureType, CommandGestureBinding pairs. */

        /**
         * The default constructor of class PluginMain.
         */
        public PluginMain()
		{
			kinect = KinectSensor.GetDefault();
			if (kinect == null)
			{
				Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nCannot find Kinect device.");
			}
			else
			{
				try
				{
					nearMode = (short)Application.GetSystemVariable("KINNEAR") == 1;
				}
				catch
				{
					nearMode = false;
				}
				// Initialise the Kinect sensor
				kinect.Open();
				frameReader = kinect.OpenMultiSourceFrameReader(FrameSourceTypes.Body);
			}

			messenger = new GestureMessenger();
			messenger.ConnectToListener();
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
				if (currentGesture == GestureType.NONE)
				{
					foreach (var binding in gestureMapping)
					{
						if (binding.Value.IsGestureActive(skeletons, kinect.BodyFrameSource.BodyCount))
						{
							currentGesture = binding.Key;
							messenger.SendGestureMessage(currentGesture);
							break;
						}
					}
				}

				if (currentGesture != GestureType.NONE)
				{
					gestureMapping[currentGesture].Update(skeletons, kinect.BodyFrameSource.BodyCount);
					if (!gestureMapping[currentGesture].IsGestureActive(skeletons, kinect.BodyFrameSource.BodyCount))
					{
						currentGesture = GestureType.NONE;
						messenger.SendGestureMessage(currentGesture);
					}
				}

				var frame = frameReader.AcquireLatestFrame();
				if (frame != null)
				{
					using (var bodyFrame = frame.BodyFrameReference.AcquireFrame())
					{
						if (bodyFrame != null)
						{
							if (skeletons == null)
							{
								skeletons = new Body[kinect.BodyFrameSource.BodyCount];
							}
							bodyFrame.GetAndRefreshBodyData(skeletons);
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
			if (kinect != null)
			{
				kinect.Close();
			}

			messenger.DisconnectFromListener();
		}
	}

    /**
     * GhostCommands is the entry point of the plugin application and is invoked by the command 'GHHOSTGO' from the AutoCAD editor. 
     * Please note that the plugin needs to be installed for this to work.
     */
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
