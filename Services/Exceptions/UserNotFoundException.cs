﻿namespace Services.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException(string email)
            : base($"User with email '{email}' was not found.") { }
    }
}
