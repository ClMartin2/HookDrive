var MyPlugin = {
    IsMobile: function() {
        return /Mobi|Android/i.test(navigator.userAgent);
    }
};

mergeInto(LibraryManager.library, MyPlugin);