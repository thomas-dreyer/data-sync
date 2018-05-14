namespace DataSyncWeb.Helpers
{
    public static class DataFormatHelper
    {
        /// <summary>
        /// Names of orders of magnitude.
        /// </summary>
        private static string[] orderOfMagnitude = new string[] 
        {
            "Bytes", "Kilobytes", "Megabytes",
            "Gigabytes", "Terabytes", "Petabytes"
        };

        /// <summary>
        /// Formats the byte size to the nearest order of magnitude.
        /// </summary>
        /// <param name="bytes">Amount of bytes.</param>
        /// <returns>formatted string</returns>
        public static string FormatSize(long bytes)
        {
            int orderOfMagnitude = 0;
            while (bytes > 1024)
            {
                bytes /= 1024;
                orderOfMagnitude++;
            }
            return string.Format("{0} {1}", bytes, DataFormatHelper.orderOfMagnitude[orderOfMagnitude]);
        }

    }
}