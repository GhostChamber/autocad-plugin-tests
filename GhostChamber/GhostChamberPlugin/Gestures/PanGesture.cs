using Microsoft.Kinect;
using System;
using System.Collections.Generic;

namespace GhostChamberPlugin.Gestures
{
	class PanGesture
	{
        private GestureType gestureType;
        private Body activeBody = null;

        public void HandlePanGesture( IList<Body> skeletons,int bodyCount )
        {

            if(gestureType == GestureType.NONE && skeletons != null)
            {
                //Parse through all skeletons and check hand position
                for(int i = 0; i < bodyCount; i++)
                {
                    Body body = skeletons[i];

                    if(body.Joints[JointType.Head].Position.Y != 0.0f 
                        && isCommandPositionActive(body, 0.2f) 
                            && isJointLessThan(body, JointType.HandTipRight, JointType.ThumbRight))
                    {
                        gestureType = GestureType.PAN;
                        activeBody = body;
                    }
                }
            } 
            else if(gestureType == GestureType.PAN)
            {
                if(activeBody != null)
                {
                    if(isCommandPositionActive(activeBody, 0.2f) && isJointLessThan(activeBody, JointType.HandTipRight, JointType.ThumbRight))
                    {
                        Editor.WriteString("Made it PAN");
                    }
                }

                if(activeBody != null)
                {
                    // kinect units are in meters. Hence left - right is scaled from minHandDistance to maxHandDistance
                    if(isCommandPositionActive(activeBody, 0.2f))
                    {
                        ReleaseGesture();
                    }
                }
            }
        }

        private bool isJointLessThan(Body currentBody, JointType jointA, JointType jointB)
        {
            if(currentBody.Joints[jointA].Position.X <= currentBody.Joints[jointB].Position.X)
            {
                if(currentBody.Joints[jointA].Position.Y <= currentBody.Joints[jointB].Position.Y)
                {
                    if(currentBody.Joints[jointA].Position.Z <= currentBody.Joints[jointB].Position.Z)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private bool isCommandPositionActive(Body activeBody, float threshhold)
        {
            if(Math.Abs(activeBody.Joints[JointType.HandLeft].Position.Y - activeBody.Joints[JointType.Head].Position.Y) > threshhold)
            {
                return false;
            }
            
            return true;
        }

        private void ReleaseGesture()
        {
            gestureType = GestureType.NONE;
            activeBody = null;
        }
    }
}
