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

    $('#table').DataTable({
        "pageLength": 10,
        "lengthMenu": [[5, 10, 25, 50], [5, 10, 25, 50]],
        fixedHeader: true,
        "language": {
            "url": languageUrl,
        }
    });

    // Toggle material status
    $(document).on('click', '.toggle-status', function () {
        const materialId = $(this).data('material-id');
        toggleMaterialStatus(materialId, this);
    });

});

async function deleteMaterial(MaterialId, element) {
    console.log('deleteMaterial called for ID:', MaterialId); // Debug

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
        const DeleteUrl = `${baseUrl}EsAdmin/Materials/Delete?id=${MaterialId}`;
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
                text: currentLanguage === 'ar-JO' ? "تم حذف المادة بنجاح." : "Material deleted successfully.",
                icon: "success",
                timer: 3000
            });

            // Remove row smoothly
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

        } else if (response.status == 400) {
            const responseText = await response.text();
            console.error('Failed to delete Material:', responseText);
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
                text: currentLanguage === 'ar-JO' ? "!! لا يمكن حذف هذه المادة رئيسية أو مرتبطة بمادة أخرى" : "Can't delete this Parent / Related to another material!",
                icon: "error",
                timer: 3000
            });

        } else if (response.status == 403) {
            const responseText = await response.text();
            console.error('Failed to delete Material:', responseText);
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "ممنوع!" : "Forbidden!",
                text: currentLanguage === 'ar-JO' ? "ليس لديك إذن لحذف هذه المادة!" : "You don't have permission to delete this material!",
                icon: "error",
                timer: 3000
            });

        }
    } catch (error) {
        console.error('Error deleting material', error);
        Swal.fire({
            title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
            text: currentLanguage === 'ar-JO' ? "حدث خطأ أثناء حذف المادة. تحقق من اتصالك وحاول مرة أخرى." : "An error occurred while deleting the material. Please check your connection and try again.",
            icon: "error",
            timer: 3000
        });
    }
}



async function toggleMaterialStatus(materialId, element) {
    try {
        const ToggleUrl = `${baseUrl}EsAdmin/Materials/ToggleStatus?id=${materialId}`;
        // Send the request
        const response = await fetch(ToggleUrl, {
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
                const isActive = badgeSpan.text().trim() === (currentLanguage === 'ar-JO' ? 'منشورة' : 'Published');
                badgeSpan.hide().text(isActive ? (currentLanguage === 'ar-JO' ? 'غير منشورة' : 'Not Published') : (currentLanguage === 'ar-JO' ? 'منشورة' : 'Published')).fadeIn(200);
                badgeSpan.toggleClass('bg-success bg-danger');
            }

            // Show success message
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "تم التحديث!" : "Updated!",
                text: currentLanguage === 'ar-JO' ? "تم تحديث حالة المادة بنجاح!" : "Material status updated successfully!",
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
                text: currentLanguage === 'ar-JO' ? "ليس لديك إذن لتعديل حالة هذه المادة!" : "You don't have permission to edit this material!",
                icon: "error"
            });
        } else {
            // Show generic error
            Swal.fire({
                title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
                text: currentLanguage === 'ar-JO' ? "تعذر تعديل حالة المادة!" : "Failed to update material status!",
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
