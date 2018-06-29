namespace AzureFuncSamples.Models
{
    public class User : BaseClass
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Gender { get; set; }
        public string Avatar { get; set; }

        public override string GetSearchableText()
        {
            return Id + FirstName + LastName + Email + Gender;
        }
    }
}