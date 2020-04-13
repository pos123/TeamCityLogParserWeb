

window.siteJsFunctions = {
    scroll_to_view_error_link: function(text) {
        document.getElementById(text).scrollIntoView({ behavior: 'auto', block: "start" });
        $("#" + text).fadeTo(100, 0.3, function () { $(this).fadeTo(500, 1.0); });
    }
}