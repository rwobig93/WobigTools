﻿namespace SharedLib.Dto
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
        Email,
        Webhook,
        Email_Webhook
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
}
