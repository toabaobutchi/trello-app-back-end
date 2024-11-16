namespace backend_apis.Utils
{
    /*
    This class is used to store constants for the response messages.
    `S_` means the successful response
    `F_` means the failed response
    */
    public static class ResponseMessage
    {
        // success messsage
        public const string S_FETCH = "Successfully fetched";
        public const string S_CREATE = "Successfully created";
        public const string S_UPDATE = "Successfully updated";
        public const string S_DELETE = "Successfully deleted";
        public const string S_LOGIN = "Successfully logged in";

        // fail messages
        public const string F_CREATE = "Failed to create";
        public const string F_DELETE = "Failed to delete";
        public const string F_UPDATE = "Failed to update";
        public const string F_FETCH = "Failed to fetch";

        // others
        public const string INVALID_TOKEN = "Invalid token";
        public const string INVALID_DATA = "Invalid data";
        public const string USER_NOT_FOUND = "User not found";
        public const string S_RENEW_TOKEN = "Successfully renew token";
        public const string NOT_FOUND = "Not found";
        public const string INTERNAL_SERVER_ERROR = "Something went wrong";
        public const string CAN_NOT_HANDLE = "Cannot handle request";
        public const string CAN_NOT_JOIN = "Cannot join";
    }
}