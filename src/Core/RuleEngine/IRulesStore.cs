#region GNU GPL Version 3 License

/// Copyright 2013 Shailesh Lolam
/// 
/// This file IRuleStore.cs is part of XecMe.
/// 
/// XecMe is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
/// 
/// XecMe is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
/// 
/// You should have received a copy of the GNU General Public License along with XecMe. If not, see http://www.gnu.org/licenses/.
/// 
/// History:
/// ______________________________________________________________
/// Created         01-2013             Shailesh Lolam

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Workflow.Activities.Rules;
using System.IO;
using System.Workflow.ComponentModel.Serialization;
using System.Xml;

namespace XecMe.Core.RuleEngine
{
    public interface IRulesStore
    {
        RuleSet GetRuleSet(RuleName ruleName);
    }

    public class RuleName
    {
        public string Name
        { get; set; }
        public short Version
        { get; set; }
        public override string ToString()
        {
            return string.Format("{0}, {1}", Name, Version);
        }
    }

    public static class RuleUtil
    {
        public static void Serialize(RuleSet ruleset, Stream stream)
        {
            using (XmlWriter rulesWriter = XmlWriter.Create(stream))
            {
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                serializer.Serialize(rulesWriter, ruleset);
            }
        }
        public static void Serialize(RuleSet ruleset, StringBuilder sb)
        {
            using (XmlWriter rulesWriter = XmlWriter.Create(sb))
            {
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                serializer.Serialize(rulesWriter, ruleset);
            }
        }
        public static RuleSet Deserialize(Stream stream)
        {
            using (XmlReader rulesReader = XmlReader.Create(stream))
            {
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                return  (RuleSet)serializer.Deserialize(rulesReader);
            }
        }

        public static RuleSet Deserialize(string xml)
        {
            using (XmlReader rulesReader = XmlReader.Create(new StringReader(xml)))
            {
                WorkflowMarkupSerializer serializer = new WorkflowMarkupSerializer();
                return (RuleSet)serializer.Deserialize(rulesReader);
            }
        }
    }
}
