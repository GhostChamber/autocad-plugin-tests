using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using GhostChamberPlugin.Commands;
using GhostChamberPlugin.Gestures;
using Microsoft.Kinect;

namespace GhostChamberPlugin.CommandGestureBindings
{
	class ZoomBinding : CommandGestureBinding
	{
		private ZoomCommand command = new ZoomCommand();
		private ZoomGesture gesture = new ZoomGesture();

		public bool IsGestureActive(IList<Body> skeletons)
		{
			return gesture.IsActive(skeletons);
		}

		public void Update()
		{
			Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;
			for (uint viewportNum = 2; viewportNum < 6; ++viewportNum)
			{
				editor.Command("CVPORT", viewportNum);
				command.Do(gesture.Update());
			}
		}
	}
}
