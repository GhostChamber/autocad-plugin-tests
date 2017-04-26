using System.Collections.Generic;
using GhostChamberPlugin.Commands;
using GhostChamberPlugin.Gestures;
using Microsoft.Kinect;

namespace GhostChamberPlugin.CommandGestureBindings
{
    class GrabBinding : CommandGestureBinding
	{
		private GrabCommand command = new GrabCommand();
		private GrabGesture gesture = new GrabGesture();

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
