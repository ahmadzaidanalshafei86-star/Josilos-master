const baseUrl = window.appBaseUrl || '/';

document.addEventListener("DOMContentLoaded", function () {
    initMediaManagement();
});

function initMediaManagement() {
    setupCopyUrlFeature();
    setupFiltering();
    setupEditableAltNames();
    setupImageCropping();
}

//  Image Cropping Logic
let cropper;
function setupImageCropping() {
    document.querySelectorAll(".edit-image-btn").forEach(button => {
        button.addEventListener("click", function () {
            let imageUrl = this.getAttribute("data-url");
            let cropperImage = document.getElementById("cropperImage");

            if (!cropperImage) {
                console.error("Cropper image element not found!");
                return;
            }

            cropperImage.src = imageUrl;

            // Destroy previous Cropper instance if it exists
            if (cropper) {
                cropper.destroy();
            }

            cropperImage.style.maxWidth = "100%";
            cropperImage.style.maxHeight = "100%";
            cropperImage.style.objectFit = "contain";

            cropper = new Cropper(cropperImage, {
                aspectRatio: NaN, // Free cropping
                viewMode: 2, // Ensures crop box stays inside the image
                autoCropArea: 0.9, // Default crop area
                responsive: true,
                scalable: true,
                zoomable: true,
                movable: true, // Allow moving the image
                rotatable: true, // Allow rotating the image
                cropBoxMovable: true, // Move the crop box
                cropBoxResizable: true, // Resize the crop box
                dragMode: "move", // Default mode is move
                zoomOnWheel: true, // Scroll to zoom
                zoomOnTouch: true, // Pinch to zoom
                toggleDragModeOnDblclick: false, // Prevent accidental switching
                restore: false
            });


            // Show the cropping modal
            let modalElement = document.getElementById('imageCropperModal');
            if (modalElement) {
                new bootstrap.Modal(modalElement).show();
            } else {
                console.error("Modal element not found!");
            }
        });
    });

    // Handle Rotation Buttons
    let rotateLeftBtn = document.querySelector(".rotate-left");
    if (rotateLeftBtn) {
        rotateLeftBtn.addEventListener("click", function () {
            if (cropper) cropper.rotate(-90);
        });
    }

    let rotateRightBtn = document.querySelector(".rotate-right");
    if (rotateRightBtn) {
        rotateRightBtn.addEventListener("click", function () {
            if (cropper) cropper.rotate(90);
        });
    }

    // Handle Crop & Save Button Click
    let cropImageBtn = document.getElementById("cropImageBtn");
    if (cropImageBtn) {
        cropImageBtn.addEventListener("click", function () {
            if (!cropper) return;

            let croppedCanvas = cropper.getCroppedCanvas();
            if (!croppedCanvas) {
                console.error("Cropping failed!");
                return;
            }

            croppedCanvas.toBlob(blob => {
                let formData = new FormData();
                formData.append("croppedImage", blob, "cropped-image.jpg");

                let cropperImage = document.getElementById("cropperImage");
                if (cropperImage) {
                    formData.append("originalImageUrl", cropperImage.src);
                }

                const Url = `${baseUrl}EsAdmin/MediaManagement/UploadCroppedImage`;
                fetch(Url, {
                    method: "POST",
                    body: formData,
                    headers: { "Accept": "application/json" }
                })
                    .then(response => {
                        if (response.status === 403) {
                            Swal.fire({
                                title: currentLanguage === 'ar-JO' ? "ممنوع!" : "Forbidden!",
                                text: currentLanguage === 'ar-JO' ? "ليس لديك إذن للتعديل !" : "You don't have permission to update!",
                                icon: "error"
                            });
                            throw new Error("Permission Denied");
                        }
                        return response.json();
                    })
                    .then(data => {
                        console.log("Server Response:", data);

                        if (!data.imageUrl) {
                            console.error("Error: imageUrl is undefined!");
                            return;
                        }

                        let baseUrl = window.location.origin;
                        let newImageUrl = data.imageUrl.startsWith("http")
                            ? data.imageUrl
                            : `${baseUrl}${data.imageUrl}?nocache=${new Date().getTime()}`;

                        // Remove old images and replace them dynamically
                        document.querySelectorAll(`[data-url="${data.imageUrl}"]`).forEach(img => {
                            let parent = img.parentNode;
                            let newImg = new Image();
                            newImg.src = newImageUrl;
                            newImg.className = img.className;
                            newImg.dataset.url = data.imageUrl;

                            // Ensure the image loads before replacing
                            newImg.onload = function () {
                                parent.replaceChild(newImg, img);
                                console.log("Image updated successfully!");
                            };
                        });

                        // Also update the cropper preview image
                        if (cropperImage) {
                            cropperImage.src = newImageUrl;
                        }

                        Swal.fire({
                            title: currentLanguage === 'ar-JO' ? "تم التعديل!" : "Updated!",
                            text: currentLanguage === 'ar-JO' ? "تم التعديل بنجاح!" : "Updated successfully!",
                            icon: "success",
                            timer: 2000,
                            showConfirmButton: false
                        });

                        // Hide the save button after a successful update
                        let saveButton = document.getElementById("saveButton");
                        if (saveButton) {
                            saveButton.classList.add("d-none");
                        }

                        // Force a refresh of the image list without reloading the page
                        setTimeout(() => {
                            document.querySelectorAll("img").forEach(img => {
                                if (img.src.includes(data.imageUrl)) {
                                    img.src = `${img.src.split("?")[0]}?nocache=${new Date().getTime()}`;
                                }
                            });
                        }, 500);
                    })
                    .catch(error => {
                        console.error("Error:", error);
                        Swal.fire({
                            title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
                            text: currentLanguage === 'ar-JO' ? "حدث خطأ أثناء معالجة الطلب!" : "An error occurred while processing the request!",
                            icon: "error"
                        });
                    });

                // Hide modal after cropping
                let modalElement = document.getElementById('imageCropperModal');
                if (modalElement) {
                    bootstrap.Modal.getInstance(modalElement).hide();
                }
            });
        });
    }
}



