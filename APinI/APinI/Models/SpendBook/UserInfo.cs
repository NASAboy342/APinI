namespace APinI.Models.SpendBook
{
    public class UserInfo
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTimeOffset UtcCreatedOn { get; set; }
        public DateTimeOffset UtcModifiedOn { get; set; }
    }
}
