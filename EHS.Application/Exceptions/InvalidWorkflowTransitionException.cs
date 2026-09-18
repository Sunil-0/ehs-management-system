namespace EHS.Application.Exceptions
{
    // Thrown when someone tries to do something the workflow doesn't allow
    // right now (e.g. approving an incident that's still Submitted).
    // The controller catches this and turns it into a 409 Conflict.
    public class InvalidWorkflowTransitionException : Exception
    {
        public InvalidWorkflowTransitionException(string message) : base(message) { }
    }

    // Thrown when a record isn't found. Controller turns this into a 404.
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }
}

