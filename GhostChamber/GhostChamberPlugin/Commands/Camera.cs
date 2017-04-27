using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace GhostChamberPlugin.Commands
{
    /**
     * The Camera class is used to perform any view transformations on the current document.
     * It communicates directly with AutoCAD to perform the four commands that we support -
     *      Grab
     *      SnapBack
     *      Orbit
     *      Zoom
     */
	public class Camera
	{
		// Members
		private Document document = null;                       /**< The document currently being viewed. */
		private ViewTableRecord vTableRecord = null;            /**< The current view being used. */
		private ViewTableRecord initialTableRecord = null;      /**< The view before performing any camera operations. */

        /**
         * Constructor for Camera class. Initializes all member variables.
         * @param doc the Document to use the Camera on. Generally, this is the current document being used in AutoCAD.
         */
		public Camera(Document doc)
		{
			document = doc;
			initialTableRecord = document.Editor.GetCurrentView();
			vTableRecord = (ViewTableRecord)initialTableRecord.Clone();
		}

		/**
         *  Reset to the initial view
         */
		public void Reset()
		{
			document.Editor.SetCurrentView(initialTableRecord);
			document.Editor.Regen();
		}

		/** 
         * Zoom in or out.
         * @param factor the factor to zoom in or out by.
         */
		public void Zoom(double factor)
		{
            vTableRecord = document.Editor.GetCurrentView();

            // Adjust the ViewTableRecord
            vTableRecord.Height *= factor;
			vTableRecord.Width *= factor;

			// Set it as the current view
			document.Editor.SetCurrentView(vTableRecord);

			// Zoom requires a regen for the gizmos to update
			document.Editor.Regen();
		}

		/** 
         * Pan in the specified direction.
         * @param leftRight amount of movement along the X-axis.
         * @param upDown amount of movement along the Y-axis.
         */
		public void Pan(double leftRight, double upDown)
		{
            vTableRecord = document.Editor.GetCurrentView();

            // Adjust the ViewTableRecord
            vTableRecord.CenterPoint = vTableRecord.CenterPoint + new Vector2d(leftRight, upDown);

			// Set it as the current view
			document.Editor.SetCurrentView(vTableRecord);
		}

		/** 
         * Orbit by angle around axis. 
         * @param axis the axis along which to rotate.
         * @param angle the angle to rotate by
         */
		public void Orbit(Vector3d axis, double angle)
		{
            // Adjust the ViewTableRecord
            vTableRecord = document.Editor.GetCurrentView();

            vTableRecord.ViewDirection = vTableRecord.ViewDirection.TransformBy(Matrix3d.Rotation(angle, axis, Point3d.Origin));
			// Set it as the current view

            document.Editor.SetCurrentView(vTableRecord);
        }

        /**
         * Accessor for Current Camera Width.
         * @return returns double value of the CameraWidth.
         */
        public double GetCameraWidth()
        {
            vTableRecord = document.Editor.GetCurrentView();
            return vTableRecord.Width;
        }

        /**
         * Accessor for the Current Camera Height.
         * @return returns the double value of the Camera Height.
         */
        public double GetCameraHeight()
        {
            vTableRecord = document.Editor.GetCurrentView();
            return vTableRecord.Width;
        }

        /**
         * If the model is out of view, brings the model back to the centre of the camera focus.
         */
        public void SnapBackInView()
        {
            vTableRecord = document.Editor.GetCurrentView();
            double distanceX = Point2d.Origin.X - vTableRecord.CenterPoint.X;
            double distanceY = Point2d.Origin.Y - vTableRecord.CenterPoint.Y;

            if ((Math.Abs(distanceX) > (vTableRecord.Width/2)) || (Math.Abs(distanceY) > (vTableRecord.Height/2)))
            {
                Pan(distanceX, distanceY);
            }
        }
	}
}
