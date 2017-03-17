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
