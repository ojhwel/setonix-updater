using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace SetonixUpdater
{
    /// <summary>
    /// Contains a number of localized strings that can be retrieved via a key (a language-independent reference to the string).
    /// <para/>
    /// Strings are retrieved for the culture specifed by the Culture property which defaults to the current system culture setting. If a string is requested 
    /// for a language that is not contained in the list, an English string (InvariantCulture) is returned.
    /// </summary>
    public class LocalizedStringCollection
    {
        /// <summary>
        /// The container of strings.
        /// <para/>
        /// The keys of the outer Dictionary are the languages, while the values are each a Dictionary of string keys and the actual localized strings.
        /// </summary>
        private readonly Dictionary<string, Dictionary<string, string>> strings = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// The culture used to retrieve the strings.
        /// </summary>
        public CultureInfo Culture { get; set; }

        /// <summary>
        /// Creates a new LocalizedStringCollection instance with the system culture.
        /// </summary>
        public LocalizedStringCollection()
        {
            Culture = CultureInfo.CurrentUICulture;
        }

        /// <summary>
        // Creates a new LocalizedStringCollection instance with the specified culture.
        /// </summary>
        /// <param name="culture">The culture to use for retrieving the strings.</param>
        public LocalizedStringCollection(CultureInfo culture)
        {
            Culture = culture;
        }

        /// <summary>
        /// Creates a new LocalizedStringCollection instance with culture of the specifed language.
        /// </summary>
        /// <param name="culture">The language to use.</param>
        /// <exception cref="CultureNotFoundException" />
        public LocalizedStringCollection(string culture)
        {
            Culture = CultureInfo.GetCultureInfo(culture);
        }

        /// <summary>
        /// Gets the string for the specified key.
        /// </summary>
        /// <param name="key">The key of the string to get.</param>
        /// <returns>The string for the key, or an empty string if no string is defined.</returns>
        public string this[string key]
        {
            get
            {
                Dictionary<string, string> dictionary = GetDictionary();
                if (dictionary != null && dictionary.ContainsKey(key))
                    return GetDictionary()[key];
                else
                    return string.Empty;
            }
        }

        /// <summary>
        /// Adds a string to the list.
        /// </summary>
        /// <param name="language">The language of the string. Must be either "languagecode2-regioncode2" (e.g. "en-GB" for British English) or  
        /// "languagecode2" (e.g. "es" for Spanish).</param>
        /// <param name="key">The key under which to store the string.</param>
        /// <param name="text">The string in the language.</param>
        public void Add(string language, string key, string text)
        {
            if (strings.ContainsKey(language))
                strings[language].Add(key, text);
            else
            {
                Dictionary<string, string> dictionary = new Dictionary<string, string>();
                strings.Add(language, dictionary);
                dictionary.Add(key, text);
            }
        }

        /// <summary>
        /// Returns the key/string Dictionary for the current language.
        /// </summary>
        private Dictionary<string, string> GetDictionary()
        {
            if (strings.ContainsKey(Culture.Name))
                return strings[Culture.Name];
            else if (strings.ContainsKey(Culture.TwoLetterISOLanguageName))
                return strings[Culture.TwoLetterISOLanguageName];
            else if (strings.ContainsKey(CultureInfo.InvariantCulture.TwoLetterISOLanguageName))
                return strings[CultureInfo.InvariantCulture.TwoLetterISOLanguageName];
            else
                return null;
        }

    }
}
