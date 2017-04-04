using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using GhostChamberPlugin.Utilities;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.ApplicationServices;

namespace GhostChamberPlugin.Gestures
{
    class SnapBackGesture
    {
        private Body activeBody = null;

        internal bool IsActive(IList<Body> skeletons, int bodyCount)
        {
            if (activeBody == null && skeletons != null)
            {
                for (int i = 0; i < bodyCount; i++)
                {
                    Body body = skeletons[i];
                    if (GestureUtils.IsSnapBackGestureActive(body))
                    {
                        Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("SNAP_BACK Gesture detected.\n");
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
