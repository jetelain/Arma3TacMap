using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;

namespace Arma3TacMapWebApp.Controllers.Converters
{
    public sealed class DateTimeAssumeUniversal : ValueConverter<DateTime?, DateTime?>
    {
        public DateTimeAssumeUniversal()
            : base(v => AssumeUniversal(v), v => AssumeUniversal(v))
        {
        }

        public static DateTime? AssumeUniversal(DateTime? dateTime)
        {
            if (dateTime == null)
            {
                return null;
            }
            if (dateTime.Value.Kind == DateTimeKind.Utc)
            {
                return dateTime;
            }
            return DateTime.SpecifyKind(dateTime.Value, DateTimeKind.Utc);
        }
    }
}
