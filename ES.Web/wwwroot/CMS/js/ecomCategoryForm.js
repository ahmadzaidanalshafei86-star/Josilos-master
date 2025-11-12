$(document).ready(function () {
    initializeSelect2('#parentCategoryDropdown');

    // Attach event listeners for all "Clear" buttons dynamically
    initializeClearButtons([
        { inputId: 'CoverImage', previewId: 'CoverImagePreview', buttonId: 'clearCoverImage', hiddenInputId: 'KeepCoverImage' },
        { inputId: 'FeaturedImage', previewId: 'FeaturedImagePreview', buttonId: 'clearFeaturedImage', hiddenInputId: 'KeepFeaturedImage' }
    ]);
});


// Initialize Select2
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
