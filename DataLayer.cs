using System;
using System.Diagnostics;
using System.IO;

namespace ContextUtils
{
    public class TextOutDatabaseContext<T> 
    {
        private readonly StreamWriter outStream;
        public TextOutDatabaseContext(string fileName)
        {
            outStream = new StreamWriter(fileName);
            outStream.AutoFlush = true;
            Database.Log = outStream.WriteLine;
        }
        ~TextOutDatabaseContext()
        {
            try
            {
                outStream.Flush();
                outStream.Close();
            }
            catch (Exception)
            {
                // ignored
            }
        }
    }


    public class ReadOnlyDatabaseContext <T>
    {
        public ReadOnlyDatabaseContext()  // use this to get in and out quick!
        {
            Configuration.ProxyCreationEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.LazyLoadingEnabled = true;
        }
    }

    public static class DebugTextWriter
    {
        public static void Write(char[] buffer, int index, int count)
        {
            Debug.Write(new String(buffer, index, count));
        }

        public static void Write(string value)
        {
            Debug.Write(value);
        }
    }
}
