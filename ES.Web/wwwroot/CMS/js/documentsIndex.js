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
            "url": languageUrl
        }
    });

});

async function deleteDocument(docId, element) {
    console.log('deleteDocument called for ID:', docId); // Debug

    // Modern confirmation dialog using SweetAlert2
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
        const DeleteUrl = `${baseUrl}EsAdmin/UploadDocuments/Delete?id=${docId}`;
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
                text: currentLanguage === 'ar-JO' ? "تم حذف الملف بنجاح!" : "Document deleted successfully!",
                icon: "success",
                timer: 2000,
                showConfirmButton: false
            });

            // Remove row smoothly
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

        } else if (response.status === 403) {
            // Show permission error message
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "ممنوع!" : "Forbidden!",
                text: currentLanguage === 'ar-JO' ? "ليس لديك إذن لحذف هذا الملف!" : "You don't have permission to delete this document!",
                icon: "error"
            });
        } else {
            // Show generic error
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
                text: currentLanguage === 'ar-JO' ? "تعذر حذف الملف!" : "Failed to delete the document!",
                icon: "error"
            });
        }
    } catch (error) {
        console.error('Error deleting document', error);
        Swal.fire({
            title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
            text: currentLanguage === 'ar-JO' ? "حدث خطأ أثناء حذف الملف. تحقق من الاتصال وحاول مرة أخرى!" : "An error occurred while deleting the document. Please check your connection and try again!",
            icon: "error"
        });
    }
}


