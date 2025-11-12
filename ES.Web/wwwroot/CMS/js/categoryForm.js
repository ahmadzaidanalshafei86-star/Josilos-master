const baseUrl = window.appBaseUrl || '/';

$(document).ready(function () {
     initializeSelect2('#parentCategoryDropdown');
     
    // Determine the placeholder language based on the current language
    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#relatedCategoriesDropdown','حدد الأقسام ذات الصلة');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#relatedCategoriesDropdown', 'Select related categories');
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

//Gallery Images
// Get existing images from the server
Dropzone.autoDiscover = false;
var removedFiles = []; // Global array to track removed files


var myDropzone = new Dropzone("#myDropzone", {
    previewsContainer: "#sortablePreviews", // Target the custom container for previews
    url: "#",
    autoProcessQueue: false,
    maxFiles: 10,
    maxFilesize: 20, // File size limit in MB
    acceptedFiles: ".jpg,.jpeg,.png,.gif",
    addRemoveLinks: true,
    dictDefaultMessage: 'Drag and drop gallery images here or click to upload',
    init: async function () {
        var dropzoneInstance = this;

        // Load existing images for editing
        if (existingGalleryImages && existingGalleryImages.length > 0) {
            for (const image of existingGalleryImages) {
                const imageUrl = '/images/Categories/GalleryImages/' + image.galleryImageUrl;
                const fileName = image.galleryImageAltName;


                // Fetch the image and convert it to a File object
                const file = await fetchImageAsFile(imageUrl, fileName);

                // Create a mockFile object to display the image in Dropzone
                const mockFile = {
                    name: fileName,
                    size: file.size,
                    accepted: true,
                    status: Dropzone.ADDED,
                    previewElement: null
                };

                // Add the mock file to Dropzone
                dropzoneInstance.emit("addedfile", mockFile);

                // Customize the preview element
                const previewElement = mockFile.previewElement;
                if (previewElement) {
                    const thumbnailElement = previewElement.querySelector('.dz-image');
                    if (thumbnailElement) {
                        thumbnailElement.innerHTML = '';
                        const imgElement = document.createElement('img');
                        imgElement.src = imageUrl;
                        imgElement.style.width = "100%";
                        imgElement.style.height = "100%";
                        thumbnailElement.appendChild(imgElement);
                    }
                }

                // Mark the file as successfully uploaded
                mockFile.status = Dropzone.SUCCESS;
                mockFile.previewElement.classList.add("dz-success", "dz-complete");

                // Add the File object to Dropzone's files array
                dropzoneInstance.files.push(file);
            }
        }

        // Track removed files
        dropzoneInstance.on("removedfile", function (file) {
            removedFiles.push(file.name); // Add the removed file's name to the list
            console.log("Removed file:", file.name);
        });

        // Make previews sortable
        Sortable.create(document.querySelector("#sortablePreviews"), {
            animation: 380,
            draggable: ".dz-preview",
            onEnd: function () {
                var sortedPreviews = document.querySelectorAll("#sortablePreviews .dz-preview");
                var newFilesOrder = [];

                sortedPreviews.forEach(function (previewElement) {
                    var fileName = previewElement.querySelector("[data-dz-name]").innerText;
                    var matchedFile = dropzoneInstance.files.find(file => file.name === fileName);
                    if (matchedFile) {
                        newFilesOrder.push(matchedFile);
                    }
                });

                dropzoneInstance.files = newFilesOrder;
                console.log("New file order:", dropzoneInstance.files.map(file => file.name));
            }
        });
    }
});


// Function to fetch an image and convert it to a File object
async function fetchImageAsFile(imageUrl, fileName) {
    const response = await fetch(imageUrl);
    const blob = await response.blob();
    return new File([blob], fileName, { type: blob.type });
}


document.getElementById("my-Form").addEventListener("submit", function (e) {
    e.preventDefault(); // Prevent default form submission

    tinymce.triggerSave();

    const formData = new FormData(this);

    let isValid = true;

    // Validate Cover Image
    if (!validateImage(document.getElementById("CoverImage"), "coverImageError")) {
        isValid = false;
    }
    // Validate Featured Image
    if (!validateImage(document.getElementById("FeaturedImage"), "featuredImageError")) {
        isValid = false;
    }
    
    // Append gallery images
    myDropzone.files.forEach((file) => {
        if (!removedFiles.includes(file.name)) {
            formData.append("GalleryImages", file);
        }
    });

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
                        // 🚀 Redirect only after create
                        //window.location.href = `${baseUrlWithSlash}EsAdmin/Categories/Edit/${data.id}`;

                        window.location.href = `${baseUrlWithSlash}EsAdmin/CategoriesTranslates?categoryId=${data.id}`;
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