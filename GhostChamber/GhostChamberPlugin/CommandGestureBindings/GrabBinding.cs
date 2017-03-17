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
	class GrabBinding : CommandGestureBinding
	{
		private GrabCommand command = new GrabCommand();
		private GrabGesture gesture = new GrabGesture();

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
