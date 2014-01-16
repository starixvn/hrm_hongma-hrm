// --------------------------------------------------------------------------
// <copyright file="Utilities.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Quinn.Vo" date="2013.1.16">Apply StyleCop</change>
// </history>

namespace Fanex.NexHR.Core
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text.RegularExpressions;
    using System.Web.Mvc;
    using Fanex.Logging;
    using Fanex.NexHR.Core.Enums;

    /// <summary>
    /// Utilities Class.
    /// </summary>
    public static class Utilities
    {
        public static IList<SelectListItem> CheckInOutTypes
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem() { Text = Fanex.NexHR.Resources.Labels.CheckIn, Value = ((short)RequestTypes.CheckIn).ToString() },
                        new SelectListItem() { Text = Fanex.NexHR.Resources.Labels.CheckOut, Value = ((short)RequestTypes.CheckOut).ToString() }
                    };
            }
        }

        public static IList<SelectListItem> LateInEarlyOutTypes
        {
            get
            {
                return new List<SelectListItem>
                    {
                        new SelectListItem() { Text = Fanex.NexHR.Resources.Labels.LateIn, Value = ((short)RequestTypes.LateIn).ToString() },
                        new SelectListItem() { Text = Fanex.NexHR.Resources.Labels.EarlyOut, Value = ((short)RequestTypes.EarlyOut).ToString() }
                    };
            }
        }

        public static DateTime AddTimeToDate(DateTime dateTime, string time)
        {
            var times = time.Split(new[] { ':' });
            TimeSpan ts = new TimeSpan(int.Parse(times[0]), int.Parse(times[1]), 0);
            return dateTime.Date + ts;
        }

        public static string GetTime(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return string.Empty;
            }

            var value = (DateTime)dateTime;
            return ZeroPrefix(value.Hour) + ":" + ZeroPrefix(value.Minute);
        }

        /// <summary>
        /// Function to copy object.
        /// </summary>
        /// <param name="s">Source object.</param>
        /// <param name="t">Destination object.</param>
        public static void CopyTo(this object s, object t)
        {
            foreach (var pS in s.GetType().GetProperties())
            {
                foreach (var pT in t.GetType().GetProperties())
                {
                    if (pT.Name != pS.Name)
                    {
                        continue;
                    }

                    pT.GetSetMethod().Invoke(t, new object[] { pS.GetGetMethod().Invoke(s, null) });
                }
            }
        }

        /// <summary>
        /// Function to read file text.
        /// </summary>
        /// <param name="filePath">Input path of file.</param>
        /// <returns>Content of file.</returns>
        public static string ReadTextFile(string filePath)
        {
            try
            {
                using (var sr = new StreamReader(filePath))
                {
                    return sr.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
                return string.Empty;
            }
        }

        /// <summary>
        /// Function to random string.
        /// </summary>
        /// <param name="length">Input length of string.</param>
        /// <returns>String value.</returns>
        public static string RandomString(int length)
        {
            // returns random string of length specified with characters including: 0-9, a-z, A-Z
            var ca = new char[length];
            var random = new byte[length];

            // RNGCryptoServiceProvider is an implementation of a random number generator.
            var rng = new RNGCryptoServiceProvider();
            rng.GetBytes(random); // The array is now filled with cryptographically strong random bytes.

            for (var i = 0; i < length; i++)
            {
                var found = false;
                var rand = (int)random[i];
                while (!found)
                {
                    if (((rand >= 48) && (rand <= 57)) || ((rand >= 65) && (rand <= 90)) || ((rand >= 97) && (rand <= 122)))
                    {
                        found = true;
                    }
                    else
                    {
                        // get a new random int. 
                        var single = new byte[1];
                        rng.GetBytes(single);
                        rand = single[0];
                    }
                }

                var ci = (char)rand;
                ca[i] = ci;
            }

            return new string(ca).ToLower();
        }

        /// <summary>
        /// Function to get First Day Of Week.
        /// </summary>
        /// <param name="dayInWeek">Day In Week.</param>
        /// <param name="cultureInfo">Input CultureInfo.</param>
        /// <returns>First Day Of Week.</returns>
        public static DateTime FirstDayOfWeek(DateTime dayInWeek, CultureInfo cultureInfo = null)
        {
            if (cultureInfo == null)
            {
                cultureInfo = CultureInfo.CurrentCulture;
            }

            DayOfWeek firstDay = cultureInfo.DateTimeFormat.FirstDayOfWeek;
            DateTime firstDayInWeek = dayInWeek.Date;
            while (firstDayInWeek.DayOfWeek != firstDay)
            {
                firstDayInWeek = firstDayInWeek.AddDays(-1);
            }

            return firstDayInWeek;
        }

        public static DateTime LastDayOfWeek(DateTime dayInWeek)
        {
            return DateTime.Now.AddDays(6 - Convert.ToDouble(dayInWeek.DayOfWeek));
        }

        /// <summary>
        /// Function to get list munites.
        /// </summary>
        /// <param name="step">Input step between two values.</param>
        /// <returns>SelectListItem List.</returns>
        public static IList<SelectListItem> MinuteSteps(int step)
        {
            var list = new List<SelectListItem>();
            var index = 60;
            while (index > 0)
            {
                list.Add(new SelectListItem()
                {
                    Text = index + "'",
                    Value = index.ToString()
                });
                index -= step;
            }

            return list;
        }

        /// <summary>
        /// Function to get list time.
        /// </summary>
        /// <param name="minuteStep">Input munites between two values.</param>
        /// <returns>SelectListItem List.</returns>
        public static IList<SelectListItem> TimeSteps(int minuteStep)
        {
            var list = new List<SelectListItem>();
            var hour = 0;
            var minute = 0;

            while (hour < 24)
            {
                var value = ZeroPrefix(hour) + ":" + ZeroPrefix(minute);
                list.Add(new SelectListItem()
                {
                    Text = value,
                    Value = value
                });
                minute += minuteStep;
                if (minute >= 60)
                {
                    minute = 0;
                    hour++;
                }
            }

            return list;
        }

        public static string ZeroPrefix(int value)
        {
            return value < 10 ? "0" + value : value.ToString();
        }

        public static string ZeroPrefix(string value)
        {
            return int.Parse(value) < 10 ? "0" + value : value;
        }

        /// <summary>
        /// Function to get string array to string.
        /// </summary>
        /// <param name="array">Input array.</param>
        /// <returns>String value.</returns>
        public static string StringArrayToString(string[] array)
        {
            if (array == null || array.Length == 0)
            {
                return string.Empty;
            }

            string result = string.Empty;

            foreach (var item in array)
            {
                result += item + ",";
            }

            return result.Substring(0, result.Length - 1);
        }

        public static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            var regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            return regex.IsMatch(email);
        }

        public static string TotalHoursToTime(float value)
        {
            int hour = (int)value;
            int minute = (int)Math.Round((value - hour) * 60);
            return hour > 0
                ? ZeroPrefix(hour) + "h" + ZeroPrefix(minute) + "'"
                : ZeroPrefix(minute) + "'";
        }

        /// <summary>
        /// Function to convert munites to time.
        /// </summary>
        /// <param name="value">Input munites.</param>
        /// <returns>Time value.</returns>
        public static string MinuteToTime(int value)
        {
            if (value == 0)
            {
                return string.Empty;
            }

            int hour = value / 60;
            int minute = value - (hour * 60);

            return hour > 0
                ? ZeroPrefix(hour) + "h" + ZeroPrefix(minute) + "'"
                : ZeroPrefix(minute) + "'";
        }
    }
}
