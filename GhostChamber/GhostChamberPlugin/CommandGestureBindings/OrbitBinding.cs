using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GhostChamberPlugin.Commands;
using GhostChamberPlugin.Gestures;
using Microsoft.Kinect;
using Autodesk.AutoCAD.Geometry;

namespace GhostChamberPlugin.CommandGestureBindings
{
    class OrbitBinding : CommandGestureBinding
    {
        private OrbitCommand command = new OrbitCommand();
        private OrbitGesture gesture = new OrbitGesture();

        public bool IsGestureActive(IList<Body> skeletons, int bodyCount)
        {
            return gesture.IsActive(skeletons, bodyCount);
        }

        public void Update(IList<Body> skeletons, int bodyCount)
        {
            Vector3d movement = gesture.Update(skeletons, bodyCount);
            if (movement.Length > 0.0)
            {
                command.Do(Vector3d.YAxis, movement.X);
                command.Do(Vector3d.XAxis, movement.Y);
            }
        }
    }
}
