using System.IO.Compression;

namespace FactorioModPackager;
internal class Program
{



    static void Main(string[] args)
    {

        string modName = "themightygugi_longreach";
        string modVersion = "0.0.0"; // Default version, will be updated from info.json
        string modDirectory = Path.Combine(Directory.GetCurrentDirectory(), modName);
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
        }
        else
        {
            Console.WriteLine($"info.json not found in {modDirectory}. Using default version {modVersion}.");
        }
        string zipFileName = $"{modName}_forked_{modVersion}.zip";

        string zipFilePath = Path.Combine(Directory.GetCurrentDirectory(), zipFileName);
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }

        // Create the zip file
        using (ZipArchive zip = ZipFile.Open(zipFilePath, ZipArchiveMode.Create))
        {
            // Add the mod directory to the zip file
            foreach (string file in Directory.GetFiles(modDirectory, "*", SearchOption.AllDirectories))
            {
                string entryName = Path.GetRelativePath(modDirectory, file);
                zip.CreateEntryFromFile(file, entryName);
            }
        }
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
