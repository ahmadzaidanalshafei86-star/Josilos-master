const baseUrl = window.appBaseUrl || '/';

async function deleteTranslation(TranslationId, element) {
    console.log('deleteTranslation called for ID:', TranslationId); // Debug

    // Confirm deletion using SweetAlert2
    const confirmDeletion = await Swal.fire({
        icon: 'warning',
        title: currentLanguage === 'ar-JO' ? 'هل أنت متأكد أنك تريد حذف هذه الترجمة؟' : 'Are you sure you want to delete this translation?',
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم' : 'Yes',
        cancelButtonText: currentLanguage === 'ar-JO' ? 'لا' : 'No'
    });

    if (!confirmDeletion.isConfirmed) {
        return;
    }

    try {
        const DeleteUrl = `${baseUrl}EsAdmin/PagesTranslates/Delete?id=${TranslationId}`;
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
            console.log('Translation deleted successfully.');
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

            // Show success message using SweetAlert2
            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم حذف الترجمة بنجاح' : 'Translation deleted successfully',
                showConfirmButton: true
            });

            setTimeout(() => {
                location.reload();
            }, 3000);
        } else if (response.status == 403) {
            // Check for permission error
            if (currentLanguage === 'ar-JO') {
                Swal.fire({
                    icon: 'error',
                    title: 'ليس لديك إذن لحذف هذه الترجمة !',
                    showConfirmButton: true
                });
            } else if (currentLanguage === 'en-JO') {
                Swal.fire({
                    icon: 'error',
                    title: 'You don\'t have permission to delete this translation !',
                    showConfirmButton: true
                });
            }

            const responseText = await response.text();
            console.error('Failed to delete Translation:', responseText);
        }
    } catch (error) {
        // Catch network or other unexpected errors
        console.error('Error deleting translation', error);

        // Show error message using SweetAlert2
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء حذف الترجمة' : 'An error occurred while deleting the translation.',
            showConfirmButton: true
        });
    }
}

