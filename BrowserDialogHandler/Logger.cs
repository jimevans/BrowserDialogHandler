// <copyright file="Logger.cs" company="BrowserDialogHandler Project">
// Copyright 2013 Jim Evans
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BrowserDialogHandler.Loggers;

namespace BrowserDialogHandler
{
    /// <summary>
    /// Provides the services required to log messages.
    /// </summary>
    public static class Logger
    {
        private static BaseLogger logger = new NullLogger(LogLevel.Debug);

        /// <summary>
        /// Sets the active logger
        /// </summary>
        /// <param name="logger">The logger to be the active logger</param>
        public static void SetActiveLogger(BaseLogger logger)
        {
            Logger.logger = logger;
        }

        /// <summary>
        /// Writes an informational message to the log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        public static void LogInfo(string message)
        {
            logger.Log(message, LogLevel.Info);
        }

        /// <summary>
        /// Writes a debug message to the log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        public static void LogDebug(string message)
        {
            logger.Log(message, LogLevel.Debug);
        }

        /// <summary>
        /// Writes an action message to the log.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        public static void LogAction(string message)
        {
            LogInfo(message);
        }
    }
}
