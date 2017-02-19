using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace GhostChamberPlugin.Commands
{
	public class OrbitCommand
	{
		private Camera camera = new Camera(Application.DocumentManager.MdiActiveDocument);
		private Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

		void Do(Vector3d axis, double angle)
		{
			camera.Orbit(axis, angle);
		}

		[CommandMethod("GHOSTPLUGINS", "GHOSTORBIT", CommandFlags.Modal)]
		public void Command()
		{
			string axisName = editor.GetString("Orbit Axis: ").StringResult;
			double angle = double.Parse(editor.GetString("Orbit angle: ").StringResult);
			Vector3d axis;
			switch (axisName.ToLower())
			{
				case "x":
					axis = Vector3d.XAxis;
					break;
				case "y":
					axis = Vector3d.YAxis;
					break;
				case "z":
				default:
					axis = Vector3d.ZAxis;
					break;
			}
			Do(axis, angle);
		}
	}
}
