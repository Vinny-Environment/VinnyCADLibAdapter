using CADLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace VinnyCADLibAdapter
{
    public partial class VinnyMenu : Form, ICADLibPlugin
    {
        public VinnyMenu(PluginsManager manager)
        {
            InitializeComponent();
            CADLibData.mManager = manager;
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
            VinnyLibConverterUI.VLC_UI_MainWindow vinnyWindow = new VinnyLibConverterUI.VLC_UI_MainWindow(true);
            if (vinnyWindow.ShowDialog() == true)
            {
                VinnyCaDLibImporter.CreateInstance().ImportFrom(vinnyWindow.VinnyParametets);
            }
        }

        private void vinnyExportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            VinnyLibConverterUI.VLC_UI_MainWindow vinnyWindow = new VinnyLibConverterUI.VLC_UI_MainWindow(false);
            if (vinnyWindow.ShowDialog() == true)
            {
                VinnyCADLibExporter.CreateInstance().ExportTo(VinnyCADLibExporter.CreateInstance().CreateData(), vinnyWindow.VinnyParametets);
            }
        }

        private void VinnyAboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/Vinny-Environment/VinnyCADLibAdapter");
        }
    }
}
