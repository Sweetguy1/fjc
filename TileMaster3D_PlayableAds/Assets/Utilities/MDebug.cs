using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Newtonsoft.Json;
using UnityEngine;

// ReSharper disable once CheckNamespace
public static class MDebug
{
    public enum Color
    {
        green,
        black,
        blue,
        cyan,
        white,
        yellow,
        red
    }

    private static readonly Stopwatch ms_stopwatch = new Stopwatch();

    public static void WatchStart()
    {
        ms_stopwatch.Start();
    }

    public static void WatchRStart()
    {
        ms_stopwatch.Restart();
    }

    public static long WatchEnd()
    {
        ms_stopwatch.Stop();
        return ms_stopwatch.ElapsedMilliseconds;
    }

    private static string Message(object message, Color color = Color.black, bool bold = false, bool italic = false)
    {
        var content = $"<color={color.ToString()}>{message}</color>";
        content = bold ? $"<b>{content}</b>" : content;
        content = italic ? $"<i>{content}</i>" : content;

        return content;
    }

    private static string Title(object title, int level, Color color)
    {
        return Message(string.Format("{0}{1}{2}{1}", Space(level * 2), Character(16 - level * 2), title), color);
    }

    private static string ObjectMessage<T>(T content)
    {
        var fields = typeof(T).GetFields();
        var result = fields.Aggregate("", (current, t) => 
            current + Space(4) + Message(t.Name + " : ", Color.green, true) 
            + ValueToString(t.GetValue(content)) + "\n");

        var properties = typeof(T).GetProperties();

        return properties.Aggregate(result, (current, t) =>
            current + Space(4) + Message(t.Name + " : ", Color.green, true) 
            + ValueToString(t.GetValue(content, null) + "\n"));
    }

    private static string ValueToString(object value)
    {
        switch (value)
        {
            case int _:
            case string _:
            case bool _:
            case float _:
            case double _:
                return value.ToString();
            default:
                return JsonConvert.SerializeObject(value);
        }
    }

    private static void _log(string message, LogType type = LogType.Log)
    {
        UnityEngine.Debug.unityLogger.logHandler.LogFormat(type, null, "{0}", message);
        // UnityEngine.Debug.Log(message);
        // UnityEngine.Debug.LogError(message);
    }
    
    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void Log(object message, Color color = Color.cyan)
    {
        _log(Message(message, color));
    }

    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogError(object message)
    {
        _log(message.ToString(), LogType.Error);
    }

    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogWarning(object message)
    {
        _log(Message(message, Color.yellow));
    }
    /// <summary>
    /// ????????????
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogBold(object message, Color color = Color.cyan)
    {
        _log(Message(message, color, true));
    }

    /// <summary>
    /// ????????????
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogItalic(object message, Color color = Color.cyan)
    {
        _log(Message(message, color, false, true));
    }

    /// <summary>
    /// ??????????????????
    /// </summary>
    /// <param name="message"></param>
    /// <param name="color"></param>
    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogBoldItalic(object message, Color color = Color.cyan)
    {
        _log(Message(message, color, true, true));
    }

    /// <summary>
    /// ????????????????????????:======Title======???
    /// </summary>
    /// <param name="title"></param>
    /// <param name="level"></param>
    /// <param name="color"></param>
    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogTitle(object title, int level, Color color)
    {
        LogBold(Title(title, level, color));
    }

    /// <summary>
    /// ??????????????????????????????
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="color"></param>
    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogObject<T>(T content, Color color = Color.cyan)
    {
        _log(Message(typeof(T).Name, color, true) + "\n" + ObjectMessage(content));
    }

    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogObjects<T>(T[] content)
    {
        if (content == null || content.Length <= 0)
        {
            return;
        }

        var result = Title(typeof(T).Name + " List", 0, Color.green) + "\n";
        for (var i = 0; i < content.Length; i++)
        {
            result += Title(typeof(T).Name + "_" + i, 1, Color.green) + "\n";
            result += ObjectMessage<T>(content[i]) + "\n";
        }

        result += Title("End", 0, Color.green);

        _log(result);
    }

