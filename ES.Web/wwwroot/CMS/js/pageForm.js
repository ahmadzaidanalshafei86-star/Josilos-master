const baseUrl = window.appBaseUrl || '/';
$(document).ready(function () {
    // Initialize Select2
    // Determine the placeholder language based on the current language
    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#CategoryDropdown', 'حدد القسم');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#CategoryDropdown', 'Select the category');
    }

    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#relatedCategoriesDropdown', 'حدد الأقسام ذات الصلة (اختياري)');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#relatedCategoriesDropdown', 'Select related categories (optional)');
    }


    if (currentLanguage === 'ar-JO') {
        initializeSelect2('#FormDropdown');
    } else if (currentLanguage === 'en-JO') {
        initializeSelect2('#FormDropdown');
    }


    initializeTinyMCE();

    const videoUrlInput = $("input[name='VideoURL']");
    const initialUrl = videoUrlInput.val();
    if (initialUrl) {
        updateVideoPreview(initialUrl);
    }
    videoUrlInput.on("input", function () {
        updateVideoPreview(this.value);
    });

    // Attach event listeners for all "Clear" buttons dynamically
    initializeClearButtons([
        { inputId: 'CoverImage', previewId: 'CoverImagePreview', buttonId: 'clearCoverImage', hiddenInputId: 'KeepCoverImage' },
        { inputId: 'FeaturedImage', previewId: 'FeaturedImagePreview', buttonId: 'clearFeaturedImage', hiddenInputId: 'KeepFeaturedImage' }
    ]);


    // List of category IDs to check against
    let categoryIds = [];
    try {
        categoryIds = JSON.parse($('#categoryIds').val());
    } catch (e) {
        console.error('Error parsing category IDs:', e);
    }

    $('#CategoryDropdown').on('change', function () {
        const selectedCategoryId = $(this).val();
        const orderInput = $('#OrderInput');

        if (categoryIds.includes(parseInt(selectedCategoryId))) {
            orderInput.prop('disabled', true);
        } else {
            orderInput.prop('disabled', false);
        }
    });


    // Get references to the radio buttons, dropdowns, and input fields
    const radio1 = document.getElementById('radio1'); // Page
    const radio2 = document.getElementById('radio2'); // Category
    const radio3 = document.getElementById('radio3'); // File
    const radio4 = document.getElementById('radio4'); // Another link
    const dropdown2 = document.getElementById('dropdown2'); // Category dropdown
    const dropdown3 = document.getElementById('dropdown3'); // File dropdown
    const textInputDiv = document.getElementById('textInputDiv'); // Text input for URL
    const linkToInput = document.getElementById('LinkTo'); // Hidden input for LinkTo
    const linkToTypeInput = document.getElementById('LinkToType'); // Hidden input for LinkToType

    // Function to initialize the form based on the LinkToType value
    function initializeForm() {
        const linkToTypeValue = linkToTypeInput.value; // Get the value of LinkToType
        const linkToValue = linkToInput.value; // Get the value of LinkTo

        // Check the value of LinkToType and select the corresponding radio button
        switch (linkToTypeValue) {
            case '1': // Page
                radio1.checked = true;
                break;
            case '2': // Category
                radio2.checked = true;
                // Pre-select the category in the dropdown
                const categoryDropdown = document.getElementById('LinkToCategoryDropdown');
                if (categoryDropdown) {
                    categoryDropdown.value = linkToValue;
                }
                break;
            case '3': // File
                radio3.checked = true;
                // Pre-select the file in the dropdown
                const fileDropdown = document.getElementById('FileDropdown');
                if (fileDropdown) {
                    fileDropdown.value = linkToValue;
                }
                break;
            case '4': // Another link
                radio4.checked = true;
                // Pre-fill the text input with the URL
                const textInput = document.getElementById('textInput');
                if (textInput) {
                    textInput.value = linkToValue;
                }
                break;
            default:
                // Default to Page if no value is set
                radio1.checked = true;
                break;
        }

        // Update visibility and values based on the selected radio button
        toggleVisibility();
        updateLinkToAndType();

        //  Cover Image Validation
        $("#CoverImage").change(function () {
            validateImage(this, "CoverImagePreview", "coverImageError");
        });

        // Bind Featured Image Validation
        $("#FeaturedImage").change(function () {
            validateImage(this, "FeaturedImagePreview", "featuredImageError");
        });

        initializeTinyMCE();
    }

    // Function to update the LinkTo and LinkToType values
    function updateLinkToAndType() {
        if (radio1?.checked) {
            // If "Page" is selected, set LinkTo to null and LinkToType to "1"
            linkToInput.value = '';
            linkToTypeInput.value = '1';
        } else if (radio2?.checked) {
            // If "Category" is selected, set LinkTo to the selected category ID and LinkToType to "2"
            const categoryDropdown = document.getElementById('LinkToCategoryDropdown');
            linkToInput.value = categoryDropdown.value;
            linkToTypeInput.value = '2';
        } else if (radio3?.checked) {
            // If "File" is selected, set LinkTo to the selected file ID and LinkToType to "3"
            const fileDropdown = document.getElementById('FileDropdown');
            linkToInput.value = fileDropdown.value;
            linkToTypeInput.value = '3';
        } else if (radio4?.checked) {
            // If "Another link" is selected, set LinkTo to the entered URL and LinkToType to "4"
            const textInput = document.getElementById('textInput');
            linkToInput.value = textInput.value;
            linkToTypeInput.value = '4';
        }

        // Log the values for debugging (optional)
        console.log('LinkTo value:', linkToInput.value);
        console.log('LinkToType value:', linkToTypeInput.value);
    }

    // Function to show/hide dropdowns and input fields based on the selected radio button
    function toggleVisibility() {
        if (radio1?.checked) {
            // Hide all additional inputs
            dropdown2?.classList.add('d-none');
            dropdown3?.classList.add('d-none');
            textInputDiv?.classList.add('d-none');
        } else if (radio2?.checked) {
            // Show category dropdown and hide others
            dropdown2?.classList.remove('d-none');
            dropdown3?.classList.add('d-none');
            textInputDiv?.classList.add('d-none');
        } else if (radio3?.checked) {
            // Show file dropdown and hide others
            dropdown2?.classList.add('d-none');
            dropdown3?.classList.remove('d-none');
            textInputDiv?.classList.add('d-none');
        } else if (radio4?.checked) {
            // Show text input and hide others
            dropdown2?.classList.add('d-none');
            dropdown3?.classList.add('d-none');
            textInputDiv?.classList.remove('d-none');
        }
    }

    // Add event listeners to the radio buttons
    if (radio1) radio1.addEventListener('change', updateLinkToAndType);
    if (radio2) radio2.addEventListener('change', updateLinkToAndType);
    if (radio3) radio3.addEventListener('change', updateLinkToAndType);
    if (radio4) radio4.addEventListener('change', updateLinkToAndType);

    // Add event listeners to the dropdowns and text input
    document.getElementById('LinkToCategoryDropdown')?.addEventListener('change', updateLinkToAndType);
    document.getElementById('FileDropdown')?.addEventListener('change', updateLinkToAndType);
    document.getElementById('textInput')?.addEventListener('input', updateLinkToAndType);

    // Add event listeners to toggle visibility
    if (radio1) radio1.addEventListener('change', toggleVisibility);
    if (radio2) radio2.addEventListener('change', toggleVisibility);
    if (radio3) radio3.addEventListener('change', toggleVisibility);
    if (radio4) radio4.addEventListener('change', toggleVisibility);

    // Initialize the form based on the LinkToType value
    initializeForm();
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

