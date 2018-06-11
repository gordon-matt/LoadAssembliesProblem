using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace LoadAssembliesProblem
{
    public static class AssemblyLoader
    {
        public static void LoadAssemblyFromFilePath(string assembliesPath)
        {
            var directoryInfo = new DirectoryInfo(assembliesPath);
            var allAssemblies = directoryInfo.GetFiles("*.dll", SearchOption.AllDirectories).ToList();

            foreach (var assembly in allAssemblies
                .Where(x => !IsAlreadyLoaded(x)))
            {
                PerformFileDeploy(assembly);
            }
        }

        private static Assembly PerformFileDeploy(FileInfo assemblyFileInfo)
        {
            return RegisterPluginDefinition(assemblyFileInfo);
        }

        private static Assembly RegisterPluginDefinition(FileInfo assemblyFileInfo)
        {
            var assemblyName = AssemblyName.GetAssemblyName(assemblyFileInfo.FullName);
            Assembly assembly;

            try
            {
                assembly = Assembly.LoadFile(assemblyFileInfo.FullName);
            }
            catch (FileLoadException)
            {
                //if an application has been copied from the web, it is flagged by Windows as being a web application,
                //even if it resides on the local computer.You can change that designation by changing the file properties,
                //or you can use the<loadFromRemoteSources> element to grant the assembly full trust.As an alternative,
                //you can use the UnsafeLoadFrom method to load a local assembly that the operating system has flagged as
                //having been loaded from the web.
                //see http://go.microsoft.com/fwlink/?LinkId=155569 for more information.
                assembly = Assembly.UnsafeLoadFrom(assemblyFileInfo.FullName);
            }

            //Debug.WriteLine("Adding to ApplicationParts: '{0}'", assembly.FullName);
            //applicationPartManager.ApplicationParts.Add(new AssemblyPart(assembly));

            byte[] bytes = File.ReadAllBytes(assemblyFileInfo.FullName);
            AppDomain.CurrentDomain.Load(bytes);

            return assembly;
        }

        private static bool IsAlreadyLoaded(FileInfo fileInfo)
        {
            //compare full assembly name
            //var fileAssemblyName = AssemblyName.GetAssemblyName(fileInfo.FullName);
            //foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
            //{
            //    if (a.FullName.Equals(fileAssemblyName.FullName, StringComparison.InvariantCultureIgnoreCase))
            //        return true;
            //}
            //return false;

            //do not compare the full assembly name, just filename
            try
            {
                string fileNameWithoutExt = Path.GetFileNameWithoutExtension(fileInfo.FullName);
                if (fileNameWithoutExt == null)
                {
                    throw new Exception(string.Format("Cannot get file extnension for {0}", fileInfo.Name));
                }

                foreach (var a in AppDomain.CurrentDomain.GetAssemblies())
                {
                    string assemblyName = a.FullName.Split(new[] { ',' }).FirstOrDefault();
                    if (fileNameWithoutExt.Equals(assemblyName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            catch (Exception x)
            {
                //Logger.Error("Cannot validate whether an assembly is already loaded.", x);
                Debug.WriteLine("Cannot validate whether an assembly is already loaded. " + x);
            }
            return false;
        }
    }
}