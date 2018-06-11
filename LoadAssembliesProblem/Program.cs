using System;
using System.IO;
using System.Reflection;
using Doxie.Core.Services;

namespace LoadAssembliesProblem
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var startupPath = new DirectoryInfo(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location)).Parent.Parent.Parent;
            string examplePath = Path.Combine(startupPath.FullName, "SomeArbitraryPath");

            // Shouldn't need to do this, but to try solve the "Can't load assembly XYZ" errors, I try to load all assemblies to this console app...
            //  NOTE: It doesn't work anyway... it still gives those errors, even when I can see in AppDomain.CurrentDomain.GetAssemblies() that the assemblies
            //      in question are in fact loaded...
            //AssemblyLoader.LoadAssemblyFromFilePath(examplePath);

            var selectedFiles = new[]
            {
                Path.Combine(examplePath, "Extenso.AspNetCore.Mvc.dll"),
                Path.Combine(examplePath, "Extenso.AspNetCore.Mvc.ExtensoUI.dll"),
                Path.Combine(examplePath, "Extenso.AspNetCore.Mvc.ExtensoUI.Foundation.dll"),
                Path.Combine(examplePath, "Extenso.AspNetCore.Mvc.ExtensoUI.JQueryUI.dll"),
                Path.Combine(examplePath, "Extenso.AspNetCore.Mvc.ExtensoUI.KendoUI.dll"),
                Path.Combine(examplePath, "Extenso.AspNetCore.OData.dll"),
                Path.Combine(examplePath, "Extenso.Core.dll"),
                Path.Combine(examplePath, "Extenso.Data.dll"),
                Path.Combine(examplePath, "Extenso.Data.MySql.dll"),
                Path.Combine(examplePath, "Extenso.Data.Npgsql.dll"),
                Path.Combine(examplePath, "Extenso.Data.QueryBuilder.dll"),
                Path.Combine(examplePath, "Extenso.Data.QueryBuilder.MySql.dll"),
                Path.Combine(examplePath, "Extenso.Data.QueryBuilder.Npgsql.dll")
            };

            string outputPath = examplePath;

            Console.WriteLine("Generating...");

            // This runs fine, but there are always errors inside the generated file (see assemblies.json in outputPath).
            // Example error: "Could not load file or assembly 'Microsoft.AspNetCore.Mvc.ViewFeatures", even thought 
            // Errors are caught and added to the generated file. See code file DocParser.cs at lines #226, #402 and #471.
            JsonHelpFileGenerator.Generate(selectedFiles, outputPath);
            Console.WriteLine("Done");

            Console.ReadLine();
        }
    }
}