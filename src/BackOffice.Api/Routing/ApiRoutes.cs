namespace BackOffice.Api.Routing {
    public static class ApiRoutes {
        public const string ServiceRequests = "api/ServiceRequests";
        public const string Auth = "api/Auth";
        public const string Login = "login";
        public const string AuthLogin = "/" + Auth + "/" + Login;
        public const string ById = "{id:guid}";
    }
}
