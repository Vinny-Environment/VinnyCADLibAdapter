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
            VinnyLibConverterCommon.ImportExportParameters inputParams = new VinnyLibConverterCommon.ImportExportParameters();
#if DEBUG
            inputParams = VinnyLibConverterCommon.ImportExportParameters.LoadFromFile(@"E:\Temp\Vinny\vinnyTestParams_CADLib1.xml");
#else
            if (vinnyWindow.ShowDialog() == true) inputParams = vinnyWindow.VinnyParametets;
#endif
            VinnyCaDLibImporter.CreateInstance().ImportFrom(inputParams);
        }
        public static void Export()
        {
            VinnyLibConverterUI.VLC_UI_MainWindow vinnyWindow = new VinnyLibConverterUI.VLC_UI_MainWindow(false);
            VinnyLibConverterCommon.ImportExportParameters outputParams = new VinnyLibConverterCommon.ImportExportParameters();
#if DEBUG
            outputParams = VinnyLibConverterCommon.ImportExportParameters.LoadFromFile(@"E:\Temp\Vinny\rengaTestParams1.XML");
#else
            if (vinnyWindow.ShowDialog() == true) outputParams = vinnyWindow.VinnyParametets;
#endif
            VinnyCADLibExporter.CreateInstance().ExportTo(VinnyCADLibExporter.CreateInstance().CreateData(), outputParams);
        }
    }
}
