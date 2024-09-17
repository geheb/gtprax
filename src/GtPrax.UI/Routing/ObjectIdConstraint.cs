namespace GtPrax.UI.Routing;

using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Routing.Constraints;

public partial class ObjectIdConstraint : RegexRouteConstraint
{
    public ObjectIdConstraint() : base(GetObjectIdRegex())
    {
    }

    [GeneratedRegex(@"^[a-z0-9]{24}$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant, 500)]
    private static partial Regex GetObjectIdRegex();
}
