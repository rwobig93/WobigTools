namespace SharedLib.Dto
{
    public enum StatusReturn
    {
        NotFound,
        Found,
        Failure,
        Success
    }

    public enum AppFile
    {
        Config,
        Log,
        SavedData
    }

    public enum AlertType
    {
        Email = 0,
        Webhook = 1,
        Email_Webhook = 2
    }

    public enum TrackInterval
    {
        OneMin,
        FiveMin
    }

    public enum MessagePrefix
    {
        Log,
        None
    }

    public enum AccessType
    {
        Read,
        Write
    }

    public enum HeadlessBrowserType
    {
        Local,
        Remote
    }

    public enum AssemblyType
    {
        Entry,
        Executing
    }
}
