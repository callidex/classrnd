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
 
public static class EAVExtensions
{

    public static bool AddPropertyByName(this GNTDynamic d, string propertyType, string propertyValue, string propertyName)
    {
        var dic = d.GetUDFS();
        if (d.GetUDFS().ContainsKey(propertyName))
            return false;
        UDF newUDF = new UDF
        {
            PropertyValue = propertyValue,
            PropertyName = propertyName,
            PropertyTypeName = propertyType,
            ParentID = d.Id
        };


        dic.Add(propertyName, newUDF);
        return true;

    }

    // should this be descructive if the properties have already been set?
    /// <summary>
    /// This method is used when an application object has just been loaded, goes back to the database and populates it's UDFs
    /// </summary>
    /// <param name="d"></param>
    public static void PopulateDynamicProperties(this GNTDynamic d, GNTDBContext db)
    {
        var dic = d.GetUDFS();
        var props = db.Properties.Where(x => x.ParentID == d.Id).Include("Layout");
        foreach (var v in props)
        {
            if (dic.ContainsKey(v.PropertyName))
            {
                dic[v.PropertyName] = v;
            }
            else
            {
                if (v.PropertyTypeName != null)
                {
                    try
                    {
                        var t = Type.GetType(v.PropertyTypeName);
                        v.InstanceObject = Activator.CreateInstance(t);
                        if (v != null)
                            v.InstanceObject = InflateUDFControl(v.PropertyTypeName, v.PropertyValue);

                    }
                    catch (Exception e)
                    {
                    }
                }
                dic.Add(v.PropertyName, v);
            }
        }
    }
    /// <summary>
    /// Try and coerse the string persisted property value into the proper type information
    /// </summary>
    /// <param name="instanceObject"></param>
    /// <param name="type"></param>
    private static object InflateUDFControl(string type, string value)
    {
        switch (type)
        {
            case "System.Boolean":
                bool b;
                bool.TryParse(value, out b);
                return b;
            case "System.Byte":
                throw new NotSupportedException();
            case "System.SByte":
                throw new NotSupportedException();
            case "System.Char":
                throw new NotSupportedException();
            case "System.Decimal":
                throw new NotSupportedException();
            case "System.Double":
                throw new NotSupportedException();
            case "System.Single":
                throw new NotSupportedException();
            case "System.Int32":
                int ix;
                Int32.TryParse(value, out ix);
                return ix;
            case "System.UInt32":
                throw new NotSupportedException();
            case "System.Int64":
                throw new NotSupportedException();
            case "System.UInt64":
                throw new NotSupportedException();
            case "System.Int16":
                throw new NotSupportedException();
            case "System.UInt16":
                ushort x;
                ushort.TryParse(value, out x);
                return x;
            case "System.String":
                return value;
            default:
                throw new NotSupportedException();
        }
    }
}