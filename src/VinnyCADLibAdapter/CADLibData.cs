using CADLib;
using CSProject3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VinnyCADLibAdapter
{
    internal class CADLibData
    {
        public static PluginsManager mManager;

        public static CAD3DLibrary CADLibrary3D => (CAD3DLibrary)mManager.Library;
    }
}
