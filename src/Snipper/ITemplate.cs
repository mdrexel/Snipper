using System;
using System.Collections.Generic;

namespace Snipper;

/// <summary>
/// Represents a template that maps a set of input files to a set of output files.
/// </summary>
public interface ITemplate
{
    /// <summary>
    /// Initializes a new instance of this template type.
    /// </summary>
    /// <returns>
    /// A new instance of this template type.
    /// </returns>
    static abstract ITemplate Create();

    bool CanReceive(IReadOnlyList<string> files);
}