using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace GhostChamberPlugin.Commands
{
    /**
     * GrabCommand runs the grab command and communicates directly with the Camera class to do so. 
     * Essentially pan movement, but we call it grab because we were excited that we got the grab gesture working.
     */
	public class GrabCommand
	{
		private Camera camera = new Camera(Application.DocumentManager.MdiActiveDocument);  /**< The camera object that this class uses to pan. */
		private Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;       /**< The AutoCAD Editor currently in use. */
        private float panFactor = 0.01f;                                                    /**< The factor to multiply the pan by so that it doesn't fly off into oblivion with the tiniest gesture. */

        /**
         * Performs the command on the camera based on the value obtained from the Update.
         * @param position The Vector2d value we obtained from Update.
         */
		public void Do(Vector2d position)
		{
            double panX = panFactor * camera.GetCameraWidth() * position.X;
            double panY = panFactor * camera.GetCameraHeight() * position.Y;

			camera.Pan(panX, panY);
		}

        /**
         * Exposed method to AutoCAD to run the command directly.
         */
		[CommandMethod("GHOSTCHAMBER", "GHOSTGRAB", CommandFlags.Modal)]
		public void Command()
		{
			double horizontal = double.Parse(editor.GetString("Pan Horizontal: ").StringResult);
			double vertical = double.Parse(editor.GetString("Pan Vertical: ").StringResult);

			Do(new Vector2d(horizontal, vertical));
		}
	}
}