using System.Collections.Generic;
using Microsoft.Kinect;

namespace GhostChamberPlugin.Gestures
{
	class GrabGesture : Gesture
	{
		public bool IsActive(IList<Body> skeletons, int bodyCount)
		{
			return false;
		}
	}
}
