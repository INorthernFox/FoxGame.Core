namespace Core.StateMachines
{
    public struct SetStateResult
    {
        public readonly bool Success;
        public readonly string Message;

        public SetStateResult(bool success, string message = "")
        {
            Success = success;
            Message = message;
        }

        public static SetStateResult Valid() =>
            new(true);

        public static SetStateResult Invalid(string message) =>
            new(false, message);
    }
}