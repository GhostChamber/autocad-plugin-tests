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
    class PanBinding : CommandGestureBinding
    {
        private PanCommand command = new PanCommand();
        private PanGesture gesture = new PanGesture();

        public bool IsGestureActive(IList<Body> skeletons, int bodyCount)
        {
            return gesture.IsActive(skeletons, bodyCount);
        }

        public void Update(IList<Body> skeletons, int bodyCount)
        {
            command.Do(gesture.Update(skeletons, bodyCount), 0.0);
        }
    }
}
