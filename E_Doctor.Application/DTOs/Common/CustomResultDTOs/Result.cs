namespace E_Doctor.Application.DTOs.Common.CustomResultDTOs;
// This file defines the generic and non-generic Result types.
// It is the core of the Result pattern implementation.

/// <summary>
/// A generic Result class that can hold either a success value or a failure error.
/// </summary>
/// <typeparam name="T">The type of the success value.</typeparam>
public class Result<T>
{
    // Properties to represent the state of the result.
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }
    public T? Value { get; }

    /// <summary>
    /// Private constructor for a success result.
    /// </summary>
    /// <param name="value">The value to return on success.</param>
    private Result(T value)
    {
        IsSuccess = true;
        Value = value;
        Error = null;
    }

    /// <summary>
    /// Private constructor for a failure result.
    /// </summary>
    /// <param name="error">The error message to return on failure.</param>
    private Result(string error)
    {
        IsSuccess = false;
        Value = default; // Use default value for the type
        Error = error;
    }

    /// <summary>
    /// Creates a successful result with a value.
    /// </summary>
    /// <param name="value">The value to be wrapped in the Result.</param>
    /// <returns>A new Result instance representing a success.</returns>
    public static Result<T> Success(T value) => new Result<T>(value);

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A new Result instance representing a failure.</returns>
    public static Result<T> Failure(string error) => new Result<T>(error);
}

/// <summary>
/// A non-generic Result class for operations that do not return a value.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; }

    /// <summary>
    /// Private constructor for a success result.
    /// </summary>
    private Result()
    {
        IsSuccess = true;
        Error = null;
    }

    /// <summary>
    /// Private constructor for a failure result.
    /// </summary>
    /// <param name="error">The error message.</param>
    private Result(string error)
    {
        IsSuccess = false;
        Error = error;
    }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    /// <returns>A new Result instance representing a success.</returns>
    public static Result Success() => new Result();

    /// <summary>
    /// Creates a failed result with an error message.
    /// </summary>
    /// <param name="error">The error message.</param>
    /// <returns>A new Result instance representing a failure.</returns>
    public static Result Failure(string error) => new Result(error);
}
