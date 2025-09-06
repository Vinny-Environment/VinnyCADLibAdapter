using CADLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyCADLibAdapter
{
    public class VinnyCADLibAdapterFunctions
    {
        public static void SetPluginsManager(PluginsManager manager)
        {
            CADLibData.mManager = manager;
        }
        public static void Import()
        {
            VinnyLibConverterUI.VLC_UI_MainWindow vinnyWindow = new VinnyLibConverterUI.VLC_UI_MainWindow(true);
            if (vinnyWindow.ShowDialog() == true)
            {
                VinnyCaDLibImporter.CreateInstance().ImportFrom(vinnyWindow.VinnyParametets);
            }
        }
        public static void Export()
        {
            VinnyLibConverterUI.VLC_UI_MainWindow vinnyWindow = new VinnyLibConverterUI.VLC_UI_MainWindow(false);
            if (vinnyWindow.ShowDialog() == true)
            {
                VinnyCADLibExporter.CreateInstance().ExportTo(VinnyCADLibExporter.CreateInstance().CreateData(), vinnyWindow.VinnyParametets);
            }
        }
    }
}
