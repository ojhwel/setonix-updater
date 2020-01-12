using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SetonixUpdater.Extensions
{
    /// <summary>
    /// Reduced copy of BerlinalePlaner.Utils.XmlExtensions.
    /// </summary>
    internal static class XmlExtensions
    {
        /// <summary>
        /// Returns the InnerText of the value node at the relative XPath to the specified node, or the default value.
        /// </summary>
        /// <param name="node">The node under which the value is situated.</param>
        /// <param name="xpath">The XPath to the value.</param>
        /// <param name="defaultValue">The default value to return if the value node does not exist.</param>
        public static string GetValue(this XmlNode node, string xpath, string defaultValue)
        {
            XmlNode valueNode = node.SelectSingleNode(xpath);
            if (valueNode != null)
                return valueNode.InnerText;
            else
                return defaultValue;
        }

        /// <summary>
        /// Returns the InnerText of the value node at the relative XPath to the specified node, or the default value.
        /// </summary>
        /// <param name="node">The node under which the value is situated.</param>
        /// <param name="xpath">The XPath to the value.</param>
        /// <param name="defaultValue">The default value to return if the value node does not exist.</param>
        /// <param name="dateFormat">The date format of the date/time string.</param>
        public static DateTime GetValue(this XmlNode node, string xpath, DateTime defaultValue, string dateFormat)
        {
            XmlNode valueNode = node.SelectSingleNode(xpath);
            if (valueNode != null)
                return DateTime.ParseExact(valueNode.InnerText, dateFormat, System.Globalization.CultureInfo.InvariantCulture);
            else
                return defaultValue;

        }

        /// <summary>
        /// Returns the InnerText of the attribute of the specified node, or the default value.
        /// </summary>
        /// <param name="node">The node under which the value is situated.</param>
        /// <param name="attribute">The name of the attribute whose value to return.</param>
        /// <param name="required">Whether the attribute is required. If the attribute is missing and this parameter is <c>false</c>, an empty string is
        /// returned; if the attribute is missing and this parameter is <c>true</c>, a <see cref="MissingXmlAttributeException"/> is thrown.</param>
        /// <exception cref="MissingXmlAttributeException">Thrown if the attribute is missing and the <c>required</c> parameter is <c>true</c>.</exception>
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
