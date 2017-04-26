using System.Collections.Generic;
using GhostChamberPlugin.Commands;
using GhostChamberPlugin.Gestures;
using Microsoft.Kinect;

namespace GhostChamberPlugin.CommandGestureBindings
{
    class SnapBackBinding : CommandGestureBinding
    {
        private SnapBackCommand command = new SnapBackCommand();
        private SnapBackGesture gesture = new SnapBackGesture();

        public bool IsGestureActive(IList<Body> skeletons, int bodyCount)
        {
            return gesture.IsActive(skeletons, bodyCount);
        }

        public void Update(IList<Body> skeletons, int bodyCount)
        {
            command.Do();
            gesture.Update();
        }
    }
}
