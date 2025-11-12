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
        languageUrl = "https://cdn.datatables.net/plug-ins/1.10.21/i18n/English.json";
    }

    // Initialize DataTable with the correct language URL
    $('#table').DataTable({
        "pageLength": 5,
        "lengthMenu": [[5, 10, 25, 50], [5, 10, 25, 50]],
        fixedHeader: true,
        "language": {
            "url": languageUrl
        }
    });
});


async function deleteRole(roleId, element) {
    console.log('deleteRole called for ID:', roleId); // Debug

    // Confirm deletion using SweetAlert2
    const confirmDeletion = await Swal.fire({
        icon: 'warning',
        title: currentLanguage === 'ar-JO' ? 'هل أنت متأكد أنك تريد حذف هذا الدور؟' : 'Are you sure you want to delete this role?',
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم' : 'Yes',
        cancelButtonText: currentLanguage === 'ar-JO' ? 'لا' : 'No'
    });

    if (!confirmDeletion.isConfirmed) {
        return;
    }

    try {
        const DeleteUrl = `${baseUrl}EsAdmin/Roles/Delete?id=${roleId}`;
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
            console.log('Role deleted successfully.');
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

            // Show success message using SweetAlert2
            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم حذف الدور بنجاح' : 'Role deleted successfully',
                showConfirmButton: true
            });
        } else {
            // Handle failure (not OK)
            const responseText = await response.text(); // or response.json() for JSON responses
            console.error('Failed to delete role:', responseText);

            // Show error message using SweetAlert2
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'لا يمكنك حذف هذا الدور!' : 'You can\'t delete this role!',
                showConfirmButton: true
            });
        }
    } catch (error) {
        // Catch network or other unexpected errors
        console.error('Error deleting role:', error);

        // Show error message using SweetAlert2
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء حذف الدور' : 'An error occurred while deleting the role.',
            showConfirmButton: true
        });
    }
}


