function setupClearButton(imgTagId, clearButtonId, fileInputId, defaultCoverImage, hiddenFieldId = null) {
    const imgTag = document.getElementById(imgTagId);
    const clearButton = document.getElementById(clearButtonId);
    const fileInput = document.getElementById(fileInputId);

    if (!imgTag || !clearButton || !fileInput) {
        console.error(`Invalid IDs provided. Make sure #${imgTagId}, #${clearButtonId}, and #${fileInputId} exist.`);
        return;
    }

    clearButton.addEventListener('click', function () {
        fileInput.value = null; // Clear the file input

        // Reset the image to the default
        imgTag.src = defaultCoverImage;

        if (hiddenFieldId != null) {
            const hiddenField = document.getElementById(hiddenFieldId);
            if (hiddenField) {
                hiddenField.value = 'true';
            }
        }
    });
}