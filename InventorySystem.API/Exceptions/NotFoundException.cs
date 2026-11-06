namespace InventorySystem.API.Exceptions
{
    public class NotFoundException : Exception
    {
        // base class Exception contains all that is needed for error handling, with this constructor we just pass the incoming error string to the 
        // parent class Exception. Because the parent class already implements everything that is needed child class body remain empty.

        public NotFoundException(string message) : base(message)
        {
        }
    }
}
