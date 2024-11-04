#pragma warning disable IDE0005 // Using directive is unnecessary.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
#pragma warning restore IDE0005 // Using directive is unnecessary.

namespace TableOfRecords
{
    /// <summary>
    /// Presents method that write in table form to the text stream a set of elements of type T.
    /// </summary>
    public static class TableOfRecordsCreator
    {
        /// <summary>
        /// Write in table form to the text stream a set of elements of type T (<see cref="ICollection{T}"/>),
        /// where the state of each object of type T is described by public properties that have only build-in
        /// type (int, char, string etc.)
        /// </summary>
        /// <typeparam name="T">Type selector.</typeparam>
        /// <param name="collection">Collection of elements of type T.</param>
        /// <param name="writer">Text stream.</param>
        /// <exception cref="ArgumentNullException">Throw if <paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentNullException">Throw if <paramref name="writer"/> is null.</exception>
        /// <exception cref="ArgumentException">Throw if <paramref name="collection"/> is empty.</exception>
        public static void WriteTable<T>(ICollection<T>? collection, TextWriter? writer)
        {
            if (collection != null)
            {
                if (writer != null)
                {
                    if (collection.Count == 0)
                    {
                        throw new ArgumentException("Collection cannot be empty.", nameof(collection));
                    }

                    var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                              .Where(p => p.PropertyType.IsPrimitive || p.PropertyType == typeof(string) || p.PropertyType == typeof(DateTime))
                                              .ToArray();

                    if (properties.Length == 0)
                    {
                        return;
                    }

                    var columnWidths = new int[properties.Length];

                    foreach (var item in collection)
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            var value = properties[i].GetValue(item)?.ToString() ?? string.Empty;
                            columnWidths[i] = Math.Max(columnWidths[i], value.Length);
                        }
                    }

                    // Write header
                    for (int i = 0; i < properties.Length; i++)
                    {
                        var header = properties[i].Name;
                        columnWidths[i] = Math.Max(columnWidths[i], header.Length);
                        writer.Write(header.PadRight(columnWidths[i] + 2));
                    }

                    writer.WriteLine();

                    // Write separator
                    for (int i = 0; i < properties.Length; i++)
                    {
                        writer.Write(new string('-', columnWidths[i] + 2));
                    }

                    writer.WriteLine();

                    // Write rows
                    foreach (var item in collection)
                    {
                        for (int i = 0; i < properties.Length; i++)
                        {
                            var value = properties[i].GetValue(item)?.ToString() ?? string.Empty;
                            if (properties[i].PropertyType == typeof(int) || properties[i].PropertyType == typeof(double) || properties[i].PropertyType == typeof(float) || properties[i].PropertyType == typeof(decimal) || properties[i].PropertyType == typeof(DateTime))
                            {
                                writer.Write(value.PadLeft(columnWidths[i] + 2));
                            }
                            else
                            {
                                writer.Write(value.PadRight(columnWidths[i] + 2));
                            }
                        }

                        writer.WriteLine();
                    }
                }
                else
                {
                    throw new ArgumentNullException(nameof(writer));
                }
            }
            else
            {
                throw new ArgumentNullException(nameof(collection));
            }
        }
    }
}
