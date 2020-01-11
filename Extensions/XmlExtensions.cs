using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SetonixUpdater.Extensions
{
    // TODO Comments
    // Reduced copy of BerlinalePlaner.Utils.XmlExtensions
    internal static class XmlExtensions
    {
        public static string GetValue(this XmlNode node, string xpath, string defaultValue)
        {
            XmlNode valueNode = node.SelectSingleNode(xpath);
            if (valueNode != null)
                return valueNode.InnerText;
            else
                return defaultValue;
        }

        public static DateTime GetValue(this XmlNode node, string xpath, DateTime defaultValue, string dateFormat)
        {
            XmlNode valueNode = node.SelectSingleNode(xpath);
            if (valueNode != null)
                return DateTime.ParseExact(valueNode.InnerText, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
            else
                return defaultValue;

        }

        public static string GetAttributeValue(this XmlNode node, string attribute, bool required)
        {
            XmlNode attributeNode = node.Attributes.GetNamedItem(attribute);
            if (attributeNode == null)
                if (!required)
                    return string.Empty;
                else
                    throw new MissingXmlAttributeException(attribute);
            return attributeNode.InnerText;

        }
    }
}
