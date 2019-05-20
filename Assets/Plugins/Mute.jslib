mergeInto(LibraryManager.library, {

    ToggleMute: function ()
    {
        toggleMute();
        return muted;
    }
});