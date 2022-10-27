namespace VideopadCleaner.Extensions
{
    public static class StringExtensions
    {
        public static string Between(this string value, string start, string end)
        {
            var startIdx = value.IndexOf(start) + start.Length;
            var endIdx = value.IndexOf(end, startIdx);

            var s = value.Substring(startIdx, endIdx - startIdx);

            return s;
        }

        public static string CheckFileExist(this string file)
        {
            if (!File.Exists(file))
            {
                throw new FileNotFoundException();
            }

            return file;
        }

        public static string CheckFolderExist(this string path, bool createIfNotExisting = false)
        {
            if (!Directory.Exists(path) && createIfNotExisting)
            {
                Directory.CreateDirectory(path);    
            }

            if (!Directory.Exists(path))
            {                
                throw new DirectoryNotFoundException();
            }

            return path;
        }
    }
}