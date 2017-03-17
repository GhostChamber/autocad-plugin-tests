using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			command.Do(gesture.Update());
		}
	}
}