// Copy URL to Clipboard
function setupCopyUrlFeature() {
    document.querySelectorAll(".copy-url").forEach(button => {
        button.addEventListener("click", function () {
            let url = this.getAttribute("data-url");

            navigator.clipboard.writeText(url).then(() => {
                Swal.fire({
                    title: currentLanguage === 'ar-JO' ? "تم النسخ!" : "Copied!",
                    text: currentLanguage === 'ar-JO' ? "تم نسخ الرابط إلى الحافظة!" : "URL copied to clipboard!",
                    icon: "success",
                    timer: 2000,
                    showConfirmButton: false
                });
            }).catch(err => {
                console.error("Failed to copy:", err);
                Swal.fire({
                    title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
                    text: currentLanguage === 'ar-JO' ? "فشل نسخ الرابط!" : "Failed to copy the URL!",
                    icon: "error"
                });
            });
        });
    });
}

// Pagination and Filtering
function setupFiltering() {
    let allMediaItems = [...document.querySelectorAll(".media-item")];
    const itemsPerPage = 6;
    let currentPage = 1;

    function filterMedia() {
        let searchTerm = document.getElementById("searchTerm").value.toLowerCase();
        let selectedType = document.getElementById("mediaType").value;
        let selectedCategory = document.getElementById("mediaCategory").value;
        let selectedPage = document.getElementById("mediaPage").value;

        let filteredItems = allMediaItems.filter(item => {
            let altName = item.getAttribute("data-altname").toLowerCase();
            let type = item.getAttribute("data-type");
            let category = item.getAttribute("data-category");
            let page = item.getAttribute("data-page");

            let matchesSearch = searchTerm === "" || altName.includes(searchTerm);
            let matchesType = selectedType === "" || type === selectedType;
            let matchesCategory = selectedCategory === "" || (category && category === selectedCategory);
            let matchesPage = selectedPage === "" || (page && page === selectedPage);

            return matchesSearch && matchesType && matchesCategory && matchesPage;
        });

        currentPage = 1; // Reset to first page when filtering
        setupPagination(filteredItems);
    }

    function setupPagination(mediaItems) {
        let totalPages = Math.ceil(mediaItems.length / itemsPerPage);
        if (totalPages === 0) totalPages = 1;

        function showPage(page) {
            let start = (page - 1) * itemsPerPage;
            let end = start + itemsPerPage;

            allMediaItems.forEach(item => item.style.display = "none");
            mediaItems.slice(start, end).forEach(item => item.style.display = "block");

            updatePagination(page, totalPages, mediaItems);
        }

        function updatePagination(page, totalPages, mediaItems) {
            let paginationContainer = document.getElementById("pagination");
            paginationContainer.innerHTML = ""; // Clear old pagination

            let paginationHTML = `
                <li class="page-item ${page === 1 ? "disabled" : ""}">
                    <a class="page-link pagination-link" href="#" data-page="${page - 1}">«</a>
                </li>
            `;

            for (let i = 1; i <= totalPages; i++) {
                paginationHTML += `
                    <li class="page-item ${i === page ? "active" : ""}">
                        <a class="page-link pagination-link" href="#" data-page="${i}">${i}</a>
                    </li>
                `;
            }

            paginationHTML += `
                <li class="page-item ${page === totalPages ? "disabled" : ""}">
                    <a class="page-link pagination-link" href="#" data-page="${page + 1}">»</a>
                </li>
            `;

            paginationContainer.innerHTML = paginationHTML;

            document.querySelectorAll(".pagination-link").forEach(link => {
                link.addEventListener("click", function (e) {
                    e.preventDefault();
                    let newPage = parseInt(this.dataset.page);
                    if (newPage > 0 && newPage <= totalPages) {
                        currentPage = newPage;
                        showPage(currentPage);
                    }
                });
            });
        }

        showPage(currentPage);
    }

    document.getElementById("searchTerm").addEventListener("keyup", filterMedia);
    document.getElementById("mediaType").addEventListener("change", filterMedia);
    document.getElementById("mediaCategory").addEventListener("change", filterMedia);
    document.getElementById("mediaPage").addEventListener("change", filterMedia);

    setupPagination(allMediaItems); // Initialize on load
}

