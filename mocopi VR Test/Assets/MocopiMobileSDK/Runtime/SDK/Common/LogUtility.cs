/*
 * Copyright 2022 Sony Corporation
 */
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Mocopi.Mobile.Sdk.Common
{
    /// <summary>
    /// For output Log
    /// </summary>
    public class LogUtility
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        /// <summary>
        /// Output Log Level(on static)
        /// </summary>
        private static readonly LogLevel StaticLogLevel = LogLevel.Debug;

        /// <summary>
        /// Output Log Level
        /// </summary>
        private readonly LogLevel logLevel = LogLevel.Debug;
#else
        /// <summary>
        /// Output Log Level(on static)
        /// </summary>
        private static readonly  LogLevel StaticLogLevel = LogLevel.Info;

        /// <summary>
        /// Output Log Level
        /// </summary>
        private readonly LogLevel logLevel = LogLevel.Info;
#endif
        /// <summary>
        /// Display header string on log
        /// </summary>
        private readonly string moduleName;

        /// <summary>
        /// Display stack trace on log
        /// </summary>
        private readonly StackTrace stackTrace = StackTrace.Default;

        /// <summary>
        /// Display stack trace on log
        /// </summary>
        private readonly Dictionary<LogLevel, LogOption> stackTraceDictionary = new Dictionary<LogLevel, LogOption>()
        {
            { LogLevel.Debug   , LogOption.NoStacktrace },
            { LogLevel.Info    , LogOption.NoStacktrace },
            { LogLevel.Warning , LogOption.NoStacktrace },
            { LogLevel.Error   , LogOption.None },
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="LogUtility" /> class.
        /// </summary>
        /// <param name="moduleName">Module name</param>
        public LogUtility(string moduleName)
        {
            if (moduleName != "")
            {
                moduleName = $"[{moduleName}]";
            }

            this.moduleName = moduleName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogUtility" /> class.
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <param name="logLevel">Log level</param>
        public LogUtility(string moduleName, LogLevel logLevel) : this(moduleName)
        {
            this.logLevel = logLevel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogUtility" /> class.
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <param name="logLevel">Log level</param>
        /// <param name="stackTrace">Stack Trace Setting</param>
        public LogUtility(string moduleName, LogLevel logLevel, StackTrace stackTrace) : this(moduleName)
        {
            this.logLevel = logLevel;
            this.stackTrace = stackTrace;
        }

        /// <summary>
        /// Enum: Log level
        /// </summary>
        public enum LogLevel
        {
            NoTest = 0,
            Debug = 10,
            Info = 20,
            Warning = 30,
            Error = 40,
        }

        /// <summary>
        /// Enum: StackTrace Setting
        /// </summary>
        public enum StackTrace
        {
            Default,
            FullTrace,
            NoTrace,
        }

        /// <summary>
        /// Output log: [Debug]
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <param name="tag">Tag name</param>
        /// <param name="message">log message</param>
        public static void Debug(string moduleName, string tag, string message)
        {
            Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.ScriptOnly);

            if (StaticLogLevel > LogLevel.Debug) return;
            
            message = TrimMessage(message);
            UnityEngine.Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null,  message);
        }

        /// <summary>
        /// Output log: [Infomation]
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <param name="tag">Tag name</param>
        /// <param name="message">log message</param>
        public static void Infomation(string moduleName, string tag, string message)
        {
	        if (StaticLogLevel > LogLevel.Info) return;
	        
	        message = TrimMessage(message);
	        UnityEngine.Debug.LogFormat(LogType.Log, LogOption.NoStacktrace, null,  $"[{moduleName}][{tag}]{message}");
        }

        /// <summary>
        /// Output log: [Warning]
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <param name="tag">Tag name</param>
        /// <param name="message">log message</param>
        public static void Warning(string moduleName, string tag, string message)
        {
	        if (StaticLogLevel > LogLevel.Warning) return;
	        
	        message = TrimMessage(message);
	        UnityEngine.Debug.LogFormat(LogType.Warning, LogOption.NoStacktrace, null,  $"[{moduleName}][{tag}]{message}");
        }

        /// <summary>
        /// Output log: [Error]
        /// </summary>
        /// <param name="moduleName">Module name</param>
        /// <param name="tag">Tag name</param>
        /// <param name="message">log message</param>
        public static void Error(string moduleName, string tag, string message)
        {
	        if (StaticLogLevel > LogLevel.Error) return;
	        
	        message = TrimMessage(message);
	        UnityEngine.Debug.LogFormat(LogType.Error, LogOption.None, null,  $"[{moduleName}][{tag}]{message}");
        }

        /// <summary>
        /// Get calling class name
        /// </summary>
        /// <param name="callStage">Hierarchy caller</param>
        /// <returns>クラス名</returns>
        public static string GetClassName(int callStage = 1)
        {
            var caller = new System.Diagnostics.StackFrame(callStage, false);
            return caller.GetMethod().DeclaringType.FullName;
        }

        /// <summary>
        /// Get calling method name
        /// </summary>
        /// <param name="callStage">Hierarchy caller</param>
        /// <returns>関数名</returns>
        public static string GetMethodName(int callStage = 1)
        {
            var caller = new System.Diagnostics.StackFrame(callStage, false);
            return caller.GetMethod().Name;
        }

        /// <summary>
        /// Output log: [Debug]
        /// </summary>
        /// <param name="message">log message</param>
        public void Debug(string message)
        {
	        if (logLevel > LogLevel.Debug) return;
	        
	        message = TrimMessage(message);
	        try
	        {
		        UnityEngine.Debug.LogFormat(LogType.Log, GetStackTrace(LogLevel.Debug), null, $"[DEBUG]{moduleName}{RemoveCurlyBrace(message)}");
	        }
	        catch (FormatException ex)
	        {
		        UnityEngine.Debug.LogFormat(LogType.Error, LogOption.None, null, ex.StackTrace);
	        }
        }

        /// <summary>
        /// Output log: [Infomation]
        /// </summary>
        /// <param name="message">log message</param>
        public void Infomation(string message)
        {
	        if (logLevel > LogLevel.Info) return;
	        
	        message = TrimMessage(message);
	        try
	        {
		        UnityEngine.Debug.LogFormat(LogType.Log, GetStackTrace(LogLevel.Info), null,  $"{moduleName}{RemoveCurlyBrace(message)}");
	        }
	        catch (FormatException ex)
	        {
		        UnityEngine.Debug.LogFormat(LogType.Error, LogOption.None, null, ex.StackTrace);
	        }
        }

        /// <summary>
        /// Output log: [Warning]
        /// </summary>
        /// <param name="message">log message</param>
        public void Warning(string message)
        {
	        if (logLevel > LogLevel.Warning) return;
	        
	        message = TrimMessage(message);
	        try
	        {
		        UnityEngine.Debug.LogFormat(LogType.Warning, GetStackTrace(LogLevel.Warning), null,  $"{moduleName}{RemoveCurlyBrace(message)}");
	        }
	        catch (FormatException ex)
	        {
		        UnityEngine.Debug.LogFormat(LogType.Error, LogOption.None, null, ex.StackTrace);
	        }
        }

        /// <summary>
        /// Output log: [Error]
        /// </summary>
        /// <param name="message">log message</param>
        public void Error(string message)
        {
	        if (logLevel > LogLevel.Error) return;
	        
	        message = TrimMessage(message);
	        try
	        {
		        UnityEngine.Debug.LogFormat(LogType.Error, GetStackTrace(LogLevel.Error), null,  $"{moduleName}{RemoveCurlyBrace(message)}");
	        }
	        catch (FormatException ex)
	        {
		        UnityEngine.Debug.LogFormat(LogType.Error, LogOption.None, null, ex.StackTrace);
	        }
        }

        /// <summary>
        /// Get stack trace setting
        /// </summary>
        /// <param name="logLevel">Log Level</param>
        /// <returns>stack trace setting</returns>
        private LogOption GetStackTrace(LogLevel logLevel)
        {
	        return stackTrace switch
	        {
		        StackTrace.Default => stackTraceDictionary[logLevel],
		        StackTrace.FullTrace => LogOption.None,
		        StackTrace.NoTrace => LogOption.NoStacktrace,
		        _ => LogOption.NoStacktrace
	        };
        }

        /// <summary>
        /// Remove curly brace in message
        /// </summary>
        /// <returns></returns>
        private static string RemoveCurlyBrace(string message)
        {
            return message.Replace("{", "{{").Replace("}", "}}");
        }

        /// <summary>
        /// Trimming message to formatable one
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private static string TrimMessage(string message)
        {
	        message = message.Trim();
	        
	        // "{0}"のような文字列を出力しようとするとフォーマットが破綻してしまうので、別の文字に置き換える
	        message = message.Replace("{", "[");
	        message = message.Replace("}", "]");
	        
	        return message;
        }
    }
}