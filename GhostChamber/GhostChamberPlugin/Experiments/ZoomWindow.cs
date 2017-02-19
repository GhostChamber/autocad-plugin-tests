using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Interop;
using System;

namespace GhostChamberPlugin.Experiments
{
	//public class ZoomCommands
	//{
	//	[CommandMethod("ZIN")]
	//	public void ZoomIn()
	//	{
	//		var cam =
	//		  new Camera(Application.DocumentManager.MdiActiveDocument);
	//		cam.Zoom(0.9);
	//	}

	//	[CommandMethod("ZOUT")]
	//	public void ZoomOut()
	//	{
	//		var cam =
	//		  new Camera(Application.DocumentManager.MdiActiveDocument);
	//		cam.Zoom(1.1);
	//	}

	//	[CommandMethod("PLEFT")]
	//	public void PanLeft()
	//	{
	//		var cam =
	//		  new Camera(Application.DocumentManager.MdiActiveDocument);
	//		cam.Pan(-1, 0);
	//	}

	//	[CommandMethod("PRIGHT")]
	//	public void PanRight()
	//	{
	//		var cam =
	//		  new Camera(Application.DocumentManager.MdiActiveDocument);
	//		cam.Pan(1, 0);
	//	}

	//	[CommandMethod("PUP")]
	//	public void PanUp()
	//	{
	//		var cam =
	//		  new Camera(Application.DocumentManager.MdiActiveDocument);
	//		cam.Pan(0, 1);
	//	}

	//	[CommandMethod("PDOWN")]
	//	public void PanDown()
	//	{
	//		var cam =
	//		  new Camera(Application.DocumentManager.MdiActiveDocument);
	//		cam.Pan(0, -1);
	//	}

	//	[CommandMethod("OLEFT")]
	//	public void OrbitLeft()
	//	{
	//		var cam =
	//		  new Camera(Application.DocumentManager.MdiActiveDocument);
	//		cam.Orbit(Vector3d.ZAxis, Math.PI / 12);
	//	}

	//	[CommandMethod("ORIGHT")]
	//	public void OrbitRight()
	//	{
	//		var cam =
	//		  new Camera(Application.DocumentManager.MdiActiveDocument);
	//		cam.Orbit(Vector3d.ZAxis, -Math.PI / 12);
	//	}

	//	[CommandMethod("OUP")]
	//	public void OrbitUp()
	//	{
	//		var cam =
	//		  new Camera(Application.DocumentManager.MdiActiveDocument);
	//		cam.Orbit(Vector3d.XAxis, Math.PI / 12);
	//	}

	//	[CommandMethod("ODOWN")]
	//	public void OrbitDown()
	//	{
	//		var cam =
	//		  new Camera(Application.DocumentManager.MdiActiveDocument);
	//		cam.Orbit(Vector3d.XAxis, -Math.PI / 12);
	//	}

	//	// Zoom to a window specified by the user
	//	[CommandMethod("ZW")]
	//	static public void ZoomWindow()
	//	{
	//		Document doc =
	//		  Application.DocumentManager.MdiActiveDocument;
	//		Database db = doc.Database;
	//		Editor ed = doc.Editor;

	//		// Get the window coordinates

	//		PromptPointOptions ppo =
	//		  new PromptPointOptions(
	//			"\nSpecify first corner:"
	//		  );

	//		PromptPointResult ppr =
	//		  ed.GetPoint(ppo);

	//		if (ppr.Status != PromptStatus.OK)
	//			return;

	//		Point3d min = ppr.Value;

	//		PromptCornerOptions pco =
	//		  new PromptCornerOptions(
	//			"\nSpecify opposite corner: ",
	//			ppr.Value
	//		  );

	//		ppr = ed.GetCorner(pco);

	//		if (ppr.Status != PromptStatus.OK)
	//			return;

	//		Point3d max = ppr.Value;

	//		// Call out helper function
	//		// [Change this to ZoomWin2 or WoomWin3 to
	//		// use different zoom techniques]

	//		ZoomWin(ed, min, max);
	//	}

	//	// Zoom to the extents of an entity

	//	[CommandMethod("ZE")]
	//	static public void ZoomToEntity()
	//	{
	//		Document doc =
	//		  Application.DocumentManager.MdiActiveDocument;
	//		Database db = doc.Database;
	//		Editor ed = doc.Editor;

