using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace GhostChamberPlugin.Commands
{
	public class ZoomCommand
	{
		private Camera camera = new Camera(Application.DocumentManager.MdiActiveDocument);
		private Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

		public void Do(double zoomFactor)
		{
			camera.Zoom(zoomFactor);
		}

		[CommandMethod("GHOSTPLUGINS", "GHOSTZOOM", CommandFlags.Modal)]
		public void Command()
		{
			double zoomFactor = double.Parse(editor.GetString("Zoom Factor: ").StringResult);
			Do(zoomFactor);
		}
	}
}
