using System.Text;

namespace Fixit.Notification.Management.Lib.Builders.Extensions
{
    public static class StringBuilderExtensions
    {
        public static string ConstructLocationAddress(string address, string city, string province, string postalCode, string country = "")
        {
            StringBuilder sb = new StringBuilder();
            string fullAddress = sb.Append(address).Append(", ")
                                   .Append(city).Append(", ")
                                   .Append(province).Append(", ")
                                   .Append(postalCode).Append(" ")
                                   .Append(country)
                                   .ToString();
            return fullAddress;
        }
    }
}
