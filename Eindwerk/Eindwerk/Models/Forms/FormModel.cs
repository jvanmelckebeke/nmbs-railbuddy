using System.ComponentModel.DataAnnotations;

namespace Eindwerk.Models.Forms
{
    public abstract class FormModel
    {
        /**
         * <summary>Should validate all inputs</summary>
         */
        public abstract bool ValidateInputs();

        protected static bool ValidateEmail(string emailToTest)
        {
            var validator = new EmailAddressAttribute();

            return emailToTest != null && validator.IsValid(emailToTest);
        }
    }
}