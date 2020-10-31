using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityObject = UnityEngine.Object;

[CustomPropertyDrawer(typeof(SerializableIntDictionary))]
public class IntDictionaryDrawer : DictionaryDrawer<int, int> {}