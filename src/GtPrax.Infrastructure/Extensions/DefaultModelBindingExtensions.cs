namespace GtPrax.Infrastructure.Extensions;

using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Metadata;

public static class DefaultModelBindingExtensions
{
    public static void SetLocale(this DefaultModelBindingMessageProvider provider)
    {
#pragma warning disable CA1863
        provider.SetAttemptedValueIsInvalidAccessor((a, b) =>
            string.Format(CultureInfo.InvariantCulture, Messages.AttemptedValueIsInvalidAccessor, a, b));
        provider.SetNonPropertyAttemptedValueIsInvalidAccessor(a =>
            string.Format(CultureInfo.InvariantCulture, Messages.NonPropertyAttemptedValueIsInvalidAccessor, a));
        provider.SetValueMustBeANumberAccessor(a =>
            string.Format(CultureInfo.InvariantCulture, Messages.FieldMustBeANumberAccessor, a));
#pragma warning restore CA1863
    }
}
