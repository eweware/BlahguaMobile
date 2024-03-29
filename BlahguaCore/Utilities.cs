using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace BlahguaMobile.BlahguaCore
{
    public static class DateTimeJavaScript
    {
        private static readonly long DatetimeMinTimeTicks =
           (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;

        public static long ToJavaScriptMilliseconds(this DateTime dt)
        {
            return (long)((dt.ToUniversalTime().Ticks - DatetimeMinTimeTicks) / 10000);
        }
    }

    
    
    public class Utilities
    {
        public static List<KeyValuePair<string, string>> BadWordList = null;

        public static string MaskProfanity(string sourceStr)
        {
            string destStr = sourceStr;

            if (BadWordList == null)
                InitBadWordList();

            foreach (KeyValuePair<string, string> badWord in BadWordList)
            {
                string strWord = "\\b" + badWord.Key + "\\b";
                destStr = Regex.Replace(destStr, strWord, badWord.Value, RegexOptions.IgnoreCase);
            }

            return destStr;
        }

        private static void InitBadWordList()
        {
            BadWordList = new List<KeyValuePair<string,string>>();
            BadWordList.Add(new KeyValuePair<string,string>("fuck", "f***"));
            BadWordList.Add(new KeyValuePair<string,string>("shit", "sh**"));
            BadWordList.Add(new KeyValuePair<string,string>("cunt", "c***"));
            BadWordList.Add(new KeyValuePair<string,string>("bitch", "b***"));
            BadWordList.Add(new KeyValuePair<string,string>("fucker", "f***er"));
            BadWordList.Add(new KeyValuePair<string, string>("fucked", "f**ed"));
            BadWordList.Add(new KeyValuePair<string, string>("fucker", "f***er"));
            BadWordList.Add(new KeyValuePair<string, string>("shithead", "sh**head"));
            BadWordList.Add(new KeyValuePair<string, string>("nigger", "n*****"));
        }

        public static string CreateDateString(DateTime theDate, bool omitDay)
        {
            string year = (theDate.Year - 2000).ToString();
            string month;

            if (theDate.Month < 10)
                month = "0" + theDate.Month.ToString();
            else
                month = theDate.Month.ToString();

            if (omitDay)
                return year + month;
            else
            {
                string day;
                if (theDate.Day < 10)
                    day = "0" + theDate.Day.ToString();
                else
                    day = theDate.Day.ToString();

                return year + month + day;
            }
        }

        public static string ElapsedDateString(DateTime theDate, bool shouldBeFuture = false)
        {
            string tailStr;
            long nowTicks = DateTime.Now.Ticks;
            long dateTicks = theDate.Ticks;
            long timeSpan;

            if (shouldBeFuture)
            {
                if (dateTicks > nowTicks)
                {
                    timeSpan = (dateTicks - nowTicks) / TimeSpan.TicksPerSecond; 
                    tailStr = " from now";
                }
                else
                    return "any time now";

            }
            else
            {
                if (dateTicks > nowTicks)
                    return "just now";
                else
                {
                    timeSpan = (nowTicks - dateTicks) / TimeSpan.TicksPerSecond;
                    tailStr = " ago";
                }
            }



            long curYears = timeSpan / 31536000;
            if (curYears > 0)
            {
                if (curYears > 2)
                {
                    return curYears + " years" + tailStr;
                }
                else
                {
                    return timeSpan / 2592000 + " months" + tailStr;
                }
            }

            long curMonths = timeSpan / 2592000; // average 30 days
            if (curMonths > 0)
            {
                if (curMonths >= 2)
                {
                    return curMonths + " months" + tailStr;
                }
                else
                {
                    return timeSpan / 604800 + " weeks" + tailStr;
                }
            }

            long curDays = timeSpan / 86400;
            if (curDays > 0)
            {
                if (curDays >= 2)
                {
                    return curDays + " days" + tailStr;
                }
                else
                {
                    return timeSpan / 3600 + " hours" + tailStr;
                }
            }

            long curHours = timeSpan / 3600;
            if (curHours > 0)
            {
                if (curHours >= 2)
                {
                    return curHours + " hours" + tailStr;
                }
                else
                {
                    return timeSpan / 60 + " minutes" + tailStr;
                }
            }

            long curMinutes = timeSpan / 60;
            if (curMinutes >= 2)
            {
                return curMinutes + " minutes" + tailStr;
            }

            if (timeSpan <= 1)
            {
                if (shouldBeFuture)
                    return "any time now";
                else
                    return "just now";
            }
            else
            {
                return timeSpan + " seconds" + tailStr;
            }

        }

    }

}
