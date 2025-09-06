using CADLib;
using System.IO;
using System.Reflection;
using System;

namespace VinnyCADLibLoader
{
    public class CADLibPluginEntryPoint
    {
        /// <summary>
        /// Статический метод регистрации плагин, вызываемый родительским приложением
        /// </summary>
        /// <param name="manager">Объект текущего окружения плагина</param>
        /// <returns>Интерфейс плагина</returns>
        public static ICADLibPlugin RegisterPlugin(PluginsManager manager)
        {
            string executingAssemblyFile = new Uri(Assembly.GetExecutingAssembly().GetName().CodeBase).LocalPath;
            string executionDirectoryPath = System.IO.Path.GetDirectoryName(executingAssemblyFile);

            //Load Vinny
            string vinnyPath = new DirectoryInfo(executionDirectoryPath).Parent.Parent.FullName;
            string VinnyLibConverterCommonPath = Path.Combine(vinnyPath, "VinnyLibConverterCommon.dll");
            string VinnyLibConverterKernelPath = Path.Combine(vinnyPath, "VinnyLibConverterKernel.dll");
            string VinnyCADLibAdapterPath = Path.Combine(executionDirectoryPath, "VinnyCADLibAdapter.dll");

            string VinnyLibConverterUIPath = Path.Combine(vinnyPath, "ui", "net48", "VinnyLibConverterUI.dll");

            AddEnv(vinnyPath);

            var ass1 = Assembly.LoadFrom(VinnyLibConverterCommonPath);
            var ass2 = Assembly.LoadFrom(VinnyLibConverterKernelPath);
            var ass3 = Assembly.LoadFrom(VinnyLibConverterUIPath);
            var ass4 = Assembly.LoadFrom(VinnyCADLibAdapterPath);

            //check autorization
            return new VinnyMenu(manager);
        }

        private static void AddEnv(string path)
        {
            string newEnwPathValue = Environment.GetEnvironmentVariable("PATH");
            if (newEnwPathValue.EndsWith(";")) newEnwPathValue += path + ";";
            else newEnwPathValue += ";" + path + ";";

            Environment.SetEnvironmentVariable("PATH", newEnwPathValue);
        }
    }
}
