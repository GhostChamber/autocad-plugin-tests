using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace GhostChamberPlugin.Commands
{
    /**
     * ZoomCommand runs the Zoom command using the calculated values from ZoomGesture.
     */
	public class ZoomCommand
	{
		private Camera camera = new Camera(Application.DocumentManager.MdiActiveDocument);  /**< The camera object that this class uses to pan. */
        private Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;       /**< The AutoCAD Editor currently in use. */

        /**
         * Performs the Zoom command using the camera object.
         * @param zoomFactor the factor to zoom to the object by.
         */
        public void Do(double zoomFactor)
		{
			camera.Zoom(zoomFactor);
		}

        /**
         * Exposed method to AutoCAD to run the command directly.
         */
        [CommandMethod("GHOSTCHAMBER", "GHOSTZOOM", CommandFlags.Modal)]
		public void Command()
		{
			double zoomFactor = double.Parse(editor.GetString("Zoom Factor: ").StringResult);
			Do(zoomFactor);
		}
	}
}
