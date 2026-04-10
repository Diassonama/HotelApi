using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Hotel.Application.Helper
{
    public static class TimeZoneHelper
    {
         private static readonly TimeZoneInfo AngolaTimeZone =
        TimeZoneInfo.FindSystemTimeZoneById("W. Central Africa Standard Time");
        // Em sistemas Linux/Mac pode ser: "Africa/Luanda"

    public static DateTime GetDateInAngola(DateTime dateTime)
    {
        // Garante que a data está em UTC antes de converter
        var utc = dateTime.Kind == DateTimeKind.Utc
            ? dateTime
            : dateTime.ToUniversalTime();

        // Converte para horário de Angola e pega só a data
        return TimeZoneInfo.ConvertTimeFromUtc(utc, AngolaTimeZone).Date;
    }
    }
}