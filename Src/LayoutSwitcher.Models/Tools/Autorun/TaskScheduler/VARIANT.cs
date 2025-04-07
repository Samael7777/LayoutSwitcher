using Windows.Win32.Foundation;
// ReSharper disable InconsistentNaming

// ReSharper disable once CheckNamespace
namespace Windows.Win32.System.Variant;

// ReSharper disable once InconsistentNaming
internal partial struct VARIANT
{
    public static VARIANT Null { get; } = default;

    public static VARIANT FromInt(int value)
    {
        var newVar = new VARIANT();

        newVar.Anonymous.Anonymous.vt = VARENUM.VT_I4;
        newVar.Anonymous.Anonymous.Anonymous.lVal = value;

        return newVar;
    }

    // ReSharper disable once IdentifierTypo
    public static VARIANT FromBSTR(BSTR value)
    {
        var newVar = new VARIANT();

        newVar.Anonymous.Anonymous.vt = VARENUM.VT_BSTR;
        newVar.Anonymous.Anonymous.Anonymous.bstrVal = value;

        return newVar;
    }
}