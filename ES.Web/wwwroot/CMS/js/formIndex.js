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
        "pageLength": 5,
        "lengthMenu": [[5, 10, 25, 50], [5, 10, 25, 50]],
        fixedHeader: true,
        "language": {
            "url": languageUrl,
        }

    });

    // Toggle form status
    $(document).on('click', '.toggle-status', function () {
        const FormId = $(this).data('form-id');
        toggleFormStatus(FormId, this);
    });

});

async function toggleFormStatus(formId, element) {
    try {
        const ToggoleUrl = `${baseUrl}EsAdmin/Forms/ToggleStatus?id=${formId}`;
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
                const isActive = badgeSpan.text().trim() === 'Published';
                badgeSpan.hide().text(isActive ? 'Not published' : 'Published').fadeIn(200);
                badgeSpan.toggleClass('bg-success bg-danger');
            }

            // Show SweetAlert2 success messages
            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم بنجاح!' : 'Done successfully!',
                showConfirmButton: true
            });

            setTimeout(function () {
                location.reload(); // Reload the Form after 3 seconds
            }, 2000);
        } else if (response.status === 403) {
            // Handle permission error with SweetAlert2
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن لتعديل حالة هذه النموذج!' : 'You don\'t have permission to edit this Form!',
                showConfirmButton: true
            });
        } else {
            // Show generic error for other status codes
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'لا يمكن نشر النموذج' : 'Can\'t publish',
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

async function deleteForm(FormId, element) {
    console.log('deleteForm called for ID:', FormId); // Debug

    // Confirm deletion using SweetAlert2
    const confirmDeletion = await Swal.fire({
        icon: 'warning',
        title: currentLanguage === 'ar-JO' ? 'هل أنت متأكد من أنك تريد حذف هذا النموذج ؟' : 'Are you sure you want to delete this Form?',
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم' : 'Yes',
        cancelButtonText: currentLanguage === 'ar-JO' ? 'لا' : 'No'
    });

    if (!confirmDeletion.isConfirmed) {
        return;
    }

    try {
        const DeleteUrl = `${baseUrl}EsAdmin/Forms/Delete?id=${FormId}`;
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
            console.log('Form deleted successfully.');
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

            // Show success message using SweetAlert2
            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم حذف النموذج بنجاح !' : 'Form deleted successfully !',
                showConfirmButton: true
            });

        } else if (response.status == 403) {
            // Check for permission error
            const responseText = await response.text();
            console.error('Failed to delete Form:', responseText);

            // Show permission error using SweetAlert2
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن لحذف هذا النموذج !' : 'You don\'t have permission to delete this Form !',
                showConfirmButton: true
            });
        }
    } catch (error) {
        // Catch network or other unexpected errors
        console.error('Error deleting Form', error);

        // Show error message using SweetAlert2
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء حذف النموذج' : 'An error occurred while deleting the Form.',
            showConfirmButton: true
        });
    }
}

function ExportSheet(formId, element) {

    let url = `${baseUrl}EsAdmin/Forms/ExportSheet/${formId}`;
    window.location.href = url;
}