// Editable Alt Names with Save Button
function setupEditableAltNames() {
    document.querySelectorAll(".editable-alt").forEach(element => {
        let saveButton = document.createElement("button");
        saveButton.innerHTML = '<i class="lni lni-save"></i>';
        saveButton.classList.add("btn", "btn-sm", "border-0", "shadow-sm", "bg-info", "text-white", "rounded", "d-none");
        saveButton.setAttribute("title", "Save Alt Name");

        // Button Styling
        saveButton.style.width = "48px";
        saveButton.style.height = "32px";
        saveButton.style.display = "flex";
        saveButton.style.alignItems = "center";
        saveButton.style.justifyContent = "center";
        saveButton.style.margin = "0 auto"; // Center horizontally  
        saveButton.style.transition = "background-color 0.2s, transform 0.2s";
        saveButton.style.fontSize = "16px"; // Adjust icon size for better visibility

        // Hover Effect
        saveButton.addEventListener("mouseenter", () => {
            saveButton.style.backgroundColor = "#28a745"; // Slightly lighter green
            saveButton.style.transform = "scale(1.05)";
        });

        saveButton.addEventListener("mouseleave", () => {
            saveButton.style.backgroundColor = "rgb(40, 167, 69)";
            saveButton.style.transform = "scale(1)";
        });


        element.insertAdjacentElement("afterend", saveButton);

        element.addEventListener("input", function () {
            saveButton.classList.remove("d-none");
        });

        saveButton.addEventListener("click", function () {
            let mediaId = element.getAttribute("data-id");
            let mediaType = element.getAttribute("data-type");
            let newAltName = element.innerText.trim();

            fetch(`/MediaManagement/UpdateAltName`, {
                method: "POST",
                headers: {
                    "Content-Type": "application/json",
                    "X-CSRF-TOKEN": document.querySelector("input[name='__RequestVerificationToken']").value
                },
                body: JSON.stringify({ id: mediaId, type: mediaType, altName: newAltName })
            })
                .then(response => {
                    if (response.status === 403) {
                        // Permission error handling
                        Swal.fire({
                            title: currentLanguage === 'ar-JO' ? "ممنوع!" : "Forbidden!",
                            text: currentLanguage === 'ar-JO' ? "ليس لديك إذن للتعديل !" : "You don't have permission to update!",
                            icon: "error"
                        });
                        throw new Error("Permission Denied");
                    }
                    return response.json();
                })
                .then(data => {
                    if (data.success) {
                        // Success handling
                        Swal.fire({
                            title: currentLanguage === 'ar-JO' ? "تم التعديل!" : "Updated!",
                            text: currentLanguage === 'ar-JO' ? "تم التعديل بنجاح!" : "Done successfully!",
                            icon: "success",
                            timer: 2000,
                            showConfirmButton: false
                        });

                        // Hide the save button after success
                        let saveButton = document.getElementById("saveButton");
                        if (saveButton) {
                            saveButton.classList.add("d-none");
                        }
                    } else {
                        // Failure handling
                        Swal.fire({
                            title: currentLanguage === 'ar-JO' ? "فشل التعديل!" : "Update Failed!",
                            text: currentLanguage === 'ar-JO' ? "فشل تحديث الاسم البديل!" : "Failed to update Alt Name!",
                            icon: "error"
                        });
                    }
                })
                .catch(error => {
                    console.error("Error:", error);
                    Swal.fire({
                        title: currentLanguage === 'ar-JO' ? "خطأ!" : "Error!",
                        text: currentLanguage === 'ar-JO' ? "حدث خطأ أثناء معالجة الطلب!" : "An error occurred while processing the request!",
                        icon: "error"
                    });
                });

        });
    });
}

