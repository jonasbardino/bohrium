﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NumCIL
{
    /// <summary>
    /// Defines the shape of a multidimensional array
    /// </summary>
    public class Shape
    {
        /// <summary>
        /// A structure that describes the shape of a single dimension
        /// </summary>
        public struct ShapeDimension
        {
            /// <summary>
            /// The number of elements in the dimension, measured in elements of the underlying dimension
            /// </summary>
            public readonly long Length;
            /// <summary>
            /// The stride, that is length of an element, in this dimension, measured in elements of the base array
            /// </summary>
            public readonly long Stride;

            /// <summary>
            /// Initializes a new instance of the <see cref="ShapeDimension"/> struct.
            /// </summary>
            /// <param name="length">The number of elements in the dimension</param>
            /// <param name="stride">The stride</param>
            public ShapeDimension(long length, long stride)
            {
                this.Length = length;
                this.Stride = stride;
            }
        }

        #region Shape definition
        /// <summary>
        /// The offset in each dimension, in elements of the previous dimension
        /// </summary>
        public long Offset;
        /// <summary>
        /// A list of shapes for each dimension
        /// </summary>
        private ShapeDimension[] m_dimensions;
        #endregion

        #region Cached values
        /// <summary>
        /// The required minimum size of the underlying array, in array elements
        /// </summary>
        public readonly long Length;
        #endregion

        /// <summary>
        /// Constructs a one dimensional shape
        /// </summary>
        /// <param name="length">The size of the array</param>
        public Shape(long length)
            : this(new long[] { length }, 0, null)
        {
        }

        /// <summary>
        /// Constructs an N-dimensional array
        /// </summary>
        /// <param name="lengths">The number of elements in each dimension</param>
        /// <param name="offset">The offset in elements of the base array where data starts</param>
        /// <param name="strides">The number of elements in each dimension, can be null</param>
        public Shape(long[] lengths, long offset = 0, long[] strides = null)
        {
            if (lengths == null)
                throw new ArgumentNullException("dimensionsizes");
            if (lengths.LongLength <= 0 || lengths.Any(x => x <= 0))
                throw new ArgumentOutOfRangeException("dimensionsizes");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");

            if (strides == null)
            {
                strides = new long[lengths.LongLength];
                strides[lengths.LongLength - 1] = 1;

                for (long i = strides.LongLength - 2; i >= 0; i--)
                    strides[i] = strides[i + 1] * lengths[i + 1];
            }
            else if (lengths.LongLength != strides.LongLength || strides.Any(x => x < 0))
                throw new ArgumentException("strides");

            this.Offset = offset;

            m_dimensions = new ShapeDimension[lengths.LongLength];
            for (long i = lengths.LongLength - 1; i >= 0; i--)
                m_dimensions[i] = new ShapeDimension(lengths[i], strides[i]);

            //Calculate how much space is required
            Length = CalculateSpaceRequired() + this.Offset;
        }

        /// <summary>
        /// Constructs an N-dimensional array
        /// </summary>
        /// <param name="dimensions">The length and stride for each dimension</param>
        /// <param name="offset">The offset in elements of the base array where data starts</param>
        public Shape(ShapeDimension[] dimensions, long offset = 0)
        {
            if (dimensions == null)
                throw new ArgumentNullException("dimensions");
            if (dimensions.LongLength <= 0 || dimensions.Any(x => x.Length <= 0 || x.Stride < 0))
                throw new ArgumentOutOfRangeException("dimensions");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");

            this.Offset = offset;
            m_dimensions = dimensions;
            Length = CalculateSpaceRequired() + this.Offset;
        }

        /// <summary>
        /// Calculates the number of elements that are required to be present in the underlying array
        /// </summary>
        /// <returns>The number of elements required</returns>
        private long CalculateSpaceRequired()
        {
            long length = 1;
            for (long i = 0; i < m_dimensions.LongLength; i++)
                length += (m_dimensions[i].Length - 1) * m_dimensions[i].Stride;

            return length;
        }

        /// <summary>
        /// Gets the shape of each dimension
        /// </summary>
        public ShapeDimension[] Dimensions { get { return m_dimensions; } }

        /// <summary>
        /// Gets the offset into the underlying array
        /// </summary>
        /// <param name="index">The multidimensional indices</param>
        /// <returns>The offset into the underlying array</returns>
        public long this[params long[] index]
        {
            get
            {
                if (index == null || index.LongLength == 0)
                    return this.Offset;
                if (index.LongLength > m_dimensions.LongLength || index.Any(x => x < 0))
                    throw new ArgumentOutOfRangeException("index");

                long p = 0;
                for (long i = 0; i < index.LongLength; i++)
                {
                    if (index[i] >= m_dimensions[i].Length)
                        throw new ArgumentOutOfRangeException("index");
                    p += (index[i] * m_dimensions[i].Stride);
                }

                return p + this.Offset;
            }
        }

        /// <summary>
        /// Generates shapes that are broadcast compatible based on their current shapes
        /// </summary>
        /// <param name="self">A shape.</param>
        /// <param name="other">Another shape.</param>
        /// <returns>A tupple with shapes are broadcast-compatible with respect to the original shapes and the combined shapes</returns>
        public static Tuple<Shape, Shape> ToBroadcastShapes(Shape self, Shape other)
        {
            long selfDims = self.Dimensions.LongLength;
            long otherDims = other.Dimensions.LongLength;

            long sharedDims = Math.Min(selfDims, otherDims);
            long resultDims = Math.Max(selfDims, otherDims);

            long[] resultDimensions = new long[resultDims];

            for (long i = 0; i < sharedDims; i++)
            {
                long sizeSelf = self.Dimensions[selfDims - i - 1].Length;
                long sizeOther = other.Dimensions[otherDims - i - 1].Length;

                if (sizeSelf == sizeOther)
                    resultDimensions[resultDims - i - 1] = sizeSelf;
                else
                {
                    if (sizeSelf != sizeOther && sizeOther != 1 && sizeSelf != 1)
                        throw new Exception("Dimension sizes do not match for broadcast");
                    resultDimensions[resultDims - i - 1] = Math.Max(sizeSelf, sizeOther);
                }
            }

            Shape largest = selfDims > otherDims ? self : other;

            for (long i = sharedDims; i < resultDims; i++)
                resultDimensions[resultDims - i - 1] = largest.Dimensions[resultDims - i - 1].Length;

            long[] selfStrides = new long[resultDims];
            long[] otherStrides = new long[resultDims];

            long diffSelf = resultDims - selfDims;
            long diffOther = resultDims - otherDims;

            //Now the shape has broadcast-able dimension size, so fix up the strides to go with it
            for (long i = 0; i < resultDims; i++)
            {
                if (i - diffSelf >= 0)
                {
                    if (self.Dimensions[i - diffSelf].Length == 1)
                        selfStrides[i] = 0;
                    else
                        selfStrides[i] = (resultDimensions[i] / self.Dimensions[i - diffSelf].Length) * self.Dimensions[i - diffSelf].Stride;
                }
                else
                    selfStrides[i] = 0;

                if (i - diffOther >= 0)
                {
                    if (other.Dimensions[i - diffOther].Length == 1)
                        otherStrides[i] = 0;
                    else
                        otherStrides[i] = (resultDimensions[i] / other.Dimensions[i - diffOther].Length) * other.Dimensions[i - diffOther].Stride;
                }
                else
                    otherStrides[i] = 0;
            }

            return new Tuple<Shape, Shape>(
                new Shape(resultDimensions, self.Offset, selfStrides),
                new Shape(resultDimensions, other.Offset, otherStrides)
            );
        }

        public static implicit operator Shape(long[] data) { return new Shape(data); }

        public override bool Equals(object obj)
        {
            Shape other = obj as Shape;
            if (other == null)
                return false;

            if (other.Offset != this.Offset || other.Length != this.Length || other.Dimensions.LongLength != this.Dimensions.LongLength)
                return false;

            for (long i = 0; i < this.Dimensions.LongLength; i++)
                if (other.Dimensions[i].Length != this.Dimensions[i].Length || other.Dimensions[i].Stride != this.Dimensions[i].Stride)
                    return false;

            return true;
        }

        public override int GetHashCode()
        {
            long code = Offset;
            foreach (var x in m_dimensions)
                code ^= x.Length ^ x.Stride;
            return (int)((code >> 32) | (code & 0xffffffff));
        }
    }
}