using UnityEngine;
using System.Collections.Generic;
using System;
using Random = System.Random;

public static class Extensions
{
	#region List

	private static readonly Random rnd = new Random();

	public static void Shuffle<T>(this IList<T> list) 
	{
		int n = list.Count;
		Random rnd = new Random();
		while (n > 1) {
			int k = (rnd.Next(0, n) % n);
			n--;
			T value = list[k];
			list[k] = list[n];
			list[n] = value;
		}
	}


	#endregion

    #region Vector2
    public static Vector2 ClosestCardinalDirection(this Vector2 vector)
    {
        if (vector == Vector2.zero)
        {
            return Vector2.zero;
        }

        if (Mathf.Abs(vector.x) > Mathf.Abs(vector.y))
        {
            return new Vector2(vector.x, 0);
        }
        else
        {
            return new Vector2(0, vector.y);
        }
    }

    public static Vector2 ClosestCardinalOrOrdinalDirection(this Vector2 vector)
    {
        Vector2 closestCardinal = vector.ClosestCardinalDirection();
        Vector2 closestOrdinal = SuperMath.ZeroSign(vector);

        if (Vector2.Angle(vector, closestCardinal) < Vector2.Angle(vector, closestOrdinal))
        {
            return closestCardinal;
        }
        else
        {
            return closestOrdinal;
        }
    }
    #endregion

    #region Vector3
    public static Vector3 XY(this Vector3 vector)
	{
		return new Vector3(vector.x, vector.y, 0);
	}

	public static Vector3 XZ(this Vector3 vector)
	{
		return new Vector3(vector.x, 0, vector.z);
	}

	public static Vector3 YZ(this Vector3 vector)
	{
		return new Vector3(0, vector.y, vector.z);
	}
	
	/// <summary>
	/// Used to set nonmodifiable vectors		
	/// </summary>
	public static Vector3 SetX(this Vector3 vector, float newX)
	{
		return new Vector3 (newX, vector.y, vector.z);
	}
	
	/// <summary>
	/// Used to set nonmodifiable vectors		
	/// </summary>
	public static Vector3 SetY(this Vector3 vector, float newY)
	{
		return new Vector3 (vector.x, newY, vector.z);
	}
	
	/// <summary>
	/// Used to set nonmodifiable vectors		
	/// </summary>
	public static Vector3 SetZ(this Vector3 vector, float newZ)
	{
		return new Vector3 (vector.x, vector.y, newZ);
	}
	#endregion

	#region List
	public static void Shuffle<T>(this List<T> list)
	{
		for (int index = list.Count - 1; index > 0; index--)
		{
			int swapPosition = UnityEngine.Random.Range(0, index + 1);
			T value = list[swapPosition];
			list[swapPosition] = list[index];
			list[index] = value;
		}
	}
	#endregion

	#region Color
	/// <summary>
	/// Used to set nonmodifiable colors		
	/// </summary>
	public static Color SetR(this Color color, float newValue)
	{
		color.r = newValue;
		return color;
	}
	
	/// <summary>
	/// Used to change nonmodifiable colors		
	/// </summary>
	public static Color ModifyR(this Color color, float amount)
	{
		color.r += amount;
		return color;
	}

	/// <summary>
	/// Used to set nonmodifiable colors		
	/// </summary>
	public static Color SetG(this Color color, float newValue)
	{
		color.g = newValue;
		return color;
	}
	
	/// <summary>
	/// Used to change nonmodifiable colors		
	/// </summary>
	public static Color ModifyG(this Color color, float amount)
	{
		color.g += amount;
		return color;
	}

	/// <summary>
	/// Used to set nonmodifiable colors		
	/// </summary>
	public static Color SetB(this Color color, float newValue)
	{
		color.b = newValue;
		return color;
	}
	
	/// <summary>
	/// Used to change nonmodifiable colors		
	/// </summary>
	public static Color ModifyB(this Color color, float amount)
	{
		color.b += amount;
		return color;
	}

	/// <summary>
	/// Used to set nonmodifiable colors		
	/// </summary>
	public static Color SetA(this Color color, float newValue)
	{
		color.a = newValue;
		return color;
	}
	
	/// <summary>
	/// Used to change nonmodifiable colors		
	/// </summary>
	public static Color ModifyA(this Color color, float amount)
	{
		color.a += amount;
		return color;
	}
	#endregion

	#region string
	/// <summary>
	/// Wraps text of a string to a newline given a line length.
	/// </summary>
	/// <returns>The wrapped string.</returns>
	/// <param name="maxLineLength">How long each line should be.</param>
	public static string WordWrap(this string stringToSplit, int maxLineLength)
	{
		string stringHolder = "";
		int lettersInCurrentLine = 0;
		
		string[] words = stringToSplit.Split (' ');
		for(int i = 0; i < words.Length; i++)
		{
			string word = words[i];
			int lettersInWord = word.Length + 1;
			if(lettersInCurrentLine + lettersInWord < maxLineLength)
			{
				stringHolder += " " + word;
				lettersInCurrentLine += lettersInWord;
			}
			else
			{
				stringHolder += "\n" + word;
				lettersInCurrentLine = lettersInWord;
			}
		}
		return stringHolder;
	}
	#endregion
}