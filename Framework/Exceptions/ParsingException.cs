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

namespace EdiFabric.Framework.Exceptions
{
    /// <summary>
    /// General parsing exceptions.
    /// </summary>
    public class ParsingException : Exception
    {
        /// <summary>
        /// The error code.
        /// </summary>
        public ErrorCodes ErrorCode { get; private set; }

        /// <summary>
        /// The line that failed.
        /// </summary>
        public string FailedLine { get; private set; }

        /// <summary>
        /// The parsing details.
        /// </summary>
        public ErrorContext ErrorContext { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <param name="failedLine">The line that failed.</param>
        /// <param name="context">The error context.</param>
        public ParsingException(ErrorCodes errorCode, string message, string failedLine = null, ErrorContext context = null)
            : base(message)
        {
            ErrorCode = errorCode;
            FailedLine = failedLine;
            ErrorContext = context;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ParsingException"/> class.
        /// </summary>
        /// <param name="errorCode">The error code.</param>
        /// <param name="message">The error message.</param>
        /// <param name="ex">The inner exception.</param>
        /// <param name="failedLine">The line that failed.</param>
        /// <param name="context">The error context.</param>
        public ParsingException(ErrorCodes errorCode, string message, Exception ex, string failedLine = null, ErrorContext context = null)
            : base(message, ex)
        {
            ErrorCode = errorCode;
            FailedLine = failedLine;
            ErrorContext = context;
        }
    }
}
