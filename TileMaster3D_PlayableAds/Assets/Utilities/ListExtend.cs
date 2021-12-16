using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public static class ListExtend {
    /// <summary>
    /// 随机排列元素
    /// </summary>
    public static void SortR<T> (this List<T> self) {
        int lenght = self.Count;
        T temp;
        for (int i = lenght - 1, rand; i > -1; lenght--, i--) {
            rand = UnityEngine.Random.Range(0, lenght);
            temp = self[rand];
            self[rand] = self[lenght - 1];
            self[lenght - 1] = temp;
        }
    }
    /// <summary>
    /// 随机返回 数组顺序不变
    /// </summary>
    public static T Random<T> (this List<T> self) {
        int rand = UnityEngine.Random.Range(0, self.Count);
        return self[rand];
    }
    /// <summary>
    /// 随机删除并返回 数组顺序不变
    /// </summary>
    public static T PopR<T> (this List<T> self) {
        int rand = UnityEngine.Random.Range(0, self.Count);
        T temp = self[rand];
        self.RemoveAt (rand);
        return temp;
    }
    /// <summary>
    /// 删除并返回  数组顺序不变
    /// </summary>
    /// <returns></returns>
    public static T Pop<T> (this List<T> self, int index = -1) {
        if (index == -1) index = self.Count - 1;
        T temp = self[index];
        self.RemoveAt (index);
        return temp;
    }
    public static T Last<T> (this List<T> self) {
        return self[self.Count - 1];
    }
}