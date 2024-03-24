namespace GtPrax.UI.Extensions;

using FluentResults;

public static partial class LogExtensions
{
    [LoggerMessage(Level = LogLevel.Error, Message = "Reset password for user {Email} failed: {@Errors}")]
    public static partial void ResetPasswordFailed(this ILogger logger, string email, IEnumerable<IError> errors);
}