    /// <summary>
    /// ??????List
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    /// <param name="title"></param>
    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogList<T>(List<T> content, string title = null)
    {
        if (content == null)
        {
            return;
        }

        if (content.Count <= 0)
        {
            _log(Title(typeof(T).Name + " List", 0, Color.yellow) + "\n\n");
            return;
        }

        var result = "";
        if (title == null) result = Title(typeof(T).Name + " List", 0, Color.green) + "\n\n";
        else result = Title(title, 0, Color.green) + "\n\n";

        for (var i = 0; i < content.Count; i++)
        {
            // result += Title (typeof (T).Name + "_" + i, 1, Color.green) + "\n";
            // result += ObjectMessage<T> (content[i]) + "\n";
            if (content[i] is string)
            {
                result += i + "==>" + content[i] + "\n";
            }
            else
            {
                result += i + "==>" + JsonConvert.SerializeObject(content[i]) + "\n";
            }
        }

        result += Title("End", 0, Color.green);

        _log(result);
    }

    public static void LogDict<k, v>(Dictionary<k, v> content)
    {
        if (content == null || content.Count <= 0)
        {
            return;
        }

        var result = Title(typeof(v).Name + " Dict", 0, Color.green) + "\n\n";
        result = content.Aggregate(result, (current, dict) => 
            current + dict.Key + "==>" + ValueToString(dict.Value) + "\n");

        result += Title("End", 0, Color.green);

        _log(result);
    }

    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogLine<T>(List<T> content, string title = null)
    {
        if (content == null || content.Count <= 0)
            return;

        var result = "";
        if (title == null) result = Title(typeof(T).Name + " List", 0, Color.green) + "\n";
        else result = Title(title, 0, Color.green) + "\n";
        result = content.Aggregate(result, (current, t) => current + (t + ","));
        result += "\n" + Title("End", 0, Color.green);

        _log(result);
    }

    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogLine<T>(T[] content, string title = null)
    {
        if (content == null || content.Length <= 0)
            return;

        var result = "";
        if (title == null) result = Title(typeof(T).Name + " List", 0, Color.green) + "\n";
        else result = Title(title, 0, Color.green) + "\n";
        result = content.Aggregate(result, (current, t) => current + (t + ","));
        result += "\n" + Title("End", 0, Color.green);

        _log(result);
    }

    /// <summary>
    /// ????????????
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="content"></param>
    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogArray<T>(T[] content)
    {
        if (content == null || content.Length <= 0)
        {
            return;
        }

        var result = Title(typeof(T).Name + " List", 0, Color.green) + "\n";
        for (var i = 0; i < content.Length; i++)
        {
            // result += Title (typeof (T).Name + "_" + i, 1, Color.green) + "\n";
            // result += ObjectMessage<T> (content[i]) + "\n";
            result += i + "==>" + ValueToString(content[i]) + "\n";
        }

        result += Title("End", 0, Color.green);

        _log(result);
    }

    [System.Diagnostics.Conditional("OPEN_LOGGER")]
    public static void LogList2<T>(List<List<T>> content)
    {
        if (content == null || content.Count <= 0)
        {
            return;
        }

        var result = Title(typeof(T).Name + " List", 0, Color.green) + "\n";
        for (var i = 0; i < content.Count; i++)
        {
            for (var j = 0; j < content[i].Count; j++)
            {
                // result += Title (typeof (T).Name + "_" + i+","+j, 1, Color.green) + "\n";
                // result += ObjectMessage<T> (content[i][j]) + "\n";
                result += i + "," + j + "==>" + ValueToString(content[i][j]) + "\n";
            }
        }

        result += Title("End", 0, Color.green);

        _log(result);
    }
    // public static void PrintTest<T> (List<List<T>> content) {
    //     if (!enable || content == null || content.Count <= 0) {
    //         return;
    //     }

    //     var result = Title (typeof (T).Name + " List", 0, Color.green) + "\n";
    //     for (int i = 0; i < content.Count; i++) {

    //         for (int j = 0; j < content[i].Count; j++) {
    //             result += ;
    //         }
    //         result += "\n";
    //     }
    //     result += Title ("End", 0, Color.green);

    //     _log (result);
    // }

    private static string Space(int number)
    {
        var space = "";
        for (var i = 0; i < number; i++)
        {
            space += " ";
        }

        return space;
    }

    private static string Character(int number, string character = "=")
    {
        var result = "";
        for (var i = 0; i < number; i++)
        {
            result += "=";
        }

        return result;
    }
}