namespace Backend.dto
{
    public class Credentials
    {
        public string Email { get; set; }

        public string Password { get; set; }


        public override string ToString()
        {
            return $"Credentials[{nameof(Email)}: {Email}, {nameof(Password)}: {Password}]";
        }
    }
}