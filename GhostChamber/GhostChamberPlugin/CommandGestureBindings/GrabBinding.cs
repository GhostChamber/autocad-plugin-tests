﻿using System.Collections.Generic;
using GhostChamberPlugin.Commands;
using GhostChamberPlugin.Gestures;
using Microsoft.Kinect;

namespace GhostChamberPlugin.CommandGestureBindings
{
    /**
     * Class implements CommandGestureBinding for the Grab gesture. 
     * The grab gesture uses a grabbing motion to 'grab' the object and pan along it in AutoCAD
     */
    class GrabBinding : CommandGestureBinding
	{
		private GrabCommand command = new GrabCommand();    /**< The GrabCommand that the Gesture will use. */
        private GrabGesture gesture = new GrabGesture();    /**< The GrabGesture that the Command requires. */

        /** Checks if this gesture is active or not. Calls the IsActive Method on gesture.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
		 */
        public bool IsGestureActive(IList<Body> skeletons, int bodyCount)
		{
			return gesture.IsActive(skeletons, bodyCount);
		}

        /** Calls the Update method of the Gesture and pass the return value to the bound command. Calls the Do method on command.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
		 */
        public void Update(IList<Body> skeletons, int bodyCount)
		{
			command.Do(gesture.Update(skeletons, bodyCount));
		}
	}
}
