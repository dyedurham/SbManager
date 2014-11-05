using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Json;

namespace SbManager
{
    public class CustomLogJsonFormatter : ITextFormatter
    {
        private readonly bool _appendNewline;
        private readonly bool _omitEnclosingObject;
        private readonly JsonFormatter _baseJsonFormatter;

        public CustomLogJsonFormatter(bool appendNewline = true, bool omitEnclosingObject = false)
        {
            _appendNewline = appendNewline;
            _omitEnclosingObject = omitEnclosingObject;
            _baseJsonFormatter = new JsonFormatter(omitEnclosingObject: omitEnclosingObject);
        }

        public void Format(LogEvent logEvent, TextWriter output)
        {
            FormatInternal(logEvent, output);
            if (_appendNewline) output.Write("\r\n");
        }

        private void FormatInternal(LogEvent logEvent, TextWriter output)
        {
            if (logEvent == null)
                throw new ArgumentNullException("logEvent");
            if (output == null)
                throw new ArgumentNullException("output");
            if (!_omitEnclosingObject)
                output.Write("{");
            var precedingDelimiter1 = "";
            WriteJsonProperty("Timestamp", logEvent.Timestamp, ref precedingDelimiter1, output);
            WriteJsonProperty("Level", logEvent.Level, ref precedingDelimiter1, output);
            WriteJsonProperty("Message", logEvent.RenderMessage(), ref precedingDelimiter1, output);
            WriteJsonProperty("MessageTemplate", logEvent.MessageTemplate.Text, ref precedingDelimiter1, output);
            if (logEvent.Exception != null)
                WriteJsonProperty("Exception", logEvent.Exception, ref precedingDelimiter1, output);
            if (logEvent.Properties.Count != 0)
            {
                output.Write(",\"Properties\":{");
                var precedingDelimiter2 = "";
                foreach (KeyValuePair<string, LogEventPropertyValue> keyValuePair in logEvent.Properties)
                    WriteJsonProperty(keyValuePair.Key, keyValuePair.Value, ref precedingDelimiter2, output);
                output.Write("}");
            }
            if (_omitEnclosingObject)
                return;
            output.Write("}");
        }

        private void WriteJsonProperty(string name, object value, ref string precedingDelimiter, TextWriter output)
        {
            output.Write(precedingDelimiter);
            output.Write("\"");
            output.Write(name);
            output.Write("\":");
            WriteLiteral(value, output);
            precedingDelimiter = ",";
        }

        private void WriteLiteral(object value, TextWriter output, bool forceQuotation = false)
        {
            CallPrivate(_baseJsonFormatter, "WriteLiteral", value, output, forceQuotation);
        }

        public static void CallPrivate(object o, string method, params object[] args)
        {
            var dynMethod = o.GetType().GetMethod(method, BindingFlags.NonPublic | BindingFlags.Instance);
            dynMethod.Invoke(o, args);
        }
    }
}
