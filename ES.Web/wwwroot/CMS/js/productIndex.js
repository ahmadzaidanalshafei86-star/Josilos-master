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

    var table = $('#table').DataTable({
        "pageLength": 25,
        "lengthMenu": [[25, 50, 75, 100], [25, 50, 75, 100]],
        fixedHeader: true,
        "language": {
            "url": languageUrl
        }
    });

    // Custom Filter by Category
    $('#categoryFilter').on('change', function () {
        var selectedCategory = $(this).val();
        if (selectedCategory) {
            table.column(2).search(selectedCategory).draw(); // Column index 2 = Categories
        } else {
            table.column(2).search("").draw();
        }
    });

    // Toggle user status
    $(document).on('click', '.toggle-status', function () {
        const product = $(this).data('product-id');
        toggleStatus(product, this);
    });


});

async function toggleStatus(productId, element) {
    try {
        const ToggoleUrl = `${baseUrl}EsAdmin/Products/ToggleStatus?id=${productId}`;
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
                location.reload(); // Reload the page after 3 seconds
            }, 2000);
        } else if (response.status === 403) {
            // Handle permission error with SweetAlert2
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن لتعديل حالة هذا المنتج!' : 'You don\'t have permission to edit this product!',
                showConfirmButton: true
            });
        } else {
            // Show generic error for other status codes
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'لا يمكن نشر المنتج' : 'Can\'t publish',
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

async function deleteProduct(productId, element) {

    // Confirm deletion using SweetAlert2
    const confirmDeletion = await Swal.fire({
        icon: 'warning',
        title: currentLanguage === 'ar-JO' ? 'هل أنت متأكد من أنك تريد حذف هذا المنتج ؟' : 'Are you sure you want to delete this Product?',
        showCancelButton: true,
        confirmButtonText: currentLanguage === 'ar-JO' ? 'نعم' : 'Yes',
        cancelButtonText: currentLanguage === 'ar-JO' ? 'لا' : 'No'
    });

    if (!confirmDeletion.isConfirmed) {
        return;
    }

    try {
        const DeleteUrl = `${baseUrl}EsAdmin/Products/Delete?id=${productId}`;
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
            console.log('product deleted successfully.');
            const row = $(element).closest('tr');
            row.fadeOut(500, () => row.remove());

            // Show success message using SweetAlert2
            Swal.fire({
                icon: 'success',
                title: currentLanguage === 'ar-JO' ? 'تم حذف الصفحة بنجاح !' : 'Product deleted successfully !',
                showConfirmButton: true
            });

        } else if (response.status == 403) {
            // Check for permission error
            const responseText = await response.text();
            console.error('Failed to delete Product:', responseText);

            // Show permission error using SweetAlert2
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO' ? 'ليس لديك إذن لحذف هذا المنتج  !' : 'You don\'t have permission to delete this product !',
                showConfirmButton: true
            });
        }
    } catch (error) {
        // Catch network or other unexpected errors
        console.error('Error deleting page', error);

        // Show error message using SweetAlert2
        Swal.fire({
            icon: 'error',
            title: currentLanguage === 'ar-JO' ? 'حدث خطأ أثناء حذف المنتج' : 'An error occurred while deleting the product.',
            showConfirmButton: true
        });
    }
}


