namespace AzureFuncSampels.Models
{
    public abstract class BaseClass
    {
        public int Id { get; set; }

        public abstract string GetSearchableText();
    }
}