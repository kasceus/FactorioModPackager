using System.IO.Compression;

namespace FactorioModPackager;
internal class Program
{



    static void Main(string[] args)
    {

        string modName = "themightygugi_longreach";
        string folderName = modName;
        string modVersion = "0.0.0"; // Default version, will be updated from info.json
        string modDirectory = Path.Combine(Directory.GetCurrentDirectory(), folderName);
        string infoJsonPath = Path.Combine(modDirectory, "info.json");
        string userDirectory = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        string modPath = Path.Combine(userDirectory, "AppData", "Roaming", "Factorio", "mods");

        if (File.Exists(infoJsonPath))
        {
            // Read the info.json file to get the version
            var infoJson = System.Text.Json.JsonDocument.Parse(File.ReadAllText(infoJsonPath));
            if (infoJson.RootElement.TryGetProperty("version", out var versionElement))
            {
                modVersion = versionElement.GetString() ?? modVersion;
            }
            if (infoJson.RootElement.TryGetProperty(modVersion, out var nameElement))
            {
                modName = nameElement.GetString() ?? modName;
            }

        }
        else
        {
            Console.WriteLine($"info.json not found in {modDirectory}. Using default version {modVersion}.");
        }
        string zipFileName = $"{modName}_{modVersion}.zip";

        string zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), zipFileName);
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }
        string outterFolder = zipFilePath.Replace(".zip", "");
        Directory.CreateDirectory(outterFolder);
        string innerInnerFolderPath = Path.Combine(outterFolder, zipFileName.Split(".zip")[0]);

        Directory.CreateDirectory(innerInnerFolderPath);


        string[] ignored = [".git", ".vs", ".gitignore"];

        foreach (string file in Directory.GetFiles(modDirectory, "*", SearchOption.AllDirectories))
        {
            if (ignored.Any(ignoredFolder => file.Contains(ignoredFolder)))
            {
                continue; // Skip ignored folders
            }
            string entryName = Path.GetRelativePath(modDirectory, file);
            //copy the files from this directory into the inner directory
            string destinationPath = Path.Combine(innerInnerFolderPath, entryName);
            if (!Directory.Exists(Path.GetDirectoryName(destinationPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath));
            }

            File.Copy(file, destinationPath, true);
        }

        // Create the zip file - should be structured like this: modName_version.zip/modName_version/*files*
        ZipFile.CreateFromDirectory(outterFolder, zipFilePath);

        string modFile = Path.Combine(modPath, $"{modName}_forked_{modVersion}.zip");
        if (File.Exists(modFile))
        {
            File.Delete(modFile);
        }
        // Copy the zip file to the Factorio mods directory
        // check if there are any other zip files in the mods directory with and older version of the name   $"{modName}_forked_{modVersion}.zip" and delete them
        foreach (string existingFile in Directory.GetFiles(modPath, $"{modName}_forked_*.zip"))
        {
            if (existingFile != modFile && Path.GetFileName(existingFile).StartsWith(modName + "_forked_"))
            {
                File.Delete(existingFile);
            }
        }

        File.Copy(zipFilePath, modFile);
        if (Directory.Exists(modPath))
        {
            System.Diagnostics.Process.Start("explorer.exe", modPath);
        }
        else
        {
            Console.WriteLine($"The mods directory {modPath} does not exist.");
        }
        Console.WriteLine($"Mod {modName} version {modVersion} has been packaged into {zipFilePath}.");
        // Wait for user input before closing the console window
        //open the modpath folder so i can see it:


    }
}
