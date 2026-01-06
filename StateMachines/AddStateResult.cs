namespace Core.StateMachines
{
    public struct AddStateResult
    {
        public readonly bool Success;
        public readonly string Message;

        public AddStateResult(bool success, string message = "")
        {
            Success = success;
            Message = message;
        }

        public static AddStateResult Valid() =>
            new(true);

        public static AddStateResult Invalid(string message) =>
            new(false, message);
    }

}