using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ExtensionMethods {
	public static T Last<T>(this T[] array, int reversedIndex=0) {
		return array[array.Length - 1 - reversedIndex];
	}
}
