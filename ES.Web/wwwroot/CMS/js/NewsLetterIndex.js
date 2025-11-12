const baseUrl = window.appBaseUrl || '/';
$(document).ready(function () {
    var languageUrl = '';

    // Determine the language URL based on the current language
    if (currentLanguage === 'ar-JO') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    } else if (currentLanguage === 'en-JO') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/English.json";
    } else {
        // Default to English if the language is not set or recognized
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    }

    $('#table').DataTable({
        "pageLength": 50,
        "lengthMenu": [[25, 50, 75, 100], [25, 50, 75, 100]],
        "language": {
            "url": languageUrl
        },
        "order": [[1, "desc"]]  // Sort by 2nd column ascending
    }); 
});






