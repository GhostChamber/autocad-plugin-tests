using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace GhostChamberPlugin.Commands
{
	public class PanCommand
	{
		private Camera camera = new Camera(Application.DocumentManager.MdiActiveDocument);
		private Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

		void Do(double horizontal, double vertical)
		{
			camera.Pan(horizontal, vertical);
		}

		[CommandMethod("GHOSTPLUGINS", "GHOSTPAN", CommandFlags.Modal)]
		public void Command()
		{
			double horizontal = double.Parse(editor.GetString("Pan Horizontal: ").StringResult);
			double vertical = double.Parse(editor.GetString("Pan Vertical: ").StringResult);
			Do(horizontal, vertical);
		}
	}
}
