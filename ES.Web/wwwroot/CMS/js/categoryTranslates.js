const baseUrl = window.appBaseUrl || '/';

async function deleteTranslation(TranslationId, CategoryId, element) {
    console.log('deleteTranslation called for ID:', TranslationId); // Debug

    // Modern confirmation dialog
    const result = await Swal.fire({
        title: currentLanguage === 'ar-JO' ? "هل أنت متأكد؟" : "Are you sure?",
        text: currentLanguage === 'ar-JO' ? "لن تتمكن من التراجع عن هذا!" : "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: currentLanguage === 'ar-JO' ? "نعم، احذفها!" : "Yes, delete it!",
        cancelButtonText: currentLanguage === 'ar-JO' ? "إلغاء" : "Cancel"
    });

    if (!result.isConfirmed) {
        return;
    }

    try {
        const DeleteUrl = `${baseUrl}EsAdmin/CategoriesTranslates/Delete?translationId=${TranslationId}&categoryId=${CategoryId}`;
        // Send DELETE request
        const response = await fetch(DeleteUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });

        console.log('Response:', response);

        if (response.ok) {
            // Success message
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "تم الحذف!" : "Deleted!",
                text: currentLanguage === 'ar-JO' ? "تم حذف الترجمة بنجاح!" : "Translation deleted successfully!",
                icon: "success",
                timer: 2000,
                showConfirmButton: false
            });

            // Remove row smoothly
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

            setTimeout(() => {
                location.reload();
            }, 2000);

        } else if (response.status == 403) {
            // Show permission error message
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "ممنوع!" : "Forbidden!",
                text: currentLanguage === 'ar-JO' ? "ليس لديك إذن لحذف هذه الترجمة!" : "You don't have permission to delete this translation!",
                icon: "error"
            });
        } else {
            // Show generic error
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
                text: currentLanguage === 'ar-JO' ? "تعذر حذف الترجمة!" : "Failed to delete the translation!",
                icon: "error"
            });
        }
    } catch (error) {
        console.error('Error deleting translation', error);
        Swal.fire({
            title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
            text: currentLanguage === 'ar-JO' ? "حدث خطأ أثناء حذف الترجمة. تحقق من الاتصال وحاول مرة أخرى!" : "An error occurred while deleting the translation. Please check your connection and try again!",
            icon: "error"
        });
    }
}
