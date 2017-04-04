using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace GhostChamberPlugin.Commands
{
    public class SnapBackCommand
    {
        private Camera camera = new Camera(Application.DocumentManager.MdiActiveDocument);
        //private Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

        public void Do()
        {
            camera.SnapBackInView();
        }

        [CommandMethod("GHOSTPLUGINS", "GHOSTSNAP", CommandFlags.Modal)]
        public void Command()
        {
            Do();
        }
    }
}
