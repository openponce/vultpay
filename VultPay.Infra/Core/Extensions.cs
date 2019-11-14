using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Newtonsoft.Json.Schema.Generation;
using SHA3.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace VultPay.Infra.Core
{
    public sealed class Singleton<T> where T : class, new()
    {
        private static T _instance;
        public static T GetInstance()
        {
            if (_instance == null)
                _instance = new T();
            return _instance;
        }
    }

    public static class Extensions
    {

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> CountryList()
        {
            return new Dictionary<string, string>() {
               { "AFG", "AF" },    // Afghanistan
               { "ALB", "AL" },    // Albania
               { "ARE", "AE" },    // U.A.E.
               { "ARG", "AR" },    // Argentina
               { "ARM", "AM" },    // Armenia
               { "AUS", "AU" },    // Australia
               { "AUT", "AT" },    // Austria
               { "AZE", "AZ" },    // Azerbaijan
               { "BEL", "BE" },    // Belgium
               { "BGD", "BD" },    // Bangladesh
               { "BGR", "BG" },    // Bulgaria
               { "BHR", "BH" },    // Bahrain
               { "BIH", "BA" },    // Bosnia and Herzegovina
               { "BLR", "BY" },    // Belarus
               { "BLZ", "BZ" },    // Belize
               { "BOL", "BO" },    // Bolivia
               { "BRA", "BR" },    // Brazil
               { "BRN", "BN" },    // Brunei Darussalam
               { "CAN", "CA" },    // Canada
               { "CHE", "CH" },    // Switzerland
               { "CHL", "CL" },    // Chile
               { "CHN", "CN" },    // People's Republic of China
               { "COL", "CO" },    // Colombia
               { "CRI", "CR" },    // Costa Rica
               { "CZE", "CZ" },    // Czech Republic
               { "DEU", "DE" },    // Germany
               { "DNK", "DK" },    // Denmark
               { "DOM", "DO" },    // Dominican Republic
               { "DZA", "DZ" },    // Algeria
               { "ECU", "EC" },    // Ecuador
               { "EGY", "EG" },    // Egypt
               { "ESP", "ES" },    // Spain
               { "EST", "EE" },    // Estonia
               { "ETH", "ET" },    // Ethiopia
               { "FIN", "FI" },    // Finland
               { "FRA", "FR" },    // France
               { "FRO", "FO" },    // Faroe Islands
               { "GBR", "GB" },    // United Kingdom
               { "GEO", "GE" },    // Georgia
               { "GRC", "GR" },    // Greece
               { "GRL", "GL" },    // Greenland
               { "GTM", "GT" },    // Guatemala
               { "HKG", "HK" },    // Hong Kong S.A.R.
               { "HND", "HN" },    // Honduras
               { "HRV", "HR" },    // Croatia
               { "HUN", "HU" },    // Hungary
               { "IDN", "ID" },    // Indonesia
               { "IND", "IN" },    // India
               { "IRL", "IE" },    // Ireland
               { "IRN", "IR" },    // Iran
               { "IRQ", "IQ" },    // Iraq
               { "ISL", "IS" },    // Iceland
               { "ISR", "IL" },    // Israel
               { "ITA", "IT" },    // Italy
               { "JAM", "JM" },    // Jamaica
               { "JOR", "JO" },    // Jordan
               { "JPN", "JP" },    // Japan
               { "KAZ", "KZ" },    // Kazakhstan
               { "KEN", "KE" },    // Kenya
               { "KGZ", "KG" },    // Kyrgyzstan
               { "KHM", "KH" },    // Cambodia
               { "KOR", "KR" },    // Korea
               { "KWT", "KW" },    // Kuwait
               { "LAO", "LA" },    // Lao P.D.R.
               { "LBN", "LB" },    // Lebanon
               { "LBY", "LY" },    // Libya
               { "LIE", "LI" },    // Liechtenstein
               { "LKA", "LK" },    // Sri Lanka
               { "LTU", "LT" },    // Lithuania
               { "LUX", "LU" },    // Luxembourg
               { "LVA", "LV" },    // Latvia
               { "MAC", "MO" },    // Macao S.A.R.
               { "MAR", "MA" },    // Morocco
               { "MCO", "MC" },    // Principality of Monaco
               { "MDV", "MV" },    // Maldives
               { "MEX", "MX" },    // Mexico
               { "MKD", "MK" },    // Macedonia (FYROM)
               { "MLT", "MT" },    // Malta
               { "MNE", "ME" },    // Montenegro
               { "MNG", "MN" },    // Mongolia
               { "MYS", "MY" },    // Malaysia
               { "NGA", "NG" },    // Nigeria
               { "NIC", "NI" },    // Nicaragua
               { "NLD", "NL" },    // Netherlands
               { "NOR", "NO" },    // Norway
               { "NPL", "NP" },    // Nepal
               { "NZL", "NZ" },    // New Zealand
               { "OMN", "OM" },    // Oman
               { "PAK", "PK" },    // Islamic Republic of Pakistan
               { "PAN", "PA" },    // Panama
               { "PER", "PE" },    // Peru
               { "PHL", "PH" },    // Republic of the Philippines
               { "POL", "PL" },    // Poland
               { "PRI", "PR" },    // Puerto Rico
               { "PRT", "PT" },    // Portugal
               { "PRY", "PY" },    // Paraguay
               { "QAT", "QA" },    // Qatar
               { "ROU", "RO" },    // Romania
               { "RUS", "RU" },    // Russia
               { "RWA", "RW" },    // Rwanda
               { "SAU", "SA" },    // Saudi Arabia
               { "SCG", "CS" },    // Serbia and Montenegro (Former)
               { "SEN", "SN" },    // Senegal
               { "SGP", "SG" },    // Singapore
               { "SLV", "SV" },    // El Salvador
               { "SRB", "RS" },    // Serbia
               { "SVK", "SK" },    // Slovakia
               { "SVN", "SI" },    // Slovenia
               { "SWE", "SE" },    // Sweden
               { "SYR", "SY" },    // Syria
               { "TAJ", "TJ" },    // Tajikistan
               { "THA", "TH" },    // Thailand
               { "TKM", "TM" },    // Turkmenistan
               { "TTO", "TT" },    // Trinidad and Tobago
               { "TUN", "TN" },    // Tunisia
               { "TUR", "TR" },    // Turkey
               { "TWN", "TW" },    // Taiwan
               { "UKR", "UA" },    // Ukraine
               { "URY", "UY" },    // Uruguay
               { "USA", "US" },    // United States
               { "UZB", "UZ" },    // Uzbekistan
               { "VEN", "VE" },    // Bolivarian Republic of Venezuela
               { "VNM", "VN" },    // Vietnam
               { "YEM", "YE" },    // Yemen
               { "ZAF", "ZA" },    // South Africa
               { "ZWE", "ZW" },    // Zimbabwe
            };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public static bool ValidateEmail(string email)
        {
            Regex rg = new Regex(@"^[A-Za-z0-9](([_\.\-]?[a-zA-Z0-9]+)*)@([A-Za-z0-9]+)(([\.\-]?[a-zA-Z0-9]+)*)\.([A-Za-z]{2,})$");

            if (rg.IsMatch(email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Divide o array especificado por <paramref name="list"/> em um array multdimensional com a quantidade de elementos especificados em <paramref name="nSize"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="nSize"></param>
        /// <returns></returns>
        public static IEnumerable<List<T>> SplitList<T>(this List<T> list, int nSize = 30)
        {
            for (int ix = 0; ix < list.Count; ix += nSize)
            {
                yield return list.GetRange(ix, Math.Min(nSize, list.Count - ix));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string RemoveLineEndings(this string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return value;
            }

            string lineSeparator = ((char)0x2028).ToString();
            string paragraphSeparator = ((char)0x2029).ToString();

            return value
                .Replace("\r\n", string.Empty)
                .Replace("\n", string.Empty)
                .Replace("\r", string.Empty)
                .Replace(lineSeparator, string.Empty)
                .Replace(paragraphSeparator, string.Empty)
                .Trim();
        }

        /// <summary>
        /// converte a lista de objetos <paramref name="obj"/> em uma lista do tipo <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static List<T> ConvertListObject<T>(this IList<object> obj)
        {
            List<T> list = new List<T>();

            obj.ToList().ForEach(c =>
            {
                list.Add(ConvertObject<T>(c));
            });

            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertObject<T>(this object value) => (T)Convert.ChangeType(value, typeof(T));

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ToXml<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public static T XmlToObject<T>(this string Xml)
        {
            try
            {
                StringReader rd = new StringReader(Xml);
                var xmlserializer = new XmlSerializer(typeof(T));
                return (T)xmlserializer.Deserialize(rd);
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Xml"></param>
        /// <returns></returns>
        public static List<T> XmlToListObject<T>(this string Xml)
        {
            try
            {
                StringReader rd = new StringReader(Xml);
                var xmlserializer = new XmlSerializer(typeof(List<T>));
                return (List<T>)xmlserializer.Deserialize(rd);
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xDocument"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(this XDocument xDocument)
        {
            var xmlDocument = new XmlDocument();
            using (var xmlReader = xDocument.CreateReader())
            {
                xmlDocument.Load(xmlReader);
            }
            return xmlDocument;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlDocument"></param>
        /// <returns></returns>
        public static XDocument ToXDocument(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XDocument.Load(nodeReader);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="subjectName"></param>
        /// <returns></returns>
        public static X509Certificate2 GetX509Certificate(string subjectName)
        {
            var certificateStore = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            certificateStore.Open(OpenFlags.ReadOnly);
            X509Certificate2 certificate;

            try
            {
                certificate = certificateStore.Certificates.OfType<X509Certificate2>().
                                                                FirstOrDefault(cert => cert.Subject.Contains(subjectName));
            }
            finally
            {
                certificateStore.Close();
            }

            if (certificate == null)
                throw new Exception(String.Format("Certificate '{0}' not found.", subjectName));

            return certificate;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static byte[] GetBytes(this string value)
        {
            return System.Text.Encoding.Default.GetBytes(value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string RemoveQuotesStartEnd(this string str)
        {
            return new Regex("[\"'](.*)[\"']")
                .Replace(str, "$1")
                .Trim();
        }

        /// <summary>
        /// Faz o HASH SHA-3 da senha
        /// </summary>
        /// <param name="Password"></param>
        /// <returns></returns>
        public static string HashSha3_256(this string Password)
        {
            StringBuilder x = new StringBuilder();

            foreach (var item in Sha3.Sha3256().ComputeHash(Encoding.UTF8.GetBytes(Password)))
            {
                x.Append(item.ToString("x2"));
            }

            return x.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringValue"></param>
        /// <returns></returns>
        public static bool IsValidJson(this string stringValue)
        {
            if (string.IsNullOrWhiteSpace(stringValue))
            {
                return false;
            }

            var value = stringValue.Trim();

            if ((value.StartsWith("{") && value.EndsWith("}")) || //For object
                (value.StartsWith("[") && value.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(value);
                    return true;
                }
                catch (JsonReaderException)
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jsonData"></param>
        /// <returns></returns>
        public static bool TryParseJson<T>(string jsonData) where T : new()
        {
            JSchemaGenerator generator = new JSchemaGenerator();
            JSchema parsedSchema = generator.Generate(typeof(T));
            JObject jObject = JObject.Parse(jsonData);

            return jObject.IsValid(parsedSchema);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <returns></returns>
        public static Exception FullException(this Exception x)
        {
            var st = new StackTrace(x, true);
            var frames = st.GetFrames();
            var traceString = new StringBuilder();

            foreach (var frame in frames)
            {
                if (frame.GetFileLineNumber() < 1)
                    continue;

                traceString.Append("File: " + frame.GetFileName());
                traceString.Append(", LineNumber: " + frame.GetFileLineNumber());
                traceString.Append(", Method:" + frame.GetMethod().Name);
                traceString.Append("  -->  ");
            }

            return new Exception(traceString.ToString());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceStream"></param>
        /// <param name="sourceStreamReader"></param>
        /// <returns></returns>
        public static async Task<string> ReadFileStreamToString(FileStream sourceStream, StreamReader sourceStreamReader)
        {
            int NumBytesToRead = Convert.ToInt32(sourceStream.Length);
            byte[] FileBytesToken = new byte[NumBytesToRead];
            await sourceStreamReader.BaseStream.ReadAsync(FileBytesToken, 0, NumBytesToRead);
            return Encoding.UTF8.GetString(FileBytesToken, 0, FileBytesToken.Length);
        }
    }
}