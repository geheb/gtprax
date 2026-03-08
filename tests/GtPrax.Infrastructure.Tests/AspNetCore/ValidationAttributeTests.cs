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

    public sealed class RequiredFieldAttributeTests
    {
        private readonly RequiredFieldAttribute _sut = new();

        [Fact]
        public void IsValid_WithValue_ShouldReturnTrue()
        {
            Assert.True(_sut.IsValid("hello"));
        }

        [Fact]
        public void IsValid_Null_ShouldReturnFalse()
        {
            Assert.False(_sut.IsValid(null));
        }

        [Fact]
        public void IsValid_EmptyString_ShouldReturnFalse()
        {
            Assert.False(_sut.IsValid(""));
        }
    }

    public sealed class TextLengthFieldAttributeTests
    {
        [Fact]
        public void IsValid_DefaultMax_ValidLength_ShouldReturnTrue()
        {
            var sut = new TextLengthFieldAttribute();
            Assert.True(sut.IsValid("ab"));
        }

        [Fact]
        public void IsValid_DefaultMax_TooShort_ShouldReturnFalse()
        {
            var sut = new TextLengthFieldAttribute();
            Assert.False(sut.IsValid("a"));
        }

        [Fact]
        public void IsValid_DefaultMax_TooLong_ShouldReturnFalse()
        {
            var sut = new TextLengthFieldAttribute();
            Assert.False(sut.IsValid(new string('x', 257)));
        }

        [Fact]
        public void IsValid_CustomMax_AtMax_ShouldReturnTrue()
        {
            var sut = new TextLengthFieldAttribute(10);
            Assert.True(sut.IsValid("1234567890"));
        }

        [Fact]
        public void IsValid_CustomMax_OverMax_ShouldReturnFalse()
        {
            var sut = new TextLengthFieldAttribute(10);
            Assert.False(sut.IsValid("12345678901"));
        }

        [Fact]
        public void IsValid_Null_ShouldReturnTrue()
        {
            var sut = new TextLengthFieldAttribute();
            Assert.True(sut.IsValid(null));
        }
    }

    public sealed class RangeFieldAttributeTests
    {
        [Fact]
        public void IsValid_IntInRange_ShouldReturnTrue()
        {
            var sut = new RangeFieldAttribute(1, 10);
            Assert.True(sut.IsValid(5));
        }

        [Fact]
        public void IsValid_IntBelowRange_ShouldReturnFalse()
        {
            var sut = new RangeFieldAttribute(1, 10);
            Assert.False(sut.IsValid(0));
        }

        [Fact]
        public void IsValid_IntAboveRange_ShouldReturnFalse()
        {
            var sut = new RangeFieldAttribute(1, 10);
            Assert.False(sut.IsValid(11));
        }

        [Fact]
        public void IsValid_DoubleInRange_ShouldReturnTrue()
        {
            var sut = new RangeFieldAttribute(0.5, 9.5);
            Assert.True(sut.IsValid(5.0));
        }

        [Fact]
        public void IsValid_DoubleBelowRange_ShouldReturnFalse()
        {
            var sut = new RangeFieldAttribute(0.5, 9.5);
            Assert.False(sut.IsValid(0.1));
        }
    }
}
