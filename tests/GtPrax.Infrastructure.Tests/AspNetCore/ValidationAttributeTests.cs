namespace GtPrax.Infrastructure.Tests.AspNetCore;

using GtPrax.Infrastructure.AspNetCore;
using Xunit;

public sealed class ValidationAttributeTests
{
    public sealed class PhoneFieldAttributeTests
    {
        private readonly PhoneFieldAttribute _sut = new();

        [Theory]
        [InlineData("1234", true)]
        [InlineData("0123456789012345", true)]
        [InlineData("12345678", true)]
        public void IsValid_ValidPhone_ShouldReturnTrue(string phone, bool expected)
        {
            Assert.Equal(expected, _sut.IsValid(phone));
        }

        [Theory]
        [InlineData("123", false)]
        [InlineData("12345678901234567", false)]
        [InlineData("abc", false)]
        [InlineData("+49123", false)]
        [InlineData("0123 456", false)]
        public void IsValid_InvalidPhone_ShouldReturnFalse(string phone, bool expected)
        {
            Assert.Equal(expected, _sut.IsValid(phone));
        }

        [Fact]
        public void IsValid_Null_ShouldReturnTrue()
        {
            // RegularExpressionAttribute returns true for null
            Assert.True(_sut.IsValid(null));
        }
    }

    public sealed class EmailFieldAttributeTests
    {
        private readonly EmailFieldAttribute _sut = new();

        [Fact]
        public void IsValid_ValidEmail_ShouldReturnTrue()
        {
            Assert.True(_sut.IsValid("user@example.com"));
        }

        [Fact]
        public void IsValid_InvalidEmail_ShouldReturnFalse()
        {
            Assert.False(_sut.IsValid("not-an-email"));
        }

        [Fact]
        public void IsValid_Null_ShouldReturnTrue()
        {
            Assert.True(_sut.IsValid(null));
        }
    }

    public sealed class EmailLengthFieldAttributeTests
    {
        private readonly EmailLengthFieldAttribute _sut = new();

        [Fact]
        public void IsValid_ValidLength_ShouldReturnTrue()
        {
            Assert.True(_sut.IsValid("a@b.com"));
        }

        [Fact]
        public void IsValid_TooShort_ShouldReturnFalse()
        {
            Assert.False(_sut.IsValid("a@b.c"));
        }

        [Fact]
        public void IsValid_TooLong_ShouldReturnFalse()
        {
            var longEmail = new string('a', 257);
            Assert.False(_sut.IsValid(longEmail));
        }
    }

    public sealed class PasswordLengthFieldAttributeTests
    {
        private readonly PasswordLengthFieldAttribute _sut = new();

        [Fact]
        public void IsValid_ValidLength_ShouldReturnTrue()
        {
            Assert.True(_sut.IsValid("1234567890"));
        }

        [Fact]
        public void IsValid_TooShort_ShouldReturnFalse()
        {
            Assert.False(_sut.IsValid("123456789"));
        }

        [Fact]
        public void MinLen_ShouldBe10()
        {
            Assert.Equal(10, PasswordLengthFieldAttribute.MinLen);
        }
    }
}
