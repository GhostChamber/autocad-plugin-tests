using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using GhostChamberPlugin.Utilities;
using Autodesk.AutoCAD.Geometry;

namespace GhostChamberPlugin.Gestures
{
    class SnapBackGesture
    {
        private Body activeBody = null;
        //private CameraSpacePoint startPosition;
        //private CameraSpacePoint previousPosition;

        internal bool IsActive(IList<Body> skeletons, int bodyCount)
        {
            if (activeBody == null && skeletons != null)
            {
                for (int i = 0; i < bodyCount; i++)
                {
                    Body body = skeletons[i];
                    if (GestureUtils.IsSnapBackGestureActive(body))
                    {
                        activeBody = body;
                        break;
                    }
                }
            }

            return (activeBody != null);
        }

        public void Update()
        {
            activeBody = null;
        }
    }
}
