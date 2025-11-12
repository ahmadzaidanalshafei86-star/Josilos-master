const baseUrl = window.appBaseUrl || '/';

$(document).ready(function () {
    var languageUrl = '';

    // Determine the language URL based on the current language
    if (currentLanguage === 'ar-JO') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    } else if (currentLanguage === 'en-JO') {
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/English.json";
    } else {
        // Default to Arabic if the language is not recognized
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/Arabic.json";
    }

    var table = $('#table').DataTable({
        "pageLength": 10,
        "lengthMenu": [[10, 20, 40, 50], [10, 20, 40, 50]],
        fixedHeader: true,
        "language": {
            "url": languageUrl
        }
    });

    // Toggle user status
    $(document).on('click', '.toggle-status', function () {
        const link = $(this).data('link-id');
        toggleStatus(link, this);
    });



});

async function toggleStatus(linkId, element) {
    try {
        const ToggoleUrl = `${baseUrl}EsAdmin/SocialMediaLinks/ToggleStatus?id=${linkId}`;
        const response = await fetch(ToggoleUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            }
        });

        // Only show success if the response is OK (status 200)
        if (response.status === 200) {
            const row = $(element).closest('tr');
            const badgeSpan = row.find('td span.badge');

            if (badgeSpan.length) {
                const isActive = badgeSpan.text().trim() === (currentLanguage === 'ar-JO' ? 'منشور' : 'Published');
                badgeSpan.hide().text(isActive ? (currentLanguage === 'ar-JO' ? 'غير منشور' : 'Not Published') : (currentLanguage === 'ar-JO' ? 'منشور' : 'Published')).fadeIn(200);
                badgeSpan.toggleClass('bg-success bg-danger');
            }

            // Show SweetAlert2 success messages
            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم بنجاح!' : 'Done successfully!',
                showConfirmButton: true
            });

            setTimeout(function () {
                location.reload(); // Reload the page after 3 seconds
            }, 2000);
        } else if (response.status === 403) {
            // Handle permission error with SweetAlert2
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك صلاحية للتعديل !' : 'You don\'t have permission to edit this!',
                showConfirmButton: true
            });
        } else {
            // Show generic error for other status codes
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'لا يمكن النشر' : 'Can\'t publish',
                showConfirmButton: true
            });
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ' : 'An error occurred',
            showConfirmButton: true
        });
    }
}

document.getElementById('addSocialMediaForm').addEventListener('submit', function (event) {
    const fileInput = document.getElementById('logoFileInput');
    const errorDiv = document.getElementById('logoError');
    const maxSize = 10 * 1024 * 1024; // 10MB in bytes
    const allowedTypes = ['image/png', 'image/jpeg', 'image/jpg'];

    // Reset error message
    errorDiv.style.display = 'none';
    errorDiv.textContent = '';

    if (fileInput.files.length > 0) {
        const file = fileInput.files[0];

        // Check file type
        if (!allowedTypes.includes(file.type)) {
            event.preventDefault();
            errorDiv.textContent = 'Only PNG, JPG, or JPEG files are allowed.';
            errorDiv.style.display = 'block';
            return;
        }

        // Check file size
        if (file.size > maxSize) {
            event.preventDefault();
            errorDiv.textContent = 'File size must be less than 10MB.';
            errorDiv.style.display = 'block';
            return;
        }
    }
});