function updateVideoPreview(url) {
    const iframe = $("#videoPreview");
    const videoContainer = $("#customVideoPreview");

    if (url.includes("youtube.com") || url.includes("youtu.be")) {
        const videoId = extractYouTubeID(url);
        iframe.attr("src", videoId ? `https://www.youtube.com/embed/${videoId}` : "").show();
        videoContainer.hide();
    } else if (url.includes("vimeo.com")) {
        const videoId = extractVimeoID(url);
        iframe.attr("src", videoId ? `https://player.vimeo.com/video/${videoId}` : "").show();
        videoContainer.hide();
    } else if (url.match(/\.(mp4|webm|ogg)$/i)) {
        // Handle direct video files
        videoContainer.html(`<video controls width="100%" autoplay loop><source src="${url}" type="video/mp4">Your browser does not support the video tag.</video>`).show();
        iframe.hide();
    } else {
        iframe.attr("src", "").hide();
        videoContainer.hide();
    }
}

function extractYouTubeID(url) {
    const match = url.match(/(?:youtube\.com\/(?:[^\/]+\/.+\/|(?:v|e(?:mbed)?)\/|.*[?&]v=)|youtu\.be\/)([^"&?\/\s]{11})/);
    return match ? match[1] : null;
}

function extractVimeoID(url) {
    const match = url.match(/vimeo\.com\/(\d+)/);
    return match ? match[1] : null;
}



function initializeTinyMCE(selector) {
    tinymce.init({
        ...tinyMCEInitSettings, // Use your global TinyMCE settings
        selector: selector
    });
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
                const imageUrl = `${baseUrl}images/Pages/GalleryImages/` + image.galleryImageUrl;
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



// Document Dropzone Configuration
var removedDocs = []; // Track removed Word/PDF files

var docDropzone = new Dropzone("#docDropzone", {
    previewsContainer: "#sortableDocPreviews",
    url: "#",
    autoProcessQueue: false,
    maxFiles: 10,
    maxFilesize: 10, // File size limit in MB
    acceptedFiles: ".pdf,.doc,.docx",
    addRemoveLinks: true,
    dictDefaultMessage: 'Drag and drop Word/PDF files here or click to upload',
    init: async function () {
        var dropzoneInstance = this;

        // Load existing documents for editing
        if (existingDocuments && existingDocuments.length > 0) {
            for (const doc of existingDocuments) {
                const docUrl = `${baseUrl}CMS/documents/Pages/` + doc.fileUrl;
                const fileName = doc.fileAltName;

                // Fetch the document and convert it to a File object
                const file = await fetchImageAsFile(docUrl, fileName);

                const mockFile = {
                    name: fileName,
                    size: file.size,
                    accepted: true,
                    status: Dropzone.ADDED,
                    previewElement: null
                };

                // Add the mock file to Dropzone
                dropzoneInstance.emit("addedfile", mockFile);

                // Add an icon for document preview
                const previewElement = mockFile.previewElement;
                if (previewElement) {
                    const thumbnailElement = previewElement.querySelector('.dz-image');
                    if (thumbnailElement) {
                        thumbnailElement.innerHTML = '';
                        const imgElement = document.createElement('img');
                        imgElement.style.width = "120"; // Adjust the size as needed
                        imgElement.style.height = "120px"; // Adjust the size as needed
                        imgElement.style.margin = "auto"; // Center the icon

                        // Choose an appropriate image based on the file type
                        if (fileName.endsWith(".pdf")) {
                            imgElement.src = `${baseUrl}CMS/assets/images/icons/PDFIc.png`; // Path to your PDF icon
                        } else if (fileName.endsWith(".doc") || fileName.endsWith(".docx")) {
                            imgElement.src = `${baseUrl}CMS/assets/images/icons/doc.png`; // Path to your Word icon
                        } else {
                            imgElement.src = `${baseUrl}CMS/assets/images/icons/fileIcon.png`; // Default icon
                        }

                        thumbnailElement.appendChild(imgElement);
                    }

                    // Add a clickable link around the file name
                    const fileNameElement = previewElement.querySelector("[data-dz-name]");
                    if (fileNameElement) {
                        const fileLink = document.createElement("a");
                        fileLink.href = docUrl; // Use the document URL here
                        fileLink.target = "_blank"; // Open in a new tab
                        fileLink.style.textDecoration = "none";
                        fileLink.style.color = "inherit";
                        fileLink.innerText = fileName;

                        // Replace the text content with the clickable link
                        fileNameElement.innerHTML = "";
                        fileNameElement.appendChild(fileLink);
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
            removedDocs.push(file.name); // Add the removed file's name to the list
            console.log("Removed file:", file.name);
        });

        // Make previews sortable
        Sortable.create(document.querySelector("#sortableDocPreviews"), {
            animation: 380,
            draggable: ".dz-preview",
            onEnd: function () {
                var sortedPreviews = document.querySelectorAll("#sortableDocPreviews .dz-preview");
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

// Form Submission Logic (Including Documents)
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

    if (!isValid) {
        console.log("Form submission prevented due to validation errors.");
        return;
    }

    // Append gallery images
    myDropzone.files.forEach((file) => {
        if (!removedFiles.includes(file.name)) {
            formData.append("GalleryImages", file);
        }
    });

    // Append documents
    docDropzone.files.forEach((file) => {
        if (!removedDocs.includes(file.name)) {
            formData.append("PageFiles", file);
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
                       // window.location.href = `${baseUrlWithSlash}EsAdmin/Pages/Edit/${data.id}`;

                        window.location.href = `${baseUrlWithSlash}EsAdmin/PagesTranslates?pageId=${data.id}`;
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

