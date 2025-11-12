const baseUrl = window.appBaseUrl || '/';

$(document).ready(function () {


    // ==============================
    // Search modal (Enter key redirect)
    // ==============================
    $('.search-input').keypress(function (e) {
        if (e.which === 13) { // Enter key
            e.preventDefault();
            let query = $(this).val().trim();
            if (query.length > 0) {
                window.location.href = baseUrl + 'Search/Index?query=' + encodeURIComponent(query);
            }
        }
    });

});


