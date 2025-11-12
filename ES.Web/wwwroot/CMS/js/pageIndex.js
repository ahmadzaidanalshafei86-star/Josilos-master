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

    // Initialize DataTable
    var table = $('#table').DataTable({
        "pageLength": 50,
        "lengthMenu": [[50, 75, 100, 120], [50, 75, 100, 120]],
        "language": {
            "url": languageUrl
        },
        "order": [[1, "asc"]]
    });

    // Category filter functionality
    $('#categoryFilter').on('change', function () {
        var selectedCategory = this.value;

        if (selectedCategory === '') {
            // Show all rows
            table.column(1).search('').draw();
        } else {
            // Filter by selected category
            table.column(1).search('^' + selectedCategory + '$', true, false).draw();
        }
    });

    // Toggle user status
    $(document).on('click', '.toggle-status', function () {
        const pageId = $(this).data('page-id');
        togglePageStatus(pageId, this);
    });

    // Store original value and handle input changes
    $('.order-input').each(function () {
        const $input = $(this);
        $input.data('original-value', $input.val());
    });

    // Show/hide save icon on change
    $('.order-input').on('input', function () {
        const $input = $(this);
        const $saveIcon = $input.next('.save-order');
        const currentValue = $input.val();
        const originalValue = $input.data('original-value');

        if (currentValue !== originalValue && currentValue && !isNaN(currentValue)) {
            $saveIcon.fadeIn(200);
        } else {
            $saveIcon.fadeOut(200);
        }
    });

    // Enter key handler
    $('.order-input').on('keypress', async function (e) {
        if (e.which === 13) {
            e.preventDefault();
            const $input = $(this);
            const pageId = $input.data('page-id');
            const newOrder = $input.val();

            if (!newOrder || isNaN(newOrder)) {
                $input.addClass('is-invalid');
                return;
            }

            await updateOrder($input, pageId, newOrder);
        }
    });

    // Save icon click handler
    $('.save-order').on('click', async function () {
        const $saveIcon = $(this);
        const pageId = $saveIcon.data('page-id');
        const $input = $saveIcon.prev('.order-input');
        const newOrder = $input.val();

        if (!newOrder || isNaN(newOrder)) {
            $input.addClass('is-invalid');
            return;
        }

        await updateOrder($input, pageId, newOrder);
    });
   
});



async function updateOrder($input, pageId, newOrder) {
    try {
        $input.prop('disabled', true);
        const $saveIcon = $input.next('.save-order');
        $saveIcon.addClass('is-loading');

        const updateUrl = `${baseUrl}EsAdmin/Pages/UpdateOrder?id=${pageId}&order=${newOrder}`;

        const response = await fetch (updateUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            }
        });

        if (response.status === 200) {
            $input.removeClass('is-loading').addClass('is-valid');
            $saveIcon.removeClass('is-loading').fadeOut(200);
            $input.data('original-value', newOrder); // Update original value after success

            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم بنجاح!' : 'Done successfully!',
                showConfirmButton: true
            });

            setTimeout(() => $input.removeClass('is-valid'), 1000);
        } else if (response.status === 403) {
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن لتعديل هذه الصفحة!' : 'You don\'t have permission to edit this page!',
                showConfirmButton: true
            });
        } else {
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'لا يمكن تحديث الترتيب' : 'Can\'t update order',
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
    } finally {
        $input.prop('disabled', false);
        $input.next('.save-order').removeClass('is-loading');
    }
}

async function togglePageStatus(pageId, element) {
    try {
        const ToggleUrl = `${baseUrl}EsAdmin/Pages/ToggleStatus?id=${pageId}`;
        const response = await fetch(ToggleUrl, {
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
                location.reload(); // Reload the page after 3 seconds
            }, 2000);
        } else if (response.status === 403) {
            // Handle permission error with SweetAlert2
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن لتعديل حالة هذه الصفحة!' : 'You don\'t have permission to edit this page!',
                showConfirmButton: true
            });
        } else {
            // Show generic error for other status codes
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'لا يمكن نشر الصفحة' : 'Can\'t publish',
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

async function deletePage(PageId, element) {
    console.log('deletePage called for ID:', PageId); // Debug

    // Confirm deletion using SweetAlert2
    const confirmDeletion = await Swal.fire({
        icon: 'warning',
        title: currentLanguage === 'ar-JO' ? 'هل أنت متأكد من أنك تريد حذف هذه الصفحة؟' : 'Are you sure you want to delete this Page?',
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم' : 'Yes',
        cancelButtonText: currentLanguage === 'ar-JO' ? 'لا' : 'No'
    });

    if (!confirmDeletion.isConfirmed) {
        return;
    }

    try {
        const DeleteUrl = `${baseUrl}EsAdmin/Pages/Delete?id=${PageId}`;
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
            console.log('Page deleted successfully.');
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

            // Show success message using SweetAlert2
            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم حذف الصفحة بنجاح !' : 'Page deleted successfully !',
                showConfirmButton: true
            });

        } else if (response.status == 403) {
            // Check for permission error
            const responseText = await response.text();
            console.error('Failed to delete Page:', responseText);

            // Show permission error using SweetAlert2
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن لحذف هذه الصفحة !' : 'You don\'t have permission to delete this page !',
                showConfirmButton: true
            });
        }
    } catch (error) {
        // Catch network or other unexpected errors
        console.error('Error deleting page', error);

        // Show error message using SweetAlert2
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء حذف الصفحة' : 'An error occurred while deleting the page.',
            showConfirmButton: true
        });
    }
}

