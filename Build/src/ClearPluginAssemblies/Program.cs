using System.Text;

namespace ClearPluginAssemblies
{
    public class Program
    {
        protected const string FILES_TO_DELETE = "dotnet-bundle.exe;Nop.Web.pdb;Nop.Web.exe;Nop.Web.exe.config;System.Security.Permissions.dll";

        //protected StringBuilder _strLog = new StringBuilder();

        protected static void Clear(string paths, IList<string> fileNames, bool saveLocalesFolders)
        {
            var _strLog = new StringBuilder();
            foreach (var pluginPath in paths.Split(';'))
            {
                try
                {
                    var pluginDirectoryInfo = new DirectoryInfo(pluginPath);
                    var allDirectoryInfo = new List<DirectoryInfo> { pluginDirectoryInfo };

                    if (!saveLocalesFolders)
                        allDirectoryInfo.AddRange(pluginDirectoryInfo.GetDirectories());
                                        
                    foreach (var directoryInfo in allDirectoryInfo)
                    {
                        foreach (var fileName in fileNames)
                        {                            
                            //delete dll file if it exists in current path
                            var dllfilePath = Path.Combine(directoryInfo.FullName, fileName + ".dll");
                            _strLog.AppendLine($"Chk. file {dllfilePath}");
                            if (File.Exists(dllfilePath))
                            {
                                File.Delete(dllfilePath);
                                _strLog.AppendLine($"Delete. file {dllfilePath}");
                            }
                            //delete pdb file if it exists in current path
                            var pdbfilePath = Path.Combine(directoryInfo.FullName, fileName + ".pdb");
                            if (File.Exists(pdbfilePath))
                                File.Delete(pdbfilePath);
                        }

                        foreach (var fileName in FILES_TO_DELETE.Split(';'))
                        {
                            //delete file if it exists in current path
                            var pdbfilePath = Path.Combine(directoryInfo.FullName, fileName);
                            if (File.Exists(pdbfilePath))
                            {
                                File.Delete(pdbfilePath);
                                _strLog.AppendLine($"Delete. file {pdbfilePath}");
                            }
                        }

                        if (directoryInfo.GetFiles().Length == 0 && directoryInfo.GetDirectories().Length == 0 && !saveLocalesFolders)
                            directoryInfo.Delete(true);
                    }
                    
                    //File.WriteAllText("d:\\temp\\clearPlugin.txt", $"ClearPluginAssemblies.Clear: {_strLog.ToString()}");
                }
                catch
                {
                    //do nothing
                }
            }
        }

        private static void Main(string[] args)
        {
            var outputPath = string.Empty;
            var pluginPaths = string.Empty;
            var saveLocalesFolders = true;

            var settings = args.FirstOrDefault(a => a.Contains('|')) ?? string.Empty;
            if(string.IsNullOrEmpty(settings))
                return;

            foreach (var arg in settings.Split('|'))
            {
                var data = arg.Split("=").Select(p => p.Trim()).ToList();

                var name = data[0];
                var value = data.Count > 1 ? data[1] : string.Empty;

                switch (name)
                {
                    case "OutputPath":
                        outputPath = value;
                        break;
                    case "PluginPath":
                        pluginPaths = value;
                        break;
                    case "SaveLocalesFolders":
                        _ = bool.TryParse(value, out saveLocalesFolders);
                        break;
                }
            }

            //File.WriteAllText($"d:\\temp\\clearPlugin.txt", $"ClearPluginAssemblies called with Output: {outputPath}, pluginpath: {pluginPaths}");

            if(!Directory.Exists(outputPath))
                return;

            File.WriteAllText("d:\\temp\\clearPlugin.txt", "Continue...");

            var di = new DirectoryInfo(outputPath);
            var separator = Path.DirectorySeparatorChar;
            var folderToIgnore = string.Concat(separator, "Plugins", separator);
            var fileNames = di.GetFiles("*.dll", SearchOption.AllDirectories)
                .Where(fi => !fi.FullName.Contains(folderToIgnore))
                .Select(fi => fi.Name.Replace(fi.Extension, "")).ToList();
           
            if (string.IsNullOrEmpty(pluginPaths) || fileNames.Count == 0)
            {
                //File.WriteAllText("d:\\temp\\clearPlugin.txt", $"ClearPluginAssemblies filesnames found: {fileNames.Count}");
                return;
            }

            Clear(pluginPaths, fileNames, saveLocalesFolders);
        }
    }
}
