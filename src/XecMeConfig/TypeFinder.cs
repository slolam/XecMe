using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace XecMeConfig
{
    public class TypeFinder : MarshalByRefObject
    {
        public string[] GetTasksTypes(string[] files, string typeName)
        {
            List<string> retVal = new List<string>();

            List<Assembly> assemblies = new List<Assembly>();

            foreach (var dll in files)
            {
                try
                {
                    assemblies.Add(Assembly.LoadFrom(dll));
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                }

            }
            foreach (var assembly in assemblies)
            {
                try
                {
                    Type[] types = new Type[0];

                    try
                    {
                        types = assembly.GetTypes();
                    }
                    catch (ReflectionTypeLoadException re)
                    {
                        types = re.Types;
                    }
                    catch (Exception e)
                    {
                        Trace.TraceError(e.ToString());
                    }

                    for (int i = 0; i < types.Length; i++)
                    {
                        Type type = types[i];

                        if (type == null)
                            continue;

                        try
                        {
                            if (!type.IsArray && type.IsClass && !type.IsAbstract)
                            {
                                foreach (var it in type.GetInterfaces())
                                {
                                    if (it.FullName == typeName)
                                    {
                                        retVal.Add(type.AssemblyQualifiedName);
                                        break;
                                    }
                                }
                                if (type.GetElementType().FullName == typeName)
                                {
                                    retVal.Add(type.AssemblyQualifiedName);
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.TraceError(e.ToString());
                        }
                    }
                }
                catch (Exception e)
                {
                    Trace.TraceError(e.ToString());
                }
            }


            return retVal.ToArray();
        }
    }
}
