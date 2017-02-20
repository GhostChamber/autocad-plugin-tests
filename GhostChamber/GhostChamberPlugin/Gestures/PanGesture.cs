using Microsoft.Kinect;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;

namespace GhostChamberPlugin.Gestures
{
    class PanGesture : Gesture
	{
        private Body activeBody = null;

        public bool IsActive( IList<Body> skeletons, int bodyCount )
        {
            if(activeBody == null && skeletons != null)
            {
                for(int i = 0; i < bodyCount; i++)
                {
                    Body body = skeletons[i];
                    if(body.Joints[JointType.Head].Position.Y != 0.0f &&
                        Utilities.GestureUtils.isCommandPositionActive(body, 0.2f))
                    {
                        activeBody = body;
                        //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Activated\n");
                        break;
                    }
                }
            }
            return (activeBody != null);
        }

        public double Update(IList<Body> skeletons, int bodyCount)
        {
            if (activeBody != null)
            {
                if (Utilities.GestureUtils.isCommandPositionActive(activeBody, 0.2f) && !Utilities.GestureUtils.IsToolPositionActive(activeBody, 0.2f))
                {
                    Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Made it PAN");
                }
            }

            if (activeBody != null)
            {
                // kinect units are in meters. Hence left - right is scaled from minHandDistance to maxHandDistance
                if (Utilities.GestureUtils.isCommandPositionActive(activeBody, 0.2f))
                {
                    Utilities.GestureUtils.ReleaseGesture(activeBody);
                }
            }

            return 0.0;
        }
    }
}
