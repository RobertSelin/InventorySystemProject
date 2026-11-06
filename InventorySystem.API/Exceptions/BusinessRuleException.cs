namespace InventorySystem.API.Exceptions
{
    public class BusinessRuleException : Exception
    {
        // base class Exception contains all that is needed for error handling, with this constructor we just pass the incoming error string to the 
        // parent class Exception. Because the parent class already implements everything that is needed child class body remain empty.

        public BusinessRuleException(string message) : base(message) 
        { 
        }
    }
}
