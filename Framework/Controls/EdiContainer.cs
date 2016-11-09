﻿//---------------------------------------------------------------------
// This file is part of ediFabric
//
// Copyright (c) ediFabric. All rights reserved.
//
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY
// KIND, WHETHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
// IMPLIED WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR
// PURPOSE.
//---------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using EdiFabric.Framework.Parsing;

namespace EdiFabric.Framework.Controls
{
    /// <summary>
    /// This class represents an EDI interchange or group
    /// </summary>
    /// <typeparam name="T">The header type.</typeparam>
    /// <typeparam name="TU">The items type - can be group or message.</typeparam>
    /// <typeparam name="TV">The trailer type.</typeparam>
    public class EdiContainer<T, TU, TV> 
    {
        /// <summary>
        /// The header 
        /// </summary>
        public T Header { get; private set; }
        private readonly Func<T, int, TV> _trailerSetter;
        private readonly List<TU> _items = new List<TU>();
        private readonly Separators _defaultSeparators;
        /// <summary>
        /// The items (groups or messages)
        /// </summary>
        public IReadOnlyCollection<TU> Items
        {
            get { return _items.AsReadOnly(); }
        }
        /// <summary>
        /// The trailer 
        /// </summary>
        public TV Trailer { get; private set; }

        /// <summary>
        /// Protected constructor. 
        /// </summary>
        /// <param name="header">The header type.</param>
        /// <param name="trailerSetter">The function to set the trailer.</param>
        /// <param name="defaultSeparators">The default separators.</param>
        protected EdiContainer(T header, Func<T, int, TV> trailerSetter, Separators defaultSeparators)
        {
            if (trailerSetter == null) throw new ArgumentNullException("trailerSetter");
            if (defaultSeparators == null) throw new ArgumentNullException("defaultSeparators");

            Header = header;
            _trailerSetter = trailerSetter;
            _defaultSeparators = defaultSeparators;
        }

        /// <summary>
        /// Adds an item.
        /// </summary>
        /// <param name="item">The item to add.</param>
        public void AddItem(TU item)
        {
            if (item == null) throw new ArgumentNullException("item");

            _items.Add(item);
            if (Header != null)
                Trailer = _trailerSetter(Header, _items.Count);
        }

        /// <summary>
        /// Adds a collection of items.
        /// </summary>
        /// <param name="items">The items to add.</param>
        public void AddItems(IEnumerable<TU> items)
        {
            if (items == null) throw new ArgumentNullException("items");

            _items.AddRange(items);
            if (Header != null)
                Trailer = _trailerSetter(Header, _items.Count);
        }

        /// <summary>
        /// Generates a collection of EDI segments.
        /// </summary>
        /// <param name="separators">The EDI separators.</param>
        /// <returns>The collection of EDI segments.</returns>
        public virtual IEnumerable<string> GenerateEdi(Separators separators = null)
        {
            var result = new List<string>();
            var currentSeparators = separators ?? _defaultSeparators;

            if (Header != null)
                result.AddRange(ToEdi(Header, currentSeparators));
            foreach (var item in Items)
            {
                result.AddRange(ToEdi(item, currentSeparators, true));
            }
            if (Trailer != null)
                result.AddRange(ToEdi(Trailer, currentSeparators));

            return result;
        }

        /// <summary>
        /// Converts a message to a collection of EDI segments.
        /// </summary>
        /// <param name="item">The message.</param>
        /// <param name="separators">The EDI separators.</param>
        /// <param name="isMessage">If it is a message item.</param>
        /// <returns>The collection of EDI segments.</returns>
        protected static IEnumerable<string> ToEdi(object item, Separators separators, bool isMessage = false)
        {
            var parseTree = ParseNode.BuldTree(item);
            var segments = parseTree.Descendants().Where(d => d.Prefix == Prefixes.S).Reverse().ToList();

            if (!isMessage) return segments.Select(segment => segment.GenerateSegment(separators));

            var segmentsNumber = segments.Count().ToString();
            var controlNumber = segments.MessageControlNumber();

            return
                segments.Select(
                    segment =>
                        ApplyMessageTrailer(segment.GenerateSegment(separators), controlNumber, segmentsNumber,
                            separators));
        }

        private static string ApplyMessageTrailer(string segment, string controlNumber, string segmentsCount, Separators separators)
        {
            string tag = null;
            if (segment.StartsWith("UNT" + separators.DataElement, StringComparison.Ordinal))
            {
                tag = "UNT";
            }

            if (segment.StartsWith("SE" + separators.DataElement, StringComparison.Ordinal))
            {
                tag = "SE";
            }

            if (!string.IsNullOrEmpty(tag))
                return tag + separators.DataElement + segmentsCount + separators.DataElement + controlNumber +
                       separators.Segment;

            return segment;
        }

        
    }
}

