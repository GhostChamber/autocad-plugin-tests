using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace GhostChamberPlugin.Commands
{
	public class GrabCommand
	{
		private Camera camera = new Camera(Application.DocumentManager.MdiActiveDocument);
		private Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

		public void Do(Vector3d position)
		{
			Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("PERFORMING GRAB\n");
			camera.Pan(position.X, position.Y);
		}

		[CommandMethod("GHOSTPLUGINS", "GHOSTGRAB", CommandFlags.Modal)]
		public void Command()
		{
			double horizontal = double.Parse(editor.GetString("Pan Horizontal: ").StringResult);
			double vertical = double.Parse(editor.GetString("Pan Vertical: ").StringResult);

			Do(new Vector3d(horizontal, vertical, 0));
		}
	}
}