namespace Eindwerk.Models
{
    public interface IDtoModel
    {
        /**
         * <summary>A method for ensuring JsonConvert doesn't convert to an empty object</summary>
         */
        bool IsFilled();
    }
}