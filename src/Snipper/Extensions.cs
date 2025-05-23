﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Snipper;

/// <summary>
/// Helper extension methods.
/// </summary>
internal static class Extensions
{
    /// <summary>
    /// Throws <see cref="ArgumentNullException"/> when <paramref name="value"/> is <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <paramref name="value"/>
    /// </typeparam>
    /// <param name="value">
    /// The value to check for <see langword="null"/>.
    /// </param>
    /// <param name="paramName">
    /// The name of <paramref name="value"/> in the caller scope, or <see langword="null"/> to use the expression in the
    /// caller scope.
    /// </param>
    /// <returns>
    /// <paramref name="value"/>, if <paramref name="value"/> is not <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    public static T ThrowIfNull<T>(
        this T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(value, paramName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="value"/> is empty.
    /// </summary>
    /// <param name="value">
    /// The value to check.
    /// </param>
    /// <param name="paramName">
    /// The name of <paramref name="value"/> in the caller scope, or <see langword="null"/> to use the expression in the
    /// caller scope.
    /// </param>
    /// <returns>
    /// <paramref name="value"/>, if <paramref name="value"/> is not empty.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="value"/> is empty.
    /// </exception>
    [return: NotNullIfNotNull(nameof(value))]
    public static string? ThrowIfEmpty(
        this string? value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
    {
        if (value is null)
        {
            return null;
        }

        ArgumentException.ThrowIfNullOrEmpty(value, paramName);
        return value;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> when <paramref name="enumerable"/> contains <see langword="null"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of <paramref name="enumerable"/>.
    /// </typeparam>
    /// <typeparam name="U">
    /// The type of item contained by <paramref name="enumerable"/>.
    /// </typeparam>
    /// <param name="enumerable">
    /// The enumerable to check.
    /// </param>
    /// <param name="paramName">
    /// The name of <paramref name="enumerable"/> in the caller scope, or <see langword="null"/> to use the expression
    /// in the caller scope.
    /// </param>
    /// <returns>
    /// <paramref name="enumerable"/>, if <paramref name="enumerable"/> does not contain <see langword="null"/>.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="enumerable"/> contains <see langword="null"/>.
    /// </exception>
    [return: NotNullIfNotNull(nameof(enumerable))]
    public static T? ThrowIfContainsNull<T, U>(
        this T? enumerable,
        [CallerArgumentExpression(nameof(enumerable))] string? paramName = null)
        where T : IEnumerable<U>
        where U : class
    {
        if (enumerable is null)
        {
            return enumerable;
        }

        if (enumerable.Any(x => x is null))
        {
            throw new ArgumentException(
                "The specified enumerable contains a null item.",
                paramName);
        }

        return enumerable;
    }

    /// <summary>
    /// Throws <see cref="ArgumentException"/> if <paramref name="value"/> is not a value defined in
    /// <typeparamref name="T"/>.
    /// </summary>
    /// <typeparam name="T">
    /// The type of enumeration that controls the set of defined values.
    /// </typeparam>
    /// <param name="value">
    /// The value to check.
    /// </param>
    /// <param name="paramName">
    /// The name of <paramref name="value"/> in the caller scope, or <see langword="null"/> to use the expression in the
    /// caller scope.
    /// </param>
    /// <returns>
    /// <paramref name="value"/>, if <paramref name="value"/> is a value defined in <typeparamref name="T"/>; otherwise,
    /// does not return.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="value"/> is not a value defined in <typeparamref name="T"/>.
    /// </exception>
    public static T ThrowIfNotDefined<T>(
        this T value,
        [CallerArgumentExpression(nameof(value))] string? paramName = null)
        where T : struct, Enum
    {
        if (!Enum.IsDefined<T>(value))
        {
            throw new ArgumentException(
                $"The specified value is not a defined enumeration value. Value: {value:D}, Expected Type: {typeof(T)}",
                paramName);
        }

        return value;
    }

    /// <inheritdoc cref="ThrowIfContainsNull{T, U}(T, string?)"/>
    public static IEnumerable<U> ThrowIfContainsNull<U>(
        this IEnumerable<U> enumerable,
        [CallerArgumentExpression(nameof(enumerable))] string? paramName = null)
        where U : class
    {
        return Extensions.ThrowIfContainsNull<IEnumerable<U>, U>(enumerable, paramName);
    }

    /// <inheritdoc cref="ThrowIfContainsNull{T, U}(T, string?)"/>
    public static IReadOnlyCollection<U> ThrowIfContainsNull<U>(
        this IReadOnlyCollection<U> enumerable,
        [CallerArgumentExpression(nameof(enumerable))] string? paramName = null)
        where U : class
    {
        return Extensions.ThrowIfContainsNull<IReadOnlyCollection<U>, U>(enumerable, paramName);
    }

    /// <inheritdoc cref="ThrowIfContainsNull{T, U}(T, string?)"/>
    public static IReadOnlyList<U> ThrowIfContainsNull<U>(
        this IReadOnlyList<U> enumerable,
        [CallerArgumentExpression(nameof(enumerable))] string? paramName = null)
        where U : class
    {
        return Extensions.ThrowIfContainsNull<IReadOnlyList<U>, U>(enumerable, paramName);
    }

    /// <inheritdoc cref="ThrowIfContainsNull{T, U}(T, string?)"/>
    public static IReadOnlySet<U> ThrowIfContainsNull<U>(
        this IReadOnlySet<U> enumerable,
        [CallerArgumentExpression(nameof(enumerable))] string? paramName = null)
        where U : class
    {
        return Extensions.ThrowIfContainsNull<IReadOnlySet<U>, U>(enumerable, paramName);
    }
}