namespace LayoutSwitcher.Extensions;

public static class ArrayExtensions
{
    /// <summary>
    /// Returns new array with removed element <c>[index]</c>
    /// </summary>
    public static T[] Remove<T>(this T[] array, int index)
    {
        CheckIndex(index, array.Length);

        var result = new T[array.Length - 1];

        if (index > 0) array[..index].CopyTo(result, 0);
        array[(index + 1)..].CopyTo(result, index);

        return result;
    }

    /// <summary>
    /// Returns new array with inserted element <c>value</c> on <c>[index]</c>
    /// </summary>
    public static T[] Insert<T>(this T[] array, T value, int index)
    {
        CheckIndex(index, array.Length);

        var result = new T[array.Length + 1];

        array[..index].CopyTo(result, 0);
        result[index] = value;
        if (index < array.Length) array[index..].CopyTo(result, index + 1);

        return result;
    }

    /// <summary>
    /// Returns new array with swapped elements <c>index1</c> and <c>index2</c>
    /// </summary>
    public static T[] Swap<T>(this T[] array, int index1, int index2)
    {
        CheckIndex(index1, array.Length);
        CheckIndex(index2, array.Length);

        var result = new T[array.Length];
        array.CopyTo(result, 0);
        (result[index1], result[index2]) = (result[index2], result[index1]);

        return result;
    }

    public static int FirstIndexOf<T>(this T[] array, T element)
        where T: notnull
    {
        for (var i = 0; i < array.Length; i++)
        {
            if(array[i].Equals(element)) return i;
        }

        return -1;
    }

    private static void CheckIndex(int index, int length)
    {
        if (index > length || index < 0) 
            throw new ArgumentOutOfRangeException(nameof(index));
    } 
}