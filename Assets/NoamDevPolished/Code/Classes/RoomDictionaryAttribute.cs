// Assets/Scripts/Rooms/RoomDictionaryAttribute.cs
//
// RoomDictionaryAttribute
//
// What this script does:
// - Marks a RoomTransformDictionary field so it gets a custom Inspector UI.
// - This file must NOT be inside an Editor folder, otherwise runtime scripts can't see the attribute.

using UnityEngine;

public sealed class RoomDictionaryAttribute : PropertyAttribute { }