	//		// Get the entity to which we'll zoom

	//		PromptEntityOptions peo =
	//		  new PromptEntityOptions(
	//			"\nSelect an entity:"
	//		  );

	//		PromptEntityResult per = ed.GetEntity(peo);

	//		if (per.Status != PromptStatus.OK)
	//			return;

	//		// Extract its extents

	//		Extents3d ext;

	//		Transaction tr =
	//		  db.TransactionManager.StartTransaction();
	//		using (tr)
	//		{
	//			Entity ent =
	//			  (Entity)tr.GetObject(
	//				per.ObjectId,
	//				OpenMode.ForRead
	//			  );
	//			ext =
	//			  ent.GeometricExtents;
	//			tr.Commit();
	//		}

	//		ext.TransformBy(
	//		  ed.CurrentUserCoordinateSystem.Inverse()
	//		);

	//		// Call our helper function
	//		// [Change this to ZoomWin2 or WoomWin3 to
	//		// use different zoom techniques]

	//		ZoomWin(ed, ext.MinPoint, ext.MaxPoint);
	//	}

	//	[CommandMethod("GHOSTLEFT")]
	//	static public void GhostPanLeft()
	//	{
	//		Document doc =
	//		  Application.DocumentManager.MdiActiveDocument;
	//		Database db = doc.Database;
	//		Editor ed = doc.Editor;

	//		// get editor view coords
	//		ViewTableRecord record = ed.GetCurrentView();
	//		record.CenterPoint = new Point2d(record.CenterPoint.X - 5, record.CenterPoint.Y);
	//		ed.SetCurrentView(record);
	//	}

	//	[CommandMethod("GHOSTRIGHT")]
	//	static public void GhostPanRight()
	//	{
	//		Document doc =
	//		  Application.DocumentManager.MdiActiveDocument;
	//		Database db = doc.Database;
	//		Editor ed = doc.Editor;

	//		// get editor view coords
	//		ViewTableRecord record = ed.GetCurrentView();
	//		record.CenterPoint = new Point2d(record.CenterPoint.X + 5, record.CenterPoint.Y);
	//		ed.SetCurrentView(record);
	//	}

	//	// Helper functions to zoom using different techniques

	//	// Zoom using a view object

	//	private static void ZoomWin(
	//	  Editor ed, Point3d min, Point3d max
	//	)
	//	{
	//		Point2d min2d = new Point2d(min.X, min.Y);
	//		Point2d max2d = new Point2d(max.X, max.Y);

	//		ViewTableRecord view =
	//		  new ViewTableRecord();

	//		view.CenterPoint =
	//		  min2d + ((max2d - min2d) / 2.0);
	//		view.Height = max2d.Y - min2d.Y;
	//		view.Width = max2d.X - min2d.X;
	//		ed.SetCurrentView(view);
	//	}

	//	// Zoom via COM

	//	private static void ZoomWin2(
	//	  Editor ed, Point3d min, Point3d max
	//	)
	//	{
	//		AcadApplication app =
	//		  (AcadApplication)Application.AcadApplication;

	//		double[] lower =
	//		  new double[3] { min.X, min.Y, min.Z };
	//		double[] upper
	//		  = new double[3] { max.X, max.Y, max.Z };

	//		app.ZoomWindow(lower, upper);
	//	}

	//	// Zoom by sending a command

	//	private static void ZoomWin3(
	//	  Editor ed, Point3d min, Point3d max
	//	)
	//	{
	//		string lower =
	//		  min.ToString().Substring(
	//			1,
	//			min.ToString().Length - 2
	//		  );
	//		string upper =
	//		  max.ToString().Substring(
	//			1,
	//			max.ToString().Length - 2
	//		  );

	//		string cmd =
	//		  "_.ZOOM _W " + lower + " " + upper + " ";

	//		// Call the command synchronously using COM

	//		AcadApplication app =
	//		  (AcadApplication)Application.AcadApplication;

	//		app.ActiveDocument.SendCommand(cmd);

	//		// Could also use async command calling:
	//		//ed.Document.SendStringToExecute(
	//		//  cmd, true, false, true
	//		//);
	//	}
	//}
}
