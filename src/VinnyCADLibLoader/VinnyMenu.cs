using CADLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace VinnyCADLibLoader
{
    public partial class VinnyMenu : Form, ICADLibPlugin
    {
        private static PluginsManager mManager;
        private static void AddEnv(string path)
        {
            string newEnwPathValue = Environment.GetEnvironmentVariable("PATH");
            if (newEnwPathValue.EndsWith(";")) newEnwPathValue += path + ";";
            else newEnwPathValue += ";" + path + ";";

            Environment.SetEnvironmentVariable("PATH", newEnwPathValue);
        }

        public VinnyMenu(PluginsManager manager)
        {
            mManager = manager;
            string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            string executionDirectoryPath = System.IO.Path.GetDirectoryName(executingAssemblyFile);

            //Load Vinny
            string vinnyPath = new DirectoryInfo(executionDirectoryPath).Parent.Parent.FullName;
            string VinnyLibConverterCommonPath = Path.Combine(vinnyPath, "VinnyLibConverterCommon.dll");
            string VinnyLibConverterKernelPath = Path.Combine(vinnyPath, "VinnyLibConverterKernel.dll");
            string VinnyCADLibAdapterPath = Path.Combine(executionDirectoryPath, "VinnyCADLibAdapter.dll");

            string VinnyLibConverterUIPath = Path.Combine(vinnyPath, "ui", "net48", "VinnyLibConverterUI.dll");

            AddEnv(vinnyPath);

            InitializeComponent();
        }

        public MenuStrip GetMenu()
        {
            return this.menuStrip1;
        }

        public ToolStripContainer GetToolbars()
        {
            return null;
        }

        public void TrackInterfaceItems(InterfaceTracker tracker)
        {
            tracker.Add(new InterfaceItemState(VinnyImportToolStripMenuItem, LibConnectionState.Connected, LibFolderState.DoesNotMatter, LibObjectState.DoesNotMatter, LibRequiredPermission.EditParametersRegistry));
            tracker.Add(new InterfaceItemState(vinnyExportToolStripMenuItem, LibConnectionState.Connected, LibFolderState.DoesNotMatter, LibObjectState.DoesNotMatter, LibRequiredPermission.EditParametersRegistry));
        }

        private void VinnyImportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VinnyCADLibAdapter.VinnyCADLibAdapterFunctions.Import(mManager);
        }

        private void vinnyExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VinnyCADLibAdapter.VinnyCADLibAdapterFunctions.Export(mManager);
        }

        private void VinnyAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Vinny-Environment/VinnyCADLibAdapter");
        }
    }
}
