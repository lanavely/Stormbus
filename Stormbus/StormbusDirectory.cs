using System;
using System.IO;

namespace Stormbus.UI
{
    public static class StormbusDirectory
    {
        public static readonly string StormbusDataFolderPath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), @"Stormbus");

        public static readonly string ConfigurationFilePath =
            Path.Combine(StormbusDataFolderPath, @"Configuration.xml");
    }
}