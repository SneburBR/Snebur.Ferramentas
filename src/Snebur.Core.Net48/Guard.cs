using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Runtime.CompilerServices;

namespace Snebur
{
    public static class Guard
    {

        public static void NotNull(
#if NET9_0_OR_GREATER
                [NotNull] object value,
                [CallerArgumentExpression(nameof(value))] string paramName = "")
#else
                object value,
                [CallerMemberName] string paramName = "")
#endif
        {
            if (value is null)
            {
                throw new ArgumentNullException($"{paramName} cannot be null.", paramName);
            }
        }

        public static void NotNullOrWhiteSpace(
#if NET9_0_OR_GREATER
                [NotNull] string value,
                [CallerArgumentExpression(nameof(value))] string paramName = "")
#else
                string value,
                [CallerMemberName] string paramName = "")
#endif

        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException($"{paramName} cannot be null, empty, or whitespace.", paramName);
            }
        }

        public static void FileExists(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Project file not found. {filePath}", filePath);
            }
        }

        public static void MustBeGreaterThanZero(
#if NET9_0_OR_GREATER
                [NotNull] int value,
                [CallerArgumentExpression(nameof(value))] string paramName = "")
#else
                int value,
                [CallerMemberName] string paramName = "")
#endif       
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be greater than zero.");
            }
        }

#if NET9_0_OR_GREATER
        public static void NotEmpty<T>(
               [NotNull] ICollection<T> value,
               [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value.Count == 0)
                throw new ArgumentException($"{paramName} cannot be empty.", paramName);
        }

        public static void MustBeEmpty<T>(
            [NotNull] ICollection<T> value,
            [CallerArgumentExpression(nameof(value))] string paramName = "")
        {
            if (value is null)
                throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");

            if (value.Count > 0)
                throw new ArgumentException($"{paramName} must be empty.", paramName);
        }
#endif

    }
    //public static class Guard
    //{
    //    public static void NotNull<T>(
    //        [NotNull] T value,
    //        [CallerArgumentExpression(nameof(value))] string? paramName = "")
    //    {
    //        if (value is null)
    //            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");
    //    }

    //    public static void NotNullOrWhiteSpace(
    //        [NotNull] string? value,
    //        [CallerArgumentExpression(nameof(value))] string? paramName = "")
    //    {
    //        if (string.IsNullOrWhiteSpace(value))
    //            throw new ArgumentException($"{paramName} cannot be null, empty, or whitespace.", paramName);
    //    }

    //    public static void FullPhoneNumber(
    //        [NotNull] string? fullNumber,
    //        [CallerArgumentExpression(nameof(fullNumber))] string? paramName = "")
    //    {
    //        if (fullNumber is null)
    //            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");

    //        if (!PhoneNumberValidationUtils.IsFullPhoneNumberValid(fullNumber))
    //            throw new ArgumentException($"{paramName} is not a valid phone number.", paramName);
    //    }

    //    public static void Positive(
    //        int value,
    //        [CallerArgumentExpression(nameof(value))] string paramName = "")
    //    {
    //        if (value <= 0)
    //            throw new ArgumentOutOfRangeException(paramName, $"{paramName} must be greater than zero.");
    //    }

    //    public static void Sha256(
    //        [NotNull] string? value,
    //        [CallerArgumentExpression(nameof(value))] string paramName = "")
    //    {
    //        if (value is null)
    //            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");

    //        if (!ValidationUtils.IsSha256(value))
    //            throw new ArgumentException($"{paramName} must be a SHA-256 hash value.", paramName);
    //    }

    //    public static void NotEmpty<T>(
    //        [NotNull] T value,
    //        [CallerArgumentExpression(nameof(value))] string paramName = "")
    //    {
    //        if (value is null)
    //            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");

    //        var underlyingDefaultValue = TypeHelper.GetUnderlyingDefaultValue<T>();

    //        if (EqualityComparer<T>.Default.Equals(value, underlyingDefaultValue))
    //            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
    //    }

    //    public static void NotEmpty<T>(
    //           [NotNull] ICollection<T> value,
    //           [CallerArgumentExpression(nameof(value))] string paramName = "")
    //    {
    //        if (value.Count == 0)
    //            throw new ArgumentException($"{paramName} cannot be empty.", paramName);
    //    }

    //    public static void MustBeEmpty<T>(T value,
    //        [CallerArgumentExpression(nameof(value))] string paramName = "")
    //    {
    //        if (value is null)
    //            return;

    //        if (!EqualityComparer<T>.Default.Equals(value, default))
    //            throw new ArgumentException($"{paramName} must be empty.", paramName);
    //    }

    //    public static void MustBeEmpty<T>(
    //        [NotNull] ICollection<T> value,
    //        [CallerArgumentExpression(nameof(value))] string paramName = "")
    //    {
    //        if (value is null)
    //            throw new ArgumentNullException(paramName, $"{paramName} cannot be null.");

    //        if (value.Count > 0)
    //            throw new ArgumentException($"{paramName} must be empty.", paramName);
    //    }

    //    public static void EnumNotDefined<TEnum>(
    //        [NotNull] TEnum value,
    //        [CallerArgumentExpression(nameof(value))] string paramName = "")
    //        where TEnum : struct, Enum
    //    {
    //        if (!EnumUtils.IsDefined(value))
    //        {
    //            throw new ArgumentException(
    //                $"Parameter '{paramName}' with value '{value}' is not defined into enum '{typeof(TEnum).Name}'. Please provide a valid enum value.",
    //                paramName);
    //        }
    //    }
    //}
}
