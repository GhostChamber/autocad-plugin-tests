using System.Collections.Generic;
using GhostChamberPlugin.Commands;
using GhostChamberPlugin.Gestures;
using Microsoft.Kinect;

namespace GhostChamberPlugin.CommandGestureBindings
{
    class ZoomBinding : CommandGestureBinding
	{
		private ZoomCommand command = new ZoomCommand();
		private ZoomGesture gesture = new ZoomGesture();

		public bool IsGestureActive(IList<Body> skeletons, int bodyCount)
		{
			return gesture.IsActive(skeletons, bodyCount);
		}

		public void Update(IList<Body> skeletons, int bodyCount)
		{
			command.Do(gesture.Update(skeletons, bodyCount));
		}
	}
}
