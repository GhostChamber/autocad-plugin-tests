using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

namespace GhostChamberPlugin.Commands
{
    class SnapBackCommand
    {
        private Camera camera = new Camera(Application.DocumentManager.MdiActiveDocument);
        //private Editor editor = Application.DocumentManager.MdiActiveDocument.Editor;

        public void Do()
        {
            //Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("PERFORMING GRAB\n");
            camera.SnapBackInView();
        }

        [CommandMethod("GHOSTPLUGINS", "GHOSTSNAP", CommandFlags.Modal)]
        public void Command()
        {
            Do();
        }
    }
}
