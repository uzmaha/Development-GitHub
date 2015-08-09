using P_CMS.Models;
using P_CMS.ViewModels;
using POS.Classes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;

namespace P_CMS.UtilityClasses
{
    public static class DataHelper
    {
        public static string ToPascalConvention(string textToChange)
        {
            // textToChange = "WARD_VS_VITAL_SIGNS";
            System.Text.StringBuilder resultBuilder = new System.Text.StringBuilder();
            foreach (char c in textToChange)
            {
                // Replace anything, but letters and digits, with space
                if (!Char.IsLetterOrDigit(c))
                {
                    resultBuilder.Append(" ");
                }
                else
                {
                    resultBuilder.Append(c);
                }
            }
            string result = resultBuilder.ToString();
            // Make result string all lowercase, because ToTitleCase does not change all uppercase correctly
            result = result.ToLower();
            // Creates a TextInfo based on the "en-US" culture.
            TextInfo myTI = new CultureInfo("en-US", false).TextInfo;
            // result = myTI.ToTitleCase(result).Replace(" ", String.Empty);
            result = myTI.ToTitleCase(result);
            return result;
        }

        internal static string getFileName(string UploadedFileName, out string fileUserId, out string fileUploaded)
        {
            string fileNAme = "";
            fileUploaded = "";
            fileUserId = "";
            if (UploadedFileName.Length > 0)
            {
                string file = UploadedFileName.Split('&')[1];
                fileUserId = UploadedFileName.Split('&')[0];
                fileUploaded = file;
                string filename = file.Split('_')[0];
                string fileExtension = Path.GetExtension(file);
                string fileNameToDisplay = filename + fileExtension;
                fileNAme = fileNameToDisplay;
            }
            return fileNAme;
        }

        internal static void filterDates(string date, out string fromdate, out string todate)
        {
            string dateFiltered = CryptorEngine.ConvertHexToString(date, System.Text.Encoding.Unicode);
            string[] dates = dateFiltered.Split('@');
            fromdate = dates[0] == "none" ? null : dates[0];
            todate = dates[1] == "none" ? null : dates[1];
        }

        internal static string convertIntegarArrayToString(int[] productArray)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < productArray.Count(); i++)
            {
                builder.Append(productArray[i]);
                if ((i+1)<productArray.Count())
                {
                    builder.Append(',');  
                }                
            }
            return builder.ToString();
        }
    }
}