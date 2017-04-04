using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using System;

namespace GhostChamberPlugin.Commands
{
	public class Camera
	{
		// Members
		private Document _doc = null;
		private ViewTableRecord _vtr = null;
		private ViewTableRecord _initial = null;

		public Camera(Document doc)
		{
			_doc = doc;
			_initial = doc.Editor.GetCurrentView();
			_vtr = (ViewTableRecord)_initial.Clone();
		}

		// Reset to the initial view
		public void Reset()
		{
			_doc.Editor.SetCurrentView(_initial);
			_doc.Editor.Regen();
		}

		// Zoom in or out
		public void Zoom(double factor)
		{
            _vtr = _doc.Editor.GetCurrentView();

            // Adjust the ViewTableRecord
            _vtr.Height *= factor;
			_vtr.Width *= factor;

			// Set it as the current view
			_doc.Editor.SetCurrentView(_vtr);

			// Zoom requires a regen for the gizmos to update
			_doc.Editor.Regen();
		}

		// Pan in the specified direction
		public void Pan(double leftRight, double upDown)
		{
            _vtr = _doc.Editor.GetCurrentView();

            // Adjust the ViewTableRecord
            _vtr.CenterPoint = _vtr.CenterPoint + new Vector2d(leftRight, upDown);

			// Set it as the current view
			_doc.Editor.SetCurrentView(_vtr);
		}

		// Orbit by angle around axis
		public void Orbit(Vector3d axis, double angle)
		{
            // Adjust the ViewTableRecord
            _vtr = _doc.Editor.GetCurrentView();

            _vtr.ViewDirection = _vtr.ViewDirection.TransformBy(Matrix3d.Rotation(angle, axis, Point3d.Origin));
			// Set it as the current view

            _doc.Editor.SetCurrentView(_vtr);
        }

        public double GetCameraWidth()
        {
            _vtr = _doc.Editor.GetCurrentView();
            return _vtr.Width;
        }

        public double GetCameraHeight()
        {
            _vtr = _doc.Editor.GetCurrentView();
            return _vtr.Width;
        }

        public void SnapBackInView()
        {
            _vtr = _doc.Editor.GetCurrentView();
            double distanceX = Point2d.Origin.X - _vtr.CenterPoint.X;
            double distanceY = Point2d.Origin.Y - _vtr.CenterPoint.Y;
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DistanceX = {0}\n", distanceX);
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("DistanceY= {0}\n", distanceY);
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Width= {0}\n", (_vtr.Width) / 2);
            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Height= {0}\n", (_vtr.Height)/2);

            if ((Math.Abs(distanceX) > (_vtr.Width/2)) || (Math.Abs(distanceY) > (_vtr.Height/2)))
            {
                Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("SNAP_BACK working\n");
                Pan(distanceX, distanceY);
            }
        }
	}
}
