using System.Collections;
using System.Resources;
using System.Globalization;
using BarcodeGenerator.Resources;

namespace BarcodeGenerator
{
    public class ClassLoadDataFromFile
    {
        // Dictionary to hold GS1 Application Identifiers (AI) and their corresponding values
        public static readonly Dictionary<string, string> dicGs1 = [];

        // Add once in the class - central place for all supported GS1 Application Identifiers (AI) (keys and data attributes)
        public static readonly Dictionary<string, string> _knownGs1AppIDs = [];
        public static readonly Dictionary<string, string> resourceSet = [];

        // Add once in the class - central place for all supported IBAN data (country and length)
        public static readonly Dictionary<string, int> _knownIbanData = [];

        // Add once in the class - central place for all supported ISO country codes (country name; country code A2; country code A3; country code N3)
        public static readonly string[,] _knownIsoCountryCodes = new string[249, 4];

        // Add once in the class - central place for all supported ISO currency codes (country name; currency name; currency code A3; currency code N3; minor unit N1)
        public static readonly string[,] _knownIsoCurrencyCodes = new string[280, 5];

        /// <summary>
        /// Copy the GS1 Application Identifiers and their values from a text file to a dictionary for validation
        /// </summary>
        public static async Task LoadKnownGs1AppIDsAsync()
        {
            // Load the GS1 Application Identifiers and their values from a text file into the dictionary
            await using var stream = await FileSystem.OpenAppPackageFileAsync("GS1ApplicationIdentifiers.csv");
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(';', 2, StringSplitOptions.TrimEntries);
                if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
                {
                    continue;
                }

                if (_knownGs1AppIDs.TryAdd(parts[0], parts[1]))
                {
                    Debug.WriteLine($"Added GS1 Application Identifier: {parts[0]} = {parts[1]}");
                }
            }

            await ReadLanguageResourceFileAsync();
        }

        /// <summary>
        /// Loads the known IBAN data from a text file into the dictionary
        /// </summary>
        /// <returns></returns>
        public static async Task LoadKnownIbanDataAsync()
        {
            // Load the IBAN data from a text file into the dictionary
            await using var stream = await FileSystem.OpenAppPackageFileAsync("IBANData.csv");
            using var reader = new StreamReader(stream);

            string? line;
            while ((line = await reader.ReadLineAsync()) is not null)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(';', 2, StringSplitOptions.TrimEntries);
                if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
                {
                    continue;
                }

                if (_knownIbanData.TryAdd(parts[0], int.Parse(parts[1])))
                {
                    Debug.WriteLine($"Added IBAN Data: {parts[0]} = {parts[1]}");
                }
            }
        }

        /// <summary>
        /// Loads the known ISO country codes from a text file into the array
        /// </summary>
        /// <returns></returns>
        public static async Task LoadKnownIsoCountryCodesAsync()
        {
            // Load the ISO country codes from a text file into the array
            await using var stream = await FileSystem.OpenAppPackageFileAsync("ISOCountryCodes.csv");
            using var reader = new StreamReader(stream);
            int index = 0;
            string? line;
            
            while ((line = await reader.ReadLineAsync()) is not null && index < _knownIsoCountryCodes.GetLength(0))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                
                string[] parts = line.Split(';', 4, StringSplitOptions.TrimEntries);
                
                if (parts.Length != 4)
                {
                    continue;
                }
                
                for (int i = 0; i < 4; i++)
                {
                    _knownIsoCountryCodes[index, i] = parts[i];
                }
                
                index++;

                Debug.WriteLine($"Added ISO Country Code: {parts[0]} = {parts[1]}, {parts[2]}, {parts[3]}");
            }
        }
        
        /// <summary>
        /// Loads the known ISO currency codes from a text file into the array
        /// </summary>
        /// <returns></returns>
        public static async Task LoadKnownIsoCurrencyCodesAsync()
        {
            // Load the ISO currency codes from a text file into the array
            await using var stream = await FileSystem.OpenAppPackageFileAsync("ISOCurrencyCodes.csv");
            using var reader = new StreamReader(stream);
            int index = 0;
            string? line;

            while ((line = await reader.ReadLineAsync()) is not null && index < _knownIsoCurrencyCodes.GetLength(0))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                string[] parts = line.Split(';', 5, StringSplitOptions.TrimEntries);

                if (parts.Length != 5)
                {
                    continue;
                }

                for (int i = 0; i < 5; i++)
                {
                    _knownIsoCurrencyCodes[index, i] = parts[i];
                }

                index++;
                Debug.WriteLine($"Added ISO Currency Code: {parts[0]} = {parts[1]}, {parts[2]}, {parts[3]}, {parts[4]}");
            }
        }

        /// <summary>
        /// Read the name and data from the current language resource file and put them in an dictionary
        /// </summary>
        /// <returns></returns>
        public static async Task ReadLanguageResourceFileAsync()
        {
            ResourceManager rm = GS1AppIdLang.ResourceManager;
            ResourceSet? languageResourceSet = rm.GetResourceSet(CultureInfo.CurrentUICulture, true, true);

            if (languageResourceSet != null)
            {
                foreach (DictionaryEntry entry in languageResourceSet)
                {
                    if (entry.Key is string key && entry.Value is string value)
                    {
                        resourceSet.TryAdd(key, value);
                        Debug.WriteLine($"ReadLanguageResourceFileAsync: {key} = {value}");
                    }
                }
            }
        }

        //public static async Task<Dictionary<string, string>> GetCurrentLanguageResources()
        //{
        //    var resourceDictionary = new Dictionary<string, string>();

        //    // 1. Access the generated ResourceManager from your .resx class
        //    ResourceManager resourceManager = GS1AppIdLang.ResourceManager;

        //    // 2. Get the ResourceSet for the current UI culture
        //    // The 'true' parameters ensure the resource file is loaded if not already in memory,
        //    // and fall back to parent/default cultures if a specific translation is missing.
        //    ResourceSet? resourceSet = resourceManager.GetResourceSet(
        //        CultureInfo.CurrentUICulture,
        //        createIfNotExists: true,
        //        tryParents: true);

        //    if (resourceSet != null)
        //    {
        //        // 3. Iterate through the entries and populate the Dictionary
        //        foreach (DictionaryEntry entry in resourceSet)
        //        {
        //            if (entry.Key is string key && entry.Value is string value)
        //            {
        //                resourceDictionary[key] = value;
        //                Debug.WriteLine($"ReadLanguageResourceFileAsync: {key} = {value}");
        //            }
        //        }
        //    }

        //    return resourceDictionary;
        //}


        public static string GetValueResourceSet(string cAppId)
        {
            string test = resourceSet.TryGetValue(cAppId, out string? value) ? value : string.Empty;
            //return resourceSet.TryGetValue(cAppId, out string? value) ? value : string.Empty;
            return test;

        }
    }
}
