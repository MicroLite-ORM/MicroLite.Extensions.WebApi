// -----------------------------------------------------------------------
// <copyright file="ODataRawQueryOptions.cs" company="MicroLite">
// Copyright 2012 Trevor Pilley
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//    http://www.apache.org/licenses/LICENSE-2.0
//
// </copyright>
// -----------------------------------------------------------------------
namespace MicroLite.Extensions.WebApi.Query
{
    using System;

    /// <summary>
    /// A class which contains the raw request values.
    /// </summary>
    public sealed class ODataRawQueryOptions
    {
        /// <summary>
        /// Initialises a new instance of the <see cref="ODataRawQueryOptions"/> class.
        /// </summary>
        /// <param name="rawQuery">The raw query.</param>
        public ODataRawQueryOptions(string rawQuery)
        {
            var pieces = rawQuery.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var piece in pieces)
            {
                if (piece.StartsWith("$expand=", StringComparison.OrdinalIgnoreCase))
                {
                    this.Expand = piece;
                }
                else if (piece.StartsWith("$filter=", StringComparison.OrdinalIgnoreCase))
                {
                    this.Filter = piece;
                }
                else if (piece.StartsWith("$format=", StringComparison.OrdinalIgnoreCase))
                {
                    this.Format = piece;
                }
                else if (piece.StartsWith("$inlineCount=", StringComparison.OrdinalIgnoreCase))
                {
                    this.InlineCount = piece;
                }
                else if (piece.StartsWith("$orderby=", StringComparison.OrdinalIgnoreCase))
                {
                    this.OrderBy = piece;
                }
                else if (piece.StartsWith("$select=", StringComparison.OrdinalIgnoreCase))
                {
                    this.Select = piece;
                }
                else if (piece.StartsWith("$skip=", StringComparison.OrdinalIgnoreCase))
                {
                    this.Skip = piece;
                }
                else if (piece.StartsWith("$skiptoken=", StringComparison.OrdinalIgnoreCase))
                {
                    this.SkipToken = piece;
                }
                else if (piece.StartsWith("$top=", StringComparison.OrdinalIgnoreCase))
                {
                    this.Top = piece;
                }
            }
        }

        /// <summary>
        /// Gets the raw $expand query value from the incoming request Uri if specified.
        /// </summary>
        public string Expand
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the raw $filter query value from the incoming request Uri if specified.
        /// </summary>
        public string Filter
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the raw $format query value from the incoming request Uri if specified.
        /// </summary>
        public string Format
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the raw $inlineCount query value from the incoming request Uri if specified.
        /// </summary>
        public string InlineCount
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the raw $order by query value from the incoming request Uri if specified.
        /// </summary>
        public string OrderBy
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the raw $select query value from the incoming request Uri if specified.
        /// </summary>
        public string Select
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the raw $skip query value from the incoming request Uri if specified.
        /// </summary>
        public string Skip
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the raw $skip token query value from the incoming request Uri if specified.
        /// </summary>
        public string SkipToken
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the raw $top query value from the incoming request Uri if specified.
        /// </summary>
        public string Top
        {
            get;
            private set;
        }
    }
}