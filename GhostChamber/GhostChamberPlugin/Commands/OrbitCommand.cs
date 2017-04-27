using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace GhostChamberPlugin.Commands
{
    /**
     * Used to orbit around the model to see it from different angles.
     */
	public class OrbitCommand
	{
		private Camera camera = new Camera(Application.DocumentManager.MdiActiveDocument);  /**< The camera object that this class uses to pan. */
        private Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;       /**< The AutoCAD Editor currently in use. */

        /**
         * Performs the orbit command using the camera. Uses the values obtained from Update.
         * @param axis the axis about which to orbit.
         * @param angle the angle by which to rotate.
         */
        public void Do(Vector3d axis, double angle)
		{
			camera.Orbit(axis, angle);
		}

        /**
         * Exposed method to AutoCAD to run the command directly.
         */
        [CommandMethod("GHOSTCHAMBER", "GHOSTORBIT", CommandFlags.Modal)]
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
