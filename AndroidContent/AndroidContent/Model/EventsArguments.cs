using System;

namespace AllContent_Client
{
    enum TypeOfFavoritesChange
    {
        Add,
        Delete
    }

    class EventAuthorizationArgs : EventArgs
    {
        public bool Result { get; private set; }
        public string Name { get; private set; }
        public EventAuthorizationArgs(bool result, string name)
        {
            Result = result;
            Name = name;
        }
    }

    class EventFavoritesArgs : EventArgs
    {

        public TypeOfFavoritesChange Type { get; }
        public string Name { get; }
        public EventFavoritesArgs(string name, TypeOfFavoritesChange type)
        {
            Type = type;
            Name = name;
        }
    }
}
