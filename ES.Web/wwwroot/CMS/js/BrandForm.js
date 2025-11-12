// Preview the image
function previewImage(event, previewElementId) {
    const input = event.target;
    const preview = document.getElementById(previewElementId);

    if (input.files && input.files[0]) {
        const reader = new FileReader();

        reader.onload = function (e) {
            preview.src = e.target.result;
        };

        reader.readAsDataURL(input.files[0]);
    }
}

// Clear the file input and reset preview to placeholder
function clearFileInput(inputId, previewElementId) {
    const input = document.getElementById(inputId);
    const preview = document.getElementById(previewElementId);

    if (input) {
        // Create a new file input element to properly clear selection
        const newInput = document.createElement("input");
        newInput.type = "file";
        newInput.id = input.id;
        newInput.name = input.name;
        newInput.className = input.className;
        newInput.accept = input.accept;
        newInput.onchange = input.onchange;

        input.parentNode.replaceChild(newInput, input);
    }

    // Reset the preview to the placeholder image
    preview.src = "/images/logo-placeholder.png";
}
