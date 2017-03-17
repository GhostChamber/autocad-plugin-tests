
using System.Collections.Generic;
using Microsoft.Kinect;

namespace GhostChamberPlugin.Gestures
{
	interface Gesture
	{
        bool IsActive(IList<Body> skeletons);
	}
}
