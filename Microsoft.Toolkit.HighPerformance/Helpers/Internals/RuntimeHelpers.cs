﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#pragma warning disable SA1512

// The portable implementation in this type is originally from CoreFX.
// See https://github.com/dotnet/corefx/blob/release/2.1/src/System.Memory/src/System/SpanHelpers.cs.

using System;
using System.Diagnostics.Contracts;
#if !SPAN_RUNTIME_SUPPORT
using System.Reflection;
#endif
using System.Runtime.CompilerServices;
using Microsoft.Toolkit.HighPerformance.Extensions;

namespace Microsoft.Toolkit.HighPerformance.Helpers.Internals
{
    /// <summary>
    /// A helper class that act as polyfill for .NET Standard 2.0 and below.
    /// </summary>
    internal static class RuntimeHelpers
    {
        /// <summary>
        /// Gets the byte offset to the first <typeparamref name="T"/> element in a 2D array.
        /// </summary>
        /// <typeparam name="T">The type of values in the array.</typeparam>
        /// <returns>The byte offset to the first <typeparamref name="T"/> element in a 2D array.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetArray2DDataByteOffset<T>()
        {
            return TypeInfo<T>.Array2DDataByteOffset;
        }

        /// <summary>
        /// Gets the byte offset to the first <typeparamref name="T"/> element in a 3D array.
        /// </summary>
        /// <typeparam name="T">The type of values in the array.</typeparam>
        /// <returns>The byte offset to the first <typeparamref name="T"/> element in a 3D array.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static IntPtr GetArray3DDataByteOffset<T>()
        {
            return TypeInfo<T>.Array3DDataByteOffset;
        }

#if !SPAN_RUNTIME_SUPPORT
        /// <summary>
        /// Gets a byte offset describing a portable pinnable reference. This can either be an
        /// interior pointer into some object data (described with a valid <see cref="object"/> reference
        /// and a reference to some of its data), or a raw pointer (described with a <see langword="null"/>
        /// reference to an <see cref="object"/>, and a reference that is assumed to refer to pinned data).
        /// </summary>
        /// <typeparam name="T">The type of field being referenced.</typeparam>
        /// <param name="obj">The input <see cref="object"/> hosting the target field.</param>
        /// <param name="data">A reference to a target field of type <typeparamref name="T"/> within <paramref name="obj"/>.</param>
        /// <returns>
        /// The <see cref="IntPtr"/> value representing the offset to the target field from the start of the object data
        /// for the parameter <paramref name="obj"/>, or the value of the raw pointer passed as a tracked reference.
        /// </returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe IntPtr GetObjectDataOrReferenceByteOffset<T>(object? obj, ref T data)
        {
            if (obj is null)
            {
                return (IntPtr)Unsafe.AsPointer(ref data);
            }

            return obj.DangerousGetObjectDataByteOffset(ref data);
        }

        /// <summary>
        /// Gets a reference from data describing a portable pinnable reference. This can either be an
        /// interior pointer into some object data (described with a valid <see cref="object"/> reference
        /// and a byte offset into its data), or a raw pointer (described with a <see langword="null"/>
        /// reference to an <see cref="object"/>, and a byte offset representing the value of the raw pointer).
        /// </summary>
        /// <typeparam name="T">The type of reference to retrieve.</typeparam>
        /// <param name="obj">The input <see cref="object"/> hosting the target field.</param>
        /// <param name="offset">The input byte offset for the <typeparamref name="T"/> reference to retrieve.</param>
        /// <returns>A <typeparamref name="T"/> reference matching the given parameters.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static unsafe ref T GetObjectDataAtOffsetOrPointerReference<T>(object? obj, IntPtr offset)
        {
            if (obj is null)
            {
                return ref Unsafe.AsRef<T>((void*)offset);
            }

            return ref obj.DangerousGetObjectDataReferenceAt<T>(offset);
        }

        /// <summary>
        /// Checks whether or not a given type is a reference type or contains references.
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns>Whether or not <typeparamref name="T"/> respects the <see langword="unmanaged"/> constraint.</returns>
        [Pure]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsReferenceOrContainsReferences<T>()
        {
            return TypeInfo<T>.IsReferenceOrContainsReferences;
        }

        /// <summary>
        /// Implements the logic for <see cref="IsReferenceOrContainsReferences{T}"/>.
        /// </summary>
        /// <param name="type">The current type to check.</param>
        /// <returns>Whether or not <paramref name="type"/> is a reference type or contains references.</returns>
        [Pure]
        private static bool IsReferenceOrContainsReferences(Type type)
        {
            // Common case, for primitive types
            if (type.GetTypeInfo().IsPrimitive)
            {
                return false;
            }

            if (!type.GetTypeInfo().IsValueType)
            {
                return true;
            }

            // Check if the type is Nullable<T>
            if (Nullable.GetUnderlyingType(type) is Type nullableType)
            {
                type = nullableType;
            }

            if (type.GetTypeInfo().IsEnum)
            {
                return false;
            }

            // Complex struct, recursively inspect all fields
            foreach (FieldInfo field in type.GetTypeInfo().DeclaredFields)
            {
                if (field.IsStatic)
                {
                    continue;
                }

                if (IsReferenceOrContainsReferences(field.FieldType))
                {
                    return true;
                }
            }

            return false;
        }
#endif

        /// <summary>
        /// A private generic class to preload type info for arbitrary runtime types.
        /// </summary>
        /// <typeparam name="T">The type to load info for.</typeparam>
        private static class TypeInfo<T>
        {
            /// <summary>
            /// The byte offset to the first <typeparamref name="T"/> element in a 2D array.
            /// </summary>
            public static readonly IntPtr Array2DDataByteOffset = MeasureArray2DDataByteOffset();

            /// <summary>
            /// The byte offset to the first <typeparamref name="T"/> element in a 3D array.
            /// </summary>
            public static readonly IntPtr Array3DDataByteOffset = MeasureArray3DDataByteOffset();

#if !SPAN_RUNTIME_SUPPORT
            /// <summary>
            /// Indicates whether <typeparamref name="T"/> does not respect the <see langword="unmanaged"/> constraint.
            /// </summary>
            public static readonly bool IsReferenceOrContainsReferences = IsReferenceOrContainsReferences(typeof(T));
#endif

            /// <summary>
            /// Computes the value for <see cref="Array2DDataByteOffset"/>.
            /// </summary>
            /// <returns>The value of <see cref="Array2DDataByteOffset"/> for the current runtime.</returns>
            [Pure]
            private static IntPtr MeasureArray2DDataByteOffset()
            {
                var array = new T[1, 1];

                return array.DangerousGetObjectDataByteOffset(ref array[0, 0]);
            }

            /// <summary>
            /// Computes the value for <see cref="Array3DDataByteOffset"/>.
            /// </summary>
            /// <returns>The value of <see cref="Array3DDataByteOffset"/> for the current runtime.</returns>
            [Pure]
            private static IntPtr MeasureArray3DDataByteOffset()
            {
                var array = new T[1, 1, 1];

                return array.DangerousGetObjectDataByteOffset(ref array[0, 0, 0]);
            }
        }
    }
}