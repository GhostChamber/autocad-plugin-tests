using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;

namespace GhostChamberPlugin.Commands
{
	public class Camera
	{
		// Members
		private Editor _editor = null;

		public Camera(Document doc)
		{
			_editor = doc.Editor;
		}

		// Zoom in or out
		public void Zoom(double factor)
		{
			ViewTableRecord _vtr = _editor.GetCurrentView();

            // Adjust the ViewTableRecord
            _vtr.Height *= factor;
			_vtr.Width *= factor;

			// Set it as the current view
			_editor.SetCurrentView(_vtr);

			// Zoom requires a regen for the gizmos to update
			_editor.Regen();
		}

		// Pan in the specified direction
		public void Pan(double leftRight, double upDown)
		{
			ViewTableRecord _vtr = _editor.GetCurrentView();

            // Adjust the ViewTableRecord
            _vtr.CenterPoint = _vtr.CenterPoint + new Vector2d(leftRight, upDown);

			// Set it as the current view
			_editor.SetCurrentView(_vtr);
		}

		// Orbit by angle around axis
		public void Orbit(Vector3d axis, double angle)
		{
			// Adjust the ViewTableRecord
			ViewTableRecord _vtr = _editor.GetCurrentView();

            _vtr.ViewDirection = _vtr.ViewDirection.TransformBy(Matrix3d.Rotation(angle, axis, Point3d.Origin));

			// Set it as the current view
			_editor.SetCurrentView(_vtr);
        }
	}
}
