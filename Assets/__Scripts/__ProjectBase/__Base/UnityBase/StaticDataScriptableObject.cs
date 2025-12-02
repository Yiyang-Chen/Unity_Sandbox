# if UNITY_5_6_OR_NEWER

using UnityEngine;
using Sirenix.OdinInspector;

public abstract class StaticDataScriptableObject : ScriptableObject, IConfigData { }

public abstract class OdinSerializedStaticDataScriptableObject : SerializedScriptableObject, IConfigData { }

# endif