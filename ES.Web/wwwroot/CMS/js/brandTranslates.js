const baseUrl = window.appBaseUrl || '/';

$(document).ready(function () {
    // Handle form submission
    $('#translationModal').on('submit', '#BrandTranslationForm', function (event) {
        event.preventDefault();

        $.ajax({
            url: `${baseUrl}EsAdmin/BrandTranslates/SaveTranslation`,
            type: 'POST',
            data: $(this).serialize(),
            success: function () {
                $('#translationModal').modal('hide');
                location.reload(); // Reload or dynamically update the table
            },
            error: function (xhr) {
                // Load form with errors if validation fails
                $('#translationModal .modal-body').html(xhr.responseText);
            }
        });
    });
});

async function deleteTranslation(brandId, element) {
    console.log('deleteRole called for ID:', brandId); // Debug

    // Confirm deletion with SweetAlert2
    const result = await Swal.fire({
        title: currentLanguage === 'ar-JO' ? 'هل أنت متأكد أنك تريد حذف هذه الترجمة ؟' : 'Are you sure you want to delete this Brand translation?',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم, احذف!' : 'Yes, delete it!',
        cancelButtonText: currentLanguage === 'ar-JO' ? 'إلغاء' : 'Cancel'
    });

    if (!result.isConfirmed) {
        return;
    }

    try {
        const DeleteUrl = `${baseUrl}EsAdmin/BrandTranslates/Delete?id=${brandId}`; 
        // Make the fetch request without CSRF token
        const response = await fetch(DeleteUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        // Debug the response object
        console.log('Response:', response);

        // Check if response is successful (status code 200-299)
        if (response.ok) {
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());
            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم الحذف بنجاح !' : 'Brand translation deleted successfully!',
                showConfirmButton: false,
                timer: 3000
            });
            setTimeout(() => {
                location.reload();
            }, 3000);

        } else if (response.status == 403) {
            // Check for a more detailed response if it's not ok
            const responseText = await response.text(); // or response.json() for JSON responses
            console.error('Failed to delete menu item:', responseText);
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن للحذف  !' : 'You don\'t have permission to delete this Brand translation!',
                showConfirmButton: true
            });
        }
    } catch (error) {
        // Catch network or other unexpected errors
        console.error('Error deleting page', error);
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء الحذف. يرجى التحقق من الاتصال والمحاولة مرة أخرى.' : 'An error occurred while deleting. Please check your connection and try again.',
            showConfirmButton: true
        });
    }
}

function openTranslationModal(brandId, translationId = null) {
    let actionUrl = translationId
        ? `${baseUrl}EsAdmin/BrandTranslates/GetTranslationForm?brandId=${brandId}&translationId=${translationId}`
        : `${baseUrl}EsAdmin/BrandTranslates/GetTranslationForm?brandId=${brandId}`;

    $.get(actionUrl, function (data) {
        $('#translationModal .modal-body').html(data); // Load the form content

        if (currentLanguage === 'ar-JO') {
            $('#translationModalLabel').text(translationId ? 'تعديل الترجمة' : 'إضافة ترجمة');
        } else if (currentLanguage === 'en-JO') {
            $('#translationModalLabel').text(translationId ? 'Edit Translation' : 'Add Translation');
        }
        $('#translationModal').modal('show');
    }).fail(function () {
        // This will be triggered if the request fails
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن للقيام بهذا الاجراء  !' : 'You don\'t have permission to do this action!',
            showConfirmButton: true
        });
    });
}

