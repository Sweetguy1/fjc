using System.Collections.Generic;

public class LinearTriggerOne {
    public string key = "";
    private int _index = 0;
    public List<System.Action> _methods = new List<System.Action>();
    public void Clear() { _index = 0; _methods.Clear(); }
    public void Init() { _index = 0; }

    public LinearTriggerOne Push(System.Action func) { _methods.Add(func); return this; }

    public LinearTriggerOne Prepend(System.Action func) { _methods.Insert(0, func); return this; }

    public void Next() {
        if (_methods.Count == _index) return;
        var func = _methods[_index++];
        func.Invoke();
    }
}
public static class LinearTrigger {
    private static Dictionary<string, LinearTriggerOne> _stream = new Dictionary<string, LinearTriggerOne>();
    private static int _sign = 0;

    public static LinearTriggerOne Create(string key = "") {
        LinearTriggerOne one;
        if (string.IsNullOrEmpty(key)) { key = (++_sign).ToString(); }
        if (_stream.ContainsKey(key)) {
            one = _stream[key];
        } else {
            one = new LinearTriggerOne();
            one.key = key;
            _stream.Add(key, one);
        }
        one.Clear();
        return one;
    }

    public static void Clear(string key = "") {
        if (string.IsNullOrEmpty(key)) key = _sign.ToString();
        if (_stream.ContainsKey(key)) _stream.Remove(key);
    }
    public static void Clear(LinearTriggerOne one) => Clear(one.key);

    public static void Push(System.Action func, string key = "") {
        if (string.IsNullOrEmpty(key)) key = _sign.ToString();
        if (!_stream.ContainsKey(key)) return;
        _stream[key].Push(func);
    }
    public static void Prepend(System.Action func, string key = "") {
        if (string.IsNullOrEmpty(key)) key = _sign.ToString();
        if (!_stream.ContainsKey(key)) return;
        _stream[key].Prepend(func);
    }
    public static void Next(string key = "") {
        if (string.IsNullOrEmpty(key)) key = _sign.ToString();
        if (!_stream.ContainsKey(key)) return;
        _stream[key].Next();
    }
}