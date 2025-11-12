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
        "lengthMenu": [[5, 10, 25, 50], [5, 10, 25, 50]],
        fixedHeader: true,
        "language": {
            "url": languageUrl
        }
    });

    // Toggle career status
    $(document).on('click', '.toggle-status', function () {
        const career = $(this).data('career-id');
        toggleStatus(career, this);
    });
});

async function toggleStatus(CareerId, element) {
    try {
        const toggleUrl = `${baseUrl}EsAdmin/Careers/ToggleStatus?id=${CareerId}`;

        const response = await fetch(toggleUrl, {
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
                const isActive = badgeSpan.attr("data-status") === "Active";

                // Update text and data attribute
                badgeSpan.hide()
                    .text(isActive ? "Not Active" : "Active")
                    .attr("data-status", isActive ? "NotActive" : "Active")
                    .fadeIn(200);

                // Toggle CSS classes
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
                title: currentLanguage === 'ar-JO' ? '! ليس لديك صلاحية لتعديل هذه الوظيفة' : 'You don\'t have permission to edit this Career!',
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

async function deleteCareer(CareerId, element) {
    // Confirm deletion using SweetAlert2
    const confirmDeletion = await Swal.fire({
        icon: 'warning',
        title: currentLanguage === 'ar-JO' ? 'هل أنت متأكد أنك تريد حذف هذه الوظيفة؟' : 'Are you sure you want to delete this Career?',
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم' : 'Yes',
        cancelButtonText: currentLanguage === 'ar-JO' ? 'لا' : 'No'
    });

    if (!confirmDeletion.isConfirmed) {
        return;
    }

    try {
        const deleteUrl = `${baseUrl}EsAdmin/Careers/Delete?id=${CareerId}`;
        // Make the fetch request without CSRF token
        const response = await fetch(deleteUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            }
        });

        // Debug the response object
        console.log('Response:', response);

        // Check if response is successful (status code 200-299)
        if (response.ok) {
            console.log('Career deleted successfully.');
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

            // Show success message using SweetAlert2
            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم حذف هذه الوظيفة بنجاح!' : 'Career deleted successfully!',
                showConfirmButton: true
            });

        } else if (response.status == 403) {
            // Check for permission error
            const responseText = await response.text();
            console.error('Failed to delete Career:', responseText);

            // Show permission error using SweetAlert2
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك صلاحية لحذف هذه الوظيفة!' : 'You don\'t have permission to delete this Career!',
                showConfirmButton: true
            });
        }
    } catch (error) {
        // Catch network or other unexpected errors
        console.error('Error deleting Career', error);

        // Show error message using SweetAlert2
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء حذف هذه الوظيفة.' : 'An error occurred while deleting the Career.',
            showConfirmButton: true
        });
    }
}
