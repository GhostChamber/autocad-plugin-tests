using System.Collections.Generic;
using GhostChamberPlugin.Commands;
using GhostChamberPlugin.Gestures;
using Microsoft.Kinect;
using Autodesk.AutoCAD.Geometry;

namespace GhostChamberPlugin.CommandGestureBindings
{
    class OrbitBinding : CommandGestureBinding
    {
        private OrbitCommand command = new OrbitCommand();      /**< The OrbitCommand that the Gesture will use. */
        private OrbitGesture gesture = new OrbitGesture();      /**< The OrbitGesture that the Command requires. */

        private const double X_ROTATION_MULTIPLIER = -3.0;      /**< Factor to multiply the X-rotation by to translate from real world space to AutoCAD space. */
        private const double Y_ROTATION_MULTIPLIER = 2.0;       /**< Factor to multiply the Y-rotation by to translate from real world space to AutoCAD space. */

        /** Checks if this gesture is active or not. Calls the IsActive Method on gesture.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
		 */
        public bool IsGestureActive(IList<Body> skeletons, int bodyCount)
        {
            return gesture.IsActive(skeletons, bodyCount);
        }

        /** Calls the Update method of the Gesture and pass the return value to the bound command. 
         * Calls the Do method on the command using for both the X and Y axes.
         * @param skeletons is the list of Body objects found by the Kinect.
         * @param bodyCount is the number of bodies found in the skeletons list.
		 */
        public void Update(IList<Body> skeletons, int bodyCount)
        {
            Vector3d movement = gesture.Update(skeletons, bodyCount);
            if (movement.Length > 0.0)
            {
                command.Do(Vector3d.ZAxis, movement.X * X_ROTATION_MULTIPLIER);
                command.Do(Vector3d.XAxis, movement.Y * Y_ROTATION_MULTIPLIER);
            }
        }
    }
}