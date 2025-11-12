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

    // Toggle user status
    $(document).on('click', '.toggle-status', function () {
        const categoryId = $(this).data('category-id');
        toggleCategoryStatus(categoryId, this);
    });

});

async function deleteCategory(CategoryId, element) {
    console.log('deleteCategory called for ID:', CategoryId); // Debug

    // General warning message
    let warningMessage = currentLanguage === 'ar-JO'
        ? "سيتم حذف جميع المنتجات التي تنتمي فقط إلى هذا القسم بشكل دائم! لا يمكنك التراجع عن هذا الإجراء."
        : "All products that belong only to this category will be permanently deleted! This action cannot be undone.";

    // Confirmation dialog
    const result = await Swal.fire({
        title: currentLanguage === 'ar-JO' ? "هل أنت متأكد؟" : "Are you sure?",
        text: warningMessage,
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#d33",
        cancelButtonColor: "#3085d6",
        confirmButtonText: currentLanguage === 'ar-JO' ? "نعم، احذفه!" : "Yes, delete it!",
        cancelButtonText: currentLanguage === 'ar-JO' ? "إلغاء" : "Cancel"
    });

    if (!result.isConfirmed) return;

    try {
        const DeleteUrl = `${baseUrl}EsAdmin/EcomCategories/Delete?id=${CategoryId}`;
        // Send DELETE request
        const response = await fetch(DeleteUrl, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' }
        });

        console.log('Response:', response);

        if (response.ok) {
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "تم الحذف!" : "Deleted!",
                text: currentLanguage === 'ar-JO' ? "تم حذف القسم بنجاح." : "Category deleted successfully.",
                icon: "success",
                timer: 3000
            });

            // Remove row smoothly
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

        } else if (response.status == 400) {
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
                text: currentLanguage === 'ar-JO' ? "!! لا يمكن حذف هذا القسم الرئيسي " : "Can't delete this Parent category!",
                icon: "error",
                timer: 3000
            });

        } else if (response.status == 403) {
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "ممنوع!" : "Forbidden!",
                text: currentLanguage === 'ar-JO' ? "ليس لديك إذن لحذف هذا القسم!" : "You don't have permission to delete this category!",
                icon: "error",
                timer: 3000
            });
        }
    } catch (error) {
        console.error('Error deleting category', error);
        Swal.fire({
            title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
            text: currentLanguage === 'ar-JO' ? "حدث خطأ أثناء حذف الفئة. تحقق من اتصالك وحاول مرة أخرى." : "An error occurred while deleting the category. Please check your connection and try again.",
            icon: "error",
            timer: 3000
        });
    }
}




async function toggleCategoryStatus(categoryId, element) {
    try {
        const ToggoleUrl = `${baseUrl}EsAdmin/EcomCategories/ToggleStatus?id=${categoryId}`;
        // Send the request
        const response = await fetch(ToggoleUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            }
        });

        if (response.status === 200) {
            // Toggle status in UI
            const row = $(element).closest('tr');
            const badgeSpan = row.find('td span.badge');

            if (badgeSpan.length) {
                const isActive = badgeSpan.text().trim() === (currentLanguage === 'ar-JO' ? 'منشور' : 'Published');
                badgeSpan.hide().text(isActive ? (currentLanguage === 'ar-JO' ? 'غير منشور' : 'Not Published') : (currentLanguage === 'ar-JO' ? 'منشور' : 'Published')).fadeIn(200);
                badgeSpan.toggleClass('bg-success bg-danger');
            }

            // Show success message with SweetAlert2
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "تم التحديث!" : "Updated!",
                text: currentLanguage === 'ar-JO' ? "تم تحديث حالة القسم بنجاح!" : "Category status updated successfully!",
                icon: "success",
                timer: 2000,
                showConfirmButton: false
            });

            setTimeout(function () {
                location.reload(); // Reload the page after 2 seconds
            }, 2000);

        } else if (response.status === 403) {
            // Show permission error message
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "ممنوع!" : "Forbidden!",
                text: currentLanguage === 'ar-JO' ? "ليس لديك إذن لتعديل حالة هذا القسم!" : "You don't have permission to edit this category!",
                icon: "error"
            });
        } else {
            // Show generic error
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
                text: currentLanguage === 'ar-JO' ? "تعذر تعديل حالة القسم!" : "Failed to update category status!",
                icon: "error"
            });
        }
    } catch (error) {
        console.error('Error:', error);
        Swal.fire({
            title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
            text: currentLanguage === 'ar-JO' ? "حدث خطأ أثناء تحديث الحالة!" : "An error occurred while updating the status!",
            icon: "error"
        });
    }
}