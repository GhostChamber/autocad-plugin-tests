using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Kinect;

namespace SampleNETPlugin
{
	class BodyTracker
	{
		KinectSensor kinectSensor = null;
		BodyFrameReader bodyFrameReader = null;
		Body[] bodies = null;

		public CameraSpacePoint leftWristPoint;
		public CameraSpacePoint leftHandPoint;
		public CameraSpacePoint leftTipPoint;

		public CameraSpacePoint rightWristPoint;
		public CameraSpacePoint rightHandPoint;
		public CameraSpacePoint rightTipPoint;

		public void Initialize()
		{
			kinectSensor = KinectSensor.GetDefault();

			if (kinectSensor != null)
			{
				kinectSensor.Open();
			}

			bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();

			if (bodyFrameReader != null)
			{
				bodyFrameReader.FrameArrived += OnBodyFrameReaderArrived;
			}

		}

		public void OnBodyFrameReaderArrived(object sender, BodyFrameArrivedEventArgs e)
		{
			bool dataReceived = false;

			using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
			{
				if (bodyFrame != null)
				{
					if (bodies == null)
					{
						bodies = new Body[bodyFrame.BodyCount];
					}

					bodyFrame.GetAndRefreshBodyData(bodies);
					dataReceived = true;
				}

				if (dataReceived)
				{
					foreach(Body body in bodies)
					{
						if (body.IsTracked)
						{
							IReadOnlyDictionary<JointType, Joint> joints = body.Joints;

							leftWristPoint = body.Joints[JointType.WristLeft].Position;
							leftHandPoint = body.Joints[JointType.HandLeft].Position;
							leftTipPoint = body.Joints[JointType.HandTipLeft].Position;

							rightWristPoint = body.Joints[JointType.WristRight].Position;
							rightHandPoint = body.Joints[JointType.HandRight].Position;
							rightTipPoint = body.Joints[JointType.HandTipRight].Position;
						}
					}
				}
			}
		}



	}
}
