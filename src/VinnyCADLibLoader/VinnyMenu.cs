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
        public VinnyMenu(PluginsManager manager)
        {
            VinnyCADLibAdapter.VinnyCADLibAdapterFunctions.SetPluginsManager(manager);
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
            VinnyCADLibAdapter.VinnyCADLibAdapterFunctions.Import();
        }

        private void vinnyExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VinnyCADLibAdapter.VinnyCADLibAdapterFunctions.Export();
        }

        private void VinnyAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Vinny-Environment/VinnyCADLibAdapter");
        }
    }
}
