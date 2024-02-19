
namespace eShop.UI.Extensions;

public static class StringExtentions
{
    public static string Truncate(this string value, int length)
    {
        if (value.Length <= length) return value; //
        return value.Trim().Substring(0, length - 4).Trim() + " ..."; // Här bestämmer vi hur många tecken som ska finnas i description för produkterna. vi säger att den ska ta bort 4 tecken av length vilket kommer bli 50. Med " ..." blir det totalt 54. (4 extra tecken). Så -4 betyder att den tar bort fyra av den totala. Trim ser till att hela resultet/innehållet som kommer in inte har något mellanslag i början och i slutet. Den Trimar även Substring.
        //return value[(length - 4)..] + " ...";

    }
}
