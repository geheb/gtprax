namespace GtPrax.Infrastructure.Extensions;

using System.Linq.Expressions;
using System.Reflection;

public static class PropertyExtensions
{
    public static bool SetValue<T, TValue>(this T target, Expression<Func<T, TValue>> memberLamda, TValue value)
    {
        if (memberLamda.Body is not MemberExpression memberSelectorExpression)
        {
            throw new InvalidCastException($"{nameof(memberLamda)} has no body");
        }

        if (memberSelectorExpression.Member is not PropertyInfo property)
        {
            throw new InvalidCastException($"{nameof(memberLamda)} has no member");
        }

        var targetValue = property.GetValue(target);

        if (Equals(targetValue, value))
        {
            return false;
        }

        property.SetValue(target, value, null);
        return true;
    }
}
