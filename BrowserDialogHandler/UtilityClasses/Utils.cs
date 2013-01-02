using System;
using System.Drawing;
using System.Threading;

namespace BrowserDialogHandler.UtilityClasses
{
    /// <summary>
    /// Class with some utility methods to explore the HTML of a <see cref="Document"/>.
    /// </summary>
    public class UtilityClass
    {
        /// <summary>
        /// Prevent creating an instance of this class (contains only static members)
        /// </summary>
        private UtilityClass() {}

        public static string EscapeSendKeysCharacters(string value)
        {
            const string sendKeysCharactersToBeEscaped = "~%^+{}[]()";

            if (value.IndexOfAny(sendKeysCharactersToBeEscaped.ToCharArray()) > -1)
            {
                string returnvalue = null;

                foreach (var c in value)
                {
                    if (sendKeysCharactersToBeEscaped.IndexOf(c) != -1)
                    {
                        // Escape sendkeys special characters
                        returnvalue = returnvalue + "{" + c + "}";
                    }
                    else
                    {
                        returnvalue = returnvalue + c;
                    }
                }
                return returnvalue;
            }

            return value;
        }
    }
}