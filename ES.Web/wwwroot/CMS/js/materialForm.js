const baseUrl = window.appBaseUrl || '/';

$(document).ready(function () {
    initializeSelect2('#parentMaterialDropdown');

    // Determine the placeholder language based on the current language
    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#relatedMaterialsDropdown', 'حدد المواد ذات الصلة');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#relatedMaterialsDropdown', 'Select related materials');
    }

    // Attach event listeners for all "Clear" buttons dynamically
    initializeClearButtons([
        { inputId: 'CoverImage', previewId: 'CoverImagePreview', buttonId: 'clearCoverImage', hiddenInputId: 'KeepCoverImage' },
        { inputId: 'FeaturedImage', previewId: 'FeaturedImagePreview', buttonId: 'clearFeaturedImage', hiddenInputId: 'KeepFeaturedImage' }
    ]);
});

// Initialize select2
function initializeSelect2(selector, placeholderText) {
    $(selector).select2({
        theme: "bootstrap-5",
        placeholder: placeholderText,
        minimumResultsForSearch: 0 // Enables search for all dropdowns
    });
}

// Dynamically attach event listeners for clear buttons
function initializeClearButtons(buttonsConfig) {
    buttonsConfig.forEach(function (config) {
        const button = document.getElementById(config.buttonId);
        if (button) {
            button.addEventListener('click', function () {
                clearFileInput(config.inputId, config.previewId, config.hiddenInputId);
            });
        }
    });
}

// Preview the image
function previewImage(event, previewElementId) {
    const input = event.target;
    const preview = document.getElementById(previewElementId);

    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;

            // Reset the hidden input to true when a new image is selected
            const hiddenInputId = input.getAttribute('data-hidden-input-id');
            if (hiddenInputId) {
                const hiddenInput = document.getElementById(hiddenInputId);
                if (hiddenInput) hiddenInput.value = "true";
            }
        };

        reader.readAsDataURL(input.files[0]);
    }
}

// Clear the file input, reset the preview, and update the hidden input
function clearFileInput(inputId, previewElementId, hiddenInputId) {
    const input = document.getElementById(inputId);
    const preview = document.getElementById(previewElementId);
    const hiddenInput = document.getElementById(hiddenInputId);

    // Clear the file input value
    input.value = null;

    // Reset the preview to the placeholder image
    preview.src = "/images/placeholder.png";

    // Set the hidden input value to false
    if (hiddenInput) {
        hiddenInput.value = "false";
    }
}

function validateImage(input, errorElementId) {
    const errorElement = document.getElementById(errorElementId);
    if (!errorElement) {
        console.warn(`Error element not found: ${errorElementId}`);
        return true;
    }

    errorElement.textContent = "";

    if (input.files.length > 0) {
        const file = input.files[0];
        const validExtensions = ["image/jpeg", "image/jpg", "image/png", "image/webp"];
        const maxSize = 20 * 1024 * 1024; // 20MB

        if (!validExtensions.includes(file.type)) {
            errorElement.textContent = "Invalid file type. Only JPG, PNG, and WEBP are allowed.";
            return false;
        }
        if (file.size > maxSize) {
            errorElement.textContent = "File size must be 20MB or less.";
            return false;
        }
    }
    return true;
}

// Gallery Images
// Get existing images from the server

var removedFiles = []; // Global array to track removed files





document.getElementById("my-Form").addEventListener("submit", function (e) {
    e.preventDefault();
    tinymce.triggerSave();
   // console.log("test");
    const formData = new FormData(this);
    let isValid = true;

   
    //console.log("test");
    // Submit the form using Fetch API
    fetch(this.action, {
        method: "POST",
        body: formData,
    })
        .then((response) => response.json())
        .then((data) => {
            if (data.success) {
                Swal.fire({
                    icon: 'success',
                    title: currentLanguage === 'ar-JO' ? 'تم بنجاح!' : 'Done successfully!',
                    showConfirmButton: false,
                    timer: 1500
                }).then(() => {
                    const baseUrlWithSlash = baseUrl.endsWith("/") ? baseUrl : baseUrl + "/";

                    if (data.id) {
                        // Redirect after create
                        //window.location.href = `${baseUrlWithSlash}EsAdmin/Materials/Edit/${data.id}`;
                        window.location.href = `${baseUrlWithSlash}EsAdmin/MaterialsTranslates?materialId=${data.id}`;
                    }
                });
            } else {
                Swal.fire({
                    icon: 'error',
                    title: currentLanguage === 'ar-JO'
                        ? 'حدث خطأ أثناء إرسال النموذج.'
                        : 'An error occurred while submitting the form.',
                    showConfirmButton: true
                });
            }
        })
        .catch((error) => {
            console.error("Form submission error:", error);
            Swal.fire({
                icon: 'error',
                title: currentLanguage === 'ar-JO'
                    ? 'حدث خطأ أثناء إرسال النموذج.'
                    : 'An error occurred while submitting the form.',
                showConfirmButton: true
            });
        });
});
