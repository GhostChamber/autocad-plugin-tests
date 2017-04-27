using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace GhostChamberPlugin.Commands
{
    /**
     * Used as to snap the object back into view if it went out of the camera view.
     */
    public class SnapBackCommand
    {
        private Camera camera = new Camera(Application.DocumentManager.MdiActiveDocument);  /**< The camera object that this class uses to pan. */

        /**
         * Performs the snapBackinView using the camera.
         */
        public void Do()
        {
            camera.SnapBackInView();
        }

        /**
         * Exposed method to AutoCAD to run the command directly.
         */
        [CommandMethod("GHOSTPLUGINS", "GHOSTSNAP", CommandFlags.Modal)]
        public void Command()
        {
            Do();
        }
    }
}
