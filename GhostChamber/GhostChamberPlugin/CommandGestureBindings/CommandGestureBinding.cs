using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace GhostChamberPlugin.CommandGestureBindings
{
	interface CommandGestureBinding
	{
		/** Checks if the current gesture is active or not
		 */
		bool IsGestureActive(IList<Body> skeletons, int bodyCount);

		/** Calls the Update method of the Gesture and pass the return value to the bound command
		 */
		void Update(IList<Body> skeletons, int bodyCount);
	}
}
